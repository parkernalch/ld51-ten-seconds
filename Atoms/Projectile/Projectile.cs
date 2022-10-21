using System;
using System.Drawing.Drawing2D;
using Godot;
using JamToolkit.Util;

public class Projectile : Area2D
{
	[Export] public float SteerForce { get; set; } = 50;

	EventBus _eventBus;
	Node2D _target;
	VisibilityNotifier2D _visibilityNotifier2D;
	private Timer _timer;
	private RayCast2D _raycast;
	[Export] public float Speed { get; set; } = 350;
	[Export] public Vector2 Velocity { get; set; } = Vector2.Zero;
	[Export] public Vector2 Acceleration { get; set; } = Vector2.Zero;
	[Export] public int Bounces { get; set; } = 5;
	[Export] public int Damage { get; set; } = 3;


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
	public void Start(Transform2D origin, Vector2 direction, Node2D target = null, float speed = 350f)
	{
		GlobalTransform = origin;
		Rotation += (float)GD.RandRange(-0.09f, 0.09f);
		Speed = speed;
		Velocity = direction.Normalized() * speed;
		_raycast.CastTo = direction.Normalized();
		_target = target;
		if (target != null)
		{
			GetNode<Trail>("Trail").DefaultColor = new Color(1, 0, 0, 1);
		}
		_timer.Start(10); // missile dies after 10 seconds
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
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_raycast = GetNode<RayCast2D>("RayCast2D");
		_visibilityNotifier2D = this.FindChildByName<VisibilityNotifier2D>("VisibilityNotifier2D");
		_timer = this.FindChildByName<Timer>("Lifetime");

		Damage = GetNode<GameManager>("/root/GameManager").CurrentRoom;
		_visibilityNotifier2D.SafeConnect("screen_exited", this, nameof(OnLifetimeTimeout));
		this.SafeConnect("body_entered", this, nameof(OnProjectileBodyEntered));
		_timer.SafeConnect("timeout", this, nameof(OnLifetimeTimeout));
		_timer.SafeConnect("tree_exiting", this, nameof(OnLifetimeExitingTree));
		_eventBus.SafeConnect(nameof(EventBus.LevelCompleted), this, nameof(OnLevelCompleted));
	}

	private void OnLevelCompleted()
	{
		this.QueueFree();
	}

	protected override void Dispose(bool disposing)
	{
		this.SafeDisconnect("body_entered", this, nameof(OnProjectileBodyEntered));
		base.Dispose(disposing);
	}

	Vector2 Seek()
	{
		if (_target == null)
			return Vector2.Zero;

		// desired vector
		var desired = (_target.GlobalPosition - GlobalPosition).Normalized() * Speed;

		// minus current velocity vector to get our course correction
		return (desired - Velocity).Normalized() * SteerForce;
	}

	void OnProjectileBodyEntered(Node body)
	{
		// TODO: explode projectile
		if (body is PlayerController pc)
		{
			if (pc.IsVulnerable())
			{
				_eventBus.ConnectMissile(this);
				pc.TakeProjectileDamage(this);
				QueueFree();
			}
		}
		else if(_target != null)
		{
			// homing arrows die on collision
			QueueFree();
		}
		else if(Bounces == 0)
		{
			QueueFree();
		}
	}

	void OnLifetimeExitingTree()
	{
		_timer.SafeDisconnect("timeout", this, nameof(OnLifetimeTimeout));
		_timer.SafeDisconnect("tree_exiting", this, nameof(OnLifetimeExitingTree));
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
		GlobalPosition += Velocity * delta;
		_raycast.CastTo = Velocity.Normalized() * 4;

		if (_raycast.IsColliding() && _raycast.GetCollider() is TileMap)
		{
			var norm = _raycast.GetCollisionNormal();
			var vCompU = norm * Velocity.Normalized().Dot(norm);
			var vCompW = Velocity.Normalized() - vCompU;
			var newVelocity = Velocity.Length() * (vCompW - vCompU);
			Velocity = newVelocity;
			_raycast.CastTo = Velocity.Normalized() * 4;
			Bounces--;
		}
	}
}
