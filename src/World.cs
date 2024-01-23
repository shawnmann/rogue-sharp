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

    public Godot.Collections.Dictionary<string, Variant> SaveData()
    {
        // Process WorldInfo
        var worldInfo = new Godot.Collections.Dictionary<string, Variant>();
        for (var x = 0; x < WorldInfo.GetLength(0); x++)
        {
            for (var y = 0; y < WorldInfo.GetLength(1); y++)
            {
                var biome = WorldInfo[x, y];
                var biomeDict = new Godot.Collections.Dictionary<string, Variant>()
                {
                    { "X", x },
                    { "Y", y },
                    { "BiomeType", (int)biome.BiomeType },
                    { "Temp", (int)biome.Temp },
                    { "Moisture", (int)biome.Moisture }
                };
                worldInfo.Add($"({x},{y})", biomeDict);
            }
        }
        
        // Create and load the Dictionary
        var saveData = new Godot.Collections.Dictionary<string, Variant>
        {
            { "Width", Width },
            { "Height", Height },
            { "WorldInfo", worldInfo }
        };

        return saveData;
    }

    public void LoadData(Godot.Collections.Dictionary saveData)
    {
        Width = (int)saveData["Width"];
        Height = (int)saveData["Height"];
        var worldInfoDict = (Godot.Collections.Dictionary<string, Variant>)saveData["WorldInfo"];
        WorldInfo = new Biome[Width, Height];
        foreach (var key in worldInfoDict.Keys)
        {
            var coords = key.Trim('(', ')').Split(',');
            var x = int.Parse(coords[0]);
            var y = int.Parse(coords[1]);
            var biomeDict = (Godot.Collections.Dictionary<string, Variant>)worldInfoDict[key];
            var biomeType = (BiomeTypes)(int)biomeDict["BiomeType"];
            var temp = (BiomeTemps)(int)biomeDict["Temp"];
            var moisture = (BiomeMoistures)(int)biomeDict["Moisture"];
            WorldInfo[x, y] = new Biome() { BiomeType = biomeType, Temp = temp, Moisture = moisture };
        }
    }
}
