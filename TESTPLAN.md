# ArcanaEngine / Thoth.API — Test Plan

Legend:
- **PASS** = valid input, the success path must happen.
- **FAIL** = invalid input / missing prerequisite / broken condition — the system must fail *loudly and correctly* (exception, 4xx, or warning flag). The test asserts the failure happens. Silent success on these = a red test.
- [pin] = pinning current behavior as-is (prototype design, loud failures intended).
- [red-until] = encodes desired behavior not yet implemented; test stays red until you implement it.

CI: `Thoth.Tests` project added to `Thoth.slnx`. Woodpecker already runs `dotnet test -c Release` on dev/master — no pipeline change needed.

---

## 1. Engine — `Practitioner.Create` (construction overloads)

- PASS — empty `Create()` returns a usable practitioner.
- PASS — `Create(birthDate)` returns a practitioner that can produce personality cards.
- FAIL — `Create("Clarice")` throws the C-rule guidance exception (name overload validates too).

## 2. Engine — fluent setters (`SetName` / `SetBirthdate` / `SetBirthTime` / `SetLocation`)

- PASS — every setter returns the same practitioner instance (chainable).
- FAIL — a lone-C name inside a chain throws and is not swallowed mid-chain.

## 3. Engine — `SetName` C-rule (aspect: lone C rejection)

- PASS — "Charlie" accepted (CH pair kept).
- PASS — "ch", "Ch", "CH" all accepted regardless of casing.
- FAIL — "Clarice" throws, message contains the K/Z instructions.
- FAIL — trailing C ("Alei c" → "Aleic") throws.
- FAIL — C followed by non-H ("Cxarlie") throws.

## 4. Engine — `SetName` sanitization (aspect: junk stripping)

- PASS — "A1le.ister" produces identical cards to "Aleister".
- FAIL — junk-only name ("123") → later `GetNameCards` throws (nothing was stored).

## 5. Engine — empty name (aspect: loud failure on unset state)

- PASS — `SetName("")` itself does not throw. [pin]
- FAIL — `GetNameCards` after `SetName("")` throws the instructional exception.

## 6. Engine — `GetNameCards` (aspect: name composition shapes)

- PASS — one name part → First + Whole roles only.
- PASS — two parts → First + Last + Whole.
- PASS — three parts → First + Middle + Last + Whole.
- PASS — known value: "Aleister" → Hierophant (5).
- FAIL — `GetNameCards` with no name ever set → throws instructional exception.

## 7. Engine — `GetPersonalityCards` (aspect: cross-sum math + prerequisites)

- PASS — 1990-05-14 → Personality = Lust (11), Teacher = High Priestess (2).
- PASS — date whose cross-sum is already ≤ 9 → no Teacher card, single card returned.
- FAIL — no birthdate set → throws instructional exception.

## 8. Engine — `GetGrowthCards` (aspect: range, order, negatives)

- PASS — year 2026 for 1990-05-14 → exactly 1 card, Lust (11).
- PASS — (2026, before:2, after:1) → 4 cards ordered 2024 → 2027.
- FAIL — negative `yearsBefore` → `ArgumentOutOfRangeException`.
- FAIL — negative `yearsBeyond` → `ArgumentOutOfRangeException`.
- FAIL — no birthdate set → throws instructional exception.
- Deferred — extreme-year bounds (per your call, leave for now).

## 9. Engine — `GetCorrespondenceCards` (aspect: data availability tiers)

- PASS — birthdate only → exactly 1 correspondence, role = ZodiacalSun, Zodiac card = Hierophant for mid-Taurus, Decan + Court populated.
- FAIL — birthdate only → no RisingSun / Moon / planets returned (partial data must not be fabricated).
- PASS — birthdate + exact time → all 8 roles present exactly once.
- FAIL — nothing set → throws instructional exception.

## 10. Engine — `GetHouseCorrespondences` (aspect: full nativity required)

- PASS — date + time + location → exactly 12 houses, numbered 1–12, degrees 0–29, valid sign names.
- FAIL — date only (no time, no location) → throws instructional exception.
- FAIL — date + time but no location → throws instructional exception.

## 11. Engine — `CheckWhetherZodiacalSunIsAccurate` (aspect: cusp alerting — the pair you called out)

- PASS — mid-sign date (mid-Taurus, e.g. 1990-05-14) → returns `true` (no cusp warning).
- FAIL — cusp date (Aries→Taurus boundary, ~1990-04-20, verified against Swiss Ephemeris at test-writing time) → returns `false` (cusp warning fires).
- FAIL — opposite cusp (Taurus→Gemini boundary, ~1990-05-21) → returns `false`.
- PASS — a known cusp date becomes `true` once an exact birth time resolves it away from the boundary (verified at test-writing time).
- [pin] — passing a *different* date than the one set rebinds the practitioner's birthdate; test documents the mutation. Flag here if unintended.

