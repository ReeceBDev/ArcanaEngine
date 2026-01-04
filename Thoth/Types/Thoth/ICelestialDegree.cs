using Thoth.Types.Zodiacal;

namespace Thoth.Types.Thoth
{
    internal interface ICelestialDegree
    {
        AstrologicalSign Sign { get; }
        int Degree { get; }
    }
}
