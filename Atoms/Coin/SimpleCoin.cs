using Godot;
using System;

public class SimpleCoin : Sprite
{
	Vector2 velocity;
	RayCast2D raycast;
	PlayerController player;
	private float dampFactor = 0.95f;
	
	public override void _Ready()
	{
		raycast = GetNode<RayCast2D>("RayCast2D");
		velocity = new Vector2(0f, 0f);
		player = GetNode<GameManager>("/root/GameManager").GetPlayer();
		dampFactor = (float)GD.RandRange(0.7, 0.95);
	}
	
	public void Start(Vector2 globalPosition, Vector2 direction)
	{
		GlobalPosition = globalPosition;
		velocity = direction;
		raycast.CastTo = direction;
	}
	
	public override void _Process(float delta)
	{
		if (velocity == new Vector2(0, 0))
		{
			return;
		}
		velocity = velocity * dampFactor;
		if (velocity.LengthSquared() <= 1f)
		{
			Stop();
		}
		GlobalPosition += velocity;
	}
	
	private void Stop()
	{
		velocity = Vector2.Zero;
		raycast.Enabled = false;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if (raycast.Enabled && raycast.IsColliding())
		{
			Vector2 norm = raycast.GetCollisionNormal();
			Vector2 vCompU = norm * velocity.Normalized().Dot(norm);
			Vector2 vCompW = velocity.Normalized() - vCompU;
			Vector2 newVelocity = velocity.Length() * (vCompW - vCompU);
			velocity = newVelocity;
			raycast.CastTo = velocity;
		}
		if (player == null) return;
		if ((player.GlobalPosition - this.GlobalPosition).LengthSquared() < 1000f)
		{
			QueueFree();
		}
	}
}
