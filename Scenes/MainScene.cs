using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using JamToolkit.Util;

public class MainScene : Node2D
{
	PackedScene _coinBagScene;
	EventBus _eventBus;
	Timer _timer;
	CoinBurst _coinBurst;
	private float time = 0;
	private PlayerController _player;
	private int _level = 1;
	Godot.Collections.Array<RayCast2D> coinCasts = new Godot.Collections.Array<RayCast2D>();
	private Vector2 _lastCoinSpawn;

	PackedScene _lobberScene = ResourceLoader.Load<PackedScene>("res://Atoms/Lobber/Lobber.tscn");
	public override void _Ready()
	{
		PackedScene coinBurstScene = ResourceLoader.Load<PackedScene>("res://Atoms/CoinBurst/CoinBurst.tscn");
		_coinBurst = coinBurstScene.Instance<CoinBurst>();
		AddChild(_coinBurst);
		_coinBagScene = ResourceLoader.Load<PackedScene>("res://Molecules/CoinBag/CoinBag.tscn");
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_timer = new Timer();
		AddChild(_timer);
		_timer.WaitTime = 3f;
		_timer.OneShot = false;
		_timer.Autostart = true;
		_timer.SafeConnect("timeout", this, nameof(OnTimerTimeout));
		GameManager gameManager = GetNode<GameManager>("/root/GameManager");
		_player = (PlayerController)this.FindNode("PlayerController");
		if (gameManager.IsFirstVisit())
		{
			SpawnLobbers(gameManager.CurrentRoom);
		}
		_eventBus.LoadPlayer(_player);
		_eventBus.EnterRoom(gameManager.CurrentRoom);
		_eventBus.SafeConnect(nameof(EventBus.MissileConnected), this, nameof(OnMissileConnected));
		for (int i = 0; i < 8; i++)
		{
			RayCast2D cast = new RayCast2D();
			_player.AddChild(cast);
			cast.SetCollisionMaskBit(0, false);
			cast.SetCollisionMaskBit(4, true);
			cast.Enabled = false;
			cast.ExcludeParent = true;
			cast.GlobalPosition = _player.GlobalPosition;
			coinCasts.Add(cast);
		}

		_timer.Start();
	}

	private void SpawnLobbers(int currentRoom)
	{
		var tileSystem = this.FindChild<TileSystem>();
		var xs = new[] { 47, 176, 336, 464 };
		var ys = new[] { 47, 144, 237 };
		var positions = (
			from x in xs
			from y in ys
			select new Vector2(x, y)
		).ToList();

		// every 3 levels we get another lobber
		var lobberCount = Math.Min(12, (currentRoom / 3) + 1);
		while (lobberCount-- > 0 || positions.Count == 0)
		{
			var i = (int)(GD.Randf() * positions.Count);
			var position = positions[i];
			var type = (ProjectileType)GD.RandRange(0, 2.99);
			positions.RemoveAt(i);

			var lobber = _lobberScene.Instance<Lobber>();
			lobber.GlobalPosition = position;
			lobber.ProjectileType = type;
			tileSystem.TrackNode(lobber);
		}
	}

	protected override void Dispose(bool disposing)
	{
		_timer?.SafeDisconnect("timeout", this, nameof(OnTimerTimeout));
		_eventBus?.SafeDisconnect(nameof(EventBus.MissileConnected), this, nameof(OnMissileConnected));
		base.Dispose(disposing);
	}

	void OnMissileConnected(Projectile p)
	{
		_coinBurst.DoBurst(p.Damage, p.GlobalPosition);
	}

	void OnTimerTimeout()
	{
		foreach(RayCast2D cast in coinCasts)
		{
			cast.Enabled = true;
			cast.ForceRaycastUpdate();
		}
		float maxDistance = 0f;
		Vector2 collisionPoint = Vector2.Zero;
		for(int i=0; i<8; i++)
		{
			Vector2 tempCollisionPoint = coinCasts[i].GetCollisionPoint();
			float dist = tempCollisionPoint.DistanceSquaredTo(_player.GlobalPosition);
			if (dist > maxDistance && tempCollisionPoint != _lastCoinSpawn)
			{
				maxDistance = dist;
				collisionPoint = tempCollisionPoint;
			}
		}
		_lastCoinSpawn = collisionPoint;
		if (collisionPoint != Vector2.Zero)
		{
			Vector2 effectVector = (collisionPoint - _player.GlobalPosition) * 0.8f;
			SpawnCoinBag(_player.GlobalPosition + effectVector);
		}
		foreach(RayCast2D cast in coinCasts)
		{
			cast.Enabled = false;
		}
	}

	public override void _Draw()
	{
		for(int i=0; i < 8; i++)
		{
			Vector2 point = coinCasts[i].GetCollisionPoint();
			DrawCircle(point, 5f, new Color(1, 1, 1, 1));
		}
	}

	void SpawnCoinBag(Vector2 globalSpawnPoint)
	{
		CoinBag coinBag = _coinBagScene.Instance<CoinBag>();
		AddChild(coinBag);
		coinBag.GlobalPosition = globalSpawnPoint;
		coinBag.Enable();
	}

	public override void _PhysicsProcess(float delta)
	{
		coinCasts[0].CastTo = Vector2.Up.Rotated(Mathf.Pi / 8) * 500f;
		coinCasts[1].CastTo = Vector2.Right.Rotated(Mathf.Pi / 8) * 500f;
		coinCasts[2].CastTo = Vector2.Left.Rotated(Mathf.Pi / 8) * 500f;
		coinCasts[3].CastTo = Vector2.Down.Rotated(Mathf.Pi / 8) * 500f;
		coinCasts[4].CastTo = (Vector2.Up + Vector2.Right).Rotated(Mathf.Pi / 8) * 500f;
		coinCasts[5].CastTo = (Vector2.Up + Vector2.Left).Rotated(Mathf.Pi / 8) * 500f;
		coinCasts[6].CastTo = (Vector2.Down + Vector2.Right).Rotated(Mathf.Pi / 8) * 500f;
		coinCasts[7].CastTo = (Vector2.Down + Vector2.Left).Rotated(Mathf.Pi / 8) * 500f;
	}

	public PlayerController GetPlayer() => _player;
}
