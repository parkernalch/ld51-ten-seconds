using Godot;
using System;
using JamToolkit.Util;

public class PlayerController : KinematicBody2D
{
	private Vector2 _inputDirection;
	private bool _isRunning;
	private Vector2 _velocity;
	[Export]public int tileSize;
	[Export]public int tilesPerSecond;
	[Export]public float timeToAccelerate;
	[Export]public float timeToDecelerate;
	
	private float _accelerationFactor;
	private float _decelerationFactor;
	private float _topSpeed;
	private float _topSpeedSquared;
	
	private bool _isDashing;
	[Export]public float dashTime;
	[Export]public float dashMultiplier;
	
	private Sprite _sprite;
		
	public override void _Ready()
	{
		_sprite = (Sprite)GetNode("Sprite");
		_velocity = new Vector2(0, 0);
		_inputDirection = new Vector2(0, 0);
		_isRunning = false;
		_isDashing = false;
		Assert.True(timeToAccelerate > 0, "acceleration time must be nonzero");
		Assert.True(timeToDecelerate > 0, "deceleration tme must be nonzero");
		_topSpeed = tilesPerSecond * tileSize;
		_topSpeedSquared = Mathf.Pow(_topSpeed, 2);
		_accelerationFactor = _topSpeed / (timeToAccelerate * timeToAccelerate);
		_decelerationFactor = _topSpeed / (timeToDecelerate);
	}
	
	public override void _Process(float delta) {
		_isRunning = Input.IsActionPressed("run");
		_inputDirection.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
		_inputDirection.y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");
	}

	public override void _Input(InputEvent e)
	{
		if (e.IsActionPressed("dash"))
		{
//			InputEventAction a = e as InputEventAction;
//			GD.Print(a);
//			if (a.Action == "dash" && a.Pressed)
//			{
			Dash();
//			}
		}
	}
	
	public override void _PhysicsProcess(float delta) {
		int runModifier = _isRunning ? 1 : 2;
		if (_isDashing)
		{
			_velocity = MoveAndSlide(_velocity);
		} else
		{
			_velocity = MoveAndSlide(ApplyForces(delta, _inputDirection));
		}
	}
	
	public Vector2 ApplyForces(float delta, Vector2 inputDirection) {
		Vector2 v = _velocity;
		float v2 = _velocity.LengthSquared();
		if (_inputDirection.LengthSquared() == 0)
		{
			// Decelerate
			v -= _velocity.Normalized() * _decelerationFactor * delta;
			if (
				_velocity.Dot(v) < 0 ||
				v.LengthSquared() <= _topSpeedSquared * 0.1
			) {
				return v * 0;
			}
		} else 
		{
			// accelerate
			v += inputDirection.Normalized() * _accelerationFactor * delta;
			if (
				_velocity.LengthSquared() >= _topSpeedSquared * 0.95f ||
				v.LengthSquared() >= _topSpeedSquared * 0.95f
			) {
				// Allow for full control at or around top speed
				return inputDirection.Normalized() * _topSpeed;
			}
		}
		return v;
	}
	
	async public void Dash()
	{
		if (_isDashing) return;
		_velocity = _inputDirection.Normalized() * _topSpeed * dashMultiplier;
		_isDashing = true;
		_sprite.SelfModulate = new Color(1, 1, 1, 0.5f);
		// TODO: disable then enable relevant area2Ds that handle interaction with
		// hazards, missiles, coins, etc. as needed
		await ToSignal(GetTree().CreateTimer(dashTime), "timeout");
		_isDashing = false;
		_sprite.SelfModulate = new Color(1, 1, 1, 1);
	}
}
