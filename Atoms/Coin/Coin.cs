using Godot;
using System;
using JamToolkit.Util;

public class Coin : Area2D
{
	EventBus _eventBus;
	private Vector2? _target;
	private Vector2 _origin;
	private float _distance;
	private Tween _tween;
	[Export] public float Speed { get; set; } = 350;

	public bool Collectable { get; set; } = false;

	public override void _Ready()
	{
		_eventBus = GetNode<EventBus>("/root/EventBus");
		this.SafeConnect("body_entered", this, nameof(OnCoinBodyEntered));
	}

	/// <summary>
	/// Send the projectile from the <paramref name="origin"/> facing the initial
	/// <paramref name="direction"/> specified. If a <paramref name="target"/> is
	/// specified, home into that location over time.
	/// </summary>
	/// <param name="origin"></param>
	/// <param name="direction"></param>
	/// <param name="target"></param>
	public void Start(Transform2D origin, Vector2 target)
	{
		this.GlobalTransform = origin;
		_target = target;

		_eventBus = GetNode<EventBus>("/root/EventBus");
	}

	private void OnCoinBodyEntered(Node body)
	{
		if (!Collectable) return;

		// _eventBus.CollectCoin(this);
		QueueFree();
	}

	public override void _PhysicsProcess(float delta)
	{
		// once we are collectable we dont move
		if(Collectable) return;

		if (_target == null)
			return;

		Position = Position.MoveToward(_target.Value, delta * Speed);

		if (Position == _target.Value)
		{
			// _eventBus.CreateCoin(this);
			Collectable = true;
		}
	}
}
