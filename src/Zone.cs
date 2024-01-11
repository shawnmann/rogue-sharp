using Godot;

namespace Rogue;

public partial class Zone : Node
{
    // TODO: Two dimensional Grid array to capture the grids within this zone
    //  Info about this Zone, like biome, etc. that helps us generate the
    //  grids when the player enters them

    public Grid[,] Grids;

    public override void _Ready()
    {
        base._Ready();
    }

    public void Initialize()
    {
        // Initialize using the Zone size
        Grids = new Grid[GameConstants.ZoneWidth, GameConstants.ZoneHeight];
        
        // TODO: Load up the grids (but really we'll do this on demand as the user
        //  enters each grid)
    }
}
