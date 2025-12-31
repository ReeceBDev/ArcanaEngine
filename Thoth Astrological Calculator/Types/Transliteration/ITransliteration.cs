using System;
using System.Collections.Generic;
using System.Text;

namespace Thoth.Types.Transliteration
{
    internal interface ITransliteration
    {
        /// <summary> The symbolic character-by-character conversion of a practioner's names. This isn't phonetic, yet is the accurate basis for producing cross-sums. </summary>
        string[] TransliteratedNames { get; }

        /// <summary> The numerical representation of the characters. The data length should be therefore be identical to the transliterated names within this interface. </summary>
        int[] TransliteratedGemetria { get; }
    }
}
