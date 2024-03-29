using Godot;

namespace Rogue;

public partial class Zone : Node
{
    // TODO: Two dimensional Grid array to capture the grids within this zone
    //  Info about this Zone, like biome, etc. that helps us generate the
    //  grids when the player enters them

    public Grid[,] Grids;
    public Vector2I WorldLocation;
    public BiomeTypes BiomeType;

    public void Initialize(Vector2I worldLocation, BiomeTypes biomeType)
    {
        WorldLocation = worldLocation;
        BiomeType = biomeType;
        
        // Initialize using the Zone size
        Grids = new Grid[GameConstants.ZoneWidth, GameConstants.ZoneHeight];
    }
}
