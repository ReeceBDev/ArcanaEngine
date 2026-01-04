using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Thoth.Managers;
using Thoth.Types.Thoth.CardDataStructure;

namespace Thoth.Types.Practitioner
{
    /// <summary> Name-specific Arcane Cards which are calculated from transliterating the practitioner's names into Hebrew gematria. </summary>
    internal class NameArcana : INameArcana
    {
        public readonly ICardProvider cardFetcher;

        /// <summary> The arcana and archetypes related to the practitioner's first name. Represents the practitioner's inner character. </summary>
        public IArchetype? FirstNameArcana { get; private set; }

        /// <summary> Possibly several arcana and archetypes related to the practitioner's middle names. Represents additional family-related challenges for the practitioner. </summary>
        public List<IArchetype> MiddleNameArcanas { get; private set; } = [];

        /// <summary> The arcana and archetypes related to the practitioner's last name. Represents the practitioner in terms of their family. </summary>
        public IArchetype? LastNameArcana { get; private set; }

        /// <summary> The arcana and archetypes related to the practitioner's full and complete name. Represents a binding of the partial names under one whole. </summary>
        public IArchetype? FullNameArcana { get; private set; }

        public NameArcana(ICardProvider setCardFetcher)
        {
            cardFetcher = setCardFetcher;
        }

        public void SetFirstNameArcana(string firstName)
            => FirstNameArcana = ValidateAndGetArcana(firstName);

        public void SetLastNameArcana(string lastName)
            => LastNameArcana = ValidateAndGetArcana(lastName);

        public void SetFullNameArcana(string fullName)
            => FullNameArcana = ValidateAndGetArcana(fullName);
        
        public void SetMiddleNameArcanas(ImmutableArray<string> middleNames)
        {
            if (middleNames.IsDefaultOrEmpty)
                throw new ArgumentNullException($"{nameof(middleNames)} was null");

            // Clear the old collection of Arcanas.
            MiddleNameArcanas.Clear();

            // Generate the new Arcanas.
            foreach (var middleName in middleNames)
            {
                var arcana = ValidateAndGetArcana(middleName);
                MiddleNameArcanas.Add(arcana);
            }
        }

        private IArchetype ValidateAndGetArcana(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName))
                throw new ArgumentNullException($"{nameof(rawName)} was null or empty");

            if (Regex.IsMatch(rawName, @"[^a-zA-Z\s-]"))
                throw new ArgumentException($"{nameof(rawName)} may only contain letters, spaces, and hyphens.");

            // Remove hyphens and spaces from double-barrelled and multi-word names
            string hypenlessName = rawName.Replace("-", "");
            string cleanName = hypenlessName.Replace(" ", "");

            // Generate the arcana card
            return cardFetcher.GetCardByHebrewName(cleanName);
        }
    }
}
