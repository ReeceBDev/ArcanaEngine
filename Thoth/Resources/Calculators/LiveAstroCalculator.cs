using SwissEphNet;
using Thoth.External.Types;
using Thoth.Types.Zodiacal;

namespace Thoth.Resources.Calculators
{
    /// <summary> Computes a live snapshot of the apparent geocentric tropical sky via Swiss Ephemeris:
    /// current placements of the ten classical bodies, sign-boundary ingresses and retrograde/direct
    /// stations within a window, and the aspect pairs currently within orb.
    /// All crossing and station times are found by coarse stepping with bisection refinement,
    /// rounded to the nearest minute. When ephemeris data files are unavailable, the Moshier
    /// fallback is used automatically. </summary>
    internal sealed class LiveAstroCalculator : ILiveAstroCalculator, IDisposable
    {
        private static readonly CelestialBody[] Bodies =
        [
            CelestialBody.Sun, CelestialBody.Moon, CelestialBody.Mercury, CelestialBody.Venus, CelestialBody.Mars,
            CelestialBody.Jupiter, CelestialBody.Saturn, CelestialBody.Uranus, CelestialBody.Neptune, CelestialBody.Pluto,
        ];

        private static readonly double[] AspectAngles = [0.0, 60.0, 90.0, 120.0, 180.0];

        private const double HourInDays = 1.0 / 24.0;
        private const double SixHoursInDays = 0.25;
        private const double MinuteInDays = 1.0 / (24.0 * 60.0);
        private const double TenYearsInDays = 3652.5;
        private const int BisectIterations = 44;

        private readonly SwissEph swissEph = new();

        private readonly record struct BodyState(double Longitude, double Speed);

        /// <summary> Returns the placements at <paramref name="now"/>, the ingresses and stations within
        /// [<paramref name="from"/>, <paramref name="to"/>], and the aspects currently within orb. </summary>
        public LiveSkySnapshot GetSnapshot(DateTimeOffset now, DateTimeOffset from, DateTimeOffset to)
        {
            DateTime utc = now.UtcDateTime;
            DateTimeOffset fetchedAt = new(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, utc.Second, TimeSpan.Zero);
            double jdNow = ToJulianDay(now);

            List<LivePlacement> placements = [];
            Dictionary<CelestialBody, BodyState> current = [];

            foreach (CelestialBody body in Bodies)
            {
                BodyState state = GetBodyState(jdNow, body);
                current[body] = state;
                placements.Add(BuildPlacement(body, state, now));
            }

            List<LiveAstroEvent> events = [];
            CollectIngressEvents(events, from, to);
            CollectStationEvents(events, from, to);
            events.Sort((left, right) => left.Time.CompareTo(right.Time));

            return new LiveSkySnapshot(fetchedAt, placements, events, BuildAspects(current, jdNow));
        }

        /// <summary> Builds the current placement of a body, including its sign-entry, mid-sign and sign-exit moments. </summary>
        private LivePlacement BuildPlacement(CelestialBody body, BodyState state, DateTimeOffset now)
        {
            int sign = SignIndexOf(state.Longitude);
            double horizon = SignSearchHorizon(body);

            return new LivePlacement(
                Planet: body.ToString(),
                Sign: ((ZodiacSign)sign).ToString(),
                DegreeInSign: Math.Round(NormalizeLongitude(state.Longitude) - sign * 30.0, 2),
                Speed: Math.Round(state.Speed, 2),
                Retrograde: state.Speed < 0,
                PeaksAt: FindMidSignCrossing(body, now, sign, horizon),
                StopsAt: FindBoundary(body, now, backward: false, now.AddDays(horizon)),
                StartedAt: FindBoundary(body, now, backward: true, now.AddDays(-horizon)));
        }

