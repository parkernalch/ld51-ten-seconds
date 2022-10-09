using Godot;
using System;
using System.Linq;
using JamToolkit.Util;

public class StickingEffect : Node2D
{
	[Export] public float MaxVelocity { get; set; }

	private PlayerController _controller;

	public override void _Ready()
	{
		_controller = this.FindSingleton<PlayerController>();
		// can only have one StickingEffect at a time
		var count = _controller.EnumerateChildren().Count(child => child is StickingEffect);
		if (count > 1)
		{
			QueueFree();
			return;
		}
		_controller.SetTopSpeed(MaxVelocity);
	}

	public void Remove()
	{
		_controller.ResetTopSpeed();
		QueueFree();
	}
}
