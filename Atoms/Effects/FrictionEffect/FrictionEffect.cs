using Godot;
using System;
using System.Linq;
using JamToolkit.Util;

public class FrictionEffect : BaseEffect
{
	/// <summary>
	/// How long the effect will be active
	/// </summary>
	[Export] public float TimeToLive { get; set; }
	[Export] public float Friction { get; set; }

	private float _timeToLive;
	private float _saveFriction;
	private bool _expiring;

	public override void Remove()
	{
		_expiring = true;
	}

	public override void _Ready()
	{
		if (controller == null)
		{
			SetPhysicsProcess(false);
			return;
		}
		Attach();
	}
	
	public override void Attach()
	{
		SetPhysicsProcess(true);
		controller = this.FindSingleton<PlayerController>();
		var count = controller.EnumerateChildren().Count(child => child is FrictionEffect);
		if (count > 1)
		{
			QueueFree();
			return;
		}
		_saveFriction = controller.Friction;
		controller.Friction = Friction;
		_timeToLive = TimeToLive;
	}

	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);

		// only start our TTL count down after we have been started
		if (!_expiring) return;

		_timeToLive -= delta;

		if (_timeToLive > 0) return;
		controller.Friction = _saveFriction;
		QueueFree();
	}
}
