using Godot;
using System;
using System.Linq;
using JamToolkit.Util;

public class FrictionEffect : Node2D
{
	/// <summary>
	/// How long the effect will be active
	/// </summary>
	[Export] public float TimeToLive { get; set; }
	[Export] public float Friction { get; set; }

	private float _timeToLive;
	private PlayerController _controller;
	private float _saveFriction;
	private bool _expiring;

	public void Start()
	{
		_expiring = true;
	}

	public override void _Ready()
	{
		_controller = this.FindSingleton<PlayerController>();
		// can only have one FrictionEffect at a time
		var count = _controller.EnumerateChildren().Count(child => child is FrictionEffect);
		if (count > 1)
		{
			QueueFree();
			return;
		}
		_saveFriction = _controller.Friction;
		_controller.Friction = Friction;
		_timeToLive = TimeToLive;
	}

	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);

		// only start our TTL count down after we have been started
		if (!_expiring) return;

		_timeToLive -= delta;

		if (_timeToLive > 0) return;
		_controller.Friction = _saveFriction;
		QueueFree();
	}
}
