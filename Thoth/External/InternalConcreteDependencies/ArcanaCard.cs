using Thoth.External.Types;
using Thoth.Types.Thoth;

namespace Thoth.External.InternalConcreteDependencies
{
    internal readonly record struct ArcanaCard : IArcanaCard
    {
        public ArcanaRole Role { get; }
        public string Name { get; }
        public int Number { get; }

        public ArcanaCard(IArchetype archetypeInput, ArcanaRole role)
        {
            Role = role;
            Name = archetypeInput.Arcana.Name;
            Number = archetypeInput.Arcana.Number;
        }
    }
}
