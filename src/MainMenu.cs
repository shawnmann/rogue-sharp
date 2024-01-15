using Godot;
using System;

public partial class MainMenu : Control
{
    private Global _global;
    private Button _newGameButton;
    private Button _quitButton;
    
    public override void _Ready()
    {
        base._Ready();
        
        _global = GetNode<Global>("/root/Global");

        _newGameButton = GetNode<Button>("%NewGameButton");
        _newGameButton.Pressed += NewGameButtonOnPressed;

        _quitButton = GetNode<Button>("%QuitButton");
        _quitButton.Pressed += QuitButtonOnPressed;
    }

    private void NewGameButtonOnPressed()
    {
        _global.GotoScene("res://scenes/main/main.tscn");
    }
    
    private void QuitButtonOnPressed()
    {
        GetTree().Quit();
    }
}
