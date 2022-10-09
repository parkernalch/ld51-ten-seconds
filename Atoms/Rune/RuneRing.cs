using Godot;
using System;

public class RuneRing : Sprite
{
	Timer _timer;
	Tween _tween;
	EventBus _eventBus;

	public override void _Ready()
	{
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_tween = new Tween();
		AddChild(_tween);
		_timer = GetNode<Timer>("Timer");
		_timer.Connect("timeout", this, nameof(OnTimeout));
		_eventBus.Connect(nameof(EventBus.LevelCompleted), this, nameof(OnLevelCompleted));
		Start();
	}
	
	void OnLevelCompleted() 
	{
		_tween.StopAll();
		ShaderMaterial mat = Material as ShaderMaterial;
		mat.SetShaderParam("fill_ratio", 0);
		_timer.Disconnect("timeout", this, nameof(OnTimeout));
		_timer.Stop();
	}
	
	void OnTimeout()
	{
		Start();
		_eventBus.EndCountdown();		
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
