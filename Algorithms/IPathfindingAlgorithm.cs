using quasi_route.Pathfinding;

namespace quasi_route.Algorithms;

public interface IPathfindingAlgorithm
{
    PathfindingResult FindPath(
        Map map,
        Position startNode,
        Position endNode
    );
}