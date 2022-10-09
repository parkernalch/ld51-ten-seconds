using Godot;
using System;
using JamToolkit.Util;

public class PlayerController : KinematicBody2D
{
	[Export] public int tileSize;
	[Export] public int tilesPerSecond;
	[Export] public float timeToAccelerate;
	[Export] public float timeToDecelerate;
	[Export] public float dashTime;
	[Export] public float dashCooldown;
	[Export] public float dashMultiplier;
	[Export] public float Friction { get; set; }
	[Export] public int Health { get; set; } = 10;

	private int _lastHealth;
	private EventBus _eventBus;
	private AnimationNodeStateMachinePlayback _stateMachine;

	private Vector2 _inputDirection;
	private Vector2 _acceleration;
	private Vector2 _velocity;

	private float _accelerationFactor;
	private float _topSpeed;
	private float _hurtDuration;

	private bool _isDashing;
	private bool _canDash;

	private Sprite _sprite;
	private AnimationTree _animationTree;
	private Tween _tween;
	private ShaderMaterial _material;

	public override void _Ready()
	{
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_tween = this.GetNode<Tween>();
		_animationTree = this.GetNode<AnimationTree>();
		_material = (ShaderMaterial)Material;
		_stateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
		_sprite = this.GetNode<Sprite>();

		_lastHealth = Health;
		_velocity = new Vector2(0, 0);
		_inputDirection = new Vector2(0, 0);
		_isDashing = false;
		_canDash = true;
		Assert.True(timeToAccelerate > 0, "acceleration time must be nonzero");
		Assert.True(timeToDecelerate > 0, "deceleration time must be nonzero");
		ResetTopSpeed();

		// _eventBus.Connect(nameof(EventBus.MissileConnected), this, nameof(OnMissileHit));
	}

	public void SetTopSpeed(float speed)
	{
		_topSpeed = speed;
		SetAccelerationFactor(_topSpeed);
	}

	public void ResetTopSpeed()
	{
		_topSpeed = tilesPerSecond * tileSize;
		SetAccelerationFactor(_topSpeed);
	}

	private void SetAccelerationFactor(float topSpeed) => _accelerationFactor = topSpeed / (timeToAccelerate * timeToAccelerate);

	public bool IsVulnerable() => !_isDashing;

	public override void _Process(float delta)
	{
		if (IsDead)
		{
			_stateMachine.Travel("Die");
			return;
		}

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
			BlendedTravel("Dash");
		}
		else if (IsRunning)
		{
			BlendedTravel("Run");
		}
		else
		{
			BlendedTravel("Idle");
		}
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
		if (IsDead) return;

		// Calculate velocity vector
		_inputDirection.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
		_inputDirection.y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

		// if we are dashing, allow our max acceleration and speed to increase
		var force = _isDashing ? _accelerationFactor * dashMultiplier : _accelerationFactor;
		var clampVelocity = _isDashing ? _topSpeed * dashMultiplier : _topSpeed;

		// standard physics calculation
		_acceleration = _inputDirection * force;

		// if we are not accelerating, apply friction
		if (_acceleration.LengthSquared() == 0)
		{
			_acceleration = _velocity * -Friction * delta;
		}

		// clamp our max velocity
		_velocity = (_velocity + _acceleration * delta).LimitLength(clampVelocity);

		// if we are not moving a minimum velocity, stop moving
		if(_velocity.LengthSquared() < 100) _velocity = Vector2.Zero;

		if (IsDead)
		{
			return;
		}

		MoveAndSlide(_velocity);

	}

	private void BlendedTravel(string animation)
	{
		_animationTree.Set($"parameters/{animation}/blend_position", _inputDirection.Normalized());
		_stateMachine.Travel(animation);
	}

	private bool IsHurt => Health != _lastHealth;

	private bool IsRunning => _velocity.LengthSquared() > 0;

	private bool IsDead => Health == 0;

	async void Dash()
	{
		if (_isDashing || !_canDash) return;
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

	public void TakeProjectileDamage(Projectile projectile)
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
