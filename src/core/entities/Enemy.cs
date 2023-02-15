using Godot;
using System;
#pragma warning disable CS8632

public partial class Enemy : CharacterBody3D
{

    [Export] private int _speed = 5;
    private PlayerController _player;
    private NavigationAgent3D _navAgent;
    //[Export] private NodePath _playerNodePath;
    //[Export] private NodePath _navAgentPath;
    
    public override void _Ready()
    {
        _player = GetNode<PlayerController>("../Player");
        _navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        UpdateTargetLocation(_player!.GlobalPosition);
    }

    public override void _PhysicsProcess(double delta)
    {
        var currentPosition = GlobalPosition;
        var nextPosition = _navAgent.GetNextPathPosition();
        var updatedVelocity = (nextPosition - currentPosition).Normalized() * _speed;

        Velocity = updatedVelocity;
        MoveAndSlide();
    }

    private void UpdateTargetLocation(Vector3 targetLocation)
    {
        _navAgent.TargetPosition = targetLocation;
    }
}
