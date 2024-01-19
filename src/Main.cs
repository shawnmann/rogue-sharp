using Godot;
using System;
using Rogue;

public partial class Main : Node
{
    private Global _global;
    private GameState _gameState;
    
    private Grid _grid;
    private WorldMap _worldMap;
    private Party _party;
    
    private SubViewport _subViewport;
    
    public override void _Ready()
    {
        base._Ready();

        _global = GetNode<Global>("/root/Global");
        _gameState = GetNode<GameState>("/root/GameState");
        
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // If this is a new game, set the zone, or set it from the save file...
        // But for now, just pick one...
        var zone = _gameState.World.Zones[5, 5];
        _gameState.CurrentZone = zone;

        // Add the game play area as a SubViewPort in the layout of Main
        _subViewport = GetNode<SubViewport>("VBoxContainer/HBoxContainer/SubViewportContainer/SubViewport"); 
        _subViewport.Size = new Vector2I(720, 400);

        // Create the initial Grid
        _grid = new Grid();
        _subViewport.AddChild(_grid);
        _grid.Initialize(zone, zone.WorldLocation);
        //_grid.PartyLeavingGrid += GridOnPartyLeavingGrid;  // Attach a handler to this signal
        
        // Add the party as a child of the Grid
        var partyScene = GD.Load<PackedScene>("res://assets/party/party.tscn");
        _party = partyScene.Instantiate<Party>();
        _grid.AddChild(_party);
        _party.PartyLeavingGrid += PartyOnPartyLeavingGrid;
        _gameState.CurrentParty = _party;

        // Add the camera to the game play area
        var gameCameraScene = GD.Load<PackedScene>("res://assets/game_camera/game_camera.tscn");
        var gameCamera = gameCameraScene.Instantiate<GameCamera>();
        _subViewport.AddChild(gameCamera);
        
        // Add the world map scene
        var worldMapScene = GD.Load<PackedScene>("res://scenes/world_map/world_map.tscn");
        _worldMap = worldMapScene.Instantiate<WorldMap>();
        AddChild(_worldMap);
    }

    private void LoadGrid(Vector2I partyGridLocation, Vector2I partyDirection)
    {
        GD.Print($"PARTY LEAVING FROM {partyGridLocation}, NEED TO LOAD NEXT GRID");
        
        // TODO: Load up the next grid, based on where they are in the zone, and in the world
        //  Proc gen based on biome, etc., then load Grid in, then place Party in the correct
        //  location.
        
        // Remove the old grid...
        _grid.RemoveChild(_party);
        _subViewport.RemoveChild(_grid);
        _grid.QueueFree();
        
        // TODO: We need to check if they've moved zones...
        var zone = _gameState.World.Zones[5, 5];
        _gameState.CurrentZone = zone;

        // Create the Grid
        _grid = new Grid();
        _subViewport.AddChild(_grid);
        _grid.Initialize(zone, zone.WorldLocation);
        
        // Add the party to the grid
        _grid.AddChild(_party);
        
        // TODO: Set the party position so they are entering from the appropriate side
        //  Also, probably need to orient them...
        //  And need to know the direction they are heading
        Vector2I partyNewPosition = Vector2I.Zero;

        // Remember (0, 0) is the top left, moving down makes
        //  positive Ys, and moving right makes positive Xs
        if (partyDirection == new Vector2I(1, 0))
        {
            // They are moving left
            partyNewPosition = new Vector2I(GameConstants.GridWidth - 1, partyGridLocation.Y);
        } else if (partyDirection == new Vector2I(-1, 0))
        {
            // They are moving right
            partyNewPosition = new Vector2I(0, partyGridLocation.Y);
        } else if (partyDirection == new Vector2I(0, -1))
        {
            // They are moving down
            partyNewPosition = new Vector2I(partyGridLocation.X, 0);
        } else if (partyDirection == new Vector2I(0, 1))
        {
            // They are moving up
            partyNewPosition = new Vector2I(partyGridLocation.X, GameConstants.GridHeight - 1);
        }
        
        // Set the party's location
        _party.SetCurrentGridPosition(partyNewPosition, partyDirection);
        
        GD.Print($"POSITION: {_party.Position} GRID POSITION: {_party.GetCurrentGridPosition()}");
    }

    private void PartyOnPartyLeavingGrid(Vector2I partyGridLocation, Vector2I partyDirection)
    {
        LoadGrid(partyGridLocation, partyDirection);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event.IsActionPressed("world_map"))
        {
            _worldMap.Visible = !_worldMap.Visible;
        }
    }
}
