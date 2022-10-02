using Godot;
using JamToolkit.Util;

public class Projectile : Area2D
{
	[Export] public float Speed { get; set; } = 350;
	[Export] public float SteerForce { get; set; } = 50;

	Vector2 _velocity = Vector2.Zero;
	Vector2 _acceleration = Vector2.Zero;

	EventBus _eventBus;
	Node2D _target;
	private Timer _timer;

	public void Start(Transform2D transform, Node2D target)
	{
		GlobalTransform = transform;
		Rotation += (float)GD.RandRange(-0.09f, 0.09f);
		_velocity = transform.x * Speed;
		_target = target;
		_timer.Start(10);
		_eventBus = GetNode<EventBus>("/root/EventBus");
	}

	public override void _Ready()
	{
		_timer = this.FindChildByName<Timer>("Lifetime");

		Connect("body_entered", this, nameof(OnProjectileBodyEntered));
		_timer.Connect("timeout", this, nameof(OnLifetimeTimeout));
		_timer.Connect("tree_exiting", this, nameof(OnLifetimeExitingTree));
	}

	protected override void Dispose(bool disposing)
	{
		Disconnect("body_entered", this, nameof(OnProjectileBodyEntered));

		base.Dispose(disposing);
	}

	Vector2 Seek()
	{
		if (_target == null) return Vector2.Zero;

		// desired vector
		var desired = (_target.Position - Position).Normalized() * Speed;

		// minus current velocity vector to get our course correction
		return (desired - _velocity).Normalized() * SteerForce;
	}

	public void OnProjectileBodyEntered(Node body)
	{
		// TODO: explode projectile, kill player
		_eventBus.EmitSignal("missile_connected", this);
		QueueFree();
	}

	void OnLifetimeExitingTree()
	{
		_timer.Disconnect("timeout", this, nameof(OnLifetimeTimeout));
		_timer.Disconnect("tree_exiting", this, nameof(OnLifetimeExitingTree));
	}

	void OnLifetimeTimeout() => QueueFree();

	public override void _PhysicsProcess(float delta)
	{
		// update acc vector to home in on target
		_acceleration += Seek();
		// regular physics calculation
		_velocity += _acceleration * delta;
		_velocity = _velocity.LimitLength(Speed);
		Rotation = _velocity.Angle();
		Position += _velocity * delta;
	}
}
