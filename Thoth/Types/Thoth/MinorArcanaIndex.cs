using Thoth.Types.Thoth.Data;

namespace Thoth.Types.Thoth
{
    internal readonly record struct MinorArcanaIndex(MinorArcanaAddedToOffset SuitOffset, int CardNumber) : IMinorArcanaIndex;
}
