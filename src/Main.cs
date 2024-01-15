using Godot;
using System;
using Rogue;

public partial class Main : Node
{
    private World _world;
    private Grid _grid;
    private Button _button;
    private WorldMap _worldMap;
    
    public override void _Ready()
    {
        base._Ready();

        _world = new World();
        _world.Initialize();

        _grid = new Grid();

        // Add the game play area as a SubViewPort in the layout of Main
        var subViewPort = GetNode<SubViewport>("VBoxContainer/HBoxContainer/SubViewportContainer/SubViewport"); 
        subViewPort.AddChild(_grid);
        subViewPort.Size = new Vector2I(720, 400);

        // Add the camera to the game play area
        var gameCameraScene = GD.Load<PackedScene>("res://assets/game_camera/game_camera.tscn");
        var gameCamera = gameCameraScene.Instantiate<GameCamera>();
        subViewPort.AddChild(gameCamera);
        
        var worldMapScene = GD.Load<PackedScene>("res://scenes/world_map/world_map.tscn");
        _worldMap = worldMapScene.Instantiate<WorldMap>();
        AddChild(_worldMap);
        _worldMap.Initialize(_world);
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
