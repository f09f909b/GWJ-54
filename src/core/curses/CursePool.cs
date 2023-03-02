using Godot;
using System;
using Godot.Collections;

public partial class CursePool : Resource
{
    [Export] public Dictionary<String, CurseData> CurseDataPool { get; private set; }
}