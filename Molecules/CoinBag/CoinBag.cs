using Godot;
using System;

public class CoinBag : ObjectiveObject
{
	Timer _timer;
	EventBus _eventBus;
	[Export]int coinsToDisburse = 10;
	PackedScene _coinScene;
	Random random;
	Godot.Collections.Array<SimpleCoin> _coins = new Godot.Collections.Array<SimpleCoin>();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_coinScene = ResourceLoader.Load<PackedScene>("res://Atoms/Coin/SimpleCoin.tscn");
		_timer = GetNode<Timer>("Timer");
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_timer.Autostart = false;
	}
	
	void OnCoinCollected(SimpleCoin coin)
	{
		_coins.Remove(coin);
		if (_coins.Count == 0)
		{
			this.NotifySuccess();
		}
	}
	
	void Burst()
	{
		int count = coinsToDisburse;
		float angle = 0;
		float angleStep = 360.0f / count;
		for(int i=0; i<count; i++)
		{
			angle += (float)GD.RandRange(0f, angleStep);
			var magnitude = GD.RandRange(20f, 30f);
			var coin = _coinScene.Instance<SimpleCoin>();
			GetParent().AddChild(coin);
			_coins.Add(coin);
			coin.Start(GlobalPosition, new Vector2(
				(float)magnitude * Mathf.Sin(angle),
				(float)magnitude * Mathf.Cos(angle)
			));
		}
		GetNode<Sprite>("Sprite").Visible = false;
	}
	
	public override ObjectiveObject Disable()
	{
		_timer.Stop();
		_timer.Disconnect("timeout", this, nameof(Burst));
		_eventBus.Disconnect(nameof(EventBus.CoinCollected), this, nameof(OnCoinCollected));
		Visible = false;
		return this;
	}

	public override void Enable()
	{
		Visible = true;
		_timer.Start();
		_timer.Connect("timeout", this, nameof(Burst));
		_eventBus.Connect(nameof(EventBus.CoinCollected), this, nameof(OnCoinCollected));
	}
}
