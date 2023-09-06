
namespace HungerGamesSimulator.Data;

public struct Coord
{
    public int X { get; set; }

    public int Y { get; set; }

    public Coord(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"({X},{Y})";
    }

    public override bool Equals(object? obj)
    {
        return obj is Coord coord && Equals(coord);
    }

    public bool Equals(Coord other)
    {
        return X == other.X && Y == other.Y;
    }

    public static bool operator ==(Coord a, Coord b) { return a.X == b.X && a.Y == b.Y; }

    public static bool operator !=(Coord a, Coord b) { return a.X != b.X || a.Y != b.Y; }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ X.GetHashCode();
    }
}