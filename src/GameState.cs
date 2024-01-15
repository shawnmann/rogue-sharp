using Godot;
using System;

public partial class GameState : Node
{
    // This should basically be the game save data...right?
    
    public override void _Ready()
    {
        base._Ready();
        
        GD.Print("GAMESTATE IS READY");
    }
}
