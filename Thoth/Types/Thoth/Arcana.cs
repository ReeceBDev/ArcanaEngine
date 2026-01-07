using Thoth.Types.Thoth.Data;

namespace Thoth.Types.Thoth
{
    internal readonly record struct Arcana : IArcana
    {
        public ArcanicCategory Categorization { get; init; }
        public string Name { get; init; }
        public int Number { get; init; }
        public string ShortDescription { get; init; }
    }
}