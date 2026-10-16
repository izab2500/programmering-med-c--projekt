using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using quasi_route.Pathfinding;
using quasi_route.Algorithms;
namespace quasi_route;

/*
Handles the main application window, including:
1. UI interaction
2. Event handling
3. Management of pathfinding algorithms and visualization
*/

public partial class MainWindow : Window
{   // 
    private Border? startCell;
    private Border? endCell;

    private Position? startPosition;

    private Map? currentMap;

    private bool draggingStart;
    private bool draggingEnd;

    private bool visualizationStarted;

    private bool cancelVisualization;

    private readonly Dictionary<Position, Border> cellBorders = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    // Resets map and creates a new map
    private void CreateMap_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmCreateNewMap())
        {
            return;
        }

        ResetVisualizationState();

        int size = GetSelectedMapSize();

        InitializeMap(size);
        ResetMapUI(size);
        CreateMapCells(size);
    }

    // Asks the user to confirm before creating a new map
    private bool ConfirmCreateNewMap()
    {
        if (currentMap == null)
        {
            return true;
        }

        MessageBoxResult answer = MessageBox.Show(
            "Det finns redan en karta. Vill du skapa en ny karta?",
            "Skapa ny karta",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question
        );

        return answer == MessageBoxResult.Yes;
    }

    // Resets the visualization and dragging states
    private void ResetVisualizationState()
    {
        visualizationStarted = false;
        cancelVisualization = true;

        draggingStart = false;
        draggingEnd = false;

        ResetResultTable();
    }

    // Returns the size of the selected map
    private int GetSelectedMapSize()
    {
        return MapSizeComboBox.SelectedIndex switch
        {
            1 => 30,
            2 => 40,
            _ => 20
        };
    }

    // Initializes the map with the specified size and resets the start and end cells
    private void InitializeMap(int size)
    {
        currentMap = new Map
        {
            Size = size,
            Cells = new()
        };

        startCell = null;
        endCell = null;
        startPosition = null;
    }

    // Resets the maps UI to default
    private void ResetMapUI(int size)
    {
        StartPointText.Text = "Klicka på en cell i kartan";
        EndPointText.Text = "Klicka på en cell i kartan";

        MapGrid.Rows = size;
        MapGrid.Columns = size;
        MapGrid.Children.Clear();

        cellBorders.Clear();
    }

    // Creates the map cells and sets up their mouse event handlers
    private void CreateMapCells(int size)
    {
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                Position position = new Position
                {
                    Row = row,
                    Column = column
                };

                Cell mapCell = new Cell
                {
                    Position = position,
                    Type = CellType.Empty,
                    //
                    Weight = Random.Shared.Next(1, 6)

                };

                currentMap.Cells.Add(mapCell);

                Border cell = new Border
                {
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(0.5),
                    Background = Brushes.Transparent,
                    Tag = position
                };

                cell.MouseLeftButtonDown += Cell_Click;
                cell.MouseMove += Cell_MouseMove;
                cell.MouseLeftButtonUp += Cell_MouseLeftButtonUp;

                MapGrid.Children.Add(cell);
                cellBorders.Add(position, cell);
            }
        }
    }

    // Handles cell clicks to set or drag the start and end points
    private void Cell_Click(object sender, MouseButtonEventArgs e)
    {
        Border cell = (Border)sender;

        Position position = (Position)cell.Tag;

        Cell mapCell = currentMap.GetCell(position);

        if (mapCell == null)
        {
            return;
        }

        if (mapCell.Type == CellType.Start)
        {
            draggingStart = true;
            cell.CaptureMouse();
            e.Handled = true;
            return;
        }

        if (mapCell.Type == CellType.End)
        {
            draggingEnd = true;
            cell.CaptureMouse();
            e.Handled = true;
            return;
        }

        if (currentMap.StartPosition == null)
        {
            SetStartCell(cell, position);
        }
        else if (currentMap.EndPosition == null)
        {
            SetEndCell(cell, position);
        }

        e.Handled = true;
    }

    // Moves the start or end cell to the position under the mouse cursor
    private void Cell_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        if (!draggingStart && !draggingEnd)
        {
            return;
        }

        Point mousePosition = e.GetPosition(MapGrid);

        UIElement? element = MapGrid.InputHitTest(mousePosition) as UIElement;

        if (element is not Border cell)
        {
            return;
        }

        Position position = (Position)cell.Tag;

        if (draggingStart)
        {
            if (currentMap.EndPosition != null &&
                currentMap.EndPosition.Row == position.Row &&
                currentMap.EndPosition.Column == position.Column)
            {
                return;
            }

            MoveStartCell(cell, position);
        }
        else if (draggingEnd)
        {
            if (currentMap.StartPosition != null &&
                currentMap.StartPosition.Row == position.Row &&
                currentMap.StartPosition.Column == position.Column)
            {
                return;
            }

            MoveEndCell(cell, position);
        }
    }
    // Stops dragging the start or end cell when the mouse button is released
    private void Cell_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        draggingStart = false;
        draggingEnd = false;

        Mouse.Capture(null);

        e.Handled = true;
    }

    // Sets the cell type and color for the start or end point
    private Cell? SetPointCell(Border cell, Position position, CellType type, Brush color)
    {
        Cell mapCell = currentMap.GetCell(position);

        if (mapCell == null)
        {
            return null;
        }

        mapCell.Type = type;
        cell.Background = color;

        return mapCell;
    }

    // Sets the start cells position on the map
    private void SetStartCell(Border cell, Position position)
    {
        Cell mapCell = SetPointCell(cell, position, CellType.Start, Brushes.Green);

        if (mapCell == null)
        {
            return;
        }

        currentMap.StartPosition = position;

        startCell = cell;
        startPosition = position;

        StartPointText.Text =
            "Rad " + position.Row + ", Kolumn " + position.Column;
    }

    // Sets the end cells position on the map
    private void SetEndCell(Border cell, Position position)
    {
        Cell mapCell = SetPointCell(cell, position, CellType.End, Brushes.Blue);

        if (mapCell == null)
        {
            return;
        }

        currentMap.EndPosition = position;

        endCell = cell;

        EndPointText.Text =
            "Rad " + position.Row + ", Kolumn " + position.Column;
    }

    // Prepares the map for moving a start or end cell
    private void PrepareForPointMove()
    {
        cancelVisualization = true;
        visualizationStarted = false;

        ResetResultTable();
        ResetMapColors();
    }

    // Resets the previous cell to its default state
    private void ClearPreviousCell(Position position, Border cell)
    {
        Cell oldCell = currentMap.GetCell(position);

        if (oldCell != null)
        {
            oldCell.Type = CellType.Empty;
        }

        cell.Background = Brushes.Transparent;
    }

    // Moves the start cell to a specific position on the map
    private void MoveStartCell(Border cell, Position position)
    {
        if (startCell == null || startCell == cell || startPosition == null)
        {
            return;
        }

        PrepareForPointMove();

        ClearPreviousCell(startPosition, startCell);

        SetStartCell(cell, position);
    }

    // Moves the end cell to a specific position on the map
    private void MoveEndCell(Border cell, Position position)
    {
        if (endCell == null || endCell == cell)
        {
            return;
        }

        PrepareForPointMove();

        Position oldEndPosition = currentMap.EndPosition;

        if (oldEndPosition != null)
        {
            ClearPreviousCell(oldEndPosition, endCell);
        }

        SetEndCell(cell, position);
    }

    // Returns the pathfinding result for the selected algorithm
    private PathfindingResult? FindPath(string algorithm)
    {
        if (currentMap == null ||
            currentMap.StartPosition == null ||
            currentMap.EndPosition == null)
        {
            return null;
        }

        if (algorithm == "BFS")
        {
            Bfs bfs = new Bfs();

            return bfs.FindPath(
                currentMap,
                currentMap.StartPosition,
                currentMap.EndPosition
            );
        }

        if (algorithm == "Dijkstra")
        {
            Dijkstar dijkstra = new Dijkstar();

            return dijkstra.FindPath(
                currentMap,
                currentMap.StartPosition,
                currentMap.EndPosition
            );
        }

        return null;
    }

    // Initializes the node visualization if the necessary values are valid
    private async void StartVisualization_Click(object sender, RoutedEventArgs e)
    {
        if (currentMap == null)
        {
            MessageBox.Show("Skapa en karta först.");
            return;
        }

        if (currentMap.StartPosition == null)
        {
            MessageBox.Show("Välj en startpunkt.");
            return;
        }

        if (currentMap.EndPosition == null)
        {
            MessageBox.Show("Välj en målpunkt.");
            return;
        }

        if (visualizationStarted)
        {
            MessageBox.Show("Visualiseringen har redan startats för denna karta.");
            return;
        }

        if (AlgorithmComboBox.SelectedItem is not ComboBoxItem selectedItem)
        {
            MessageBox.Show("Välj en algoritm.");
            return;
        }

        string algorithm = selectedItem.Content?.ToString() ?? "";

        cancelVisualization = false;
        visualizationStarted = true;

        Stopwatch stopwatch = Stopwatch.StartNew();

        PathfindingResult? result = FindPath(algorithm);

        stopwatch.Stop();

        if (result == null)
        {
            MessageBox.Show("Okänd algoritm.");
            visualizationStarted = false;
            return;
        }

        UpdateResultTable(algorithm, result, stopwatch.Elapsed);

        await VisualizeVisitedNodes(result);
        if (cancelVisualization)
        {
            return;
        }

        await VisualizePath(result);
    }

    // Visualizes the visited nodes during search
    private async Task VisualizeVisitedNodes(PathfindingResult result)
    {
        foreach (Position position in result.VisitedPositions)
        {
            Border? cell = GetCellBorder(position);

            if (cell == null)
            {
                continue;
            }

            if (position.Row == currentMap.StartPosition.Row &&
                position.Column == currentMap.StartPosition.Column)
            {
                continue;
            }

            if (position.Row == currentMap.EndPosition.Row &&
                position.Column == currentMap.EndPosition.Column)
            {
                continue;
            }

            cell.Background = Brushes.LightGray;

            await Task.Delay(30);

            if (cancelVisualization)
            {
                return;
            }
        }
    }

    // Visualizes the path from the start cell to the end cell
    private async Task VisualizePath(PathfindingResult result)
    {
        foreach (Position position in result.Path)
        {
            Border? cell = GetCellBorder(position);

            if (cell == null)
            {
                continue;
            }

            if (position.Row == currentMap.StartPosition.Row &&
                position.Column == currentMap.StartPosition.Column)
            {
                continue;
            }

            if (position.Row == currentMap.EndPosition.Row &&
                position.Column == currentMap.EndPosition.Column)
            {
                continue;
            }

            cell.Background = Brushes.Cyan;

            await Task.Delay(100);

            if (cancelVisualization)
            {
                return;
            }
        }
    }

    // Updates the result table with the pathfinding results
    private void UpdateResultTable(
        string algorithm,
        PathfindingResult result,
        TimeSpan executionTime
    )
    {
        ResultAlgorithmText.Text = algorithm;

        ResultMapSizeText.Text =
        currentMap.Size + " × " + currentMap.Size;

        ResultPathLengthText.Text =
        result.PathLength.ToString();

        ResultVisitedNodesText.Text =
        result.VisitedNodes.ToString();

        ResultExecutionTimeText.Text =
        executionTime.TotalMilliseconds.ToString("F2") + " ms";
    }

    // Returns the border of the cell at the specified position
    private Border? GetCellBorder(Position position)
    {
        cellBorders.TryGetValue(position, out Border? cell);
        return cell;
    }

    // Resets the colors of all map cells (not start and end cells)
    private void ResetMapColors()
    {
        foreach (UIElement element in MapGrid.Children)
        {
            if (element is Border cell)
            {
                Position position = (Position)cell.Tag;

                if (currentMap.StartPosition != null &&
                    position.Row == currentMap.StartPosition.Row &&
                    position.Column == currentMap.StartPosition.Column)
                {
                    continue;
                }

                if (currentMap.EndPosition != null &&
                    position.Row == currentMap.EndPosition.Row &&
                    position.Column == currentMap.EndPosition.Column)
                {
                    continue;
                }

                cell.Background = Brushes.Transparent;
            }
        }
    }
    // Resets the result table to its default values
    private void ResetResultTable()
    {
        ResultAlgorithmText.Text = "-";
        ResultMapSizeText.Text = "-";
        ResultPathLengthText.Text = "-";
        ResultVisitedNodesText.Text = "-";
        ResultExecutionTimeText.Text = "-";
    }

}