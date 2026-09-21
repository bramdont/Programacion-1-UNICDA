static class SavingsAccount
{
    private const decimal PercentDivisor = 100m;

    public static float InterestRate(decimal balance) => (float)InterestRatePercent(balance);

    public static decimal Interest(decimal balance) =>
        balance * InterestRatePercent(balance) / PercentDivisor;

    public static decimal AnnualBalanceUpdate(decimal balance) =>
        balance + Interest(balance);

    public static int YearsBeforeDesiredBalance(decimal balance, decimal targetBalance)
    {
        var years = 0;

        while (balance < targetBalance)
        {
            balance = AnnualBalanceUpdate(balance);
            years++;
        }

        return years;
    }

    // Se calcula en decimal para evitar errores de precisión de float en el dinero.
    private static decimal InterestRatePercent(decimal balance) => balance switch
    {
        < 0m => 3.213m,
        < 1_000m => 0.5m,
        < 5_000m => 1.621m,
        _ => 2.475m
    };
}
