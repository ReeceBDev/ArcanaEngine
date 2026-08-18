using System.Collections.Immutable;
using Thoth.External.Types;

namespace Thoth.External
{
    public interface IPractitioner
    {
        /// <summary> Sets the practitioner's name. Allows Name Cards to be drawn. </summary>
        IPractitioner SetName(string rawName);

        /// <summary> Sets the practitioner's birth date. Allows Personality Cards to be drawn, along with the Zodiacal Sun Correspondence Card. </summary>
        IPractitioner SetBirthdate(DateTime birthDate);

        /// <summary> Sets the practitioner's exact birth time. This must adhere to its correct timezone. Allows most Correspondence Cards to be drawn. </summary>
        IPractitioner SetBirthTime(DateTimeOffset birthTime);

        /// <summary> Sets the practitioner's location of their nativety. When set alongside an exact time of birth, allows drawing Ascendant Cards. </summary>
        IPractitioner SetLocation(double latitude, double longitude);


        /// <summary> Retrieves the practitioner's Name Cards. The practitioner's name must have been set. </summary>
        ImmutableArray<IArcanaCard> GetNameCards();

        /// <summary> Retrieves the practitioner's Personality Cards. The practitioner's birth date must have been set. </summary>
        ImmutableArray<IArcanaCard> GetPersonalityCards();

        /// <summary> Retrieves the practitioner's Zodiacal Cards. For the primary zodiacal Sun Sign, only the practitioner's birth date must have been set.
        /// For all of the possible relevant correspondence cards, both the practitioner's timezone and birth date must have been set. </summary>
        ImmutableArray<ICorrespondence> GetCorrespondenceCards();

        /// <summary> Retrieves the practitioner's House Cards — the twelve fixed houses of their natal celestial wheel, each with its cusp sign, degree and related arcana.
        /// Both the practitioner's exact birth time and birth location must have been set. </summary>
        ImmutableArray<IHouseCorrespondence> GetHouseCorrespondences();

        /// <summary> Retrieves the practitioner's Growth Cards. The practitioner's birth date must have been set. Can be configured to target specific years, before and beyond the target year. </summary>
        public ImmutableArray<IArcanaCard> GetGrowthCards(int year, int yearsBefore = 0, int yearsBeyond = 0);


        /// <summary> Sets Birth Date and checks it for accuracy. Returns true when the zodiacal sun sign is accurate based on this birthdate. 
        /// Returns false when a more specific BirthTime is required, due to the birth date being near the cusp of a change. </summary>
        public bool CheckWhetherZodiacalSunIsAccurate(DateTime birthDate);
    }
}
