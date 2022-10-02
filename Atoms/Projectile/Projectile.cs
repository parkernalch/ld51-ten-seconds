using System;
using System.Drawing.Drawing2D;
using Godot;
using JamToolkit.Util;

public class Projectile : Area2D
{
	[Export] public float SteerForce { get; set; } = 50;

	EventBus _eventBus;
	Node2D _target;
	private Timer _timer;
	[Export] public float Speed { get; set; } = 350;
	[Export] public Vector2 Velocity { get; set; } = Vector2.Zero;
	[Export] public Vector2 Acceleration { get; set; } = Vector2.Zero;


	/// <summary>
	/// Send the projectile from the <paramref name="origin"/> facing the initial
	/// <paramref name="direction"/> specified. If a <paramref name="target"/> is
	/// specified, home into that location over time.
	/// </summary>
	/// <param name="origin"></param>
	/// <param name="direction"></param>
	/// <param name="target"></param>
	public void Start(Transform2D origin, float direction, Node2D target = null) =>
		Start(origin, new Vector2(Mathf.Cos(direction), Mathf.Sin(direction)), target);

	/// <summary>
	/// Send the projectile from the <paramref name="origin"/> facing the initial
	/// <paramref name="direction"/> specified. If a <paramref name="target"/> is
	/// specified, home into that location over time.
	/// </summary>
	/// <param name="origin"></param>
	/// <param name="direction"></param>
	/// <param name="target"></param>
	public void Start(Transform2D origin, Vector2 direction, Node2D target = null)
	{
		GlobalTransform = origin;
		Rotation += (float)GD.RandRange(-0.09f, 0.09f);
		Velocity = direction.Normalized() * Speed;
		_target = target;
		_timer.Start(10); // missile dies after 10 seconds
		_eventBus = GetNode<EventBus>("/root/EventBus");
	}

	/// <summary>
	/// Send the projectile from the <paramref name="origin"/> facing down
	/// </summary>
	/// <param name="origin"></param>
	/// <param name="direction"></param>
	/// <param name="target"></param>
	public void Start(Transform2D origin) => Start(origin, Vector2.Down);

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
		return (desired - Velocity).Normalized() * SteerForce;
	}

	void OnProjectileBodyEntered(Node body)
	{
		// TODO: explode projectile
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
		Acceleration += Seek();
		// regular physics calculation
		Velocity += Acceleration * delta;
		Velocity = Velocity.LimitLength(Speed); // cap our max velocity
		Rotation = Velocity.Angle();
		Position += Velocity * delta;
	}
}
