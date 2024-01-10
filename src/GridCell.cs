using Godot;
using System;
using System.Collections.Generic;

namespace Rogue;

public class GridCell
{
    // This is the list of the tiles that are still acceptable
    //  to place in this GridCell
    public List<Tile> PossibleTiles { get; init; }
        
    // This is the entropy of this cell
    public int Entropy => PossibleTiles.Count;
        
    // x and y location of this cell
    public int X { get; init; }
    public int Y { get; init; }
        
    // Is this cell solved?
    public bool Solved { get; set; }
        
    // Which tile has been assigned to this cell?
    public Tile Tile { get; set; }
}
