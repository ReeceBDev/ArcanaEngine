namespace Thoth.External.Types
{
    public interface IArcanaCard
    {
        ArcanaRole Role { get; }
        string Name { get; }
        int Number { get; }

        /// <summary> The suit of this card when it is a Minor Arcana card. Null when this card is a Major Arcana card. </summary>
        Suit? Suit { get; }
    }
}
