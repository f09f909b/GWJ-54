using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
    [Export] private int _health = 100;
    [Export] private float _speed = 5.0f;
    [Export] private float _jumpVelocity = 4.5f;
    [Export] private float _minVerticalAngle = -25f;
    [Export] private float _maxVerticalAngle = 25f;
    [Export] private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    [Export] private float _mouseSensitivity = 0.075f;
    private Node3D _pivot;
    private Camera3D _camera;
    [Export] private NodePath _pivotPath;
    [Export] private NodePath _cameraPath;

    public override void _Ready()
    {
        _pivot = GetNode<Node3D>(_pivotPath);
        _camera = GetNode<Camera3D>(_cameraPath);
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y -= _gravity * (float) delta;

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
            velocity.Y = _jumpVelocity;

        Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * _speed;
            velocity.Z = direction.Z * _speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, _speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, _speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion motionEvent)
        {
            Vector3 rotationDegrees = RotationDegrees;
            rotationDegrees.Y -= motionEvent.Relative.X * _mouseSensitivity;
            RotationDegrees = rotationDegrees;

            Vector3 pivotRotationDegrees = _pivot.RotationDegrees;
            pivotRotationDegrees.X -= motionEvent.Relative.Y * _mouseSensitivity;
            pivotRotationDegrees.X = Mathf.Clamp(pivotRotationDegrees.X, _minVerticalAngle, _maxVerticalAngle);
            _pivot.RotationDegrees = pivotRotationDegrees;
        }
    }
}