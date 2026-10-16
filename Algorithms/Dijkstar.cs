using quasi_route.Pathfinding;

namespace quasi_route.Algorithms;

/*
Dijkstra's Algorithm

The algorithm starts at the startNode with a distance of 0.
It always selects the unvisited node with the shortest distance.

It checks the neighbors in four directions: up, down, left and right.
Each move is determinde by the costs 1 - 5. If a shorter path to a neighbor is found,
its distance and previous node are saved.

The search continues until the endNode is found or there are
no more nodes to visit.

Data structures

- Dictionary/Hash table: Stores the shortest distance from the startNode 
  to each discovered node.
- List: Keeps track of visited nodes.
- Dictionary: Saves the previous node to rebuild the path.

Summary

Dijkstra finds the shortest path by always choosing the
unvisited node with the shortest distance. It then rebuilds
the path from the endNode back to the startNode.
*/

public class Dijkstar : IPathfindingAlgorithm
{
    public PathfindingResult FindPath(
        Map map,
        Position startNode,
        Position endNode)
    {
        // Datastructures
        Dictionary<Position, int> distances = new();
        List<Position> visited = new();
        Dictionary<Position, Position> previous = new();

        // Initialize start distance
        distances[startNode] = 0;

        // Search
        while (true)
        {
            Position? current = null;
            int shortestDistance = int.MaxValue;

            // Find the unvisited position with the shortest distance
            foreach (KeyValuePair<Position, int> entry in distances)
            {
                if (!visited.Contains(entry.Key) &&
                    entry.Value < shortestDistance)
                {
                    current = entry.Key;
                    shortestDistance = entry.Value;
                }
            }

            // No more positions to visit
            if (current == null)
            {
                break;
            }

            visited.Add(current);

            // Found the end position
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

            VisitNeighbor(map, up, current, distances, visited, previous);

            // Move down
            Position down = new()
            {
                Row = current.Row + 1,
                Column = current.Column
            };

            VisitNeighbor(map, down, current, distances, visited, previous);

            // Move left
            Position left = new()
            {
                Row = current.Row,
                Column = current.Column - 1
            };

            VisitNeighbor(map, left, current, distances, visited, previous);

            // Move right
            Position right = new()
            {
                Row = current.Row,
                Column = current.Column + 1
            };

            VisitNeighbor(map, right, current, distances, visited, previous);
        }

        // Reconstruct path
        List<Position> path = new();

        if (distances.ContainsKey(endNode))
        {
            Position currentPath = endNode;

            while (currentPath.Row != startNode.Row ||
                   currentPath.Column != startNode.Column)
            {
                path.Add(currentPath);
                currentPath = previous[currentPath];
            }

            path.Add(startNode);
            path.Reverse();
        }

        // Results
        return new PathfindingResult
        {
            Path = path,
            VisitedPositions = visited,
            PathLength = path.Count > 0 ? path.Count - 1 : 0,
            VisitedNodes = visited.Count
        };
    }

    private void VisitNeighbor(
        Map map,
        Position neighbor,
        Position current,
        Dictionary<Position, int> distances,
        List<Position> visited,
        Dictionary<Position, Position> previous)
    {
        Cell cell = map.GetCell(neighbor);

        if (cell == null || visited.Contains(neighbor))
        {
            return;
        }

        //int newDistance = distances[current] + 1;
        // The cost of entering a cell is its weight
        int newDistance = distances[current] + cell.Weight;

        // Add the neighbor or update its distance if a shorte path is found
        if (!distances.ContainsKey(neighbor) ||
            newDistance < distances[neighbor])
        {
            distances[neighbor] = newDistance;
            previous[neighbor] = current;
        }
    }
}