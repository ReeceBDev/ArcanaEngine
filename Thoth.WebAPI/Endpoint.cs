using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Immutable;
using Thoth.External;
using Thoth.External.Types;
using Thoth.WebAPI.Requests;
using Thoth.WebAPI.Responses;

namespace Thoth.WebAPI
{
    /// <summary> Registers all reading endpoints onto the application. </summary>
    internal static class Endpoint
    {
        /// <summary> Maps all reading routes onto <paramref name="app"/>. </summary>
        internal static void MapReadingEndpoints(this WebApplication app)
        {
            app.MapPost("/reading/birthdate", HandleBirthdate);
            app.MapPost("/reading/name", HandleName);
            app.MapPost("/reading/full", HandleFull);
            app.MapGet("/reading/growth", HandleGrowth);
        }

        /// <summary> Maps all live astrology routes onto <paramref name="app"/>. </summary>
        internal static void MapAstroEndpoints(this WebApplication app)
        {
            app.MapGet("/astro/live", HandleAstroLive);
        }

        /// <summary> Returns a live snapshot of the sky: current placements of the ten classical bodies,
        /// ingresses and stations within the requested window, and aspects currently within orb. </summary>
        private static IResult HandleAstroLive([AsParameters] AstroLiveRequest request)
        {
            try
            {
                DateTimeOffset from = DateTimeOffset.Parse(request.From);
                DateTimeOffset to = DateTimeOffset.Parse(request.To);

                if (from > to)
                    return Results.BadRequest(new ErrorResponse("invalid_range", "'from' must not be later than 'to'"));

                if (to - from > TimeSpan.FromDays(366))
                    return Results.BadRequest(new ErrorResponse("invalid_range", "the requested window must not exceed 366 days"));

                LiveSkySnapshot snapshot = LiveSky.GetSnapshot(DateTimeOffset.UtcNow, from, to);

                return Results.Ok(new AstroLiveResponse(
                    FetchedAt: snapshot.FetchedAt,
                    Placements: snapshot.Placements.Select(MapPlacement),
                    Events: snapshot.Events.Select(MapAstroEvent),
                    Aspects: snapshot.Aspects.Select(MapAspect)));
            }
            catch (FormatException)
            {
                return Results.BadRequest(new ErrorResponse(
                    "invalid_date",
                    "Could not parse 'from'/'to'. Expected ISO-8601 with offset, e.g. 2026-08-18T09:23:11.000Z"));
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
        }

        /// <summary> Returns all cards derivable from a given birth date alone:
        /// personality-derived cards plus zodiacal-sun-derived cards.
        /// Also indicates whether the birth date falls near a zodiacal cusp,
        /// in which case a birth time should be supplied for a more accurate reading. </summary>
        private static IResult HandleBirthdate(BirthdateRequest request)
        {
            try
            {
                DateTime birthDate = DateTime.Parse(request.BirthDate);
                IPractitioner practitioner = Practitioner.Create(birthDate);

                ImmutableArray<IArcanaCard> zodiacalSunCards = practitioner.GetCorrespondenceCards()
                    .Where(i => i.Role == CorrespondenceOption.ZodiacalSun)
                    .SelectMany(i => ImmutableArray.Create(i.Court, i.Decan, i.Zodiac))
                    .ToImmutableArray();

                bool cuspWarning = !practitioner.CheckWhetherZodiacalSunIsAccurate(birthDate);

                return Results.Ok(new BirthdateReadingResponse(
                    PersonalityCards: MapCards(practitioner.GetPersonalityCards()),
                    ZodiacalSunCards: MapCards(zodiacalSunCards),
                    CuspWarning: cuspWarning,
                    CuspWarningMessage: cuspWarning ? CuspWarningMessages.NearCusp : null));
            }
            catch (FormatException)
            {
                return Results.BadRequest(new ErrorResponse(
                    "invalid_date",
                    $"'{request.BirthDate}' could not be parsed. Expected format: yyyy-MM-dd"));
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
        }

        /// <summary> Returns personality cards and name cards for a given birth date and name.
        /// Returns a 422 with guidance when the name contains an unresolved letter C. </summary>
        private static IResult HandleName(NameRequest request)
        {
            try
            {
                DateTime birthDate = DateTime.Parse(request.BirthDate);
                IPractitioner practitioner = Practitioner.Create(birthDate);

                try
                {
                    practitioner.SetName(request.Name);
                }
                catch (Exception e) when (e.Message.Contains("letter C"))
                {
                    return Results.UnprocessableEntity(new NameReadingResponse(
                        PersonalityCards: null,
                        NameCards: null,
                        InvalidNameError: e.Message));
                }

                return Results.Ok(new NameReadingResponse(
                    PersonalityCards: MapCards(practitioner.GetPersonalityCards()),
                    NameCards: MapCards(practitioner.GetNameCards()),
                    InvalidNameError: null));
            }
            catch (FormatException)
            {
                return Results.BadRequest(new ErrorResponse(
                    "invalid_date",
                    $"'{request.BirthDate}' could not be parsed. Expected format: yyyy-MM-dd"));
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
        }

        /// <summary> Returns all available cards for a fully configured practitioner nativety.
        /// Requires birth date, name, an exact birth time with timezone, and birth coordinates. </summary>
        private static IResult HandleFull(FullRequest request)
        {
            try
            {
                DateTime birthDate = DateTime.Parse(request.BirthDate);
                DateTimeOffset birthTime = DateTimeOffset.Parse(request.BirthTime);

                IPractitioner practitioner = Practitioner.Create(birthDate);
                practitioner.SetLocation(request.Latitude, request.Longitude);
                practitioner.SetBirthTime(birthTime);

                try
                {
                    practitioner.SetName(request.Name);
                }
                catch (Exception e) when (e.Message.Contains("letter C"))
                {
                    return Results.UnprocessableEntity(new FullReadingResponse(
                        PersonalityCards: null,
                        NameCards: null,
                        Correspondences: null,
                        Houses: null,
                        CuspWarning: false,
                        CuspWarningMessage: null,
                        InvalidNameError: e.Message));
                }

                bool cuspWarning = !practitioner.CheckWhetherZodiacalSunIsAccurate(birthDate);

                return Results.Ok(new FullReadingResponse(
                    PersonalityCards: MapCards(practitioner.GetPersonalityCards()),
                    NameCards: MapCards(practitioner.GetNameCards()),
                    Correspondences: MapCorrespondences(practitioner.GetCorrespondenceCards()),
                    Houses: MapHouses(practitioner.GetHouseCorrespondences()),
                    CuspWarning: cuspWarning,
                    CuspWarningMessage: cuspWarning ? CuspWarningMessages.NearCusp : null,
                    InvalidNameError: null));
            }
            catch (FormatException e)
            {
                string field = e.Message.Contains("DateTimeOffset") ? "BirthTime" : "BirthDate";
                return Results.BadRequest(new ErrorResponse(
                    "invalid_date",
                    $"Could not parse '{field}'. Expected BirthDate: yyyy-MM-dd, BirthTime: yyyy-MM-ddTHH:mm:sszzz"));
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
        }

        /// <summary> Returns growth cards for a practitioner in a target year,
        /// with optional years before and after the target year. </summary>
        private static IResult HandleGrowth([AsParameters] GrowthCardRequest request)
        {
            try
            {
                DateTime birthDate = DateTime.Parse(request.BirthDate);
                IPractitioner practitioner = Practitioner.Create(birthDate);

                int before = request.Before ?? 0;
                int after = request.After ?? 0;

                if (before < 0)
                    return Results.BadRequest(new ErrorResponse("invalid_range", "'before' must be >= 0"));

                if (after < 0)
                    return Results.BadRequest(new ErrorResponse("invalid_range", "'after' must be >= 0"));

                ImmutableArray<IArcanaCard> growthCards = practitioner.GetGrowthCards(request.Year, before, after);

                List<GrowthCardResponse> cards = [];
                int firstYear = request.Year - before;

                for (int i = 0; i < growthCards.Length; i++)
                    cards.Add(new GrowthCardResponse(firstYear + i, ArcanaCardResponse.From(growthCards[i])));

                return Results.Ok(new GrowthReadingResponse(
                    TargetYear: request.Year,
                    Cards: cards));
            }
            catch (FormatException)
            {
                return Results.BadRequest(new ErrorResponse(
                    "invalid_date",
                    $"'{request.BirthDate}' could not be parsed. Expected format: yyyy-MM-dd"));
            }
            catch (ArgumentOutOfRangeException e)
            {
                return Results.BadRequest(new ErrorResponse("invalid_range", e.ParamName ?? "Invalid range."));
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
        }

        private static PlanetPlacementResponse MapPlacement(LivePlacement placement) => new(
            placement.Planet,
            placement.Sign,
            placement.DegreeInSign,
            placement.Speed,
            placement.Retrograde,
            placement.PeaksAt,
            placement.StopsAt,
            placement.StartedAt);

        private static AstroEventResponse MapAstroEvent(LiveAstroEvent astroEvent) => new(
            astroEvent.Id,
            astroEvent.Kind,
            astroEvent.Planet,
            astroEvent.Sign,
            astroEvent.Time,
            astroEvent.PeaksAt,
            astroEvent.StopsAt,
            astroEvent.StartedAt,
            astroEvent.Direction);

        private static AspectInfoResponse MapAspect(LiveAspect aspect) => new(
            aspect.PlanetA,
            aspect.SignA,
            aspect.PlanetB,
            aspect.SignB,
            aspect.Aspect,
            aspect.OrbDeg,
            aspect.Applying,
            aspect.PeaksAt,
            aspect.StopsAt,
            aspect.StartedAt);

        private static IEnumerable<ArcanaCardResponse> MapCards(ImmutableArray<IArcanaCard> cards)
        {
            foreach (IArcanaCard card in cards)
                yield return ArcanaCardResponse.From(card);
        }

        private static IEnumerable<CorrespondenceResponse> MapCorrespondences(ImmutableArray<ICorrespondence> correspondences)
        {
            foreach (ICorrespondence correspondence in correspondences)
                yield return CorrespondenceResponse.From(correspondence);
        }

        private static IEnumerable<HouseCorrespondenceResponse> MapHouses(ImmutableArray<IHouseCorrespondence> houses)
        {
            foreach (IHouseCorrespondence house in houses)
                yield return HouseCorrespondenceResponse.From(house);
        }
    }
}