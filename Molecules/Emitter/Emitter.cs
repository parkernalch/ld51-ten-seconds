using Godot;
using JamToolkit.Util;

public class Emitter : Node2D
{
	private PackedScene _projectile;
	private PackedScene _coin;
	private PlayerController _player;
	const float TwoPi = Mathf.Pi * 2;

	public override void _Ready()
	{
		_projectile = ResourceLoader.Load<PackedScene>("res://Atoms/Projectile/Projectile.tscn");
		_coin = ResourceLoader.Load<PackedScene>("res://Atoms/Coin/Coin.tscn");
		_player = this.FindSingleton<PlayerController>();
	}

	public void EmitProjectile(bool homing, float angle)
	{
		var bullet = _projectile.Instance<Projectile>();
		this.AddChild(bullet);
		bullet.Start(this.Transform, angle, homing ? _player : null);
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
			EmitProjectile(false, CircleClamp(angle));

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

		SprayProjectiles(count, rotationOffset + 0, rotationOffset + arcWidth, delay);
	}

	private float CircleClamp(float value) => value % TwoPi;

	public void EmitCoin()
	{
		var coin = _coin.Instance<Coin>();
		this.AddChild(coin);

		// currently we would want to check this position to verify
		// there is nothing of interest at that location
		// can we check the tile map for what the tile in that location is?
		// if its something we dont want to we guess a new position
		// if guessing the position has too many failures we may need to collect
		// all the open positions of the tile map and randomly select one
		var rect = this.GetViewportRect().Size;
		var x = (float)GD.RandRange(0, rect.x);
		var y = (float)GD.RandRange(0, rect.y);
		var target = new Vector2(x, y);

		coin.Start(this.Transform, target);
	}

	public void Clear()
	{
		this.RemoveChildren<Projectile>();
		this.RemoveChildren<Coin>();
	}
}
