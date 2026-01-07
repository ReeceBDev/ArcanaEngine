using Thoth.External.Types;
using Thoth.Types.Thoth;

namespace Thoth.External.InternalConcreteDependencies
{
    internal sealed class ArcanaCard : IArcanaCard
    {
        public ArcanaRole Role { get; }

        public ArcanaCard(IArchetype archetypeInput, ArcanaRole role)
        {
            Role = role;
        }
    }
}
