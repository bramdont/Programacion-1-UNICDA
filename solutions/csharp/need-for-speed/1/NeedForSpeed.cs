class RemoteControlCar
{
    private const int FullBattery = 100;

    private readonly int speed;
    private readonly int batteryDrain;
    private int battery = FullBattery;
    private int distanceDriven;

    public RemoteControlCar(int speed, int batteryDrain)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(speed);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(batteryDrain);

        this.speed = speed;
        this.batteryDrain = batteryDrain;
    }

    public bool BatteryDrained() => battery < batteryDrain;

    public int DistanceDriven() => distanceDriven;

    public void Drive()
    {
        if (BatteryDrained())
        {
            return;
        }

        distanceDriven += speed;
        battery -= batteryDrain;
    }

    public static RemoteControlCar Nitro() => new(speed: 50, batteryDrain: 4);

    // Distancia que aún puede recorrer con la batería restante, sin modificar el coche.
    internal bool CanDrive(int distance) => (battery / batteryDrain) * speed >= distance;
}

class RaceTrack
{
    private readonly int distance;

    public RaceTrack(int distance)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(distance);

        this.distance = distance;
    }

    public bool TryFinishTrack(RemoteControlCar car) => car.CanDrive(distance);
}
