using Godot;
using System;
using System.Collections.Generic;

public partial class CurseManager : Node
{
    private Godot.Collections.Dictionary<string, int> _availableCurses;
    private List<Godot.Collections.Dictionary<string, int>> _cursePool;

    public void InitializeCurses()
    {
        _availableCurses = new Godot.Collections.Dictionary<string, int>();
    }

    private void FetchCursePool()
    {
        
    }

    private void FilterFromCursePool(string sacrificedLimb)
    {
        
    }

    private void ApplyNewCurse()
    {
        
    }
}
