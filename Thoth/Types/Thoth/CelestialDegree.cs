using Thoth.Types.Zodiacal;

namespace Thoth.Types.Thoth
{
    internal readonly record struct CelestialDegree(AstrologicalSign Sign, int Degree) : ICelestialDegree;
}