        /// <summary> Finds when the body next crosses 15° (mid-sign) of its current sign, honoring its
        /// direction of travel: direct bodies below 15° peak ahead of them, retrograde bodies above 15°
        /// peak backing down toward 15°. Null when the midpoint already lies behind the direction of travel. </summary>
        private DateTimeOffset? FindMidSignCrossing(CelestialBody body, DateTimeOffset now, int sign, double horizonDays)
        {
            BodyState state = GetBodyState(ToJulianDay(now), body);
            double degreeInSign = NormalizeLongitude(state.Longitude) - sign * 30.0;

            bool directAndBefore = state.Speed >= 0 && degreeInSign < 15.0;
            bool retrogradeAndAfter = state.Speed < 0 && degreeInSign > 15.0;

            return directAndBefore || retrogradeAndAfter
                ? FindLongitudeZero(body, now, sign * 30.0 + 15.0, horizonDays)
                : null;
        }

        /// <summary> Collects every sign-boundary ingress whose moment falls within [from, to]. </summary>
        private void CollectIngressEvents(List<LiveAstroEvent> events, DateTimeOffset from, DateTimeOffset to)
        {
            foreach (CelestialBody body in Bodies)
            {
                DateTimeOffset cursor = from;

                while (true)
                {
                    DateTimeOffset? crossing = FindBoundary(body, cursor, backward: false, to);
                    if (crossing is not { } ingress || ingress > to)
                        break;

                    events.Add(BuildIngressEvent(body, ingress));
                    cursor = ingress.AddMinutes(1);
                }
            }
        }

        private LiveAstroEvent BuildIngressEvent(CelestialBody body, DateTimeOffset time)
        {
            int sign = SignIndexOf(GetBodyState(ToJulianDay(time) + MinuteInDays, body).Longitude);
            double horizon = SignSearchHorizon(body);

            return new LiveAstroEvent(
                Id: EventId("ingress", body, sign, time),
                Kind: "ingress",
                Planet: body.ToString(),
                Sign: ((ZodiacSign)sign).ToString(),
                Direction: null,
                Time: time,
                PeaksAt: FindLongitudeZero(body, time, sign * 30.0 + 15.0, horizon),
                StopsAt: FindBoundary(body, time.AddMinutes(1), backward: false, time.AddDays(horizon)),
                StartedAt: time);
        }

        /// <summary> Collects every retrograde/direct station whose moment falls within [from, to]. </summary>
        private void CollectStationEvents(List<LiveAstroEvent> events, DateTimeOffset from, DateTimeOffset to)
        {
            foreach (CelestialBody body in Bodies)
            {
                if (body is CelestialBody.Sun or CelestialBody.Moon)
                    continue; // the luminaries never station

                DateTimeOffset cursor = from;

                while (true)
                {
                    double? jdStation = FindSpeedZero(body, cursor, to);
                    if (jdStation is not { } station)
                        break;

                    DateTimeOffset time = RoundToMinute(FromJulianDay(station));
                    if (time > to)
                        break;

                    events.Add(BuildStationEvent(body, time));
                    cursor = time.AddMinutes(1);
                }
            }
        }

        private LiveAstroEvent BuildStationEvent(CelestialBody body, DateTimeOffset time)
        {
            double jd = ToJulianDay(time);
            BodyState at = GetBodyState(jd, body);
            string direction = GetBodyState(jd - HourInDays, body).Speed > 0 ? "retrograde" : "direct";
            int sign = SignIndexOf(at.Longitude);

            // The opposite station ends the new-direction arc; the arc's longitude midpoint is its peak.
            DateTimeOffset? stopsAt = null;
            DateTimeOffset? peaksAt = null;
            double? jdOpposite = FindSpeedZero(body, time.AddMinutes(1), time.AddYears(6));

            if (jdOpposite is { } opposite)
            {
                stopsAt = RoundToMinute(FromJulianDay(opposite));
                double arcMidpoint = at.Longitude + FoldSigned(GetBodyState(opposite, body).Longitude - at.Longitude) / 2.0;

                if (FindLongitudeZero(body, time, arcMidpoint, opposite - jd + 1.0) is { } midpoint)
                    peaksAt = midpoint;
            }

            return new LiveAstroEvent(
                Id: EventId("station", body, sign, time),
                Kind: "station",
                Planet: body.ToString(),
                Sign: ((ZodiacSign)sign).ToString(),
                Direction: direction,
                Time: time,
                PeaksAt: peaksAt,
                StopsAt: stopsAt,
                StartedAt: time);
        }

