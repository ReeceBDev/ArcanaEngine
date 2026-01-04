using System.Collections.Immutable;
using Thoth.External.Types;

namespace Thoth.External
{
    public interface IPractitionerService
    {
        /// <summary> Sets the practitioner's name. Allows Name Cards to be drawn. </summary>
        IPractitionerService SetName(string rawName);

        /// <summary> Sets the practitioner's birth date. Allows Personality Cards to be drawn, along with the Zodiacal Sun Correspondence Card. </summary>
        IPractitionerService SetBirthdate(DateTime birthDate);

        /// <summary> Sets the practitioner's exact birth time. This must adhere to its correct timezone. Allows most Correspondence Cards to be drawn. </summary>
        IPractitionerService SetBirthTime(DateTimeOffset birthTime);

        /// <summary> Sets the practitioner's location of their nativety. When set alongside an exact time of birth, allows drawing Ascendant Cards. </summary>
        IPractitionerService SetLocation(double latitude, double longitude);


        /// <summary> Retrieves the practitioner's Name Cards. The practitioner's name must have been set. </summary>
        ImmutableArray<IArcanaCard> GetNameCards();

        /// <summary> Retrieves the practitioner's Personality Cards. The practitioner's birth date must have been set. </summary>
        ImmutableArray<IArcanaCard> GetPersonalityCards();

        /// <summary> Retrieves the practitioner's Zodiacal Cards. For the primary zodiacal Sun Sign, only the practitioner's birth date must have been set.
        /// For all of the possible relevant correspondence cards, both the practitioner's timezone and birth date must have been set. </summary>
        ImmutableArray<ICorrespondenceKey> GetCorrespondenceCards();
    }
}
