using Thoth.Types.Thoth;
using Thoth.Types.Zodiacal;
using Thoth.Managers;
using Thoth.Resources.Calculators;

namespace Thoth.Types.Practitioner
{

    /// <summary> Collection of celestial correspondencies which relate to each of the celestial bodies. </summary>
    internal class CelestialWheel : ICelestialCorrespondences
    {
        private readonly ICardProvider cardFetcher;
        private readonly IAstrologicalCalculator astrologicalCalculator;

        private DateTime? dateOfBirth;
        private DateTimeOffset? birthTime;
        private (double latitude, double longitude)? birthLocation;

        /// <summary> The primary sun sign's zodiacal correspondence. Represents the internal, describes the practitioner's inner characteristics. </summary>
        public IZodiacalArcanaCorrespondence? ZodiacalSunSign { get; set; }

        /// <summary> The secondary rising sun sign's zodiacal correspondence. Represents the external, how a practitioner behaves and is perceived by others. </summary>
        public IZodiacalArcanaCorrespondence? RisingSunSign { get; set; }

        /// <summary> The tertiary sign's zodiacal correspondence. Contrasts the sun. May present lacking characteristics for a practitioner. </summary>
        public IZodiacalArcanaCorrespondence? MoonSign { get; set; }

        /// <summary> Mercury's zodiacal correspondence. </summary>
        public IZodiacalArcanaCorrespondence? MercurySign { get; set; }

        /// <summary> Venus' zodiacal correspondence. </summary>
        public IZodiacalArcanaCorrespondence? VenusSign { get; set; }

        /// <summary> The zodiacal correspondence of Mars. </summary>
        public IZodiacalArcanaCorrespondence? MarsSign { get; set; }

        /// <summary> The zodiacal correspondence of Jupiter. </summary>
        public IZodiacalArcanaCorrespondence? JupiterSign { get; set; }

        /// <summary> The zodiacal correspondence of Saturn. </summary>
        public IZodiacalArcanaCorrespondence? SaturnSign { get; set; }

        public CelestialWheel(ICardProvider setCardFetcher, IAstrologicalCalculator setAstrologicalCalculator, DateTime birthDate)
        {
            cardFetcher = setCardFetcher;
            astrologicalCalculator = setAstrologicalCalculator;

            SetBirthDate(birthDate);
        }

        public CelestialWheel(ICardProvider setCardFetcher, IAstrologicalCalculator setAstrologicalCalculator, DateTime birthDate, DateTimeOffset birthTime)
            : this(setCardFetcher, setAstrologicalCalculator, birthDate)
        {
            SetBirthTime(birthTime);
        }

        public CelestialWheel(ICardProvider setCardFetcher, IAstrologicalCalculator setAstrologicalCalculator, DateTime birthDate, DateTimeOffset birthTime,
            double latitude, double longitude) : this(setCardFetcher, setAstrologicalCalculator, birthDate, birthTime)
        {
            SetLocation(latitude, longitude);
        }

        public void SetBirthDate(DateTime birthDate)
        {
            dateOfBirth = birthDate;
            RefreshAllSigns();
        }

        public void SetBirthTime(DateTimeOffset setBirthTime)
        {
            birthTime = setBirthTime;
            RefreshAllSigns();
        }

        public void SetLocation(double latitude, double longitude)
        {
            birthLocation = (latitude, longitude);
            RefreshAllSigns();
        }

        private IZodiacalArcanaCorrespondence GenerateSunSignByDate()
        {
            if (!dateOfBirth.HasValue)
                throw new InvalidOperationException($"{nameof(dateOfBirth)} was null! Cannot generate sun sign.");

            IEclipticDegree absoluteDegree;

            // Try to generate the most accurate celestial degree we have available for the sun sign
            absoluteDegree = birthTime.HasValue ? 
                absoluteDegree = astrologicalCalculator.GetCelestialPositionByTime(CelestialBody.Sun, birthTime.Value) :
                absoluteDegree = astrologicalCalculator.GetZodiacalSunDegree(dateOfBirth.Value);

            return new ZodiacalCorrespondence(cardFetcher, absoluteDegree);
        }

        private IZodiacalArcanaCorrespondence GenerateSignByTime(CelestialBody body)
        {
            if (!birthTime.HasValue)
                throw new InvalidOperationException($"{nameof(birthTime)} was null! Cannot generate {body} sign.");

            IEclipticDegree absoluteDegree = astrologicalCalculator.GetCelestialPositionByTime(body, birthTime.Value);
            return new ZodiacalCorrespondence(cardFetcher, absoluteDegree);
        }

        private IZodiacalArcanaCorrespondence GenerateAscendantSign()
        {
            if (!birthTime.HasValue || !birthLocation.HasValue)
                throw new InvalidOperationException("Birth time and location required to generate ascendant sign.");

            IEclipticDegree absoluteDegree = astrologicalCalculator.GetAscendantByTime(birthTime.Value, birthLocation.Value.latitude, birthLocation.Value.longitude);
            return new ZodiacalCorrespondence(cardFetcher, absoluteDegree);
        }

        private void RefreshAllSigns()
        {
            if (!dateOfBirth.HasValue)
                throw new InvalidOperationException($"{nameof(dateOfBirth)} was not set! Cannot generate any signs at all! Ensure this has been set, upon this class's instantiation!");

            if (!birthTime.HasValue)
            {
                //Generate signs using only the practitioner's birth date, which may lack accuracy upon some edge cases, so only use sparingly, i.e. for the zodiac.
                ZodiacalSunSign = GenerateSunSignByDate();
                return;
            }

            //Generate all available signs accurately using the practitioner's birth time.
            ZodiacalSunSign = GenerateSignByTime(CelestialBody.Sun);
            MoonSign = GenerateSignByTime(CelestialBody.Moon);
            MercurySign = GenerateSignByTime(CelestialBody.Mercury);
            VenusSign = GenerateSignByTime(CelestialBody.Venus);
            MarsSign = GenerateSignByTime(CelestialBody.Mars);
            JupiterSign = GenerateSignByTime(CelestialBody.Jupiter);
            SaturnSign = GenerateSignByTime(CelestialBody.Saturn);

            if (birthLocation.HasValue)
            {
                //Generate signs which use the practitioner's birth location.
                RisingSunSign = GenerateAscendantSign();
            }
        }
    }
}