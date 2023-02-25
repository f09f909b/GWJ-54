using Godot;
using Godot.Collections;

public partial class HitScanEmitter : Node3D
{
	[Export] private int _castDistance = 10000;
	private Array _bodiesToIgnore;
	private Marker3D _secondaryArmMuzzle;

	private RayCast3D _aimCast;
	private PackedScene _bulletHitGfx;

	[Export] private NodePath _aimCastPath;
	
	public override void _Ready()
	{
		_aimCast = GetNode<RayCast3D>(_aimCastPath);
		_bulletHitGfx = GD.Load<PackedScene>("res://src/gfx/BulletHitGfx.tscn");
	}

	private void Fire()
	{
		PhysicsDirectSpaceState3D bullet = GetWorld3D().DirectSpaceState;
		PhysicsRayQueryParameters3D rayParameters =
			PhysicsRayQueryParameters3D.Create(_secondaryArmMuzzle.Transform.Origin,
				_aimCast.GetCollisionPoint());
		Dictionary collisionResult = bullet.IntersectRay(rayParameters);

		if (collisionResult.Count == 0) return;

		Node collidedNode = (Node) collisionResult["collider"];

		if (collidedNode.IsInGroup("Enemy"))
		{
			var enemy = (Enemy) collidedNode;
			enemy.TakeDamage(10);
		}
		else
		{
			var hitEffect = (Node3D) _bulletHitGfx.Instantiate();

			GetTree().Root.AddChild(hitEffect);

			Transform3D hitEffectGlobalTransform = hitEffect.GlobalTransform;
			hitEffectGlobalTransform.Origin = (Vector3) collisionResult["position"];
			hitEffect.Transform = hitEffectGlobalTransform;

			var resultNormal = (Vector3) collisionResult["normal"];

			if (resultNormal.AngleTo(Vector3.Up) < 0.0005f) return;
			if (resultNormal.AngleTo(Vector3.Down) < 0.0005f)
			{
				hitEffect.Rotate(Vector3.Up, Mathf.Pi);
			}

			Vector3 yVec = resultNormal;
			Vector3 xVec = yVec.Cross(Vector3.Up);
			Vector3 zVec = xVec.Cross(yVec);

			hitEffectGlobalTransform.Basis = new Basis(xVec, yVec, zVec);
			hitEffect.Transform = hitEffectGlobalTransform;
		}
	}
}
