using Godot;
using System;

[GlobalClass]
public partial class StatsResource : Resource
{
    [Export] public int Moxie { get; set; } = 1;
    [Export] public int Psyche { get; set; } = 1;
    [Export] public int Smarts { get; set; } = 1;
    [Export] public int Wits { get; set; } = 1;
    [Export] public int Muscle { get; set; } = 1;
    [Export] public int Cool { get; set; } = 1;
    [Export] public int Quick { get; set; } = 1;
}
