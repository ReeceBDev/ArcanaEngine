using System;
using System.Collections.Generic;
using System.Text;
using Thoth.Types.Thoth;
using Thoth_Astrological_Calculator.Types.Thoth;

namespace Thoth.Types
{
    /// <summary> Name-specific Arcane Cards which are calculated from transliterating the practitioner's names into Hebrew gematria. </summary>
    internal interface INameArcana
    {
        /// <summary> The arcana and archetypes related to the practitioner's first name. Represents the practitioner's inner character. </summary>
        IArchetype? FirstNameArcana { get; }

        /// <summary> Possibly several arcana and archetypes related to the practitioner's middle names. Represents additional family-related challenges for the practitioner. </summary>
        IArchetype[]? MiddleNameArcanas { get; }

        /// <summary> The arcana and archetypes related to the practitioner's last name. Represents the practitioner in terms of their family. </summary>
        IArchetype? LastNameArcana { get; }

        /// <summary> The arcana and archetypes related to the practitioner's full and complete name. Represents a binding of the partial names under one whole. </summary>
        IArchetype? FullNameArcana { get; }
    }
}
