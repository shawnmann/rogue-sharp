using Godot;

namespace Rogue;

public partial class World : Node
{
    public Zone[,] Zones;

    public override void _Ready()
    {
        base._Ready();
    }

    public void Initialize()
    {
        // Initialize the Zones using the world size
        Zones = new Zone[GameConstants.WorldWidth, GameConstants.WorldHeight];
        
        // TODO: Load up the zones...
        for (var x = 0; x < Zones.GetLength(0); x++)
        {
            for (var y = 0; y < Zones.GetLength(1); y++)
            {
                // TODO: Basically here should tell the zone something about itself
                //  like it's biome, etc., to help in its generation
                var z = new Zone();
                z.Initialize();
                Zones[x, y] = z;
            }
        }
    }
}
