namespace Thoth.External.Types
{
    public interface IArcanaCard
    {
        ArcanaRole Role { get; }
        string Name { get; }
        int Number { get; }
    }
}
