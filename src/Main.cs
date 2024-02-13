using Godot;
using System;
using Rogue;

public partial class Main : Node
{
    private Global _global;
    private GameState _gameState;
    
    private Grid _grid;
    private WorldMap _worldMap;
    private GameMenu _gameMenu;
    private Party _party;
    private Looker _looker;
    
    private SubViewport _subViewport;
    private HBoxContainer _headerHBoxContainer;
    private Label _headerGameModeLabel;
    private Label _headerZoneLabel;
    private Label _headerGridLabel;
    
    public override void _Ready()
    {
        base._Ready();

        _global = GetNode<Global>("/root/Global");
        _gameState = GetNode<GameState>("/root/GameState");

        // Construct the header...
        _headerHBoxContainer = GetNode<HBoxContainer>("VBoxContainer/HeaderHBoxContainer");
        _headerGameModeLabel = new Label();
        _headerHBoxContainer.AddChild(_headerGameModeLabel);
        _headerZoneLabel = new Label();
        _headerHBoxContainer.AddChild(_headerZoneLabel);
        _headerGridLabel = new Label();
        _headerHBoxContainer.AddChild(_headerGridLabel);
        
        InitializeGrid();

        var gameMenuScene = GD.Load<PackedScene>("res://scenes/game_menu/game_menu.tscn");
        _gameMenu = gameMenuScene.Instantiate<GameMenu>();
        AddChild(_gameMenu);

        // Start the game in "playing" mode
        _gameState.CurrentGameState = GameStates.Game;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        _headerGameModeLabel.Text = $"Mode: {_gameState.CurrentGameState}";
        _headerZoneLabel.Text = $" - Zone: {_gameState.CurrentZone.WorldLocation}";
        _headerGridLabel.Text = $" - Grid: {_gameState.CurrentGrid.GridZoneLocation}";
    }

    private void InitializeGrid()
    {
        // If this is a new game, set the starting zone, if not, this was already set from the save file...
        if (_global.LoadedGameData == null)
        {
            var zone = _gameState.World.Zones[5, 5];
            _gameState.CurrentZone = zone;
        }

        // Add the game play area as a SubViewPort in the layout of Main
        _subViewport = GetNode<SubViewport>("VBoxContainer/HBoxContainer/SubViewportContainer/SubViewport"); 
        _subViewport.Size = new Vector2I(720, 400);

        // Create the initial Grid
        _grid = new Grid();
        _subViewport.AddChild(_grid);
        // Need to load this from a save later, but for now, let's new game it and make the starting Grid
        //  the center section of the Zone...
        _grid.Initialize(_gameState.CurrentZone, new Vector2I(1, 1));
        _gameState.CurrentGrid = _grid;
        
        // Add the party as a child of the Grid
        var partyScene = GD.Load<PackedScene>("res://assets/party/party.tscn");
        _party = partyScene.Instantiate<Party>();
        _grid.AddChild(_party);
        _party.PartyLeavingGrid += PartyOnPartyLeavingGrid;
        _gameState.CurrentParty = _party;

        // Add the camera to the game play area
        var gameCameraScene = GD.Load<PackedScene>("res://assets/game_camera/game_camera.tscn");
        var gameCamera = gameCameraScene.Instantiate<GameCamera>();
        _subViewport.AddChild(gameCamera);
        
        // Add the world map scene
        var worldMapScene = GD.Load<PackedScene>("res://scenes/world_map/world_map.tscn");
        _worldMap = worldMapScene.Instantiate<WorldMap>();
        AddChild(_worldMap);

        var treeScene = GD.Load<PackedScene>("res://assets/tree/tree.tscn");
        var tree = treeScene.Instantiate<Tree>();
        _grid.AddChild(tree);
        tree.Initialize();
        tree.Position = new Vector2(10 * GameConstants.TileWidth, 10 * GameConstants.TileHeight);
        GD.Print($"{tree.EntityComponent.EntityName} HP: {tree.HealthComponent.CurrentHealth}/{tree.HealthComponent.MaxHealth}");
        GD.Print($"{tree.EntityComponent.EntityName} {tree.StatsComponent.QuickName}: {tree.StatsComponent.Quick}");
        
        // Add the looker scene
        var lookerScene = GD.Load<PackedScene>("res://assets/looker/looker.tscn");
        _looker = lookerScene.Instantiate<Looker>();
        _grid.AddChild(_looker);
        _looker.LookerLooked += LookerOnLookerLooked;
    }

