using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Thoth.External.Types;
using Thoth.Resources;

namespace Thoth
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var practitioner = Practitioner.Create();
            ImmutableArray<IArcanaCard> personalityCards;
            ImmutableArray<IArcanaCard> nameCards;


            practitioner.SetBirthdate(ReadBirthDate());
            practitioner.SetName(ReadName());


            personalityCards = practitioner.GetPersonalityCards();
            nameCards = practitioner.GetNameCards();

            PrintCardsToConsole(personalityCards, nameCards);


            Console.WriteLine("Finished all operations.");
            PauseConsole();
        }

        static DateTime ReadBirthDate()
        {
            DateTime output;

            string rawBirthDate = string.Empty;
            ImmutableArray<string> birthDateStrings;
            int[] birthDateNumbers = new int[3];
            Regex validBirthDate = new Regex("^(0[1-9]|[12][0-9]|3[0-2])/(0[1-9]|1[0-2])/\\d+$");


            while (!validBirthDate.IsMatch(rawBirthDate))
            {
                Console.WriteLine("Enter Birthdate dd/mm/yyyy");
                rawBirthDate = Console.ReadLine() ?? string.Empty;
            }

            birthDateStrings = TextUtilities.DelimitText(rawBirthDate, '/');

            for (var i = 0; i < birthDateStrings.Length; i++)
            {
                birthDateNumbers[i] = int.Parse(birthDateStrings[i]);
            }

            output = new DateTime(birthDateNumbers[2], birthDateNumbers[1], birthDateNumbers[0]);
            return output;
        }

        static string ReadName()
        {
            string output;
            string fullName = string.Empty;

            while (fullName != string.Empty)
            {
                Console.WriteLine("Enter Birthdate dd/mm/yyyy");
                fullName = Console.ReadLine() ?? string.Empty;
            }

            output = fullName;
            return output;
        }

        static void PrintCardsToConsole(params ImmutableArray<IArcanaCard>[] setsOfCards)
        {
            foreach (ImmutableArray<IArcanaCard> set in setsOfCards)
            {
                foreach (IArcanaCard card in set)
                {
                    Console.WriteLine(card.Role.ToString());
                }
            }
        }

        static void PauseConsole()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
