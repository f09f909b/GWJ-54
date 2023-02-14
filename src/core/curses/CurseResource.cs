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
    [Export] public Limbs Type { get; set; }
    [Export] public string Name { get; set; }
    [Export] public string Description { get; set; }
}