    private void LoadGrid(Vector2I partyGridLocation, Vector2I partyDirection)
    {
        GD.Print($"PARTY LEAVING FROM {partyGridLocation}, NEED TO LOAD NEXT GRID");
        
        // TODO: Load up the next grid, based on where they are in the zone, and in the world
        //  Proc gen based on biome, etc., then load Grid in, then place Party in the correct
        //  location.
        
        // Remove the old grid...
        _grid.RemoveChild(_party);
        _subViewport.RemoveChild(_grid);
        _grid.QueueFree();
        
        // Handle everything that cares about direction party is coming from
        Vector2I partyNewGridPosition = Vector2I.Zero;  // The Party's grid position within the new Grid
        Vector2I newGridPositionWithinZone = Vector2I.Zero;  // The new Grid's location within its Zone
        Vector2I newZoneWorldPosition = _gameState.CurrentZone.WorldLocation;  // The new Zone's location in the World

        // Remember (0, 0) is the top left, moving down makes
        //  positive Ys, and moving right makes positive Xs
        if (partyDirection == new Vector2I(1, 0))  // They moved left, so 4 (old x) - 3 (new x) = 1
        {
            // They are moving left
            GD.Print("EXITING LEFT");
            
            partyNewGridPosition = new Vector2I(GameConstants.GridWidth - 1, partyGridLocation.Y);

            // Check if they are moving out of the zone
            if (_gameState.CurrentGrid.GridZoneLocation.X == 0)
            {
                // They are moving out of the zone
                GD.Print("MOVING OUT OF THE ZONE");
                
                // Grab the new Zone
                var newZone = _gameState.World.Zones[_gameState.CurrentZone.WorldLocation.X - 1, _gameState.CurrentZone.WorldLocation.Y];
                _gameState.CurrentZone = newZone;
                
                // Set the Grid's location within the new zone
                newGridPositionWithinZone = new Vector2I(GameConstants.ZoneWidth - 1, _gameState.CurrentGrid.GridZoneLocation.Y);
            }
            else
            {
                // Set the Grid's location within its zone
                newGridPositionWithinZone = new Vector2I(_gameState.CurrentGrid.GridZoneLocation.X - 1, _gameState.CurrentGrid.GridZoneLocation.Y);
            }
        } else if (partyDirection == new Vector2I(-1, 0))
        {
            // They are moving right
            GD.Print("EXITING RIGHT");

            if (_gameState.CurrentGrid.GridZoneLocation.X == GameConstants.ZoneWidth - 1)
            {
                // They are moving out of the zone
                GD.Print("MOVING OUT OF THE ZONE");
                
                // TODO: Check if they are moving out of the world
                
                // Grab the new Zone
                var newZone = _gameState.World.Zones[_gameState.CurrentZone.WorldLocation.X + 1, _gameState.CurrentZone.WorldLocation.Y];
                _gameState.CurrentZone = newZone;
                
                // Set the Grid's location within the new zone
                newGridPositionWithinZone = new Vector2I(0, _gameState.CurrentGrid.GridZoneLocation.Y);
            }
            else
            {
                // Set the Grid's location within its zone
                newGridPositionWithinZone = new Vector2I(_gameState.CurrentGrid.GridZoneLocation.X + 1, _gameState.CurrentGrid.GridZoneLocation.Y);
            }
            
            partyNewGridPosition = new Vector2I(0, partyGridLocation.Y);
        } else if (partyDirection == new Vector2I(0, -1))
        {
            // They are moving down
            GD.Print("EXITING DOWN");

            if (_gameState.CurrentGrid.GridZoneLocation.Y == GameConstants.ZoneHeight - 1)
            {
                // They are moving out of the zone
                GD.Print("MOVING OUT OF THE ZONE");
                
                // TODO: Check if they are moving out of the world
                
                // Grab the new Zone
                var newZone = _gameState.World.Zones[_gameState.CurrentZone.WorldLocation.X, _gameState.CurrentZone.WorldLocation.Y + 1];
                _gameState.CurrentZone = newZone;
                
                // Set the Grid's location within the new zone
                newGridPositionWithinZone = new Vector2I(_gameState.CurrentGrid.GridZoneLocation.X, 0);
            }
            else
            {
                // Set the Grid's location within its zone
                newGridPositionWithinZone = new Vector2I(_gameState.CurrentGrid.GridZoneLocation.X, _gameState.CurrentGrid.GridZoneLocation.Y + 1);
            }
            
            partyNewGridPosition = new Vector2I(partyGridLocation.X, 0);
        } else if (partyDirection == new Vector2I(0, 1))
        {
            // They are moving up
            GD.Print("EXITING UP");
            
            if (_gameState.CurrentGrid.GridZoneLocation.Y == 0)
            {
                // They are moving out of the zone
                GD.Print("MOVING OUT OF THE ZONE");
                
                // TODO: Check if they are moving out of the world
                
                // Grab the new Zone
                var newZone = _gameState.World.Zones[_gameState.CurrentZone.WorldLocation.X, _gameState.CurrentZone.WorldLocation.Y - 1];
                _gameState.CurrentZone = newZone;
                
                // Set the Grid's location within the new zone
                newGridPositionWithinZone = new Vector2I(_gameState.CurrentGrid.GridZoneLocation.X, GameConstants.ZoneHeight - 1);
            }
            else
            {
                // Set the Grid's location within its zone
                newGridPositionWithinZone = new Vector2I(_gameState.CurrentGrid.GridZoneLocation.X, _gameState.CurrentGrid.GridZoneLocation.Y - 1);
            }
            
            partyNewGridPosition = new Vector2I(partyGridLocation.X, GameConstants.GridHeight - 1);
        }

        // Create the Grid
        _grid = new Grid();
        _subViewport.AddChild(_grid);
        _grid.Initialize(_gameState.CurrentZone, newGridPositionWithinZone);
        _gameState.CurrentGrid = _grid;
        
        // Add the party to the grid
        _grid.AddChild(_party);
        
        // Set the party's location
        _party.SetCurrentGridPosition(partyNewGridPosition, partyDirection);
        
        GD.Print($"POSITION: {_party.Position} GRID POSITION: {_party.GetCurrentGridPosition()}");
    }

