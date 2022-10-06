using Godot;
using System;
using JamToolkit.Util;

public class PlayerController : KinematicBody2D
{
	EventBus _eventBus;
	private Vector2 _inputDirection;

	private AnimationNodeStateMachinePlayback _stateMachine;
	private Vector2 _velocity;
	[Export] public int tileSize;
	[Export] public int tilesPerSecond;
	[Export] public float timeToAccelerate;
	[Export] public float timeToDecelerate;
	[Export] public int Health { get; set; } = 10;

	private int _lastHealth;

	private float _accelerationFactor;
	private float _decelerationFactor;
	private float _topSpeed;
	private float _topSpeedSquared;
	private float _hurtDuration;

	private bool _isDashing;
	private bool _canDash;

	[Export] public float dashTime;
	[Export] public float dashCooldown;
	[Export] public float dashMultiplier;

	private Sprite _sprite;
	private AnimationTree _animationTree;
	private Tween _tween;
	private ShaderMaterial _material;

	public override void _Ready()
	{
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_tween = this.GetNode<Tween>();
		_animationTree = this.GetNode<AnimationTree>();
		_material = (ShaderMaterial)this.Material;
		_stateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");

		_sprite = this.FindChild<Sprite>();
		_lastHealth = Health;
		_velocity = new Vector2(0, 0);
		_inputDirection = new Vector2(0, 0);
		_isDashing = false;
		_canDash = true;
		Assert.True(timeToAccelerate > 0, "acceleration time must be nonzero");
		Assert.True(timeToDecelerate > 0, "deceleration tme must be nonzero");
		_topSpeed = tilesPerSecond * tileSize;
		_topSpeedSquared = Mathf.Pow(_topSpeed, 2);
		_accelerationFactor = _topSpeed / (timeToAccelerate * timeToAccelerate);
		_decelerationFactor = _topSpeed / timeToDecelerate;

		_eventBus.Connect(nameof(EventBus.MissileConnected), this, nameof(OnMissileHit));
	}
	
	public bool IsVulnerable()
	{
		return !_isDashing;
	}

	public override void _Process(float delta)
	{
		Input.IsActionPressed("run");

		if (IsDead) return;
		_inputDirection.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
		_inputDirection.y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");
	}

	public override void _Input(InputEvent e)
	{
		if (e is InputEventKey key && key.Scancode == (int)KeyList.Escape)
		{
			GetTree().Quit();
		}

		if (IsDead) return;


		if (e.IsActionPressed("dash") && IsRunning)
		{
			//			InputEventAction a = e as InputEventAction;
			//			GD.Print(a);
			//			if (a.Action == "dash" && a.Pressed)
			//			{
			Dash();
			//			}
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (IsDead)
		{
			_stateMachine.Travel("Die");
			return;
		}

		var dashVelocity = _isDashing ? _velocity : ApplyForces(delta, _inputDirection);
		_velocity = MoveAndSlide(dashVelocity);

		if (IsHurt)
		{
			_stateMachine.Travel("Hurt");
			_hurtDuration += delta;
			if (_hurtDuration > 0.25)
			{
				_hurtDuration = 0;
				_lastHealth = Health;
			}
		}
		else if (_isDashing)
		{
			_stateMachine.Travel("Dash");
		}
		else if (IsRunning)
		{
			_stateMachine.Travel("Run");;
			_animationTree.Set("parameters/Run/blend_position", _velocity);
		}
		else
		{
			_stateMachine.Travel("Idle");
			_animationTree.Set("parameters/Idle/blend_position", _velocity);
		}
	}

	private bool IsHurt => Health != _lastHealth;

	private bool IsRunning => _velocity.LengthSquared() > 0;

	private bool IsDead => Health == 0;


	Vector2 ApplyForces(float delta, Vector2 inputDirection)
	{
		var v = _velocity;
		if (_inputDirection.LengthSquared() == 0)
		{
			// Decelerate
			v -= _velocity.Normalized() * _decelerationFactor * delta;
			if (
				_velocity.Dot(v) < 0 ||
				v.LengthSquared() <= _topSpeedSquared * 0.1
			)
			{
				return Vector2.Zero;
			}
		}
		else
		{
			// accelerate
			v += inputDirection.Normalized() * _accelerationFactor * delta;
			if (
				_velocity.LengthSquared() >= _topSpeedSquared * 0.95f ||
				v.LengthSquared() >= _topSpeedSquared * 0.95f
			)
			{
				// Allow for full control at or around top speed
				return inputDirection.Normalized() * _topSpeed;
			}
		}

		return v;
	}

	async void Dash()
	{
		if (_isDashing || !_canDash) return;
		_velocity = _inputDirection.Normalized() * _topSpeed * dashMultiplier;
		_isDashing = true;
		_sprite.SelfModulate = new Color(1, 1, 1, 0.5f);
		await Delay(dashTime);
		_isDashing = false;
		_sprite.SelfModulate = new Color(1, 1, 1);
		_canDash = false;
		await Delay(dashCooldown);
		_canDash = true;
	}

	private SignalAwaiter Delay(float timeSec) => ToSignal(GetTree().CreateTimer(timeSec), "timeout");

	void OnMissileHit(Projectile projectile)
	{
		if (Health == 0) return;

		PlayShaderDamage();

		Health--;
		if (Health == 0)
		{
			_velocity = Vector2.Zero;
		}
	}

	private void PlayShaderDamage()
	{
		_tween.StopAll();
		_tween.InterpolateProperty(
			_material,
			"shader_param/flash_modifier",
			0,
			1,
			0.1f,
			Tween.TransitionType.Sine,
			Tween.EaseType.Out
		);
		_tween.InterpolateProperty(
			_material,
			"shader_param/flash_modifier",
			1,
			0,
			0.1f,
			Tween.TransitionType.Sine,
			Tween.EaseType.Out
		);
		_tween.Start();
	}
}
