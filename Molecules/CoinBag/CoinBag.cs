using Godot;
using System;
using JamToolkit.Util;

public class CoinBag : ObjectiveObject
{
	Timer _timer;
	EventBus _eventBus;
	[Export]int coinsToDisburse = 10;
	PackedScene _coinScene;
	Random random;
	Godot.Collections.Array<SimpleCoin> _coins = new Godot.Collections.Array<SimpleCoin>();
	AnimationPlayer _anim;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_anim = GetNode<AnimationPlayer>("AnimationPlayer");
		_coinScene = ResourceLoader.Load<PackedScene>("res://Atoms/Coin/SimpleCoin.tscn");
		_timer = GetNode<Timer>("Timer");
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_timer.Autostart = false;
		_anim.Play("squash");
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
			var magnitude = GD.RandRange(15f, 25f);
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

	public override void Disable()
	{
		if (IsConnected(nameof(EventBus.CoinCollected), this, nameof(OnCoinCollected)))
		{
			_timer.Stop();
			_timer.SafeDisconnect("timeout", this, nameof(Burst));
			_eventBus.SafeDisconnect(nameof(EventBus.CoinCollected), this, nameof(OnCoinCollected));
			Visible = false;
		}
	}

	public override void Enable()
	{
		Visible = true;
		_timer.Start();
		_timer.SafeConnect("timeout", this, nameof(Burst));
		_eventBus.SafeConnect(nameof(EventBus.CoinCollected), this, nameof(OnCoinCollected));
	}
}
