using Godot;
using System;
using JamToolkit.Util;

public class Lobber : Node2D
{
	Sprite _sprite;
	PlayerController _pc;
	PackedScene _lobbedProjectileScene;
	float cooldown = 4f;
	Timer _timer;
	Tween _tween;
	AnimationPlayer _anim;
	float followSpeed = 0f;

	[Export] public ProjectileType ProjectileType { get; set; }

	public override void _Ready()
	{
		_timer = new Timer();
		_tween = new Tween();
		_anim = GetNode<AnimationPlayer>("AnimationPlayer");
		AddChild(_timer);
		AddChild(_tween);
		_timer.WaitTime = cooldown;
		_timer.OneShot = true;
		_timer.SafeConnect("timeout", this, nameof(OnCooldownEnded));
		_sprite = GetNode<Sprite>("TargetSprite");
		_pc = this.FindSingleton<PlayerController>();
		_lobbedProjectileScene = ResourceLoader.Load<PackedScene>("res://Atoms/Lobber/LobbedProjectile/LobbedProjectile.tscn");
		TweenFollowSpeed();
	}

	void TweenFollowSpeed()
	{
		_tween.StopAll();
		_tween.InterpolateProperty(
			this,
			"followSpeed",
			0f,
			1f,
			1f
		);
		_tween.Start();
		_anim.Play("follow");
	}

	void OnCooldownEnded()
	{
		_sprite.GlobalPosition = this.GlobalPosition;
		_sprite.Visible = true;
		TweenFollowSpeed();
		SetProcess(true);
	}

	Vector2 VectorLerp(Vector2 v1, Vector2 v2, float weight)
	{
		float yComp = Mathf.Lerp(v1.y, v2.y, weight);
		float xComp = Mathf.Lerp(v1.x, v2.x, weight);
		return new Vector2(xComp, yComp);
	}

	void Fire()
	{
		LobbedProjectile proj = _lobbedProjectileScene.Instance<LobbedProjectile>();
		AddChild(proj);
		proj.ProjectileType = this.ProjectileType;
		proj.GlobalPosition = this.GlobalPosition;
		proj.Shoot(_pc.GlobalPosition);
		_timer.Start();
		_sprite.Visible = false;
		SetProcess(false);
	}

	public override void _Process(float delta)
	{
		_sprite.GlobalPosition = VectorLerp(_sprite.GlobalPosition, _pc.GlobalPosition, delta * 2f * followSpeed);
		if (_sprite.GlobalPosition.DistanceSquaredTo(_pc.GlobalPosition) < 100f)
		{
			Fire();
		}
	}
}
