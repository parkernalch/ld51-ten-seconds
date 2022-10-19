using System;
using Godot;
using JamToolkit.Util;

public class GameManager : Node
{
	int objectiveCompletedCount;
	int objectiveFailedCount;

	public int CurrentRoom
	{
		get => _currentRoom;
		set => _currentRoom = Math.Max(value, 0);
	}

	public int coinCount;

	int objectivesPerRoom = 5;

	EventBus _eventBus;
	SceneManager _sceneManager;
	PlayerController _player;
	private int _currentRoom;

	public override void _Ready()
	{
		_sceneManager = GetNode<SceneManager>("/root/SceneManager");
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_eventBus.SafeConnect(nameof(EventBus.EnteredRoom), this, nameof(OnEnteredRoom));
		_eventBus.SafeConnect(nameof(EventBus.ObjectiveCompleted), this, nameof(OnObjectiveCompleted));
		_eventBus.SafeConnect(nameof(EventBus.ObjectiveFailed), this, nameof(OnObjectiveFailed));
		_eventBus.SafeConnect(nameof(EventBus.CountdownEnded), this, nameof(OnCountdownEnded));
		_eventBus.SafeConnect(nameof(EventBus.PlayerChanged), this, nameof(OnPlayerChanged));
		_eventBus.SafeConnect(nameof(EventBus.CoinCollected), this, nameof(OnCoinCollected));
		_eventBus.SafeConnect(nameof(EventBus.MissileConnected), this, nameof(OnMissileConnected));
	}

	void OnPlayerChanged(PlayerController player)
	{
		_player = player;
	}

	public PlayerController Player => _player;

	void OnObjectiveCompleted()
	{
		objectiveCompletedCount++;
		_eventBus.ChangeObjectiveCount(objectiveCompletedCount, objectiveFailedCount);
		if (objectiveCompletedCount == objectivesPerRoom)
		{
			_eventBus.CompleteLevel();
		}
	}
	void OnObjectiveFailed()
	{
		objectiveFailedCount++;
		_eventBus.ChangeObjectiveCount(objectiveCompletedCount, objectiveFailedCount);
		// the further you get in the dungeon, the less forgiving it is
		if (objectiveFailedCount == Mathf.Max(6 - CurrentRoom, 1))
		{
			_eventBus.FailLevel();
		}
	}

	void OnEnteredRoom(int roomIndex)
	{
		objectiveCompletedCount = 0;
		objectiveFailedCount = 0;
	}

	private void OnCountdownEnded()
	{
		// TODO: Get Next Objective
		// _sceneManager.StartObjective(objective);
	}

	private void StartGame()
	{
		objectiveCompletedCount = 0;
		objectiveFailedCount = 0;
	}

	public void NextLevel()
	{
		GD.Print("Next Level");
		// _sceneManager.LoadNextLevel();
	}
	public void PrevLevel()
	{
		GD.Print("Next Level");
		// _sceneManager.LoadNextLevel();
	}

	public int GetCurrentDifficulty() => objectiveCompletedCount + objectiveFailedCount;

	public int OnCoinCollected(SimpleCoin coin)
	{
		coinCount += coin.value;
		_eventBus.ChangeCoinCount(coinCount);
		return coinCount;
	}

	void OnMissileConnected(Projectile p)
	{
		coinCount = Mathf.Max(coinCount - p.Damage, 0);
		_eventBus.ChangeCoinCount(coinCount);
	}
}
