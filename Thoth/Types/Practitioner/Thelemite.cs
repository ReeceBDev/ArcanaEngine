using System.Collections.Immutable;
using Thoth.Managers;
using Thoth.Resources;
using Thoth.Resources.Calculators;
using Thoth.Types.Thoth;
using Thoth.Types.Transliteration;

namespace Thoth.Types.Practitioner
{
    internal class Thelemite : IThelemite
    {
        private readonly ICardProvider cardFetcher;
        private readonly IAstrologicalCalculator astrologicalCalculator;
        private readonly IThothCalculator thothCalculator;

        /// <summary> The names of the practitioner in order from first to last. </summary>
        public ImmutableArray<string>? Names { get; private set; }


        /// <summary> A practitioner's date of birth. </summary>
        public DateTime? DateOfBirth { get; private set; }

        /// <summary> A practitioner's precise time of birth. Must be accurate to the hour and include a timezone. Prefers at least an accuracy of ~30 minutes or better. </summary>
        public DateTimeOffset? TimeOfBirth { get; private set; }

        /// <summary> A practitioner's general location at the time of their birth. Must be accurate to their time zone at the time. </summary>
        public (double latitude, double longitude)? LocationOfBirth { get; private set; }


        /// <summary> Arcana and archetypical represention of a practitioner's personality. </summary>
        public IArchetype? PersonalityCard { get; private set; }

        /// <summary> Arcana and archetypical represention of a practitioner's character. </summary>
        public IArchetype? TeacherCard { get; private set; }

        /// <summary> Growth cards are cyclical. To represent a greater count, an offset is cached instead of a particular growth card.
        /// This is an offset representing the practitioner's growth relative to the infinite 22-count growth card cycle. </summary>
        public int? GrowthCardOffset { get; private set; }


        /// <summary> Name-specific Arcane Cards which are calculated from transliterating the practitioner's names into Hebrew gematria. </summary>
        public INameArcana? NameCards { get; private set; }

        /// <summary> Transliterated hewbrew symbols from latin, which acts as the letter-by-letter basis which is then in turn used to produce a practitioner's Name Cards.</summary>
        public ITransliteration? HebrewGematria { get; private set; }


        /// <summary> The practitioner's own celestial wheel configuration at the time of their nativety. </summary>
        public ICelestialCorrespondences? CelestialWheel { get; private set; }

        /// <summary> The absolute ecliptic degree at the practitioner's nativety, used around the Celestial Wheel. </summary>
        public int? AbsoluteEclipticDegree { get; private set; }

        public Thelemite(ICardProvider setCardFetcher, IAstrologicalCalculator setAstrologicalCalculator, IThothCalculator setThothCalculator)
        {
            cardFetcher = setCardFetcher;
            astrologicalCalculator = setAstrologicalCalculator;
            thothCalculator = setThothCalculator;
        }

        public IThelemite SetName(string fullName)
        {
            // Guard clause to return early when we have no name.
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return this;
            }

            ImmutableArray<string> delimitedNames = TextUtilities.DelimitText(fullName, ' ');
            Names = delimitedNames;

            RefreshNameArcana();

            // Returns this to enable method chaining syntactically.
            return this;
        }

        public IThelemite SetDateOfBirth(DateTime dateOfBirth)
        {
            DateOfBirth = dateOfBirth;

            //Regenerate all cards which require Date of Birth as a prerequisite.
            RefreshCelestialWheel();
            RefreshPersonalityCards();

            // Returns this to enable method chaining syntactically.
            return this;
        }

        public IThelemite SetTimeOfBirth(DateTimeOffset exactBirthTime)
        {
            TimeOfBirth = exactBirthTime;
            RefreshCelestialWheel();

            // Returns this to enable method chaining syntactically.
            return this;
        }

        public IThelemite SetLocation(double latitude, double longitude)
        {
            LocationOfBirth = (latitude, longitude);
            RefreshCelestialWheel();

            // Returns this to enable method chaining syntactically.
            return this;
        }

        private void RefreshPersonalityCards()
        {
            // Guard clause against a missing date of birth value. Personality cards require the Practitioner's date of nativety to function.
            if (!DateOfBirth.HasValue)
                return;

            PersonalityCard = cardFetcher.GetPersonalityCard(DateOfBirth.Value);
            TeacherCard = cardFetcher.GetTeacherCard(DateOfBirth.Value);
            GrowthCardOffset = thothCalculator.CalculateGrowthOffset(DateOfBirth.Value, DateTime.Now.Year);
        }

        private void RefreshCelestialWheel()
        {
            // Guard clause against a missing date of birth value. The celestial wheel requires at least a date of birth to function.
            if (!DateOfBirth.HasValue)
                return;

            // Generate the Celestial Wheel if it does not already exist.
            if (CelestialWheel == null)
            {
                CelestialWheel = new CelestialWheel(cardFetcher, astrologicalCalculator, DateOfBirth.Value);
            }

            // Update the Celestial Wheel with the Practioner's nativety time when it is available.
            if (TimeOfBirth.HasValue)
            {
                CelestialWheel.SetBirthTime(TimeOfBirth.Value);
            }

            // Update the Celestial Wheel with the Practioner's nativety location when it is available.
            if (LocationOfBirth.HasValue)
            {
                CelestialWheel.SetLocation(LocationOfBirth.Value.latitude, LocationOfBirth.Value.longitude);
            }
        }

        private void RefreshNameArcana()
        {
            // Guard clause to return early when this Practitioner has no name.
            if (Names is not ImmutableArray<string> array || array.IsDefaultOrEmpty)
                return;

            // Generate a names collection if one does not already exist, since we will now be popuplating it...
            NameCards ??= new NameArcana(cardFetcher);

            // Refresh each name arcana
            RefreshFirstNameArcana();
            RefreshMiddleNameArcana();
            RefreshLastNameArcana();
            RefreshFullNameArcana();
        }

        private void RefreshFirstNameArcana()
        {
            if (Names is not ImmutableArray<string> names || names.IsDefaultOrEmpty)
                return;

            var firstName = names.First();
            NameCards ??= new NameArcana(cardFetcher);

            NameCards.SetFirstNameArcana(firstName);
        }

        private void RefreshLastNameArcana()
        {
            if (Names is not ImmutableArray<string> names || names.Length < 2)
                return;

            var lastName = names.Last();
            NameCards ??= new NameArcana(cardFetcher);

            NameCards.SetLastNameArcana(lastName);
        }

        private void RefreshMiddleNameArcana()
        {
            if (Names is not ImmutableArray<string> names || names.Length < 3)
                return;

            ImmutableArray<string> middleNames = names[1..^1];
            NameCards ??= new NameArcana(cardFetcher);

            NameCards.SetMiddleNameArcanas(middleNames);
        }

        private void RefreshFullNameArcana()
        {
            if (Names is not ImmutableArray<string> names || names.IsDefaultOrEmpty)
                return;

            string fullName = string.Join(" ", names);
            NameCards ??= new NameArcana(cardFetcher);

            NameCards.SetFullNameArcana(fullName);
        }
    }
}