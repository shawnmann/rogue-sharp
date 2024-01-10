using Godot;
using System;

public static class GameConstants
{
    // World size in zones
    public static int WorldWidth = 80;
    public static int WorldHeight = 25;
    
    // Zone size in grids
    public static int ZoneWidth = 3;
    public static int ZoneHeight = 3;
    
    // Grid size in tiles
    public static int GridWidth => 80;
    public static int GridHeight => 25;
    
    // Tile size in pixels
    public static int TileWidth => 16;
    public static int TileHeight => 16;
    
    public static Vector2 ConvertTileToPosition(int x, int y)
    {
        return new Vector2(x * TileWidth, y * TileHeight);
    }
}
