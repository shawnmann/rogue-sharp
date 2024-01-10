using Godot;
using System;

public partial class MainMenu : Control
{
    private Button _newGameButton;
    private Button _quitButton;
    
    public override void _Ready()
    {
        base._Ready();

        _newGameButton = GetNode<Button>("%NewGameButton");
        _newGameButton.Pressed += NewGameButtonOnPressed;

        _quitButton = GetNode<Button>("%QuitButton");
        _quitButton.Pressed += QuitButtonOnPressed;
    }

    private void NewGameButtonOnPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/main/main.tscn");
    }
    
    private void QuitButtonOnPressed()
    {
        GetTree().Quit();
    }
}
