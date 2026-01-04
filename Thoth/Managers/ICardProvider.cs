using Thoth.Types.Thoth.CardDataStructure;
using Thoth.Types.Zodiacal;

namespace Thoth.Managers
{
    internal interface ICardProvider
    {
        /// <summary> Returns a Growth Card by a given year and the Practitioner's nativety date. </summary>
        IArchetype GetGrowthCardByYear(int year, DateTime nativetyDate);

        /// <summary> Returns a card by its name via its numerological interpretation of its symbol-by-symbol hewbrew transliteration. </summary>
        IArchetype GetCardByHebrewName(string name);

        /// <summary> Calculates a Practitioner's Teacher/Soul/Character Card by their Nativety date. This will return null when the Teacher Card is identical to the Personality Card.
        /// This is the mediating intelligence between the being (Zodiacal Sun Card) and guides the Personality towards it (and so it is aptly named as Teacher.)
        /// Note: If the Practitioner does indeed get no Teacher Card. then their 'teacher' is considered "internal" rather than "external".  </summary>
        IArchetype? GetTeacherCard(DateTime birthDate);

        /// <summary> Calculates a Practitioner's Personality Card by their Nativety date. Represents a persons flavour, their aspect with which they express their Being (Zodiacal Sun Card.) </summary>
        IArchetype GetPersonalityCard(DateTime birthDate);

        /// <summary> Represents a zodiacal representation of a side of the Practitioner's soul, i.e. internal, external, how they relate to people, etc. 
        /// When in its innermost concentric sun form, this would be analogous to the colour of the Practitioner - their inner essence, drive and being. </summary>
        IArchetype GetZodiacCard(int absoluteDegree);

        /// <summary> Calculates a Practitioner's Decan based on their absolute eliptic degree. </summary>
        IArchetype GetDecanCard(int absoluteDegree);

        /// <summary> Calculates a Practitioner's Court Card based on their absolute eliptic degree. </summary>
        IArchetype GetCourtCard(int absoluteDegree);
    }
}
