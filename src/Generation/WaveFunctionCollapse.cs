using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Rogue;

public partial class WaveFunctionCollapse
{
    private readonly Random _random;
    public GridCell[,] Grid { get; }
    
    // TODO: Small Object Heap: code that allocates a lot of memory in SOH
    //  Allocated object type: GridCell[]
    //  at List<PropertyInfo>.set_Capacity(int)
    //  at List<PropertyInfo>.AddWithResize()
    //  at WaveFunctionCollapse.GetCellWithLowestEntropy() in /Users/shawnmann/rogue/src/Generation/WaveFunctionCollapse.cs:line 65 column 17
    //  at WaveFunctionCollapse.Observe(GridCell) in /Users/shawnmann/rogue/src/Generation/WaveFunctionCollapse.cs:line 201 column 9
    //  at WaveFunctionCollapse.RunAlgorithm(bool, int, int) in /Users/shawnmann/rogue/src/Generation/WaveFunctionCollapse.cs:line 82 column 13
    //  at WaveFunctionCollapse.RunAlgorithm(bool, int, int) in /Users/shawnmann/rogue/src/Generation/WaveFunctionCollapse.cs:line 92 column 21
    //  the above line repeated...
    
    public WaveFunctionCollapse(int width, int height, List<Tile> sample)
    {
        _random = new Random();
        Grid = new GridCell[width, height];
            
        GD.Print($"GRID INFO: {Grid.GetLength(0)} by {Grid.GetLength(1)}");
            
        // Set up initial grid superposition
        //  so loop through the grid and add a GridCell to each location
        //  and then fill that GridCell's PossibleTiles with every tile
        //  because to start, every tile is possible for every spot
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                // Make a new GridCell for this location
                var gridCell = new GridCell
                {
                    PossibleTiles = new List<Tile>(),
                    X = x,
                    Y = y,
                    Solved = false
                };

                // Load it with all the tiles from the sample
                foreach (var tile in sample)
                {
                    gridCell.PossibleTiles.Add(tile);
                }

                // Add the cell to the grid
                Grid[x, y] = gridCell;
            }
        }
    }
    
    public GridCell GetCellWithLowestEntropy()
    {
        var minEntropy = int.MaxValue;
        List<GridCell> lowList = null;

        // Load the list up with all the cells with the lowest entropy
        foreach (var gridCell in Grid)
        {
            if (gridCell.Entropy < minEntropy && !gridCell.Solved)
            {
                // This cell is the new lowest entropy
                minEntropy = gridCell.Entropy;
                // So make a new list and add it
                lowList = new List<GridCell> { gridCell };
            } else if (gridCell.Entropy == minEntropy && !gridCell.Solved)
            {
                // This is tied, so add it to the list
                lowList?.Add(gridCell);
            }
        }
            
        // Then pick a random cell from the list...
        return lowList?[_random.Next(lowList.Count)];
    }
    
    public void RunAlgorithm(bool solve, int x = -1, int y = -1)
    {
        // Check if there are any cells that aren't collapsed
        //  If there are, we still need to keep going
        if (!IsCollapsed())
        {
            // Find the cell with the least entropy and observe it
            //  i.e. choose a final state for it
            // TODO: ERROR: System.IndexOutOfRangeException: Index was outside the bounds of the array.
            // at WaveFunctionCollapse.RunAlgorithm(Boolean solve, Int32 x, Int32 y) in /Users/shawnmann/rogue/src/Generation/WaveFunctionCollapse.cs:line 91
            // (this next line:)
            GridCell preselectedCell = x == -1 && y == -1 ? null : Grid[x,y];
            var cell = Observe(preselectedCell);

            if (cell != null)
            {
                // Now that that cell is collapsed (resolved) propagate
                //  that info to the rest of the cells
                Propagate(cell);
                
                if (solve)
                {
                    RunAlgorithm(true);
                }
            }
        }
    }
    
    // Propagate new info to the grid cells
    private void Propagate(GridCell startCell)
    {
        // Queues are "first-in, first-out" collections of objects
        var queue = new Queue<GridCell>();
        
        // Add the starter cell to the (end) of the queue
        queue.Enqueue(startCell);
        
        // Go through the queue
        while (queue.Count > 0)
        {
            // Remove (and return) the object at the beginning of the queue
            var gc = queue.Dequeue();
            
            // Go through all the neighbors of this cell
            foreach (TileDirections tileDirection in Enum.GetValues(typeof(TileDirections)))
            {
                // Grab the starting point, so we can calculate the
                //  neighbor's x and y
                var neighborX = gc.X;
                var neighborY = gc.Y;

                switch (tileDirection)
                {
                    case TileDirections.Up:
                        neighborY--;
                        break;
                    case TileDirections.Down:
                        neighborY++;
                        break;
                    case TileDirections.Left:
                        neighborX--;
                        break;
                    case TileDirections.Right:
                        neighborX++;
                        break;
                }
                
                // Now we have the target tile's direction, so let's make
                //  sure it's inside the grid's bounds
                if (IsTargetWithinGrid(neighborX, neighborY))
                {
                    // Get the GridCell for the target (this neighbor)
                    var neighborCell = Grid[neighborX, neighborY];
                    if (UpdateNeighborCell(gc, neighborCell, tileDirection))
                    {
                        // If the neighbor cell was updated, add it back to the queue
                        queue.Enqueue(neighborCell);
                    }
                }
            }
        }
    }
    
    private bool UpdateNeighborCell(GridCell cell, GridCell neighborCell, TileDirections direction)
    {
        var isUpdated = false;

        var compatibleTiles = new HashSet<int>();

        // Go through all of the possible tiles in the passed in grid cell
        foreach (var t in cell.PossibleTiles)
        {
            // Go through each socket on this tile
            foreach (var socket in t.Sockets)
            {
                // Check if this socket is in the direction we care about
                if (socket.Direction == direction)
                {
                    // It is, so let's do some union-ing
                    compatibleTiles.UnionWith(socket.Connectors);
                }
            }
            
            // Remove incompatible tiles from the neighbor cell's possibilities
            for (var i = neighborCell.PossibleTiles.Count - 1; i >= 0; i--)
            {
                // Check if this is a compatible tile or not
                if (!compatibleTiles.Contains(neighborCell.PossibleTiles[i].Id))
                {
                    // This one is incompatible, so remove it
                    neighborCell.PossibleTiles.RemoveAt(i);
                    isUpdated = true;
                }
            }
            
        }
        
        return isUpdated;
    }

    private bool IsTargetWithinGrid(int x, int y)
    {
        return x >= 0 && x < Grid.GetLength(0) &&
               y >= 0 && y < Grid.GetLength(1);
    }
    
    // Resolve a cell
    private GridCell Observe(GridCell preselectedCell = null)
    {
        // If they preselected a cell, use that one, if not, get the cell
        //  with the lowest entropy
        var cell = preselectedCell ?? GetCellWithLowestEntropy();
        
        // DEBUG:
        //GD.Print($"CHOSEN: ({cell.X},{cell.Y}) E: {cell.Entropy} S: {cell.Solved}");

        // Collapse that cell's state (choose a tile for it to be)
        // Choose a tile out of this cell's possible tiles at random
        var selection = _random.Next(cell.PossibleTiles.Count);
        var selectedTile = cell.PossibleTiles[selection];
        
        // Empty this cell's possible tiles
        cell.PossibleTiles.Clear();
        
        // And load it with only the selection
        cell.PossibleTiles.Add(selectedTile);
        
        // This cell was observed, so mark it solved
        cell.Solved = true;
        
        // And load it with the tile that was selected
        cell.Tile = selectedTile;
        cell.Tile.Position = new Vector2I(cell.X, cell.Y);
        
        return cell;
    }

    // See if the grid is completely collapsed
    private bool IsCollapsed()
    {
        // See if every GridCell in the Grid is solved
        return Grid.Cast<GridCell>().All(cell => cell.Solved);
    }
}
