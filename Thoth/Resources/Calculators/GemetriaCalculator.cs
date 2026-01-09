using System.Linq;
using System.Text;
using Thoth.Types.Transliteration;

namespace Thoth.Resources.Calculators
{
    internal class GemetriaCalculator : IGemetriaCalculator
    {
        /// <summary> Attempts to perform numerology on latin names by casting their literal characters to hebrew apiece. This is an accepted practice, but not necessarily wholly accurate... </summary>
        public int GetGemetriaHebrewAppromimation(string rawName)
        {
            if (rawName is null)
                throw new ArgumentNullException(nameof(rawName));

            List<int> gemetriaValues = [];
            string name = rawName.ToUpperInvariant();


            // Extract the double-letter and remaining single-letter combinations, in their proper order...
            for (int i = 0; i < name.Length; i++)
            {
                string testLetters;
                char currentLetter = name[i];
                char? additionalLetter = null;
                int gemetriac;

                bool hasExtraLetters = (i + 1) < name.Length;

                //Extract the additional letter when there are enough remaining
                if (hasExtraLetters)
                {
                    additionalLetter = name[i + 1];
                }

                testLetters = additionalLetter is null ? $"{currentLetter}" : $"{currentLetter}{additionalLetter}";

                //Convert the extracted letters to numerics
                if (Enum.IsDefined(typeof(LatinToHebrewNumerologyApproximations), testLetters))
                {
                    //Handle two valid letters, when merged into one applicable grouping
                    gemetriac = (int)Enum.Parse<LatinToHebrewNumerologyApproximations>(testLetters);
                }
                else if (Enum.IsDefined(typeof(LatinToHebrewNumerologyApproximations), testLetters[0].ToString()))
                {
                    //Handle just one valid letter
                    gemetriac = (int)Enum.Parse<LatinToHebrewNumerologyApproximations>([testLetters[0]]);
                }
                else 
                {
                    //Handle no valid letters
                    throw new ArgumentException($"Character '{i}' has no Hebrew gematria mapping.", nameof(name));
                }

                gemetriaValues.Add(gemetriac);

                //Skip an additional letter when we had two which tested true at once.
                if (testLetters.Length > 1)
                {
                    i++;
                }
            }

            // Add the values together
            return gemetriaValues.Sum();
        }
    }
}