## 12. Internal — `ThothDeck` (deck integrity)

- PASS — all 22 Major Arcana JSONs deserialize; names non-empty; numbers 0–21.
- PASS — all 56 Minor Arcana JSONs deserialize; suit set; numbers in range.
- PASS — `FetchMajorArcana(22)` → The Fool (0/22 wrap).
- FAIL — `FetchMajorArcana(23)` throws.
- FAIL — `FetchMajorArcana(-1)` throws.

## 13. Internal — `ThothCalculator` (pure math)

- PASS — `CalculateCrossSum(2009)` = 11; `(23)` = 5; `(999)` = 9; `(22)` = 22; `(≤9)` unchanged.
- PASS — decan degree 0 vs 10 → different decans (boundary stepping).
- FAIL — `GetDecanCardByAbsoluteDegree(-1)` throws; `(360)` throws.
- PASS — court degree 20 vs 21 → boundary flips court card.
- FAIL — `GetCourtCardByAbsoluteDegree(-1)` throws; `(360)` throws.

## 14. Internal — `GemetriaCalculator` (aspect: letter → number mapping)

- PASS — "ALEISTER" = 320.
- PASS — double letters SH / TH / QU group correctly (not summed as singles).
- FAIL — unmappable character ("A1") throws `ArgumentException`.

## 15. Engine — `LiveSky.GetSnapshot` (live sky math)

- PASS — fixed historical window → exactly 10 placements, one per classical body.
- PASS — every `DegreeInSign` within [0, 30).
- PASS — all events within the window and sorted by time.
- PASS — stations never occur for Sun or Moon; ingresses have null Direction.
- PASS — every aspect name in {conjunction, sextile, square, trine, opposition} and every orb within its limit.
- [pin] — inverted window (from > to) at engine level → no crash, empty event list (range validation is the endpoint's job).

## 16. WebAPI — `POST /reading/birthdate`

- PASS — mid-sign date → 200; personality cards present; exactly 3 zodiacal sun cards; `CuspWarning=false`; message null.
- FAIL — cusp date → 200 with `CuspWarning=true` and message populated (alert fires — opposite of the above).
- FAIL — unparseable date ("not-a-date") → 400 `invalid_date`.
- FAIL — missing body / null BirthDate → 400.

## 17. WebAPI — `POST /reading/name`

- PASS — valid name + date → 200; both card sets present; `InvalidNameError` null.
- PASS — "Charlie" → 200 (CH accepted).
- FAIL — "Clarice" → 422; `InvalidNameError` mentions the C/K/Z rule; card sets null.
- FAIL — unparseable date with a valid name → 400 (date is validated before name).
- [pin] — empty name → loud failure via 500 Problem (engine throws, endpoint has no 422 mapping for this case). Flag if you want 422 instead.

## 18. WebAPI — `POST /reading/full`

- PASS — complete valid nativity → 200 with every section: personality cards, name cards, 8 correspondences, 12 houses, correct cusp flag.
- FAIL — lone-C name → 422; `InvalidNameError` set; all card sections null.
- FAIL — unparseable BirthTime → 400 naming BirthTime.
- FAIL — unparseable BirthDate → 400 naming BirthDate.
- PASS — boundary coordinates accepted: latitude ±90, longitude ±180 → 200.
- FAIL [red-until] — latitude 999 → 400 (currently 500; needs validation added).
- FAIL [red-until] — longitude -200 → 400 (currently 500; needs validation added).

## 19. WebAPI — `GET /reading/growth`

- PASS — 1990-05-14, year 2026 → 200; TargetYear echoed; 1 card = Lust labeled 2026.
- PASS — before=2, after=1 → 200; 4 cards labeled 2024–2027 in order.
- FAIL — before=-1 → 400 `invalid_range`.
- FAIL — after=-1 → 400 `invalid_range`.
- FAIL — missing BirthDate or Year → 400.
- FAIL — unparseable date → 400 `invalid_date`.
- Deferred — extreme year bounds (leave for now, per your call).

## 20. WebAPI — `GET /astro/live`

- PASS — valid 30-day window → 200 with 10 placements.
- PASS — zero-length window (from == to) → 200, no events (boundary).
- PASS — 366-day window → 200 (boundary accepted).
- FAIL — from later than to → 400 `invalid_range`.
- FAIL — 367-day window → 400 `invalid_range`.
- FAIL — garbage from/to values → 400 `invalid_date`.
- FAIL — missing from/to query params → 400.

---

## Flagged for your review (not blocking)

1. lat/lon 400 validation is `[red-until]` — I add the tests, they stay red until you implement the bound check (or you implement it now and they go green).
2. Empty name over HTTP is pinned as a loud 500 [pin] — say the word if it should be a 422.
3. `CheckWhetherZodiacalSunIsAccurate` date-rebinding is pinned as a test — flag if unintended.
4. Growth-card extreme-year bounds deferred.
