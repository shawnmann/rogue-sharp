using Godot;
using System;

[GlobalClass]
public partial class GameEntityResource : Resource
{
    [Export] public string Name { get; set; }
}
