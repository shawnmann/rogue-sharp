using Godot;
using System;
using Rogue;

public partial class WorldMap : Node2D
{
    private Button _button;
    
    public override void _Ready()
    {
        base._Ready();
        
        _button = GetNode<Button>("Button");
        _button.Pressed += ButtonOnPressed;
        _button.GlobalPosition = new Vector2(0, 300);
    }

    public void Initialize(World world)
    {
        
    }
    
    private void ButtonOnPressed()
    {
        foreach (var node in GetTree().GetNodesInGroup("test_tiles"))
        {
            node.QueueFree();
        }
        
        var width = GameConstants.WorldWidth;
        var height = GameConstants.WorldHeight;
        var noise = new FastNoise(width, height);
        
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                ColorRect rect = new ColorRect();
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
