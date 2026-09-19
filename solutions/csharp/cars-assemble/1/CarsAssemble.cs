static class AssemblyLine
{
    private const int CarsPerHourAtSpeedOne = 221;
    private const int MinutesPerHour = 60;

    public static double SuccessRate(int speed) => speed switch
    {
        0 => 0.0,
        >= 1 and <= 4 => 1.0,
        >= 5 and <= 8 => 0.9,
        9 => 0.8,
        10 => 0.77,
        _ => throw new ArgumentOutOfRangeException(nameof(speed), speed, "Speed must be between 0 and 10.")
    };

    public static double ProductionRatePerHour(int speed) =>
        CarsPerHourAtSpeedOne * speed * SuccessRate(speed);

    public static int WorkingItemsPerMinute(int speed) =>
        (int)(ProductionRatePerHour(speed) / MinutesPerHour);
}
