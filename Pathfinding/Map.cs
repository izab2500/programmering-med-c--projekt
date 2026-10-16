namespace quasi_route.Pathfinding;

/* Represents the map and it's properties:

- Size: the number of rows and columns (20x20, 30x30 or 40x40)

- Cell: a square with a defined position row and column 

- Position: a unique coordinate on the map (row, column)
*/

public class Map
{
    public int Size { get; set; }
    public List<Cell> Cells { get; set; }

    public Position StartPosition { get; set; }
    public Position EndPosition { get; set; }

    public Cell GetCell(Position position)
    {
        foreach (Cell cell in Cells)
        {
            if (cell.Position.Row == position.Row &&
                cell.Position.Column == position.Column)
            {
                return cell;
            }
        }

        return null;
    }
}