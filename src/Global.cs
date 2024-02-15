using Godot;
using System;

public partial class Global : Node
{
    // This will handle functions across the game, like loading scenes, etc.
    
    // This is the current scene that we are, in the sense of "main" scenes (MainMenu, Main, etc.)
    public Node CurrentScene { get; set; }
    
    // Holds loaded game save data
    public Godot.Collections.Dictionary<string, Variant> LoadedGameData;
    
    public override void _Ready()
    {
        base._Ready();
        
        GD.Print("GLOBAL IS READY");

        // The last child of root is always the loaded scene
        Viewport root = GetTree().Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
    }

    public void GotoScene(string path)
    {
        // This function will usually be called from a signal callback,
        // or some other function from the current scene.
        // Deleting the current scene at this point is
        // a bad idea, because it may still be executing code.
        // This will result in a crash or unexpected behavior.

        // The solution is to defer the load to a later time, when
        // we can be sure that no code from the current scene is running:

        CallDeferred(MethodName.DeferredGotoScene, path);
    }

    private void DeferredGotoScene(string path)
    {
        // It is now safe to remove the current scene...
        CurrentScene.Free();
        
        // Load a new scene
        var nextScene = GD.Load<PackedScene>(path);
        CurrentScene = nextScene.Instantiate();
        
        // Add it to the active scene as a child of root
        GetTree().Root.AddChild(CurrentScene);
        
        // Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
        GetTree().CurrentScene = CurrentScene;
    }

    public T GetFirstChildOfType<T>(Node node) where T : Node
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is T typedChild)
                return typedChild;
        }
        return null;
    }

    public void SaveGame(GameState gameState)
    {
        using var saveFile = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
        var s = Json.Stringify(gameState.SaveData());
        saveFile.StoreLine(s);
    }
    
    public void LoadGame()
    {
        if (!FileAccess.FileExists("user://savegame.save"))
        {
            GD.Print("FILE NOT FOUND!");
            return;
        }
        
        // TODO: Clear game out if we ever load from a running game...
        
        // Load the file
        using var saveFile = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);

        // Read the file line by line
        while (saveFile.GetPosition() < saveFile.GetLength())
        {
            var jsonString = saveFile.GetLine();

            var json = new Json();
            var parseResult = json.Parse(jsonString);
            
            if (parseResult != Error.Ok)
            {
                GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
            }
            
            // Get the data from Json and put it into a Dictionary
            var nodeData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)json.Data);
            
            // Set the property on Global so that GameState can use it to load the game up
            LoadedGameData = nodeData;
            
            GD.Print("GAME LOAD COMPLETE");
        }
    }
}
