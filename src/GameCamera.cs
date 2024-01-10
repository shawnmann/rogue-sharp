using Godot;
using System;

public partial class GameCamera : Camera2D
{
    public override void _Ready()
    {
        base._Ready();
        
        // Set this as the current camera
        MakeCurrent();
        
        // Set initial position...
        PositionOnParty();
    }
    
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event.IsActionPressed("move_left") || @event.IsActionPressed("move_right"))
        {
            // TODO: Probably need to have Party emit a successful move signal, and
            //  have the camera move based on that
            PositionOnParty();
        }
    }

    private void PositionOnParty()
    {
        // The party is in a "party" group, which we will probably refactor
        //  There should be only one in here, but this is how we'll grab it for now
        var party = GetTree().GetFirstNodeInGroup("party") as Party;

        // If this didn't return anything, get the hell out of here
        if (party == null)
        {
            return;
        }
        
        var viewportSize = GetViewport().GetVisibleRect().Size;
        var currentGridPosition = party.GetCurrentGridPosition();
        
        // Set the initial x position to where the party's x is
        float positionX = currentGridPosition.X * GameConstants.TileWidth;

        // The y position of the camera will just be halfway down the view port
        var positionY = viewportSize.Y / 2;
        
        // But the x position will care about how close the party is to the 
        //  sides of the grid:
        
        // Check if they are close to the left edge
        positionX = positionX > viewportSize.X / 2 ? positionX : viewportSize.X / 2;
        
        // Check if they are close to the right edge
        positionX = currentGridPosition.X > GameConstants.GridWidth - (viewportSize.X / GameConstants.TileWidth) / 2 ? 
            GameConstants.GridWidth * GameConstants.TileWidth - viewportSize.X / 2 :
            positionX;
        
        Position = new Vector2(positionX, positionY);
    }
}