        /// <summary> Builds every pair of bodies currently within orb of a classical aspect. </summary>
        private List<LiveAspect> BuildAspects(Dictionary<CelestialBody, BodyState> current, double jdNow)
        {
            const double moonOrbLimit = 8.0;

            List<LiveAspect> aspects = [];

            for (int i = 0; i < Bodies.Length; i++)
            {
                for (int j = i + 1; j < Bodies.Length; j++)
                {
                    CelestialBody bodyA = Bodies[i];
                    CelestialBody bodyB = Bodies[j];
                    double separation = Math.Abs(FoldSigned(current[bodyA].Longitude - current[bodyB].Longitude));
                    bool moonInvolved = bodyA == CelestialBody.Moon || bodyB == CelestialBody.Moon;

                    foreach (double angle in AspectAngles)
                    {
                        double limit = moonInvolved ? moonOrbLimit : OrbLimit(angle);
                        double orb = Math.Abs(separation - angle);
                        if (orb > limit)
                            continue;

                        aspects.Add(BuildAspect(bodyA, bodyB, current[bodyA], current[bodyB], angle, limit, jdNow));
                        break; // orb limits guarantee at most one aspect per pair
                    }
                }
            }

            return aspects;
        }

        /// <summary> Orb limits: 6° for conjunctions and oppositions, 4° for sextiles, squares and trines. </summary>
        private static double OrbLimit(double angle) => angle is 0.0 or 180.0 ? 6.0 : 4.0;

        private LiveAspect BuildAspect(CelestialBody bodyA, CelestialBody bodyB, BodyState stateA, BodyState stateB,
            double angle, double limit, double jdNow)
        {
            // A pair perfects an aspect on one of two mirrored branches: Δ ≡ +angle or Δ ≡ −angle (mod 360°).
            // Pin the branch nearer to the current separation so the difference crosses zero exactly at perfection.
            double delta = FoldSigned(stateA.Longitude - stateB.Longitude);
            double branchAngle = Math.Abs(FoldSigned(delta - angle)) <= Math.Abs(FoldSigned(delta + angle)) ? angle : -angle;

            double Difference(double jd) => FoldSigned(GetLongitude(jd, bodyA) - GetLongitude(jd, bodyB) - branchAngle);

            bool applying = Math.Abs(Difference(jdNow + HourInDays)) < Math.Abs(Difference(jdNow));

            DateTimeOffset? startedAt = FindOrbEdge(Difference, jdNow, backward: true, limit);
            DateTimeOffset? stopsAt = FindOrbEdge(Difference, jdNow, backward: false, limit);

            // The perfection moment is searched across the whole in-orb span — including past perfections.
            double jdFrom = startedAt.HasValue ? ToJulianDay(startedAt.Value) : jdNow - TenYearsInDays;
            double jdTo = stopsAt.HasValue ? ToJulianDay(stopsAt.Value) : jdNow + TenYearsInDays;
            DateTimeOffset? peaksAt = FindZero(Difference, jdFrom, jdTo) is { } perfection
                ? RoundToMinute(FromJulianDay(perfection))
                : null;

            return new LiveAspect(
                PlanetA: bodyA.ToString(),
                SignA: ((ZodiacSign)SignIndexOf(stateA.Longitude)).ToString(),
                PlanetB: bodyB.ToString(),
                SignB: ((ZodiacSign)SignIndexOf(stateB.Longitude)).ToString(),
                Aspect: AspectName(angle),
                OrbDeg: Math.Round(Math.Abs(Difference(jdNow)), 2),
                Applying: applying,
                PeaksAt: peaksAt,
                StopsAt: stopsAt,
                StartedAt: startedAt);
        }

        private static string AspectName(double angle) => angle switch
        {
            0.0 => "conjunction",
            60.0 => "sextile",
            90.0 => "square",
            120.0 => "trine",
            _ => "opposition",
        };

