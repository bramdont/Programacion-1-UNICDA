class BirdCount
{
    private int[] birdsPerDay;

    public BirdCount(int[] birdsPerDay)
    {
        this.birdsPerDay = birdsPerDay;
    }

    public static int[] LastWeek()
    {
        return new[] { 0, 2, 5, 3, 7, 8, 4 };
    }

    public int Today()
    {
        return birdsPerDay[birdsPerDay.Length - 1];
    }

    public void IncrementTodaysCount()
    {
        birdsPerDay[birdsPerDay.Length - 1]++;
    }

    public bool HasDayWithoutBirds()
    {
        foreach (int count in birdsPerDay)
        {
            if (count == 0)
            {
                return true;
            }
        }

        return false;
    }

    public int CountForFirstDays(int numberOfDays)
    {
        if (numberOfDays <= 0)
        {
            return 0;
        }

        int total = 0;
        int daysToCount = Math.Min(numberOfDays, birdsPerDay.Length);

        for (int i = 0; i < daysToCount; i++)
        {
            total += birdsPerDay[i];
        }

        return total;
    }

    public int BusyDays()
    {
        int busyDays = 0;

        foreach (int count in birdsPerDay)
        {
            if (count >= 5)
            {
                busyDays++;
            }
        }

        return busyDays;
    }
}
