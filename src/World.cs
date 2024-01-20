using Godot;
using Rogue;
using Rogue.Generation.World;

namespace Rogue;

public partial class World : Node
{
    public int Width = GameConstants.WorldWidth;
    public int Height = GameConstants.WorldHeight;
    public Biome[,] WorldInfo;
    public Zone[,] Zones;

    public override void _Ready()
    {
        base._Ready();
    }

    public void Initialize()
    {
        // Generate the world
        GenerateWorldBiomes();
        
        // Initialize the Zones using the world size
        Zones = new Zone[GameConstants.WorldWidth, GameConstants.WorldHeight];
        
        // TODO: Load up the zones...
        for (var x = 0; x < Zones.GetLength(0); x++)
        {
            for (var y = 0; y < Zones.GetLength(1); y++)
            {
                // TODO: Basically here should tell the zone something about itself
                //  like it's biome, etc., to help in its generation
                var biomeType = WorldInfo[x, y].BiomeType;
                
                var z = new Zone();
                z.Initialize(new Vector2I(x, y), biomeType);
                Zones[x, y] = z;
            }
        }
    }

    public void GenerateWorldBiomes()
    {
        var noise = new FastNoise(GameConstants.WorldWidth, GameConstants.WorldHeight);
        WorldInfo = noise.MapInfo;
    }
}
