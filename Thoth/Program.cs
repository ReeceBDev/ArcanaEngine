using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Threading;
using Thoth.External;
using Thoth.External.InternalConcreteDependencies;
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
            DateTime birthDate = ReadBirthDate();

            // Set practitioner up. Only birth-date is necessary, the rest are optional.
            practitioner.SetBirthdate(birthDate);
            practitioner.SetName(ReadName());


            // Get and display cards
            personalityCards = practitioner.GetPersonalityCards();
            nameCards = practitioner.GetNameCards();

            zodiacalSunCards = practitioner.GetCorrespondenceCards()
               .Where(i => i.Role == CorrespondenceOption.ZodiacalSun)
                .SelectMany(i => ImmutableArray.Create(i.Court, i.Decan, i.Zodiac))
                .ToImmutableArray();

            PrintCardsToConsole(personalityCards, nameCards, zodiacalSunCards);


            // Optionally, check whether the date of birth was sufficient on its own to get an accurate zodiacal sun sign.
            PerformCuspCheck(practitioner, birthDate);

            // Optionally, request the user's birth Time for additional correspondence cards.
            practitioner.SetBirthTime(GetBirthTime(birthDate));
            PrintDetailedCorrespondenceCards(practitioner, birthDate);

            Console.WriteLine($"\n\nThe letters in the hebrew alphabet have three - fold meanings, as letters, as numbers, as several various symbols..." +
                             " ~ I believe there may be alternative latin to hebrew gemetria techniques that may be explored later. ~");


            Console.WriteLine("\n\nFinished all operations.");
            PauseConsole();
        }

        static void PrintDetailedCorrespondenceCards(IPractitioner practitioner, DateTime birthDate)
        {
            var birthCorrespondences = practitioner.GetCorrespondenceCards();

            foreach (ICorrespondence correspondence in birthCorrespondences)
            {
                ImmutableArray<IArcanaCard> cards = [correspondence.Zodiac, correspondence.Court, correspondence.Decan];

                Console.WriteLine($"\n{correspondence.Role} Cards: ");
                PrintCardsToConsole(cards); 
            }
        }

        static DateTimeOffset GetBirthTime(DateTime birthDate)
        {
            int birthHour;
            int birthMinutes;
            string rawBirthTime = string.Empty;
            TimeSpan timeZone;
            Regex validBirthTime = new Regex("^(?:[01]\\d|2[0-3]):[0-5]\\d$");

            Console.WriteLine("\n\n Would you like to enter your Time of birth to get more correspondence cards?");
            

            while (!validBirthTime.IsMatch(rawBirthTime))
            {
                Console.WriteLine("\nEnter Birth Time hh:mm");
                rawBirthTime = Console.ReadLine() ?? string.Empty;
            }

            var parts = rawBirthTime.Split(':');
            birthHour = int.Parse(parts[0]);
            birthMinutes = int.Parse(parts[1]);

            timeZone = SelectTimezone();

            return new DateTimeOffset(birthDate.Year, birthDate.Month, birthDate.Day, birthHour, birthMinutes, 0, timeZone);
        }

        static TimeSpan SelectTimezone()
        {
            var zones = TimeZoneInfo.GetSystemTimeZones();
            for (int i = 0; i < zones.Count; i++)
                Console.WriteLine($"{i}: {zones[i].DisplayName}");

            TimeZoneInfo selected;
            while (true)
            {
                Console.Write("Select a number: ");
                if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < zones.Count)
                {
                    selected = zones[index];
                    break;
                }
                Console.WriteLine("Invalid selection, try again.");
            }

            Console.WriteLine($"Selected: {selected.Id}");
            return selected.BaseUtcOffset;
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
            Regex invalidC = new Regex("C(?!H)", RegexOptions.IgnoreCase);

            while (fullName == string.Empty)
            {
                Console.WriteLine("\nEnter Full Name (First) (Middle) (etc.)");
                fullName = Console.ReadLine() ?? string.Empty;

                if (invalidC.IsMatch(fullName))
                {
                    fullName = string.Empty;

                    Console.WriteLine("""
                        Apologies, but, the letter 'C' is not viable for converting to hebrew, unless it is in the letter pair 'CH'. Adhere to the following instructions:'
                        
                            - Usually when the letter 'C' appears in your name, it must be turned into the letter 'K' or the letter 'Z'.
                            - This should be done based on its objective 'Hardness', the quality of the sound you make as you say it.
                            
                            - Abrupt sounds like the 'Kuh' sound in "Cat", should be written as the letter K.
                            - Soft sounds like the 'Sss' sound in "Spice" should be written as the letter Z.

                        For example, the name "Clarice" pronounced klah-rhys should be entered as "Klarize".

                            - There is one exception to this rule:
                              Never change the 'C' when it is directly followed by the letter 'H', as in 'CH'!
                              
                        For example, if your name is 'Charlie' then it should remain as 'Charlie', always!
                        """);
                }
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
                    Console.WriteLine($"""
                        ~----~ I present your: ~----~ {card.Role}
                            Introducing: {card.Number} - {card.Name}

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
