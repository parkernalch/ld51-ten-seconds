using Godot;
using System;

public class CoinBag : Node2D
{
	PackedScene _coinScene;
	Random random;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_coinScene = ResourceLoader.Load<PackedScene>("res://Atoms/Coin/SimpleCoin.tscn");
		GetNode<Timer>("Timer").Connect("timeout", this, nameof(Burst));
	}
	
	void Burst()
	{
		int count = 100;
		float angle = 0;
		float angleStep = 360.0f / count;
		for(int i=0; i<count; i++)
		{
			angle += (float)GD.RandRange(0f, angleStep);
			var magnitude = GD.RandRange(20f, 30f);
			var coin = _coinScene.Instance<SimpleCoin>();
			GetParent().AddChild(coin);
			coin.Start(GlobalPosition, new Vector2(
				(float)magnitude * Mathf.Sin(angle),
				(float)magnitude * Mathf.Cos(angle)
			));
		}
		QueueFree();
	}
}
