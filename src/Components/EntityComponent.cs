using Godot;

[GlobalClass]
public partial class EntityComponent : Node
{
	[Export] public EntityResource EntityResource;
	public string EntityName { get; set; }
	
	public override void _Ready()
	{
		base._Ready();

		EntityName = EntityResource.EntityName;
	}
}
