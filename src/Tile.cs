using Godot;
using System;
using System.Collections.Generic;
using Rogue;

public class Tile
{
    // This is the location of the Tile within the Grid. This
    //  will also serve as the "id" of the grid
    public int Id { get; init; }
    
    // This is the "value" of the cell,
    //  which for now is a placeholder for the graphical
    //  representation of the tile
    public char Value { get; init; }
    
    // Sockets are the list of tiles that can be placed on
    //  each side of this tile
    public List<TileSocket> Sockets { get; set; }
}
