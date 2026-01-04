using System.Collections.Immutable;

namespace Thoth.Resources.Calculators
{
    internal interface IGemetriaCalculator
    {
        int GetGemetriaValues(string inputHebrew);
    }
}