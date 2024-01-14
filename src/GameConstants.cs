using Godot;
using System;
using System.Collections.Generic;
using Rogue;
using Rogue.Generation.World;

public static class GameConstants
{
    // World size in zones
    public const int WorldWidth = 80;
    public const int WorldHeight = 25;
    
    // Zone size in grids
    public const int ZoneWidth = 3;
    public const int ZoneHeight = 3;
    
    // Grid size in tiles
    public static int GridWidth => 80;
    public static int GridHeight => 25;
    
    // Tile size in pixels
    public static int TileWidth => 16;
    public static int TileHeight => 16;

    public static readonly List<Biome> Biomes = new()
    {
        // Deserts
        new Biome() { BiomeType = BiomeTypes.Desert, Moisture = BiomeMoistures.Dry, Temp = BiomeTemps.Hot },
        new Biome() { BiomeType = BiomeTypes.Scrubland, Moisture = BiomeMoistures.Dry, Temp = BiomeTemps.Temperate },
        new Biome() { BiomeType = BiomeTypes.Tundra, Moisture = BiomeMoistures.Dry, Temp = BiomeTemps.Cold },
        // Plains
        new Biome() { BiomeType = BiomeTypes.Savanna, Moisture = BiomeMoistures.Moderate, Temp = BiomeTemps.Hot },
        new Biome() { BiomeType = BiomeTypes.Plains, Moisture = BiomeMoistures.Moderate, Temp = BiomeTemps.Temperate },
        new Biome() { BiomeType = BiomeTypes.Steppe, Moisture = BiomeMoistures.Moderate, Temp = BiomeTemps.Cold },
        // Forests
        new Biome() { BiomeType = BiomeTypes.Jungle, Moisture = BiomeMoistures.Wet, Temp = BiomeTemps.Hot },
        new Biome() { BiomeType = BiomeTypes.Forest, Moisture = BiomeMoistures.Wet, Temp = BiomeTemps.Temperate },
        new Biome() { BiomeType = BiomeTypes.Boreal, Moisture = BiomeMoistures.Wet, Temp = BiomeTemps.Cold }
    };

    public static Biome GetBiome(BiomeTypes? biomeType)
    {
        return Biomes.Find(b => b.BiomeType == biomeType);
    }

    public static Biome GetBiome(BiomeMoistures moisture, BiomeTemps temp)
    {
        return Biomes.Find(b => b.Moisture == moisture && b.Temp == temp);
    }
    
    public static Vector2 ConvertTileToPosition(int x, int y)
    {
        return new Vector2(x * TileWidth, y * TileHeight);
    }
}
