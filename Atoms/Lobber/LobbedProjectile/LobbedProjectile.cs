using Godot;
using System;
using JamToolkit.Util;

public class LobbedProjectile : Node2D
{
	Path2D _path;
	PathFollow2D _pathFollow;
	Sprite _shadowSprite;
	Sprite _projectileSprite;
	Node2D _targetPosition;
	CPUParticles2D _poofParticles;
	CPUParticles2D _shardParticles;
	Tween _tween;
	Timer _timer;
	SoundManager _sounds;

	float speed = 10f;
	[Export] float airTime = 1f;
	[Export] public ProjectileType ProjectileType { get; set; }
	bool isActive = false;

	public override void _Ready()
	{
		_sounds = GetNode<SoundManager>("/root/SoundManager");
		_timer = new Timer();
		AddChild(_timer);
		_timer.SafeConnect("timeout", this, nameof(OnTimerFinished));
		_tween = new Tween();
		AddChild(_tween);
		_poofParticles = GetNode<CPUParticles2D>("PoofParticles");
		_shardParticles = GetNode<CPUParticles2D>("ShardParticles");
		_targetPosition = GetNode<Node2D>("TargetPosition");
		_shadowSprite = GetNode<Sprite>("ShadowSprite");
		_projectileSprite = GetNode<Sprite>("ProjectilePath/PathFollow2D/ProjectileSprite");
		_pathFollow = GetNode<PathFollow2D>("ProjectilePath/PathFollow2D");
		_path = GetNode<Path2D>("ProjectilePath");
		var curveLength = _path.Curve.GetBakedLength();
		speed = curveLength / airTime;
		Shoot(_targetPosition.GlobalPosition);
		_pathFollow.Visible = false;
		_shadowSprite.Visible = false;
	}

	public void Shoot(Vector2 targetPos)
	{
		_targetPosition.GlobalPosition = targetPos;
		_path.GlobalPosition = Vector2.Zero;
		Curve2D curve = new Curve2D();
		curve.AddPoint(this.GlobalPosition, @out: Vector2.Up * 100f);
		// For low-altitude cruise
		// curve.AddPoint(targetPos, @in: (this.GlobalPosition - targetPos) * 0.5f);
		// For High-altitude strike
		curve.AddPoint(targetPos, @in: Vector2.Up * 50f);
		_path.Curve = curve;
		speed = curve.GetBakedLength() / airTime;

		Vector2 startSize = _shadowSprite.Scale;
		Vector2 midSize = new Vector2(0.1f, 0.1f);
		Color startColor = _shadowSprite.SelfModulate;
		Color endColor = new Color(0, 0, 0, 0.1f);
		_tween.InterpolateProperty(
			_pathFollow,
			"unit_offset",
			0f,
			1f,
			airTime,
			Tween.TransitionType.Linear,
			Tween.EaseType.In
		);
		_tween.InterpolateProperty(
			_shadowSprite,
			"global_position",
			curve.GetPointPosition(0),
			_targetPosition.GlobalPosition,
			airTime,
			Tween.TransitionType.Linear,
			Tween.EaseType.In
		);
		_timer.WaitTime = airTime;
		_timer.OneShot = true;
		_timer.Start();
		_tween.Start();
		_poofParticles.Emitting = true;
		_pathFollow.Visible = true;
		_shadowSprite.Visible = true;
		isActive = true;
	}

	void OnTimerFinished()
	{
		_shardParticles.GlobalPosition = _targetPosition.GlobalPosition;
		_shardParticles.Emitting = true;
		_pathFollow.Visible = false;
		_shadowSprite.Visible = false;
		GetNode<EventBus>("/root/EventBus").NotifyProjectileImpact(ProjectileType, _targetPosition.GlobalPosition, 1);
		switch(ProjectileType)
		{
			case ProjectileType.Cannon:
				_sounds.PlaySound(SoundManager.SOUND_TYPE.CANNONBALL);
				break;
			case ProjectileType.Ice:
				_sounds.PlaySound(SoundManager.SOUND_TYPE.ICE);
				break;
			case ProjectileType.Web:
				_sounds.PlaySound(SoundManager.SOUND_TYPE.WEB);
				break;
		}

		SetProcess(true);
		isActive = false;
	}

	public override void _Process(float delta)
	{
		_projectileSprite.Rotation = 0;
		if (Input.GetMouseButtonMask() == 1)
		{
			Shoot(GetGlobalMousePosition());
			SetProcess(false);
		}
	}
}
