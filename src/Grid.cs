using System;
using System.Collections.Generic;
using Godot;
using Rogue;

namespace Rogue;

public partial class Grid : Node2D, ISaveable
{
    private Global _global;
    private GameState _gameState;

    // The position of this Grid's Zone in the World
    public Vector2I ZoneWorldLocation;
    // The position of this Grid in its Zone
    public Vector2I GridZoneLocation;
    
    // Godot TileMap and TileSet to help do Godot tiling stuff
    private TileMap _tileMap;
    private TileSet _tileSet;
    
    // Proc gen to help make the Grid
    private WaveFunctionCollapse _wfc;

    public override void _Ready()
    {
        base._Ready();
        
        _global = GetNode<Global>("/root/Global");
        _gameState = GetNode<GameState>("/root/GameState");
        
        // Load (and cache) the TileSet resource
        _tileSet = (TileSet)ResourceLoader.Load("res://assets/tilesets/colored_packed.tres");
        
        GD.Print("GRID IS READY");
    }

    public void Initialize(Zone zone, Vector2I locationWithinZone)
    {
        // Party is entering a new Grid
        GD.Print($"ENTERING NEW GRID");
        
        // Add this grid to this zone...
        _gameState.World
            .Zones[zone.WorldLocation.X, zone.WorldLocation.Y]
            .Grids[locationWithinZone.X, locationWithinZone.Y] = this;
        
        // Store the location within the world of the Zone that this
        //  Grid belongs to
        ZoneWorldLocation = zone.WorldLocation;

        // Set this Grid's location within its Zone
        GridZoneLocation = locationWithinZone;
        
        // Create the TileMap
        _tileMap = new TileMap();
        
        // Add it as a child of this node (Grid)
        AddChild(_tileMap);
        
        // Set the TileSet for this map
        _tileMap.TileSet = _tileSet;
        
        // Set up a layer
        _tileMap.AddLayer(0);
        _tileMap.SetLayerEnabled(0, true);
        _tileMap.SetLayerName(0, "Floor");
        _tileMap.SetLayerZIndex(0, 0);
        
        // Generate a Grid using WFC
        // TODO: Or load a saved grid from save data...
        var wfcGrid = GenerateGridWFC();
        
        // Display the WFC map
        for (var x = 0; x < wfcGrid.GetLength(0); x++)
        {
            for (var y = 0; y < wfcGrid.GetLength(1); y++)
            {
                // Grab this cell...
                var c = wfcGrid[x, y];

                if (c.Tile != null)
                {
                    // Figure out what to graphically display...
                    var tileType = Vector2I.Zero;
                    
                    switch (c.Tile.Value)
                    {
                        case 'X':
                            tileType = new Vector2I(23, 3);
                            break;
                        case '.':
                            tileType = new Vector2I(6, 0);
                            break;
                        case 'W':
                            tileType = new Vector2I(8, 5);
                            break;
                    }
                    
                    _tileMap.SetCell(0, new Vector2I(x, y), 0, tileType);
                }
            }
        }
    }

    private GridCell[,] GenerateGridWFC()
    {
        // Create a sample to load the WFC with
        var sample = new List<Tile>();

        var t1 = new Tile
        {
            Id = 1,
            Value = '.'
        };
        var t1Sockets = new List<TileSocket>
        {
            new() { Direction = TileDirections.Up, Connectors = new[] { 1, 2, 3 } },
            new() { Direction = TileDirections.Down, Connectors = new[] { 1, 2, 3 } },
            new() { Direction = TileDirections.Left, Connectors = new[] { 1, 2, 3 } },
            new() { Direction = TileDirections.Right, Connectors = new[] { 1, 2, 3 } }
        };
        t1.Sockets = t1Sockets;
        sample.Add(t1);

        var t2 = new Tile
        {
            Id = 2,
            Value = 'X'
        };
        var t2Sockets = new List<TileSocket>
        {
            new() { Direction = TileDirections.Up, Connectors = new[] { 1, 2 } },
            new() { Direction = TileDirections.Down, Connectors = new[] { 1 } },
            new() { Direction = TileDirections.Left, Connectors = new[] { 1 } },
            new() { Direction = TileDirections.Right, Connectors = new[] { 1 } }
        };
        t2.Sockets = t2Sockets;
        sample.Add(t2);
        
        var t3 = new Tile
        {
            Id = 3,
            Value = 'W'
        };
        var t3Sockets = new List<TileSocket>
        {
            new() { Direction = TileDirections.Up, Connectors = new[] { 1, 3 } },
            new() { Direction = TileDirections.Down, Connectors = new[] { 1, 3 } },
            new() { Direction = TileDirections.Left, Connectors = new[] { 1, 3 } },
            new() { Direction = TileDirections.Right, Connectors = new[] { 1, 3 } }
        };
        t3.Sockets = t3Sockets;
        sample.Add(t3);

        // Instantiate the WFC
        _wfc = new WaveFunctionCollapse(GameConstants.GridWidth, GameConstants.GridHeight, sample);
        
        // Grab a random starting cell to begin generation from
        var random = new Random();
        var randomX = random.Next(0, GameConstants.GridWidth + 1);
        var randomY = random.Next(0, GameConstants.GridHeight + 1);
        
        // Generate the initial grid!
        _wfc.RunAlgorithm(true, randomX, randomY);

        return _wfc.Grid;
    }

    public Godot.Collections.Dictionary<string, Variant> SaveData()
    {
        // Serialize this Grid...

        var zoneWorldLocation = new Godot.Collections.Dictionary<string, Variant>()
        {
            { "X", ZoneWorldLocation.X },
            { "Y", ZoneWorldLocation.Y }
        };

        var gridZoneLocation = new Godot.Collections.Dictionary<string, Variant>()
        {
            { "X", GridZoneLocation.X },
            { "Y", GridZoneLocation.Y }
        };

        var saveData = new Godot.Collections.Dictionary<string, Variant>()
        {
            { "ZoneWorldLocation", zoneWorldLocation },
            { "GridZoneLocation", gridZoneLocation }
        };

        return saveData;
    }

    public void LoadData(Godot.Collections.Dictionary<string, Variant> saveData)
    {
        // Deserialize this Grid...
        
        if (saveData == null)
        {
            GD.Print("LOADED GRID DATA IS NULL");
            // TODO: Need to handle this gracefully, because even returning here is going to cause the game to error...
            return;
        }

        var zoneWorldLocation = (Godot.Collections.Dictionary<string, Variant>)saveData["ZoneWorldLocation"];
        ZoneWorldLocation = new Vector2I((int)zoneWorldLocation["X"], (int)zoneWorldLocation["Y"]);
        
        var gridZoneLocation = (Godot.Collections.Dictionary<string, Variant>)saveData["GridZoneLocation"];
        GridZoneLocation = new Vector2I((int)gridZoneLocation["X"], (int)gridZoneLocation["Y"]);
        
        
    }
}
