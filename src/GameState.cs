using Godot;
using System;
using Rogue;

public partial class GameState : Node
{
    // This should basically be the game save data...right?
    
    public World World { get; set; }
    public Zone CurrentZone { get; set; }
    public Grid CurrentGrid { get; set; }
    public Party CurrentParty { get; set; }
    
    public override void _Ready()
    {
        base._Ready();
        
        GD.Print("GAMESTATE IS READY");
    }

    public Godot.Collections.Dictionary<string, Variant> SaveData()
    {
        return new Godot.Collections.Dictionary<string, Variant>()
        {
            { "World", World.SaveData() }
        };
    }

    public void LoadData(Godot.Collections.Dictionary<string, Variant> saveData)
    {
        GD.Print("GAME STATE LOAD DATA");
        
        // TODO: Call "LoadData" on game objects instead of starting a new game

        var world = new World();
        world.LoadData(saveData);
        //world.Initialize();  // This initializes a new World, so we prob don't need it here
        World = world;
    }
}
