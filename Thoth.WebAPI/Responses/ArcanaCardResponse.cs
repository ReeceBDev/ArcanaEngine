using Thoth.External.Types;
using Thoth.Types;

namespace Thoth.WebAPI.Responses
{
    /// <summary> A serialisable representation of an <see cref="IArcanaCard"/>. </summary>
    /// <param name="Role"> The card's role within the reading, e.g. <see cref="ArcanaRole.PersonalityCard"/>. </param>
    /// <param name="Name"> The card's name. </param>
    /// <param name="Number"> The card's number: suit-relative for Minor Arcana (Ace = 1 .. Knight = 14), the Major Arcana index (0-21) for Major Arcana. </param>
    /// <param name="Suit"> The card's suit when it is a Minor Arcana card, e.g. <see cref="Suit.Wands"/>. Null when the card is a Major Arcana card. </param>
    internal record ArcanaCardResponse(string Role, string Name, int Number, string? Suit)
    {
        /// <summary> Maps an <see cref="IArcanaCard"/> to its serialisable response shape. </summary>
        internal static ArcanaCardResponse From(IArcanaCard card)
            => new(card.Role.ToString(), card.Name, card.Number, card.Suit?.ToString());
    }
}
