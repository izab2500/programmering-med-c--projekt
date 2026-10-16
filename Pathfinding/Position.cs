namespace quasi_route.Pathfinding;

// Represents a unique coordinate on the map (row, column)
public class Position
{
    public int Row { get; set; }
    public int Column { get; set; }

    public override bool Equals(object obj)
    {
        if (obj is not Position other)
        {
            return false;
        }

        return Row == other.Row &&
               Column == other.Column;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }
}