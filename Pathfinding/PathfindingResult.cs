namespace quasi_route.Pathfinding;

// Stores the pathfinding results
public class PathfindingResult
{
    public List<Position> Path { get; set; }

    public List<Position> VisitedPositions { get; set; }

    public int PathLength { get; set; }

    public int VisitedNodes { get; set; }
}