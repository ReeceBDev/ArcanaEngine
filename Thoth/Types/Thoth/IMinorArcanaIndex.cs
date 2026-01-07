using Thoth.Types.Thoth.Data;

namespace Thoth.Types.Thoth
{
    internal interface IMinorArcanaIndex
    {
        MinorArcanaAddedToOffset SuitOffset { get; }
        int CardNumber { get; }
    }
}