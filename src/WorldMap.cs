using Godot;
using System;
using Rogue;

public partial class WorldMap : Node2D
{
    private Global _global;
    private GameState _gameState;

    private ColorRect _location;
    private Button _button;
    
    public override void _Ready()
    {
        base._Ready();
        
        _global = GetNode<Global>("/root/Global");
        _gameState = GetNode<GameState>("/root/GameState");

        _location = new ColorRect();
        _location.Size = new Vector2(10, 10);
        _location.Color = new Color(1, 0, 0);
        
        _button = GetNode<Button>("Button");
        _button.Pressed += ButtonOnPressed;
        _button.GlobalPosition = new Vector2(0, 300);
        
        DrawMap();

        Visible = false;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        HighlightLocation();
    }

    private void DrawMap()
    {
        var width = _gameState.World.Width;
        var height = _gameState.World.Height;
        
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                ColorRect rect = new ColorRect();
                rect.Color = _gameState.World.WorldInfo[x, y].BiomeType switch
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

                var biome = _gameState.World.WorldInfo[x, y].BiomeType;
                rect.MouseEntered += () => GD.Print(biome);
            }
        }
        
        AddChild(_location);
        HighlightLocation();
    }

    private void HighlightLocation()
    {
        _location.GlobalPosition = new Vector2(_gameState.CurrentZone.WorldLocation.X * 10, 
            _gameState.CurrentZone.WorldLocation.Y * 10);
    }
    
    private void ButtonOnPressed()
    {
        foreach (var node in GetTree().GetNodesInGroup("test_tiles"))
        {
            node.QueueFree();
        }
        
        // TODO: Generate world...
        //_gameState.World.GenerateWorldBiomes();
    }
}
