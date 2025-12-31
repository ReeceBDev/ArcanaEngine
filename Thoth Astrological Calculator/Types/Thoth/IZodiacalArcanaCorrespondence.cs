using System;
using System.Collections.Generic;
using System.Text;
using Thoth.Types.Thoth;
using Thoth_Astrological_Calculator.Types.Thoth;

namespace Thoth.Types.ThothArcana
{
    /// <summary> A set of Arcana which relate to a Zodiacal correspondence upon a practitioner's natevity. </summary>
    internal interface IZodiacalArcanaCorrespondence
    {
        /// <summary> The major arcana card which relates to this correspondence's Zodiac, and its archetypical relationships. </summary>
        IArchetype Zodiac { get; }

        /// <summary> The Decane card which relates to this correspondence, and its archetypical relationships. </summary>
        IArchetype Decan { get; }

        /// <summary> The Court card which relates to this correspondence, and its archetypical relationships. </summary>
        IArchetype Court { get; }
    }
}