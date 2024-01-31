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
        var currentZone = new Godot.Collections.Dictionary<string, Variant>()
        {
            { "X", CurrentZone.WorldLocation.X },
            { "Y", CurrentZone.WorldLocation.Y }
        };

        var currentGrid = new Godot.Collections.Dictionary<string, Variant>()
        {
            { "X", CurrentGrid.GridZoneLocation.X },
            { "Y", CurrentGrid.GridZoneLocation.Y }
        };
        
        var saveData = new Godot.Collections.Dictionary<string, Variant>()
        {
            { "CurrentZone", currentZone },
            { "CurrentGrid", currentGrid },
            { "World", World.SaveData() }
        };

        return saveData;
    }

    public void LoadData(Godot.Collections.Dictionary<string, Variant> saveData)
    {
        GD.Print("GAME STATE LOAD DATA");

        // Process the World...
        var world = new World();
        world.LoadData(saveData);
        World = world;

        // Process the CurrentZone...
        var currentZoneDict = (Godot.Collections.Dictionary<string, Variant>)saveData["CurrentZone"];
        var currentZoneWorldLocation = new Vector2I((int)currentZoneDict["X"], (int)currentZoneDict["Y"]);
        CurrentZone = World.Zones[currentZoneWorldLocation.X, currentZoneWorldLocation.Y];
        GD.Print($"CURRENT ZONE WORLD LOC: {currentZoneWorldLocation}");
        
        // Process the CurrentGrid...
        var currentGridDict = (Godot.Collections.Dictionary<string, Variant>)saveData["CurrentGrid"];
        var currentGridZoneLocation = new Vector2I((int)currentGridDict["X"], (int)currentGridDict["Y"]);
        
    }
}
