using System.Collections.Immutable;
using Thoth.Types.Thoth.CardDataStructure;
using Thoth.Types.Transliteration;

namespace Thoth.Types.Practitioner
{
    internal interface IThelemite
    {
        /// <summary> The names of the practitioner in order from first to last. </summary>
        ImmutableArray<string>? Names { get; }


        /// <summary> A practitioner's date of birth. </summary>
        DateTime? DateOfBirth { get; }

        /// <summary> A practitioner's precise time of birth. Must be accurate to the hour and include a timezone. Prefers at least an accuracy of ~30 minutes or better. </summary>
        DateTimeOffset? TimeOfBirth { get; }

        /// <summary> A practitioner's general location at the time of their birth. Must be accurate to their time zone at the time. </summary>
        (double latitude, double longitude)? LocationOfBirth { get; }


        /// <summary> Arcana and archetypical represention of a practitioner's personality. </summary>
        IArchetype? PersonalityCard { get; }

        /// <summary> Arcana and archetypical represention of a practitioner's character. </summary>
        IArchetype? TeacherCard { get; }

        /// <summary> Growth cards are cyclical. To represent a greater count, an offset is cached instead of a particular growth card.
        /// This is an offset representing the practitioner's growth relative to the infinite 22-count growth card cycle. </summary>
        int? GrowthCardOffset { get; }


        /// <summary> Name-specific Arcane Cards which are calculated from transliterating the practitioner's names into Hebrew gematria. </summary>
        INameArcana? NameCards { get; }

        /// <summary> Transliterated hewbrew symbols from latin, which acts as the letter-by-letter basis which is then in turn used to produce a practitioner's Name Cards.</summary>
        ITransliteration? HebrewGematria { get; }


        /// <summary> The practitioner's own celestial wheel configuration at the time of their nativety. </summary>
        ICelestialCorrespondences? CelestialWheel { get; }

        /// <summary> The absolute ecliptic degree at the practitioner's nativety, used around the Celestial Wheel. </summary>
        int? AbsoluteEclipticDegree { get; }


        IThelemite SetName(string fullName);
        IThelemite SetDateOfBirth(DateTime dateOfBirth);
        IThelemite SetTimeOfBirth(DateTimeOffset exactBirthTime);
        IThelemite SetLocation(double latitude, double longitude);
    }
}