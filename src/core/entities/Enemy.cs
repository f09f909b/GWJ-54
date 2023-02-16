using System;
using Godot;

public partial class Enemy : CharacterBody3D
{
    [Export] public string Name { get; private set; } = "Test";
    [Export] public int Health { get; private set; } = 10;
    [Export] public int Speed { get; private set; }  = 1;
    [Export] public int Damage = 5;
    private bool _isFiring;

    private PlayerController _player;
    private RayCast3D _sightRaycast;
    private Area3D _hearingArea;
    private NavigationAgent3D _navAgent;
    private Timer _attackTimer;
    private State _currentState;

    [Export] private NodePath _sightRangePath;
    [Export] private NodePath _hearingAreaPath;
    [Export] private NodePath _navigationAgent3DPath;
    [Export] private NodePath _attackTimerPath;

    private enum State
    {
        Pursue,
        Wander,
        Dead
    }

    public override void _Ready()
    {
        _player = GetNode<PlayerController>("../Player");
        _sightRaycast = GetNode<RayCast3D>(_sightRangePath);
        _hearingArea = GetNode<Area3D>(_hearingAreaPath);
        _navAgent = GetNode<NavigationAgent3D>(_navigationAgent3DPath);
        _attackTimer = GetNode<Timer>(_attackTimerPath);
    }

    public override void _PhysicsProcess(double delta)
    {
        TrackTarget();
        
        switch (_currentState)
        {
            case State.Pursue:
            {
                if (TargetHitByRayCheck("Player") && !_isFiring)
                {
                    EnableFire();
                }

                ChaseTarget();
                break;
            }
            case State.Wander:
                Wander();
                break;
            case State.Dead:
                Die();
                break;
            default:
                Wander();
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    private void TrackTarget()
    {
        _sightRaycast.LookAt(_player.GlobalTransform.Origin);

        if (TargetHitByRayCheck("Player"))
        {
            _currentState = State.Pursue;
        }
        else if (TargetEnterHearingZoneCheck(_hearingArea))
        {
            _currentState = State.Pursue;
        }
        else
        {
            _currentState = State.Wander;
        }
    }

    private bool TargetHitByRayCheck(string targetGroup)
    {
        if (_sightRaycast.IsColliding())
        {
            var collidedObject = (Node) _sightRaycast.GetCollider();
            if (collidedObject.IsInGroup(targetGroup))
            {
                return true;
            }
        }

        return false;
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

    private void Wander()
    {
        // TODO: Insert wandering code
    }

    private void EnableFire()
    {
        _isFiring = true;
        _attackTimer.Start();
    }

    private void Fire()
    {
        _player.TakeDamage(Damage);
        _isFiring = false;
        _attackTimer.Stop();
    }

    private void ChaseTarget()
    {
        UpdateTargetLocation(_player.GlobalTransform.Origin);
        Vector3 currentPosition = GlobalPosition;
        Vector3 nextPosition = _navAgent.GetNextPathPosition();
        Vector3 updatedVelocity = (nextPosition - currentPosition).Normalized() * Speed;

        Velocity = updatedVelocity;
        MoveAndSlide();
    }

    private void UpdateTargetLocation(Vector3 targetLocation)
    {
        _navAgent.TargetPosition = targetLocation;
    }

    private void Die()
    {
        _currentState = State.Dead;
        // Play dead animation
        SetProcess(false);
    }
}