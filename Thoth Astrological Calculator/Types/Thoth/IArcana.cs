using System;
using System.Collections.Generic;
using System.Text;
using Thoth.Types.Thoth;

namespace Thoth_Astrological_Calculator.Types.Thoth
{
    /// <summary> An Arcana as represented by its card. </summary>
    internal interface IArcana
    {
        /// <summary> The deck's category which this Arcana sits within. </summary>
        ArcanicCategory Categorization { get; }

        /// <summary> The number of this Arcana. This is best ultimately displayed as roman numerals when this card is a Major Arcana. </summary>
        int Number { get; }

        /// <summary> The name of this Arcana. </summary>
        string Name { get; }

        /// <summary> A short description of this Arcana. </summary>
        string ShortDescription { get; }
    }
}

