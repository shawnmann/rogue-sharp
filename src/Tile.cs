using Godot;
using System;
using System.Collections.Generic;
using Rogue;

public class Tile
{
    // This is the id to help compare tiles...
    public int Id { get; init; }
    
    // This is this Tile's position within its Grid...
    public Vector2I Position { get; set; }
    
    // This is the "value" of the cell,
    //  which for now is a placeholder for the graphical
    //  representation of the tile
    public char Value { get; init; }
    
    // Sockets are the list of tiles that can be placed on
    //  each side of this tile
    public List<TileSocket> Sockets { get; set; }
}
