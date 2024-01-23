using Godot;
using System;

public partial class GameMenu : Control
{
    private Global _global;
    private GameState _gameState;
    
    private Button _saveButton;
    private Button _quitButton;
    
    public override void _Ready()
    {
        base._Ready();
        
        _global = GetNode<Global>("/root/Global");
        _gameState = GetNode<GameState>("/root/GameState");

        _saveButton = GetNode<Button>("%SaveButton");
        _saveButton.Pressed += SaveButtonOnPressed;

        _quitButton = GetNode<Button>("%QuitButton");
        _quitButton.Pressed += QuitButtonOnPressed;

        Visible = false;
    }

    private void QuitButtonOnPressed()
    {
        GetTree().Quit();
    }

    private void SaveButtonOnPressed()
    {
        _global.SaveGame(_gameState);
    }
}
