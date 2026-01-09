using System.Collections.Immutable;
using Thoth.External.InternalConcreteDependencies;
using Thoth.Managers;
using Thoth.Resources;
using Thoth.Resources.Calculators;
using Thoth.Resources.Json;
using Thoth.Types.Practitioner;

namespace Thoth.External.Types
{
    public sealed class Practitioner : IPractitioner
    {
        private static readonly IAstrologicalCalculator astrologicalCalculator = new AstrologicalCalculator();
        private static readonly IGemetriaCalculator gemetriaCalculator = new GemetriaCalculator();
        private static readonly IThothDeck cardBuilder = new ThothDeck();
        private static readonly IThothCalculator thothCalculator = new ThothCalculator(cardBuilder);
        private static readonly ICardProvider cardFetcher = new CardProvider(thothCalculator, cardBuilder, gemetriaCalculator, astrologicalCalculator);

        private readonly IThelemite currentPractitioner;


        /// <summary> Returns an empty practitioner. The practitioner's name or birthdate must be set before values can be pulled. </summary>
        public static IPractitioner Empty { get => new Practitioner(new Thelemite(cardFetcher, astrologicalCalculator, thothCalculator)); }

        private Practitioner(IThelemite setPractitioner)
        {
            currentPractitioner = setPractitioner;
        }

        /// <summary> Returns an empty practitioner. The practitioner's name or birthdate must be set before values can be pulled. </summary>
        public static IPractitioner Create()
            => Practitioner.Empty;

        /// <summary> Returns a practitioner with their name set. This allows Name Cards to be pulled. </summary>
        public static IPractitioner Create(string rawName)
            => Practitioner.Create().SetName(rawName);

        /// <summary> Returns a practitioner with their birth date set. This allows Personality Cards and the Sun Sign from Correspondence Cards to be pulled. </summary>
        public static IPractitioner Create(DateTime birthDate)
            => Practitioner.Create().SetBirthdate(birthDate);

        /// <summary> Returns a practitioner with their name and birth date set. This allows Name Cards, Personality Cards and the Sun Sign from Correspondence Cards to be pulled. </summary>
        public static IPractitioner Create(string rawName, DateTime birthDate)
            => Practitioner.Create(rawName).SetBirthdate(birthDate);

        /// <summary> Returns a fully initiailised practitioner. This allows all cards to be pulled. </summary>
        public static IPractitioner Create(string rawName, DateTime birthDate, double latitude, double longitude)
            => Practitioner.Create(rawName, birthDate).SetLocation(latitude, longitude);


        /// <summary> Sets the practitioner's name. Allows Name Cards to be drawn. </summary>
        public IPractitioner SetName(string rawName)
        {
            string cleanName = TextUtilities.SanitizeInput(rawName);
            currentPractitioner.SetName(cleanName);

            //Return this for enabling method chaining.
            return this;
        }

        /// <summary> Sets the practitioner's birth date. Allows Personality Cards to be drawn, along with the Zodiacal Sun Correspondence Card. </summary>
        public IPractitioner SetBirthdate(DateTime birthDate)
        {
            currentPractitioner.SetDateOfBirth(birthDate);

            //Return this for enabling method chaining.
            return this;
        }

        /// <summary> Sets the practitioner's exact birth time. This must adhere to its correct timezone. Allows most Correspondence Cards to be drawn. </summary>
        public IPractitioner SetBirthTime(DateTimeOffset birthTime)
        {
            currentPractitioner.SetTimeOfBirth(birthTime);

            //Return this for enabling method chaining.
            return this;
        }


        /// <summary> Sets the practitioner's location of their nativety. When set alongside an exact time of birth, allows drawing Ascendant Cards. </summary>
        public IPractitioner SetLocation(double latitude, double longitude)
        {
            currentPractitioner.SetLocation(latitude, longitude);

            //Return this for enabling method chaining.
            return this;
        }


        /// <summary> Retrieves the practitioner's Name Cards. The practitioner's name must have been set. </summary>
        public ImmutableArray<IArcanaCard> GetNameCards()
        {
            //Guard clause against name cards not having been set.
            if (currentPractitioner.NameCards is null)
            {
                //Instructional errors for teaching users how to use this API correctly.
                if (currentPractitioner.Names is null)
                    throw new Exception($"In order to retrieve correspondence cards, {nameof(Practitioner)} must have been configured using {nameof(SetName)}");

                return [];
            }

            ImmutableArray<IArcanaCard> output;

            List<IArcanaCard> aggregatedCards = [];
            var practitionerFirstNameCard = currentPractitioner.NameCards?.FirstNameArcana;
            var practitionerLastNameCard = currentPractitioner.NameCards?.LastNameArcana;
            var practitionerWholeNameCard = currentPractitioner.NameCards?.FullNameArcana;
            var pracitionerMiddleNameCards = currentPractitioner.NameCards?.MiddleNameArcanas;


            //Select and aggregate valid name cards on a case by case basis.
            if (practitionerFirstNameCard is not null) { aggregatedCards.Add(new ArcanaCard(practitionerFirstNameCard, ArcanaRole.FirstNameCard)); }
            if (practitionerLastNameCard is not null) { aggregatedCards.Add(new ArcanaCard(practitionerLastNameCard, ArcanaRole.LastNameCard)); }
            if (practitionerWholeNameCard is not null) { aggregatedCards.Add(new ArcanaCard(practitionerWholeNameCard, ArcanaRole.WholeNameCard)); }

            if (pracitionerMiddleNameCards is not null)
            {
                foreach (var card in pracitionerMiddleNameCards)
                {
                    aggregatedCards.Add(new ArcanaCard(card, ArcanaRole.MiddleNameCard));
                }
            }


            output = [.. aggregatedCards];
            return output;
        }

