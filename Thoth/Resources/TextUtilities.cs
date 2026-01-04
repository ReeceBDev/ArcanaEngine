using System.Collections.Immutable;
using System.Text;

namespace Thoth.Resources
{
    internal static class TextUtilities
    {
        public static string SanitizeInput(string rawInput)
        {
            string sanitizedText;
            StringBuilder sb = new StringBuilder();

            rawInput = rawInput.Trim();

            foreach (char i in rawInput)
            {
                //Ensure the string consists only of letters and ordinary spaces.
                if (char.IsLetter(i) || i == ' ')
                {
                    sb.Append(i);
                }
            }

            sanitizedText = sb.ToString();

            return sanitizedText;
        }

        public static ImmutableArray<string> DelimitText(string input, char delimiter)
            => input.Split([delimiter]).ToImmutableArray();
    }
}
