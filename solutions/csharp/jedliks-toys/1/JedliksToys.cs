class RemoteControlCar
{
    private int _distanceDriven;
    private int _batteryPercentage;

    public RemoteControlCar()
    {
        _distanceDriven = 0;
        _batteryPercentage = 100;
    }

    public static RemoteControlCar Buy()
    {
        return new RemoteControlCar();
    }

    public string DistanceDisplay()
    {
        return $"Driven {_distanceDriven} meters";
    }

    public string BatteryDisplay()
    {
        if (_batteryPercentage == 0)
        {
            return "Battery empty";
        }

        return $"Battery at {_batteryPercentage}%";
    }

    public void Drive()
    {
        if (_batteryPercentage == 0)
        {
            return;
        }

        _distanceDriven += 20;
        _batteryPercentage--;
    }
}
