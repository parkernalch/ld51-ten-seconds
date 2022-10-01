using Godot;
using System;
using JamToolkit.Util;

/// <summary>
/// TODO: this just sets up a "Player" that will be replaced with the real player and creates missiles every 10s
/// </summary>
public class MainScene : Node2D
{
	private float time = 0;
	private PackedScene _projectile;
	private PlayerController _player;


	public override void _Ready()
	{
		_projectile = ResourceLoader.Load<PackedScene>("res://Atoms/Projectile/Projectile.tscn");
		_player = (PlayerController)this.FindNode("PlayerController");
	}

	public override void _Process(float delta)
	{
		time += delta;

		if (time > 10)
		{
			time -= 10;
			var bullet = _projectile.Instance<Projectile>();
			this.AddChild(bullet);
			bullet.Start(this.Transform, _player);
		}
	}
}
