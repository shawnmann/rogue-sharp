using Godot;

[GlobalClass]
public partial class HealthComponent : Node
{
    [Export] public HealthResource HealthResource;
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }

    public override void _Ready()
    {
        base._Ready();

        MaxHealth = HealthResource.MaxHealth;
        CurrentHealth = HealthResource.MaxHealth;
    }
}
