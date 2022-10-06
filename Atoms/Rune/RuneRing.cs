using Godot;
using System;

public class RuneRing : Sprite
{
	Timer _timer;
	Tween _tween;

	public override void _Ready()
	{
		_tween = new Tween();
		AddChild(_tween);
		_timer = GetNode<Timer>("Timer");
		_timer.Connect("timeout", this, nameof(OnTimeout));
		Start();
	}
	
	void OnTimeout()
	{
		GD.Print("TIMEOUT");
	}
	
	void Start()
	{
		_timer.Start();
		ShaderMaterial mat = Material as ShaderMaterial;
		_tween.InterpolateProperty(
			mat,
			"shader_param/fill_ratio",
			1f,
			0f,
			10f,
			0,
			0
		);
		_tween.Start();
	}
}
