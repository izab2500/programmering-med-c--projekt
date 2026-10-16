namespace quasi_route.Pathfinding;

// Represents a cell/square on the map
public class Cell
{
    public Position Position { get; set; }
    public CellType Type { get; set; }
    public int Weight { get; set; } = 1;
}