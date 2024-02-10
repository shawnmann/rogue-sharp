using Godot;
using System;

[GlobalClass]
public partial class HealthResource : Resource
{
    [Export] public int MaxHealth { get; set; } = 10;
}
