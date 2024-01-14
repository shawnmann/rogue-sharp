using Godot;
using System;
using Rogue;

public partial class Main : Node
{
    private World _world;
    private Grid _grid;
    private Button _button;
    
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

        _button = GetNode<Button>("Button");
        _button.Pressed += ButtonOnPressed;
        _button.GlobalPosition = new Vector2(0, 300);
    }

    private void ButtonOnPressed()
    {
        foreach (var node in GetTree().GetNodesInGroup("test_tiles"))
        {
            node.QueueFree();
        }
        
        var width = 80;
        var height = 25;
        var noise = new FastNoise(width, height);
        
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                // Get the noise coordinate as an
                //  absolute value (which represents the layer)
                //float absNoise = Math.Abs(noise.elevation.GetNoise2D(x, y));

                ColorRect rect = new ColorRect();
                //rect.Color = new Color(absNoise, 0, 0);
                rect.Color = noise.MapInfo[x, y].BiomeType switch
                {
                    BiomeTypes.Desert => new Color(1, 0.9f, 0),
                    BiomeTypes.Scrubland => new Color(1, 0.7f, 0),
                    BiomeTypes.Tundra => new Color(0.8f, 0.9f, 1),
                    
                    BiomeTypes.Savanna => new Color(0.9f, 0.9f, 0),
                    BiomeTypes.Plains => new Color(0.7f, 0.9f, 0),
                    BiomeTypes.Steppe => new Color(0.7f, 0.9f, 0.7f),
                    
                    BiomeTypes.Jungle => new Color(0, 0.6f, 0.125f),
                    BiomeTypes.Forest => new Color(0, 0.8f, 0.125f),
                    BiomeTypes.Boreal => new Color(0, 1, 0.125f),
                    _ => rect.Color
                };
                rect.Size = new Vector2(10, 10);
                rect.GlobalPosition = new Vector2(x * 10, y * 10);
                AddChild(rect);
                rect.AddToGroup("test_tiles");

                var biome = noise.MapInfo[x, y].BiomeType;
                rect.MouseEntered += () => GD.Print(biome);
            }
        }
    }
}
