using Godot;
using System;

public interface ISaveable
{
    public Godot.Collections.Dictionary<string, Variant> SaveData();
    public void LoadData(Godot.Collections.Dictionary<string, Variant> saveData);
}
