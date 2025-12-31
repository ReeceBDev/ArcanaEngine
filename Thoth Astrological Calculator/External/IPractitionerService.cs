using System;
using System.Collections.Generic;
using System.Text;
using Thoth_Astrological_Calculator.External.Types;
using Thoth_Astrological_Calculator.Types.Thoth;

namespace Thoth_Astrological_Calculator.External
{
    public interface IPractitionerService
    {
        /// <summary> Returns an empty practitioner. The practitioner's name or birthdate must be set before values can be pulled. </summary>
        IPractitionerService Empty { get; }


        /// <summary> Returns an empty practitioner. The practitioner's name or birthdate must be set before values can be pulled. </summary>
        IPractitionerService Create();
        
        /// <summary> Returns a practitioner with their name set. This allows Name Cards to be pulled. </summary>
        IPractitionerService Create(string rawName);

        /// <summary> Returns a practitioner with their birth date set. This allows Personality Cards and the Sun Sign from Correspondence Cards to be pulled. </summary>
        IPractitionerService Create(DateTime birthDate);

        /// <summary> Returns a practitioner with their birth date and timezone set. This allows Personality Cards and all Correspondence Cards to be pulled. </summary>
        IPractitionerService Create(DateTime birthDate, TimeZoneInfo birthTimeZone);

        /// <summary> Returns a practitioner with their name and birth date set. This allows Name Cards, Personality Cards and the Sun Sign from Correspondence Cards to be pulled. </summary>
        IPractitionerService Create(string rawName, DateTime birthDate);

        /// <summary> Returns a fully initiailised practitioner. This allows all cards to be pulled. </summary>
        IPractitionerService Create(string rawName, DateTime birthDate, TimeZoneInfo birthTimeZone);


        /// <summary> Sets the practitioner's name. Allows Name Cards to be drawn. </summary>
        void SetName(string rawName);

        /// <summary> Sets the practitioner's birth date. Allows Personality Cards to be drawn. </summary>
        void SetBirthdate(DateTime birthDate);

        /// <summary> Sets the practitioner's timezone at their time and location of their nativety. When set alongside a birthdate, allows Correspondence Cards to be drawn. </summary>
        void SetBirthTimezone(TimeZoneInfo birthTimeZone);


        /// <summary> Retrieves the practitioner's Name Cards. The practitioner's name must have been set. </summary>
        ICard[] GetNameCards();

        /// <summary> Retrieves the practitioner's Personality Cards. The practitioner's birth date must have been set. </summary>
        ICard[] GetPersonalityCards();

        /// <summary> Retrieves the practitioner's Zodiacal Cards. For the primary zodiacal Sun Sign, only the practitioner's birth date must have been set.
        /// For all of the possible relevant correspondence cards, both the practitioner's timezone and birth date must have been set. </summary>
        ICard[] GetCorrespondenceCards();
    }
}
