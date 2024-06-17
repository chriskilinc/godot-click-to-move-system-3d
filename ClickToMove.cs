using Godot;
using System;

// Based on
// https://www.youtube.com/watch?v=KT06pv06Q1U

public partial class ClickToMove : CharacterBody3D
{
    NavigationAgent3D _navigationAgent;
    Camera3D _camera;
    static StringName _inputMouseLeft = new StringName("mouse_left");

    float _speed = 10;

    public override void _Ready()
    {
        _navigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        _camera = GetNode<Camera3D>("%Camera3D");
        GD.Print(_camera);
    }

    public override void _Process(double delta)
    {
        if (!_navigationAgent.IsNavigationFinished())
        {
            MoveToPoint(delta, _speed);
        }
    }

    // input event
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.IsActionPressed(_inputMouseLeft))
            {
                var mousePosition = GetViewport().GetMousePosition();
                var rayLength = 100;
                var from = _camera.ProjectRayOrigin(mousePosition);
                var to = from + _camera.ProjectRayNormal(mousePosition) * rayLength;    // Normal is a straght line from camera to mouse position
                var space = GetWorld3D().DirectSpaceState;
                var rayQuery = new PhysicsRayQueryParameters3D
                {
                    From = from,
                    To = to
                };
                var result = space.IntersectRay(rayQuery);

                if (result.Count > 0)
                {
                    _navigationAgent.Set(NavigationAgent3D.PropertyName.TargetPosition, result["position"]);
                }
            }
        }
    }

    public void MoveToPoint(double delta, float speed)
    {
        var targetPosition = _navigationAgent.GetNextPathPosition();
        var direction = GlobalPosition.DirectionTo(targetPosition);
        Velocity = direction * speed;   // TODO: Use delta?
        // Velocity = direction.Normalized() * speed;   // look into this?
        FaceDirection(targetPosition);
        MoveAndSlide();
    }

    public void FaceDirection(Vector3 direction)
    {
        LookAt(new Vector3(direction.X, GlobalPosition.Y, direction.Z), Vector3.Up);    // TODO: Problem before movement has started? (first click)
    }
}
