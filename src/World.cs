using Godot;
using Rogue;
using Rogue.Generation.World;

namespace Rogue;

public partial class World : Node, ISaveable
{
    private Global _global;
    private GameState _gameState;
    
    public int Width = GameConstants.WorldWidth;
    public int Height = GameConstants.WorldHeight;
    public Biome[,] WorldInfo;
    public Zone[,] Zones;

    public void Initialize()
    {
        // Procedurally generate the world
        GenerateWorldBiomes();
        
        // Create the Zones
        InitializeZones();
    }

    public void InitializeZones()
    {
        // Initialize the Zones using the world size
        Zones = new Zone[GameConstants.WorldWidth, GameConstants.WorldHeight];
        
        for (var x = 0; x < Zones.GetLength(0); x++)
        {
            for (var y = 0; y < Zones.GetLength(1); y++)
            {
                var biomeType = WorldInfo[x, y].BiomeType;
                var z = new Zone();
                z.Initialize(new Vector2I(x, y), biomeType);
                Zones[x, y] = z;
            }
        }
    }

    public void GenerateWorldBiomes()
    {
        // Proc gen the biomes of a world map
        var noise = new FastNoise(GameConstants.WorldWidth, GameConstants.WorldHeight);
        WorldInfo = noise.MapInfo;
    }

    public Godot.Collections.Dictionary<string, Variant> SaveData()
    {
        // Serialize this World...
        
        // Process WorldInfo...
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
        
        // Process Zones...
        /*var zones = new Godot.Collections.Dictionary<string, Variant>();
        for (var x = 0; x < Zones.GetLength(0); x++)
        {
            for (var y = 0; y < Zones.GetLength(1); y++)
            {
                var zone = Zones[x, y];
            }
        }*/
        
        // Create and load the Dictionary
        var saveData = new Godot.Collections.Dictionary<string, Variant>
        {
            { "Width", Width },
            { "Height", Height },
            { "WorldInfo", worldInfo }
        };

        return saveData;
    }

    public void LoadData(Godot.Collections.Dictionary<string, Variant> saveData)
    {
        // Deserialize this World...
        
        if (saveData == null)
        {
            GD.Print("LOADED GAME DATA IS NULL");
            // TODO: Need to handle this gracefully, because even returning here is going to cause the game to error...
            return;
        }
        saveData = (Godot.Collections.Dictionary<string, Variant>)saveData["World"];
        
        Width = (int)saveData["Width"];
        Height = (int)saveData["Height"];
        
        // Process WorldInfo...
        var worldInfoDict = (Godot.Collections.Dictionary<string, Variant>)saveData["WorldInfo"];
        WorldInfo = new Biome[Width, Height];
        
        foreach (var key in worldInfoDict.Keys)
        {
            // Grab the coords out...
            var coords = key.Trim('(', ')').Split(',');
            var x = int.Parse(coords[0]);
            var y = int.Parse(coords[1]);
            
            // Grab the biome info out...
            var biomeDict = (Godot.Collections.Dictionary<string, Variant>)worldInfoDict[key];
            var biomeType = (BiomeTypes)(int)biomeDict["BiomeType"];
            var temp = (BiomeTemps)(int)biomeDict["Temp"];
            var moisture = (BiomeMoistures)(int)biomeDict["Moisture"];
            
            // Set the WorldInfo for this location...
            WorldInfo[x, y] = new Biome() { BiomeType = biomeType, Temp = temp, Moisture = moisture };
        }
        
        // Process Zones...
        InitializeZones();
    }
}