        /// <summary> Finds the next moment the body crosses a specific ecliptic longitude (in either direction),
        /// searching forward from <paramref name="start"/> within <paramref name="horizonDays"/>. </summary>
        private DateTimeOffset? FindLongitudeZero(CelestialBody body, DateTimeOffset start, double targetLongitude, double horizonDays)
        {
            double jd = ToJulianDay(start);
            double jdLimit = jd + horizonDays;
            double step = InitialStep(body);
            double previous = FoldSigned(GetLongitude(jd, body) - targetLongitude);

            while (jd < jdLimit)
            {
                double stepDays = Math.Min(step, jdLimit - jd);
                if (stepDays <= 0)
                    break;

                double jdNext = jd + stepDays;
                double next = FoldSigned(GetLongitude(jdNext, body) - targetLongitude);

                if (Math.Sign(previous) != Math.Sign(next) && Math.Abs(previous - next) < 180.0)
                    return RoundToMinute(FromJulianDay(Bisect(jd => FoldSigned(GetLongitude(jd, body) - targetLongitude), jd, jdNext, previous)));

                jd = jdNext;
                previous = next;
                step = Math.Min(step * 2, 45.0);
            }

            return null;
        }

        /// <summary> Finds the next sign-boundary crossing after (or, when <paramref name="backward"/>, before)
        /// <paramref name="start"/>, in either direction of travel. </summary>
        private DateTimeOffset? FindBoundary(CelestialBody body, DateTimeOffset start, bool backward, DateTimeOffset limit)
        {
            double jd = ToJulianDay(start);
            double jdLimit = ToJulianDay(limit);
            double step = InitialStep(body);
            BodyState state = GetBodyState(jd, body);

            while (backward ? jd > jdLimit : jd < jdLimit)
            {
                // never traverse more than 10° of longitude per step, so at most one boundary lies between samples
                double travelCap = 10.0 / Math.Max(Math.Abs(state.Speed), 1e-6);
                double stepDays = Math.Min(Math.Min(step, travelCap), Math.Abs(jdLimit - jd));
                if (stepDays <= 0)
                    break;

                double jdNext = jd + (backward ? -stepDays : stepDays);
                BodyState next = GetBodyState(jdNext, body);

                double earlyLongitude = backward ? next.Longitude : state.Longitude;
                double lateLongitude = backward ? state.Longitude : next.Longitude;
                int earlySign = (int)(NormalizeLongitude(earlyLongitude) / 30.0);
                int lateSign = (int)(NormalizeLongitude(lateLongitude) / 30.0);

                if (earlySign != lateSign)
                {
                    double motion = FoldSigned(lateLongitude - earlyLongitude);
                    double edge = motion > 0 ? 30.0 * (earlySign + 1) : 30.0 * earlySign;

                    double currentSide = FoldSigned(state.Longitude - edge);
                    double nextSide = FoldSigned(next.Longitude - edge);

                    if (Math.Sign(currentSide) != Math.Sign(nextSide) && Math.Abs(currentSide - nextSide) < 180.0)
                        return RoundToMinute(FromJulianDay(Bisect(jd => FoldSigned(GetLongitude(jd, body) - edge), jd, jdNext, currentSide)));
                }

                jd = jdNext;
                state = next;
                step = Math.Min(step * 2, 45.0);
            }

            return null;
        }

        /// <summary> Finds the first moment forward of <paramref name="start"/> (and before <paramref name="limit"/>)
        /// at which the body's daily speed changes sign — a station. </summary>
        private double? FindSpeedZero(CelestialBody body, DateTimeOffset start, DateTimeOffset limit)
        {
            double jd = ToJulianDay(start);
            double jdLimit = ToJulianDay(limit);
            double step = SixHoursInDays;
            double previous = GetBodyState(jd, body).Speed;

            while (jd < jdLimit)
            {
                // cap steps at 10 days so a full retrograde arc can never fit between two samples
                double stepDays = Math.Min(step, jdLimit - jd);
                if (stepDays <= 0)
                    break;

                double jdNext = jd + stepDays;
                double next = GetBodyState(jdNext, body).Speed;

                if (Math.Sign(previous) != Math.Sign(next))
                    return Bisect(jd => GetBodyState(jd, body).Speed, jd, jdNext, previous);

                jd = jdNext;
                previous = next;
                step = Math.Min(step * 2, 10.0);
            }

            return null;
        }