        /// <summary> Retrieves the practitioner's Personality Cards. The practitioner's birth date must have been set. </summary>
        public ImmutableArray<IArcanaCard> GetPersonalityCards()
        {
            //Guard clause against personality cards not having been set. If PersonalityCard is null, so should be the others.
            if (currentPractitioner.PersonalityCard is null)
            {
                //Instructional errors for teaching users how to use this API correctly.
                if (currentPractitioner.DateOfBirth is null)
                    throw new Exception($"In order to retrieve correspondence cards, {nameof(Practitioner)} must have been configured using {nameof(SetBirthdate)}");

                return [];
            }

            ImmutableArray<IArcanaCard> output;

            List<IArcanaCard> aggregatedCards = [];
            var personalityCard = currentPractitioner.PersonalityCard;
            var characterCard = currentPractitioner.TeacherCard;

            //Select and aggregate valid cards on a case by case basis.
            if (personalityCard is not null) { aggregatedCards.Add(new ArcanaCard(personalityCard, ArcanaRole.PersonalityCard)); }
            if (characterCard is not null) { aggregatedCards.Add(new ArcanaCard(characterCard, ArcanaRole.CharacterCard)); }

            output = [.. aggregatedCards];
            return output;
        }

        /// <summary> Retrieves the practitioner's Growth Cards. Can be configured to target specific years, relative to the current year. </summary>
        public ImmutableArray<IArcanaCard> GetGrowthCards(int yearsBeforeNow = 0, int yearsBeyondNow = 0)
        {
            ImmutableArray<IArcanaCard> output;

            DateTime? nativetyDate = currentPractitioner.DateOfBirth;
            List<IArcanaCard> aggregatedCards = [];
            int earliestYear = DateTime.Now.Year - yearsBeforeNow;
            int latestYear = DateTime.Now.Year + yearsBeyondNow;

            // Instructional errors for teaching users how to use this API correctly.
            if (nativetyDate is null)
                throw new Exception($"In order to retrieve growth cards, {nameof(Practitioner)} must have been configured using {nameof(SetBirthdate)}");
            else
                //Select and aggregate valid cards on a case by case basis.
                for (int i = earliestYear; i <= latestYear; i++)
                {
                    var growthCard = cardFetcher.GetGrowthCardByYear(i, (DateTime)nativetyDate);
                    aggregatedCards.Add(new ArcanaCard(growthCard, ArcanaRole.GrowthCard));
                }

            output = [.. aggregatedCards];
            return output;
        }

        /// <summary> Retrieves the practitioner's Zodiacal Cards. For the primary zodiacal Sun Sign, only the practitioner's birth date must have been set.
        /// For all of the possible relevant correspondence cards, both the practitioner's timezone and birth date must have been set. </summary>
        public ImmutableArray<ICorrespondence> GetCorrespondenceCards()
        {
            if (currentPractitioner.CelestialWheel is null)
            {
                if (currentPractitioner.DateOfBirth is null)
                    throw new Exception($"In order to retrieve correspondence cards, {nameof(Practitioner)} must have been configured using {nameof(SetBirthdate)}");

                return [];
            }

            ImmutableArray<ICorrespondence> output;
            List<ICorrespondence> aggregatedCards = [];

            var zodiacalSunKey = currentPractitioner.CelestialWheel?.ZodiacalSunSign;
            var risingSunKey = currentPractitioner.CelestialWheel?.RisingSunSign;
            var moonKey = currentPractitioner.CelestialWheel?.MoonSign;
            var mercuryKey = currentPractitioner.CelestialWheel?.MercurySign;
            var venusKey = currentPractitioner.CelestialWheel?.VenusSign;
            var jupiterKey = currentPractitioner.CelestialWheel?.JupiterSign;
            var saturnKey = currentPractitioner.CelestialWheel?.SaturnSign;


            //Select and aggregate valid cards on a case by case basis.
            if (zodiacalSunKey is not null) { aggregatedCards.Add(new Correspondence(zodiacalSunKey, CorrespondenceOption.ZodiacalSun)); }
            if (risingSunKey is not null) { aggregatedCards.Add(new Correspondence(risingSunKey, CorrespondenceOption.RisingSun)); }
            if (moonKey is not null) { aggregatedCards.Add(new Correspondence(moonKey, CorrespondenceOption.Moon)); }
            if (mercuryKey is not null) { aggregatedCards.Add(new Correspondence(mercuryKey, CorrespondenceOption.Mercury)); }
            if (venusKey is not null) { aggregatedCards.Add(new Correspondence(venusKey, CorrespondenceOption.Venus)); }
            if (jupiterKey is not null) { aggregatedCards.Add(new Correspondence(jupiterKey, CorrespondenceOption.Jupiter)); }
            if (saturnKey is not null) { aggregatedCards.Add(new Correspondence(saturnKey, CorrespondenceOption.Saturn)); }


            output = [.. aggregatedCards];
            return output;
        }

        public bool CheckWhetherZodiacalSunIsAccurate(DateTime birthDate)
        {
            SetBirthdate(birthDate);
            return astrologicalCalculator.CheckIfNearCusp(birthDate);
        }
    }
}
