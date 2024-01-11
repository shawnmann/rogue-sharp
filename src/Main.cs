using Godot;
using System;
using Rogue;

public partial class Main : Node
{
    private World _world;
    private Grid _grid;
    
    public override void _Ready()
    {
        base._Ready();

        _world = new World();
        _world.Initialize();
        GD.Print("CREATE WORLD:");
        GD.Print(_world.Zones);

        _grid = new Grid();

        // Add the game play area as a SubViewPort in the layout of Main
        var subViewPort = GetNode<SubViewport>("VBoxContainer/HBoxContainer/SubViewportContainer/SubViewport"); 
        subViewPort.AddChild(_grid);
        subViewPort.Size = new Vector2I(720, 400);

        // Add the camera to the game play area
        var gameCameraScene = GD.Load<PackedScene>("res://assets/game_camera/game_camera.tscn");
        var gameCamera = gameCameraScene.Instantiate<GameCamera>();
        subViewPort.AddChild(gameCamera);
    }
}
