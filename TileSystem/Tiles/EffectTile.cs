using Godot;
using System;
using JamToolkit.Util;

public class EffectTile : Area2D
{
	BaseEffect _effect;
	public override void _Ready()
	{
		this.SafeConnect("body_entered", this, nameof(OnBodyEntered));
		this.SafeConnect("body_exited", this, nameof(OnBodyExited));
		_effect = this.FindChild<BaseEffect>();
	}

	void OnBodyEntered(Node2D body)
	{
		if (body is PlayerController)
		{
			BaseEffect newEffect = (BaseEffect)_effect.Duplicate();
			(body as PlayerController).AddChild(newEffect);
			newEffect.Attach();
		}
	}

	void OnBodyExited(Node2D body)
	{
		if (body is PlayerController)
		{
			BaseEffect playerCurrentEffect = (body as PlayerController).FindChild<BaseEffect>();
			if (playerCurrentEffect != null)
			{
				playerCurrentEffect.Remove();
			}
		}
	}
}
