using Godot;
using System;
using System.Collections.Generic;
using Rogue;

public partial class Party : Node2D
{
	private Global _global;
	private GameState _gameState;
	
	[Signal]
	public delegate void PartyLeavingGridEventHandler(Vector2I partyGridPosition, Vector2I partyDirection);
	
	// This will hold where on the grid the party is "located",
	//  which is just where the party leader is located
    private Vector2I _currentGridPosition = new(10, 5);
    private List<PlayerCharacter> _playerCharacters;
    
    public override void _Ready()
    {
        base._Ready();
        
        _gameState = GetNode<GameState>("/root/GameState");
        
        // Load the PlayerCharacter scene
        var playerCharacterScene = GD.Load<PackedScene>("res://assets/player_character/player_character.tscn");
        
        // Load the tile set texture
        var texture = GD.Load<Texture2D>("res://assets/tilesets/colored_packed.png");
        
        // Instance a player_character so we can make a player character
		var pc1 = playerCharacterScene.Instantiate<PlayerCharacter>();
		
		// Add this instance as a child of the Squad
		AddChild(pc1);
		
		// Load the sprite node of the pc
		var sprite = pc1.GetNode<Sprite2D>("Sprite2D");
		sprite.Texture = texture;
		sprite.RegionEnabled = true;
		sprite.RegionRect = new Rect2(new Vector2(448, 0), new Vector2(GameConstants.TileWidth, GameConstants.TileHeight));
		sprite.Centered = false;
		pc1.Position = GameConstants.ConvertTileToPosition(_currentGridPosition.X, _currentGridPosition.Y);
		
		// Instance a player_character so we can make a player character
		var pc2 = playerCharacterScene.Instantiate<PlayerCharacter>();
		
		// Add this instance as a child of the Squad
		AddChild(pc2);
		
		// Load the sprite node of the pc
		var sprite2 = pc2.GetNode<Sprite2D>("Sprite2D");
		sprite2.Texture = texture;
		sprite2.RegionEnabled = true;
		sprite2.RegionRect = new Rect2(new Vector2(448, 16), new Vector2(GameConstants.TileWidth, GameConstants.TileHeight));
		sprite2.Centered = false;
		pc2.Position = GameConstants.ConvertTileToPosition(_currentGridPosition.X - 1, _currentGridPosition.Y);
		
		// Instance a player_character so we can make a player character
		var pc3 = playerCharacterScene.Instantiate<PlayerCharacter>();
		
		// Add this instance as a child of the Squad
		AddChild(pc3);
		
		// Load the sprite node of the pc
		var sprite3 = pc3.GetNode<Sprite2D>("Sprite2D");
		sprite3.Texture = texture;
		sprite3.RegionEnabled = true;
		sprite3.RegionRect = new Rect2(new Vector2(416, 16), new Vector2(GameConstants.TileWidth, GameConstants.TileHeight));
		sprite3.Centered = false;
		pc3.Position = GameConstants.ConvertTileToPosition(_currentGridPosition.X - 2, _currentGridPosition.Y);
		
		// Instance a player_character so we can make a player character
		var pc4 = playerCharacterScene.Instantiate<PlayerCharacter>();
		
		// Add this instance as a child of the Squad
		AddChild(pc4);
		
		// Load the sprite node of the pc
		var sprite4 = pc4.GetNode<Sprite2D>("Sprite2D");
		sprite4.Texture = texture;
		sprite4.RegionEnabled = true;
		sprite4.RegionRect = new Rect2(new Vector2(384, 16), new Vector2(GameConstants.TileWidth, GameConstants.TileHeight));
		sprite4.Centered = false;
		pc4.Position = GameConstants.ConvertTileToPosition(_currentGridPosition.X - 3, _currentGridPosition.Y);
		
		// Add characters to list of pcs in the party
		_playerCharacters = new List<PlayerCharacter>()
		{
			pc1, pc2, pc3, pc4
		};
		
		// TODO: There has to be a better way to do this
		// Add the party to the "party" group so other scenes
		//  can grab it
		AddToGroup("party");
    }

