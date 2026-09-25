public interface ITelemetry
{
    void Calibrate();
    bool SelfTest();
    void ShowSponsor(string sponsorName);
    void SetSpeed(decimal amount, string unitsString);
}

public class RemoteControlCar
{
    public string CurrentSponsor { get; private set; } = "";

    public ITelemetry Telemetry { get; }

    private Speed currentSpeed;

    public RemoteControlCar()
    {
        Telemetry = new TelemetrySystem(this);
    }

    public string GetSpeed()
    {
        return currentSpeed.ToString();
    }

    private void SetSponsor(string sponsorName)
    {
        CurrentSponsor = sponsorName;
    }

    private void SetSpeed(Speed speed)
    {
        currentSpeed = speed;
    }

    private enum SpeedUnits
    {
        MetersPerSecond,
        CentimetersPerSecond
    }

    private struct Speed
    {
        public decimal Amount { get; }
        public SpeedUnits SpeedUnits { get; }

        public Speed(decimal amount, SpeedUnits speedUnits)
        {
            Amount = amount;
            SpeedUnits = speedUnits;
        }

        public override string ToString()
        {
            string unitsString = "meters per second";
            if (SpeedUnits == SpeedUnits.CentimetersPerSecond)
            {
                unitsString = "centimeters per second";
            }

            return Amount + " " + unitsString;
        }
    }

    private class TelemetrySystem : ITelemetry
    {
        private readonly RemoteControlCar car;

        public TelemetrySystem(RemoteControlCar car)
        {
            this.car = car;
        }

        public void Calibrate()
        {

        }

        public bool SelfTest()
        {
            return true;
        }

        public void ShowSponsor(string sponsorName)
        {
            car.SetSponsor(sponsorName);
        }

        public void SetSpeed(decimal amount, string unitsString)
        {
            SpeedUnits speedUnits = SpeedUnits.MetersPerSecond;
            if (unitsString == "cps")
            {
                speedUnits = SpeedUnits.CentimetersPerSecond;
            }

            car.SetSpeed(new Speed(amount, speedUnits));
        }
    }
}
