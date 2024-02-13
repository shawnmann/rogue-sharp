using Godot;
using System;
using Rogue;

public partial class Looker : Node2D
{
    private GameState _gameState;

    [Signal]
    public delegate void LookerLookedEventHandler(Vector2I gridPosition);

    private ColorRect _looker;
    private Vector2I _gridPosition;
	
    public override void _Ready()
    {
        base._Ready();
		
        _gameState = GetNode<GameState>("/root/GameState");

        _looker = new ColorRect();
        _looker.Size = new Vector2(GameConstants.TileWidth, GameConstants.TileHeight);
        _looker.Color = new Color(0, 1, 0);
        AddChild(_looker);

        Visible = false;
    }

    public override void _Process(double delta)
    {
    }

    public void StartLooking(Vector2I gridPosition)
    {
        _gridPosition = gridPosition;
        GD.Print($"Started looking at {gridPosition}");
        Position = GameConstants.ConvertTileToPosition(gridPosition.X, gridPosition.Y);
        Visible = true;
    }

    public void StopLooking()
    {
        // TODO: Should stop this script from running...
        Visible = false;
    }
    
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        
        if (_gameState.CurrentGameState != GameStates.Look)
            return;
        
        var newPosition = Position;
        var didMove = false;

        if (@event.IsActionPressed("move_up"))
        {
            _gridPosition.Y--;
            newPosition.Y = Position.Y - GameConstants.TileHeight; // TODO: Should this track the tile?
            didMove = true;
        } else if (@event.IsActionPressed("move_down"))
        {
            _gridPosition.Y++;
            newPosition.Y = Position.Y + GameConstants.TileHeight;
            didMove = true;
        } else if (@event.IsActionPressed("move_left"))
        {
            _gridPosition.X--;
            newPosition.X = Position.X - GameConstants.TileWidth;
            didMove = true;
        } else if (@event.IsActionPressed("move_right"))
        {
            _gridPosition.X++;
            newPosition.X = Position.X + GameConstants.TileWidth;
            didMove = true;
        }

        if (didMove)
        {
            Position = newPosition;
            EmitSignal(SignalName.LookerLooked, _gridPosition);
        }
    }
}