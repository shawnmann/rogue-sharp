using Godot;
using System;

public partial class Tree : Node2D
{
    public EntityComponent EntityComponent;
    public HealthComponent HealthComponent;
    public StatsComponent StatsComponent;
    
    public override void _Ready()
    {
        base._Ready();

        EntityComponent = GetNode<EntityComponent>("EntityComponent");
        HealthComponent = GetNode<HealthComponent>("HealthComponent");
        StatsComponent = GetNode<StatsComponent>("StatsComponent");
    }

    public void Initialize()
    {
        // What can trees support?
        //  Type of tree
        //  Flammable
        //  Can be chopped down
        
        // Load the tile set texture
        var texture = GD.Load<Texture2D>("res://assets/tilesets/colored_packed.png");
        
        var sprite = GetNode<Sprite2D>("Sprite2D");
        sprite.Texture = texture;
        sprite.RegionEnabled = true;
        sprite.RegionRect = new Rect2(new Vector2(16, 16), new Vector2(GameConstants.TileWidth, GameConstants.TileHeight));
        sprite.Centered = false;
    }
}
