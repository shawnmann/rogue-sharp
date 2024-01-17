using Godot;
using System;
using Rogue;

public partial class Main : Node
{
    private Global _global;
    private GameState _gameState;
    
    private Grid _grid;
    private Button _button;
    private WorldMap _worldMap;
    
    public override void _Ready()
    {
        base._Ready();

        _global = GetNode<Global>("/root/Global");
        _gameState = GetNode<GameState>("/root/GameState");
        
        // Get the zone the player is in when loading, but for now, just pick one so we can try it
        var zone = _gameState.World.Zones[5, 5];
        _gameState.CurrentZone = zone;

        // Add the game play area as a SubViewPort in the layout of Main
        var subViewPort = GetNode<SubViewport>("VBoxContainer/HBoxContainer/SubViewportContainer/SubViewport"); 
        subViewPort.Size = new Vector2I(720, 400);

        // Create the initial Grid
        _grid = new Grid();
        subViewPort.AddChild(_grid);
        _grid.Initialize(zone, zone.WorldLocation);
        _grid.PartyLeavingGrid += GridOnPartyLeavingGrid;

        // Add the camera to the game play area
        var gameCameraScene = GD.Load<PackedScene>("res://assets/game_camera/game_camera.tscn");
        var gameCamera = gameCameraScene.Instantiate<GameCamera>();
        subViewPort.AddChild(gameCamera);
        
        var worldMapScene = GD.Load<PackedScene>("res://scenes/world_map/world_map.tscn");
        _worldMap = worldMapScene.Instantiate<WorldMap>();
        AddChild(_worldMap);
    }

    private void LoadGrid(Vector2 partyGridLocation)
    {
        GD.Print($"PARTY LEAVING FROM {partyGridLocation}, NEED TO LOAD NEXT GRID");
        
        // TODO: Load up the next grid, based on where they are in the zone, and in the world
        //  Proc gen based on biome, etc., then load Grid in, then place Party in the correct
        //  location.
    }

    private void GridOnPartyLeavingGrid(Vector2 partyGridLocation)
    {
        LoadGrid(partyGridLocation);
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