    private void PartyOnPartyLeavingGrid(Vector2I partyGridLocation, Vector2I partyDirection)
    {
        LoadGrid(partyGridLocation, partyDirection);
    }
    
    private void LookerOnLookerLooked(Vector2I gridPosition)
    {
        // TODO: Ok, now need to look at the actual tile...
        GD.Print($"Looked at {gridPosition}");
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        
        // Let's use the main input handler for opening and closing modals, etc.
        //  but not specific inputs.

        if (@event.IsActionPressed("world_map"))
        {
            // Open or close the world map
            _worldMap.Visible = !_worldMap.Visible;
            _gameState.CurrentGameState = _worldMap.Visible ? GameStates.Map : GameStates.Game;
        } else if (@event.IsActionPressed("escape"))
        {
            if (_gameState.CurrentGameState != GameStates.Game)
            {
                // It's not Game mode, so escape out of whatever GameStates mode we are in, and close any
                // modal that is open, then switch to Game mode...
                _worldMap.Visible = false;
                _gameMenu.Visible = false;
                _looker.StopLooking();
                _gameState.CurrentGameState = GameStates.Game;
            }
            else
            {
                // We're in Game mode, so open or close the game menu
                _gameMenu.Visible = !_gameMenu.Visible;
                _gameState.CurrentGameState = _gameMenu.Visible ? GameStates.GameMenu : GameStates.Game;
            }
        } else if (@event.IsActionPressed("look"))
        {
            // Look around
            _gameState.CurrentGameState = GameStates.Look;

            // Just default to the player position, for now...
            var position= _gameState.CurrentParty.GetCurrentGridPosition();
            
            GD.Print($"LOOK AT: {position}");
            
            // Look!
            _looker.StartLooking(position);
        }
    }
}
