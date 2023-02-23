using Godot;
using System;

public partial class CurseResource : Resource
{
    public enum Limbs
    {
        Head,
        RightArm,
        LightArm,
        Body,
        LeftLeg,
        RightLeg
    }

    [Export] public Limbs Type { get; private set; }
    [Export] public string Name { get; private set; }
    [Export] public string Description { get; private set; }
    [Export] public int HealthAdjustment { get; private set; }
    [Export] public int DamageAdjustment { get; private set; }
    [Export] public int SpeedAdjustment { get; private set; }
    [Export] public int DefenseAdjustment { get; private set; }
}