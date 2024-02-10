using Godot;
using System;

[GlobalClass]
public partial class EntityResource : Resource
{
    [Export] public string EntityName { get; set; }
}
