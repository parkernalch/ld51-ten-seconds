using Godot;
using System;
using JamToolkit.Util;

/// <summary>
/// TODO: this just sets up a "Player" that will be replaced with the real player and creates missiles every 10s
/// </summary>
public class MainScene : Node2D
{
	EventBus _eventBus;
	private float time = 0;
	private PlayerController _player;
	private Emitter _emitter;
	private const float Interval = 5f;
	private int _level = 1;
	// If GameManager.currentLevel > 0, load increasingly difficult Obstacles
	public override void _Ready()
	{
		_eventBus = GetNode<EventBus>("/root/EventBus");
		GameManager _gameManager = GetNode<GameManager>("/root/GameManager");
		_emitter = this.GetNode<Emitter>("Emitter");
		_player = (PlayerController)this.FindNode("PlayerController");
		_eventBus.LoadPlayer(_player);
		_eventBus.Connect(nameof(EventBus.CoinCollected), this, nameof(OnCoinCollected));
		_eventBus.Connect(nameof(EventBus.ObjectiveCompleted), this, nameof(OnObjectiveCompleted));
		_eventBus.Connect(nameof(EventBus.ObjectiveFailed), this, nameof(OnObjectiveFailed));
		_eventBus.Connect(nameof(EventBus.LevelCompleted), this, nameof(OnLevelCompleted));
		_eventBus.EnterRoom(_gameManager.currentRoom);
	}
	
	public PlayerController GetPlayer() => _player;

	void OnCoinCollected(Coin coin) => GD.Print("Got a COIN!");

	void OnObjectiveCompleted() => GD.Print("Objective Complete!");
	void OnObjectiveFailed() => GD.Print("Objective Failed...");

	void OnLevelCompleted()
	{
		SetProcess(false);
	}

	public override void _Process(float delta)
	{
		time += delta;

		if (time > Interval)
		{
			time -= Interval;

			// if (_level % 2 == 0)
			// {
			// 	_emitter.Clear();
			// }
			// else
			// {
			_emitter.EmitCoin();
			// }

			// normal arc spray
			_emitter.SprayArcWave(5, Mathf.Pi / 4, Mathf.Pi / 8, _level, 0);

			// sprinkler spray
			_emitter.SprayArcWave(10, Mathf.Pi / 2, Mathf.Pi / 4, 1, .2f);

			_level += 1;
		}
	}
}
