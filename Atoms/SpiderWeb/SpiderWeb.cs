using Godot;
using System;
using JamToolkit.Util;

public class SpiderWeb : Area2D
{
	readonly PackedScene _stickingEffect = ResourceLoader.Load<PackedScene>("res://Atoms/Effects/StickingEffect/StickingEffect.tscn");

    public override void _Ready()
    {
	    Connect("body_entered", this, nameof(OnBodyEntered));
	    Connect("body_exited", this, nameof(OnBodyExited));
    }

    void OnBodyEntered(Node body)
    {
	    if (body is PlayerController pc)
	    {
		    pc.AddChild(_stickingEffect.Instance<StickingEffect>());
	    }
    }

    void OnBodyExited(Node body)
    {
	    if (body is PlayerController pc)
	    {
		    var effect = pc.FindChild<StickingEffect>();
		    if (effect == null) return;

		    effect.Remove();
	    }
    }
}