    public override void _Input(InputEvent @event)
    {
	    base._Input(@event);
	    
	    if (_gameState.CurrentGameState != GameStates.Game)
		    return;

	    // This will hold where the party is trying to move to, if they are moving
	    var targetCellPosition = _currentGridPosition;
	    // Can the party move to their target?
	    var isMoving = false;
	    
	    // Check for any actions the party cares about
	    if (@event.IsActionPressed("move_up"))
	    {
		    targetCellPosition.Y--;
		    isMoving = true;
	    } else if (@event.IsActionPressed("move_down"))
	    {
		    targetCellPosition.Y++;
		    isMoving = true;
	    } else if (@event.IsActionPressed("move_left"))
	    {
		    targetCellPosition.X--;
		    isMoving = true;
	    } else if (@event.IsActionPressed("move_right"))
	    {
		    targetCellPosition.X++;
		    isMoving = true;
	    }
	    
	    // If the party can move, then move them to the target
	    if (isMoving) ProcessMovement(targetCellPosition);
    }

    private void ProcessMovement(Vector2I targetCellPosition)
    {
	    // Where the party started from
	    var initialCellPosition = _currentGridPosition;
	    
	    // What direction they are trying to move
	    var direction = new Vector2I(initialCellPosition.X - targetCellPosition.X, 
		    initialCellPosition.Y - targetCellPosition.Y);
	    
	    // Check if they are going off the grid
	    if (targetCellPosition.X >= GameConstants.GridWidth 
	        || targetCellPosition.Y >= GameConstants.GridHeight
	        || targetCellPosition.X < 0
	        || targetCellPosition.Y < 0
	        )
	    {
		    // We're trying to move outside the bounds of the grid,
		    //  eventually that will move them to the next screen,
		    //  but for now just don't let them move
		    GD.Print("CAN'T MOVE OFF THE GRID");

		    EmitSignal(SignalName.PartyLeavingGrid, _currentGridPosition, direction);
		    
		    return;
	    }
	    
	    // We're ok to move here, so set the party's position
	    _currentGridPosition = targetCellPosition;
			
	    // Process movement for the rest of the pcs
	    for (var i = _playerCharacters.Count - 1; i > 0; i--)
	    {
		    _playerCharacters[i].Position = _playerCharacters[i - 1].Position;
	    }
			
	    // Process movement for first pc
	    _playerCharacters[0].Position = GameConstants.ConvertTileToPosition(_currentGridPosition.X, _currentGridPosition.Y);
	    
	    GD.Print($"MOVE FROM ({initialCellPosition.X},{initialCellPosition.Y}) TO ({_currentGridPosition.X},{_currentGridPosition.Y})");
    }

    public Vector2I GetCurrentGridPosition()
    {
	    return _currentGridPosition;
    }

    public void SetCurrentGridPosition(Vector2I position, Vector2I direction)
    {
	    var newPosition = GameConstants.ConvertTileToPosition(position.X, position.Y);
	    _playerCharacters[0].Position = newPosition;
	    
	    // Need to set party's actual position
	    for (var i = 1; i < _playerCharacters.Count; i++)
	    {
		    if (direction == new Vector2I(0, -1) || direction == new Vector2I(0, 1)) {
			    // Line them up right to left, because they're entering from the top or bottom...
			    _playerCharacters[i].Position = new Vector2(newPosition.X - GameConstants.TileWidth * i, newPosition.Y);
		    }
		    else
		    {
			    // Line them up top to bottom, because they're coming from the left or right...
			    _playerCharacters[i].Position = new Vector2(newPosition.X, newPosition.Y - GameConstants.TileHeight * i);
		    }
	    }
	    
	    // Set the party's current grid position
	    _currentGridPosition = position;
    }
}
