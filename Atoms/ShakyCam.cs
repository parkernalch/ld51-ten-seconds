using Godot;
using System;
using JamToolkit.Util;

public class ShakyCam : Camera2D
{
	EventBus _eventBus;
	RandomNumberGenerator _rand;
	private float _shakeMagnitude = 0;
	private Vector2 _defaultOffset;
	private Vector2 _shakeOffset = new Vector2(0, 0);
	private Tween _tween;
	private bool playerIsDead = false;

	public override void _Ready()
	{
		Current = true;
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_tween = new Tween();
		_rand = new RandomNumberGenerator();
		AddChild(_tween);
		SetProcess(false);
		_defaultOffset = Offset;
		_eventBus.SafeConnect(nameof(EventBus.MissileConnected), this, nameof(OnMissileConnected));
		_eventBus.SafeConnect(nameof(EventBus.LobbedProjectileImpacted), this, nameof(OnLobbedProjectileLanded));
		_eventBus.SafeConnect(nameof(EventBus.PlayerDied), this, nameof(OnPlayerDied));
	}
	
	void OnPlayerDied()
	{
		playerIsDead = true;
	}
	
	void OnMissileConnected(Projectile p)
	{
		DoShake(1f, 0.1f);
	}
	
	void OnLobbedProjectileLanded(Vector2 point, int radius)
	{
		DoShake(2f, 0.2f);
	}
	
	public async void DoShake(float magnitude, float duration)
	{
		if (playerIsDead) return;
		_tween.StopAll();
		SetProcess(true);
		_shakeMagnitude = magnitude;
		await ToSignal(GetTree().CreateTimer(duration), "timeout");
		SetProcess(false);
		LerpBackToDefault();
	}
	
	void LerpBackToDefault()
	{
		_tween.InterpolateProperty( this, "offset", Offset, _defaultOffset, 0.2f);
		_tween.Start();
	}

	public override void _Process(float delta)
	{
		_shakeOffset.x = _rand.Randf() * _shakeMagnitude * 2f - _shakeMagnitude;
		_shakeOffset.y = _rand.Randf() * _shakeMagnitude * 2f - _shakeMagnitude;
		Offset = _defaultOffset + _shakeOffset;
	}
}
