using System.Text;
using Thoth.Types.Transliteration;

namespace Thoth.Resources.Calculators
{
    internal class GemetriaCalculator : IGemetriaCalculator
    {
        /// <summary> Attempts to perform numerology on latin names by casting their literal characters to hebrew apiece. This is an accepted practice, but not necessarily wholly accurate... </summary>
        public int GetGemetriaHebrewAppromimation(string name)
        {
            if (name is null)
                throw new ArgumentNullException("name");

            int outputNumbers;
            string assembledNumbers;
            StringBuilder sb = new();

            //First extract the double-letter combinations, in their proper order...
            for (int i = 0; i < name.Length; i++)
            {
                string testLetters;
                char currentLetter = name[i];
                char? additionalLetter = null;

                bool hasMoreThanOneLetter = name.Length > 1;
                bool hasExtraLetters = (name.Length - 1) >= i;

                //Extract the additional letter if there are enough remaining
                if (hasExtraLetters && hasMoreThanOneLetter)
                {
                    additionalLetter = name[i + 1];
                }

                testLetters = additionalLetter is null ? $"{currentLetter}" : $"{currentLetter + additionalLetter}";

                //Check the extracted letters
                if (Enum.IsDefined(typeof(LatinToHebrewNumerologyApproximations), testLetters))
                {
                    sb.Append(testLetters);

                    //Skip an additional letter when we had two which tested true at once.
                    if (testLetters.Length > 1)
                    {
                        i++;
                    }
                }
            }

            assembledNumbers = sb.ToString();
            outputNumbers = int.Parse(assembledNumbers);

            return outputNumbers;
        }

    }
}