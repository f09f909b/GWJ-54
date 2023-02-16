using System.Collections.Generic;
using Godot;

public partial class Enemy : CharacterBody3D
{
    [Export] private int _speed = 1;
    [Export] private int _damage = 5;
    private PlayerController _player;
    private RayCast3D _sightRaycast;
    private Area3D _hearingArea;
    private NavigationAgent3D _navAgent;
    private State _currentState;

    private enum State
    {
        Wander,
        Chase,
        Attack
    }

    public override void _Ready()
    {
        _player = GetNode<PlayerController>("../Player");
        _sightRaycast = GetNode<RayCast3D>("SightRange");
        _hearingArea = GetNode<Area3D>("HearingArea");
        _navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
    }

    public override void _PhysicsProcess(double delta)
    {
        TrackTarget();
        GD.Print(_currentState);
        switch (_currentState)
        {
            case State.Chase:
            {
                ChaseTarget();
                break;
            }
            case State.Attack:
            {
                Attack();
                break;
            }
            case State.Wander:
                Wander();
                break;
            default:
                Wander();
                break;
        }
    }

    private void TrackTarget()
    {
        _sightRaycast.LookAt(_player.GlobalTransform.Origin);
        if (_sightRaycast.IsColliding())
        {
            var collidedObject = (Node) _sightRaycast.GetCollider();
            if (collidedObject.IsInGroup("Player"))
            {
                OnTargetDetected();
            }
        }
        else if (TargetEnterHearingZoneCheck(_hearingArea))
        {
            OnTargetDetected();
        }
        else
        {
            OnTargetLost();
        }
    }

    private bool TargetEnterHearingZoneCheck(Area3D area)
    {
        var inHearingAreaCheckList = area.GetOverlappingBodies();
        foreach (Node3D n in inHearingAreaCheckList)
        {
            if (n.IsInGroup("Player"))
            {
                return true;
            }
        }

        return false;
    }

    private void OnTargetDetected()
    {
        _currentState = State.Chase;
    }

    private void OnTargetLost()
    {
        _currentState = State.Wander;
    }

    private void Wander()
    {
    }

    private void Attack()
    {
        _currentState = State.Attack;
    }

    private void ChaseTarget()
    {
        UpdateTargetLocation(_player.GlobalTransform.Origin);
        Vector3 currentPosition = GlobalPosition;
        Vector3 nextPosition = _navAgent.GetNextPathPosition();
        Vector3 updatedVelocity = (nextPosition - currentPosition).Normalized() * _speed;

        Velocity = updatedVelocity;
        MoveAndSlide();
    }

    private void UpdateTargetLocation(Vector3 targetLocation)
    {
        _navAgent.TargetPosition = targetLocation;
    }
}