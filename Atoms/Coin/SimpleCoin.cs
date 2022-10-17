using Godot;
using System;

public class SimpleCoin : Sprite
{
	public int value = 1;
	EventBus _eventBus;
	Vector2 velocity;
	RayCast2D raycast;
	PlayerController player;
	private float dampFactor = 0.95f;
	bool isAttracting = false;
	
	public override void _Ready()
	{
		_eventBus = GetNode<EventBus>("/root/EventBus");
		raycast = GetNode<RayCast2D>("RayCast2D");
		velocity = new Vector2(0f, 0f);
		player = GetNode<GameManager>("/root/GameManager").GetPlayer();
		dampFactor = (float)GD.RandRange(0.7, 0.95);
	}
	
	public void Start(Vector2 globalPosition, Vector2 direction)
	{
		GlobalPosition = globalPosition;
		velocity = direction;
		raycast.CastTo = direction * 1.5f;
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
			raycast.CastTo = velocity * 1.5f;
		}
		if (player == null) return;
		if (isAttracting)
		{
			float x = Mathf.Lerp(GlobalPosition.x, player.GlobalPosition.x, delta * 10f);
			float y = Mathf.Lerp(GlobalPosition.y, player.GlobalPosition.y, delta * 10f);
			GlobalPosition = new Vector2(x, y); 
		}
		if ((player.GlobalPosition.DistanceSquaredTo(this.GlobalPosition)) < player.GetCollectionRadius())
		{
			SetCollecting();
		}
		if (isAttracting && player.GlobalPosition.DistanceSquaredTo(this.GlobalPosition) < 100f)
		{
			Collect();
		}
	}
	
	void SetCollecting()
	{
		isAttracting = true;
	}
	
	async void Collect()
	{
		_eventBus.CollectCoin(this);
		player.GiveCoin(value);
		await ToSignal(GetTree(), "idle_frame");
		QueueFree();
	}
}
