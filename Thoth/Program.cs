using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Thoth.External;
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
            ImmutableArray<IArcanaCard> zodiacalSunCards;
            ImmutableArray<IArcanaCard> nameCards = [];
            var birthDate = ReadBirthDate();

            // Set practitioner up. Only birth-date is necessary, the rest are optional.
            practitioner.SetBirthdate(birthDate);
            //practitioner.SetName(ReadName());

            // Get and display cards
            personalityCards = practitioner.GetPersonalityCards();
            //nameCards = practitioner.GetNameCards();

            zodiacalSunCards = practitioner.GetCorrespondenceCards()
               .Where(i => i.Role == CorrespondenceOption.ZodiacalSun)
                .SelectMany(i => ImmutableArray.Create(i.Court, i.Decan, i.Zodiac))
                .ToImmutableArray();

            PrintCardsToConsole(personalityCards, nameCards, zodiacalSunCards);

            // Optionally, check whether the date of birth was sufficient on its own to get an accurate zodiacal sun sign.
            PerformCuspCheck(practitioner, birthDate);

            Console.WriteLine("\n\nFinished all operations.");
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
                Console.WriteLine("\nEnter Birthdate dd/mm/yyyy");
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

            while (fullName == string.Empty)
            {
                Console.WriteLine("\nEnter Full Name (First) (Middle) (etc.)");
                fullName = Console.ReadLine() ?? string.Empty;
            }

            output = fullName;
            return output;
        }

        static void PrintCardsToConsole(params ImmutableArray<IArcanaCard>[] setsOfCards)
        {
            Console.WriteLine("\n\nCards: \n");

            foreach (ImmutableArray<IArcanaCard> set in setsOfCards)
            {
                foreach (IArcanaCard card in set)
                {
                    Console.WriteLine($"""
                        
                         ----------------- 
                        
                        I present your: 
                        	{card.Role.ToString()}

                        Introducing: 
                          {card.Number} - {card.Name.ToString()}
                        
                         ----------------- 
                        
                        """);
                }
            }
        }

        static void PerformCuspCheck(IPractitioner practitioner, DateTime birthDate)
        {
            // Check that they are valid
            if (!practitioner.CheckWhetherZodiacalSunIsAccurate(birthDate))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                Console.WriteLine("\n Warning: Your date of birth is near the cusp of a zodiacal change." +
                    "\n Your birth-date cards have adjacent days with different results." +
                    "\n Please enter your date of birth so that more accurate results will be obtained.");

                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static void PauseConsole()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
