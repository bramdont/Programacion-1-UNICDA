using System.Linq;

public struct Coord
{
    public Coord(ushort x, ushort y)
    {
        X = x;
        Y = y;
    }

    public ushort X { get; }
    public ushort Y { get; }
}

public struct Plot
{
    public Coord Corner1 { get; }
    public Coord Corner2 { get; }
    public Coord Corner3 { get; }
    public Coord Corner4 { get; }

    public Plot(Coord corner1, Coord corner2, Coord corner3, Coord corner4)
    {
        Corner1 = corner1;
        Corner2 = corner2;
        Corner3 = corner3;
        Corner4 = corner4;
    }

    public int LongestSide
    {
        get
        {
            var xs = new[] { Corner1.X, Corner2.X, Corner3.X, Corner4.X };
            var ys = new[] { Corner1.Y, Corner2.Y, Corner3.Y, Corner4.Y };
            int width = xs.Max() - xs.Min();
            int height = ys.Max() - ys.Min();
            return Math.Max(width, height);
        }
    }
}


public class ClaimsHandler
{
    private readonly List<Plot> claims = new List<Plot>();

    public void StakeClaim(Plot plot)
    {
        claims.Add(plot);
    }

    public bool IsClaimStaked(Plot plot)
    {
        return claims.Contains(plot);
    }

    public bool IsLastClaim(Plot plot)
    {
        return claims.Count > 0 && claims[^1].Equals(plot);
    }

    public Plot GetClaimWithLongestSide()
    {
        return claims.OrderByDescending(plot => plot.LongestSide).First();
    }
}
