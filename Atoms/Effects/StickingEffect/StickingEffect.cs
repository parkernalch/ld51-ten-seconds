using Godot;
using System;
using System.Linq;
using JamToolkit.Util;

public class StickingEffect : BaseEffect
{
	[Export] public float MaxVelocity { get; set; } = 1f;

	public override void _Ready()
	{
		if (controller == null)
		{
			return;
		}
		Attach();
	}

	public override void Attach()
	{
		controller = (PlayerController)GetParent();
		// can only have one StickingEffect at a time
		var count = controller.EnumerateChildren().Count(child => child is StickingEffect);
		if (count > 1)
		{
			QueueFree();
			return;
		}
		controller.SetTopSpeed(MaxVelocity);

	}

	public override void Remove()
	{
		controller.ResetTopSpeed();
		QueueFree();
	}
}
