namespace Thoth.Resources.Calculators
{
    internal interface IThothCalculator
    {
        int CalculateGrowthOffset(DateTime birthDate, int targetYear);
        int CalculateCrossSum(int number);
    }
}