        /// <summary> Finds the moment the aspect pair left (backward) or will leave (forward) the orb,
        /// sampling at 12-hour resolution for the first 8 days and doubling thereafter, up to ten years.
        /// Bisection keeps the inside/outside roles of the bracket endpoints orientation-agnostic. </summary>
        private DateTimeOffset? FindOrbEdge(Func<double, double> aspectDifference, double jdNow, bool backward, double limit)
        {
            double direction = backward ? -1 : 1;
            double jdInside = jdNow;

            double offset = 0.5;
            while (offset <= TenYearsInDays)
            {
                double jdOutside = jdNow + direction * offset;
                if (Math.Abs(aspectDifference(jdOutside)) > limit)
                {
                    double jdLow = Math.Min(jdInside, jdOutside);
                    double jdHigh = Math.Max(jdInside, jdOutside);
                    bool insideAtLow = Math.Abs(aspectDifference(jdLow)) <= limit;

                    for (int i = 0; i < BisectIterations; i++)
                    {
                        double middle = (jdLow + jdHigh) / 2;
                        if (Math.Abs(aspectDifference(middle)) <= limit == insideAtLow) jdLow = middle;
                        else jdHigh = middle;
                    }

                    return RoundToMinute(FromJulianDay((jdLow + jdHigh) / 2));
                }

                jdInside = jdOutside;
                offset = offset >= 8.0 ? offset * 2 : offset + 0.5;
            }

            return null;
        }

        /// <summary> Finds the first zero of a continuous function between two Julian days by stepping and bisection. </summary>
        private static double? FindZero(Func<double, double> value, double jdFrom, double jdTo)
        {
            if (jdTo - jdFrom < MinuteInDays)
                return null;

            double jd = jdFrom;
            double step = SixHoursInDays;
            double previous = value(jd);

            while (jd < jdTo)
            {
                double stepDays = Math.Min(step, jdTo - jd);
                if (stepDays <= 0)
                    break;

                double jdNext = jd + stepDays;
                double next = value(jdNext);

                if (Math.Sign(previous) != Math.Sign(next) && Math.Abs(previous - next) < 180.0)
                    return Bisect(value, jd, jdNext, previous);

                jd = jdNext;
                previous = next;
                step = Math.Min(step * 2, 45.0);
            }

            return null;
        }

        /// <summary> Bisects a bracketed zero of a continuous function. </summary>
        private static double Bisect(Func<double, double> value, double jdLow, double jdHigh, double valueAtLow)
        {
            double low = jdLow, high = jdHigh, atLow = valueAtLow;

            for (int i = 0; i < BisectIterations; i++)
            {
                double middle = (low + high) / 2;
                double atMiddle = value(middle);

                if (Math.Sign(atMiddle) == Math.Sign(atLow))
                {
                    low = middle;
                    atLow = atMiddle;
                }
                else
                {
                    high = middle;
                }
            }

            return (low + high) / 2;
        }

        /// <summary> Calculates the apparent geocentric position and daily speed of a body.
        /// Falls back to the Moshier ephemeris when Swiss Ephemeris data files are unavailable. </summary>
        private BodyState GetBodyState(double jd, CelestialBody body)
        {
            double[] position = new double[6];
            string serr = "";

            int result = swissEph.swe_calc_ut(jd, GetSwissPlanetId(body), SwissEph.SEFLG_SWIEPH | SwissEph.SEFLG_SPEED, position, ref serr);

            if (result < 0)
            {
                serr = "";
                result = swissEph.swe_calc_ut(jd, GetSwissPlanetId(body), SwissEph.SEFLG_MOSEPH | SwissEph.SEFLG_SPEED, position, ref serr);

                if (result < 0)
                    throw new InvalidOperationException($"Failed to calculate {body} position: {serr}");
            }

            return new BodyState(position[0], position[3]);
        }

        private double GetLongitude(double jd, CelestialBody body) => GetBodyState(jd, body).Longitude;

