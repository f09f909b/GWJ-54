using Godot;
using System;

public partial class DungeonExit : Area3D
{
    [Signal]
    public delegate void OnLevelClearedEventHandler();

    private void OnExitAreaCollided(Area3D area)
    {
        if (area.IsInGroup("Player"))
        {
            EmitSignal(nameof(OnLevelCleared));
        }
    }
}