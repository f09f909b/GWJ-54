using Godot;
using System;

public partial class Curse : Node
{
    [Export] public CurseData CurseData;
    
    public string Name;
    public string Description;
    public int HealthAdjustment;
    public int DamageAdjustment;
    public int SpeedAdjustment;
    public int DefenseAdjustment;

    public void AssignCurseData()
    {
        Name = CurseData.Name;
        Description = CurseData.Description;
        HealthAdjustment = CurseData.HealthAdjustment;
        DamageAdjustment = CurseData.DamageAdjustment;
        SpeedAdjustment = CurseData.SpeedAdjustment;
        DefenseAdjustment = CurseData.DefenseAdjustment;
    }

    public void PlaceOnPlayer()
    {
        //var limbNode = GetNode();
        //var targetLimb = limbNode.RemoveChild();
    }

    protected virtual void OnEquipped()
    {
        GD.Print("sometihn");
    }
}