
using quasi_route.Pathfinding;

namespace quasi_route.Algorithms;

/*
BFS - Breadth-First Search

BFS starts searching from the startNode. It then checks its
neighbors in four directions: up, down, left and right.

If a neighbor is valid and has not been visited, it is added
to the queue and the visited list. The current node is also
saved as its previous node.

BFS continues searchin from the nodes in the queue until
it finds the endNode or the queue is empty.

Since the queue follows the FIFO principle.
BFS searches nodes that are closer to the startNode before
searching nodes that are further away. This means BFS finds
the shortest path when every move has the same cost.

Data structures

- Queue: Decides the order in which nodes are searche.
  The first node added is the first node to be searched.

- List: Keeps track of visited nodes to avoid visiting
  the same node more than once.

- Dictionary/Hash table: Saves the previous node for each visited node.
  This is used to reconstruct the path from the endNode
  back to the startNode.

Summary

BFS searches the map level by level using a queue.
When the endNode is found, the path is reconstructed
by following the previous nodes back to the startNode.
The path is then reversed so it goes from start to end.
*/

public class Bfs : IPathfindingAlgorithm
{
    public PathfindingResult FindPath(
        Map map,
        Position startNode,
        Position endNode)
    {
        Queue<Position> queue = new();
        List<Position> visited = new();
        List<Position> processed = new();
        Dictionary<Position, Position> previous = new();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        while (queue.Count > 0)
        {
            Position current = queue.Dequeue();

            // Register the node when it is processed.
            processed.Add(current);

            // Stop when the end node is reached.
            if (current.Row == endNode.Row &&
                current.Column == endNode.Column)
            {
                break;
            }

            // Move up
            Position up = new()
            {
                Row = current.Row - 1,
                Column = current.Column
            };

            VisitNeighbor(map, up, current, visited, queue, previous);

            // Move down
            Position down = new()
            {
                Row = current.Row + 1,
                Column = current.Column
            };

            VisitNeighbor(map, down, current, visited, queue, previous);

            // Move left
            Position left = new()
            {
                Row = current.Row,
                Column = current.Column - 1
            };

            VisitNeighbor(map, left, current, visited, queue, previous);

            // Move right
            Position right = new()
            {
                Row = current.Row,
                Column = current.Column + 1
            };

            VisitNeighbor(map, right, current, visited, queue, previous);
        }

        // Reconstruct the path
        List<Position> path = new();
        Position currentPath = endNode;

        while (currentPath.Row != startNode.Row ||
               currentPath.Column != startNode.Column)
        {
            path.Add(currentPath);
            currentPath = previous[currentPath];
        }

        path.Add(startNode);
        path.Reverse();

        return new PathfindingResult
        {
            Path = path,
            VisitedPositions = processed,
            PathLength = path.Count - 1,
            VisitedNodes = processed.Count
        };
    }

    private void VisitNeighbor(
        Map map,
        Position neighbor,
        Position current,
        List<Position> visited,
        Queue<Position> queue,
        Dictionary<Position, Position> previous)
    {
        Cell? cell = map.GetCell(neighbor);

        if (cell != null && !visited.Contains(neighbor))
        {
            visited.Add(neighbor);
            queue.Enqueue(neighbor);
            previous[neighbor] = current;
        }
    }
}
