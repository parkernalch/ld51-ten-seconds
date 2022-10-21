using Godot;
using JamToolkit.Util;

public class Emitter : ObjectiveObject
{
	private PackedScene _projectile;
	private PlayerController _player;
	const float TwoPi = Mathf.Pi * 2;
	EventBus _eventBus;
	GameManager _gameManager;
	[Export]bool homing = false;
	[Export]float volleyInterval = 1f;
	Timer _timer;
	AnimationPlayer _anim;

	[Export]int missileCount = 0;

	[Export]int angleMin = 0;
	float angleMinRad;
	[Export]int angleMax = 180;
	float angleMaxRad;
	bool isActive = false;

	public override async void _Ready()
	{
		this.state = ObjectiveObject.OBJECTIVE_STATE.SUCCESS;
		_gameManager = GetNode<GameManager>("/root/GameManager");
		missileCount = Mathf.CeilToInt(_gameManager.CurrentRoom / 3f);
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_anim = GetNode<AnimationPlayer>("AnimationPlayer");
		_timer = GetNode<Timer>("Timer");
		_timer.WaitTime = volleyInterval;
		_timer.SafeConnect("timeout", this, nameof(StartNextWave));
		_projectile = ResourceLoader.Load<PackedScene>("res://Atoms/Projectile/Projectile.tscn");
		await ToSignal(GetTree(), "idle_frame");
		_eventBus.SafeConnect(nameof(EventBus.MissileConnected), this, nameof(OnMissileConnected));
		_player = this.FindSingleton<PlayerController>();
		angleMinRad = Mathf.Deg2Rad(angleMin);
		angleMaxRad = Mathf.Deg2Rad(angleMax);
		Enable();
		_anim.Play("RESET");
	}

	void OnMissileConnected(Projectile projectile)
	{
		this.NotifyFailure();
	}

	private void StartNextWave()
	{
		_anim.Play("flash");
	}

	private void FireNextWave()
	{
		if (!isActive) return;
		// normal arc spray
		// SprayArcWave(missileCount, angleMaxRad - angleMinRad, Mathf.Pi / 8, 1, 0);

		// sprinkler spray
		SprayArcWave(missileCount, angleMaxRad - angleMinRad, Mathf.Pi / 8, 1, .2f);
	}

	public async void EmitProjectile(bool homing, float angle)
	{
		var bullet = _projectile.Instance<Projectile>();
		this.AddChild(bullet);
		await ToSignal(GetTree(), "idle_frame");
		var speed = Mathf.Min(_gameManager.CurrentRoom * 10 + 250f, 350f);
		bullet.Start(this.Transform, new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)), homing ? _player : null, speed);
	}

	/// <summary>
	/// Spray <paramref name="count"/> projectiles in an arc from
	/// <paramref name="fromRotation"/> to <paramref name="toRotation"/>.
	/// </summary>
	/// <param name="count"></param>
	/// <param name="fromRotation">The starting angle for the first projectile</param>
	/// <param name="toRotation">The ending angle for the last projectile</param>
	public async void SprayProjectiles(int count, float fromRotation, float toRotation, float delay)
	{
		if (fromRotation > toRotation)
		{
			(fromRotation, toRotation) = (toRotation, fromRotation);
		}

		var delta = (toRotation - fromRotation) / count;

		for (var angle = fromRotation; angle < toRotation; angle += delta)
		{
			EmitProjectile(homing, CircleClamp(angle));

			await ToSignal(GetTree().CreateTimer(delay), "timeout");
		}
	}

	/// <summary>
	/// Spray <paramref name="count"/> projectiles out that span <paramref name="arcWidth"/>.
	///
	/// For each <paramref name="step"/> rotate the spray by <paramref name="rotationPerStep"/>.
	/// </summary>
	/// <param name="count">The number of missiles</param>
	/// <param name="arcWidth">width of the arc in radians</param>
	/// <param name="rotationPerStep">the offset per step in radians</param>
	/// <param name="step">arbitrary number in an increasing sequence</param>
	public void SprayArcWave(int count, float arcWidth, float rotationPerStep, int step, float delay)
	{
		var rotationOffset = rotationPerStep * step;

		SprayProjectiles(count, angleMinRad + rotationOffset + 0, angleMinRad + rotationOffset + arcWidth, delay);
	}

	private float CircleClamp(float value) => value % TwoPi;

	public void Clear() => this.RemoveChildren<Projectile>();

	public override void Disable()
	{
		isActive = false;
		Visible = false;
		_timer.Stop();
		_timer.SafeDisconnect("timeout", this, nameof(StartNextWave));
	}

	public override void Enable()
	{
		isActive = true;
		Visible = true;
		_timer.SafeConnect("timeout", this, nameof(StartNextWave));
		_timer.Start();
		_anim.Play("float");
	}
}
