using Godot;
using System;
using Rogue;

public partial class Generator : Node
{
    private Global _global;
    private GameState _gameState;
    
    private World _world;
    
    public override void _Ready()
    {
        base._Ready();
        
        GD.Print("GENERATOR IS READY");
        
        _global = GetNode<Global>("/root/Global");
        _gameState = GetNode<GameState>("/root/GameState");
        
        _world = new World();
        _world.Initialize();
        _gameState.World = _world;
        
        _global.GotoScene("res://scenes/main/main.tscn");
    }
}
