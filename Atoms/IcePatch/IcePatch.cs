using Godot;
using System;
using JamToolkit.Util;

public class IcePatch : Area2D
{
	readonly PackedScene _stickingEffect = ResourceLoader.Load<PackedScene>("res://Atoms/Effects/FrictionEffect/FrictionEffect.tscn");

	public override void _Ready()
	{
		this.SafeConnect("body_entered", this, nameof(OnBodyEntered));
		this.SafeConnect("body_exited", this, nameof(OnBodyExited));
	}

	void OnBodyEntered(Node body)
	{
		if (body is PlayerController pc)
		{
			pc.AddChild(_stickingEffect.Instance<FrictionEffect>());
		}
	}

	void OnBodyExited(Node body)
	{
		if (body is PlayerController pc)
		{
			var effect = pc.FindChild<FrictionEffect>();
			if (effect == null) return;

			effect.Remove();
		}
	}
}
