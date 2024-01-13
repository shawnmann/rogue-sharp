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
                switch (noise.MapInfo[x, y])
                {
                    case "W":
                        rect.Color = new Color(0, 0.569f, 1);
                        break; 
                    case "TROPICAL_RAINFOREST":
                        rect.Color = new Color(0, 0.769f, 0.239f);
                        break;
                    case "SAVANNA":
                        rect.Color = new Color(0.922f, 0.902f, 0);
                        break;
                    case "DESERT":
                        rect.Color = new Color(0.922f, 0.561f, 0);
                        break;
                    case "TEMPERATE_RAINFOREST":
                        rect.Color = new Color(0, 0.631f, 0.71f);
                        break;
                    case "TEMPERATE_SEASONAL_RAINFOREST":
                        rect.Color = new Color(0, 0.678f, 0.851f);
                        break;
                    case "SHRUBLAND":
                        rect.Color = new Color(0.988f, 0.69f, 0);
                        break;
                    case "TEMPERATE_GRASSLAND":
                        rect.Color = new Color(0.812f, 0.69f, 0.31f);
                        break;
                    case "BOREAL_FOREST":
                        rect.Color = new Color(0.29f, 0.89f, 0.69f);
                        break;
                    case "TUNDRA":
                        rect.Color = new Color(0.91f, 0.949f, 0.941f);
                        break;
                    case "X":
                        rect.Color = new Color(0, 0, 0);
                        break;
                }
                rect.Size = new Vector2(10, 10);
                rect.GlobalPosition = new Vector2(x * 10, y * 10);
                AddChild(rect);
                rect.AddToGroup("test_tiles");
            }
        }
    }
}