        private int GetSwissPlanetId(CelestialBody body) => body switch
        {
            CelestialBody.Sun => SwissEph.SE_SUN,
            CelestialBody.Moon => SwissEph.SE_MOON,
            CelestialBody.Mercury => SwissEph.SE_MERCURY,
            CelestialBody.Venus => SwissEph.SE_VENUS,
            CelestialBody.Mars => SwissEph.SE_MARS,
            CelestialBody.Jupiter => SwissEph.SE_JUPITER,
            CelestialBody.Saturn => SwissEph.SE_SATURN,
            CelestialBody.Uranus => SwissEph.SE_URANUS,
            CelestialBody.Neptune => SwissEph.SE_NEPTUNE,
            CelestialBody.Pluto => SwissEph.SE_PLUTO,
            CelestialBody.Earth => SwissEph.SE_EARTH,
            _ => throw new ArgumentException($"Unsupported celestial body: {body}"),
        };

        private static double InitialStep(CelestialBody body) => body == CelestialBody.Moon ? HourInDays : SixHoursInDays;

        /// <summary> How far (in days) to search for a body's sign entry and exit — comfortably beyond
        /// the longest possible residence of each body within one sign. </summary>
        private static double SignSearchHorizon(CelestialBody body) => body switch
        {
            CelestialBody.Moon => 8.0,
            CelestialBody.Sun => 40.0,
            CelestialBody.Mercury or CelestialBody.Venus => 400.0,
            CelestialBody.Mars => 500.0,
            CelestialBody.Jupiter => 800.0,
            CelestialBody.Saturn => 1200.0,
            CelestialBody.Uranus => 3000.0,
            CelestialBody.Neptune => 6000.0,
            _ => 12000.0,
        };

        private static int SignIndexOf(double longitude) => (int)(NormalizeLongitude(longitude) / 30.0);

        private static double NormalizeLongitude(double longitude)
        {
            double normalized = longitude % 360.0;
            return normalized < 0 ? normalized + 360.0 : normalized;
        }

        /// <summary> Folds an angle into [-180°, 180°). </summary>
        private static double FoldSigned(double angle)
        {
            angle %= 360.0;
            if (angle >= 180.0) angle -= 360.0;
            else if (angle < -180.0) angle += 360.0;
            return angle;
        }

        private static DateTimeOffset RoundToMinute(DateTimeOffset time)
        {
            DateTimeOffset nearest = time.AddSeconds(30);
            return new DateTimeOffset(nearest.Year, nearest.Month, nearest.Day, nearest.Hour, nearest.Minute, 0, TimeSpan.Zero);
        }

        private static string EventId(string kind, CelestialBody body, int sign, DateTimeOffset time) =>
            $"{kind}-{body.ToString().ToLowerInvariant()}-{((ZodiacSign)sign).ToString().ToLowerInvariant()}-{time:yyyy-MM-dd'T'HH:mm:ss'Z'}";

        private double ToJulianDay(DateTimeOffset time)
        {
            DateTime utc = time.UtcDateTime;
            double hour = utc.Hour + utc.Minute / 60.0 + utc.Second / 3600.0 + utc.Millisecond / 3600000.0;
            return swissEph.swe_julday(utc.Year, utc.Month, utc.Day, hour, SwissEph.SE_GREG_CAL);
        }

        /// <summary> Converts a Julian Day (UT) to a UTC DateTimeOffset via the standard Gregorian algorithm. </summary>
        private static DateTimeOffset FromJulianDay(double jd)
        {
            double z = Math.Floor(jd + 0.5);
            double fraction = jd + 0.5 - z;
            double alpha = Math.Floor((z - 1867216.25) / 36524.25);
            double a = z + 1 + alpha - Math.Floor(alpha / 4.0);
            double b = a + 1524.0;
            double c = Math.Floor((b - 122.1) / 365.25);
            double d = Math.Floor(365.25 * c);
            double e = Math.Floor((b - d) / 30.6001);
            double dayWithFraction = b - d - Math.Floor(30.6001 * e) + fraction;
            int month = (int)(e < 14.0 ? e - 1 : e - 13);
            int year = (int)(month > 2 ? c - 4716 : c - 4715);
            int day = (int)Math.Floor(dayWithFraction);
            double seconds = Math.Round((dayWithFraction - day) * 86400.0);

            return new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero).AddSeconds(seconds);
        }

        public void Dispose() => swissEph.Dispose();
    }
}
