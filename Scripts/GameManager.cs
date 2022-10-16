using Godot;
using System;

public class GameManager : Node
{
	
	int objectiveCompletedCount;
	int objectiveFailedCount;
	
	public int currentRoom = 0;
	public int coinCount = 0;
	
	int objectivesPerRoom = 5;
	
	EventBus _eventBus;
	SceneManager _sceneManager;
	PlayerController _player;
	
	public override void _Ready()
	{
		_sceneManager = GetNode<SceneManager>("/root/SceneManager");
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_eventBus.Connect(nameof(EventBus.EnteredRoom), this, nameof(OnEnteredRoom));
		_eventBus.Connect(nameof(EventBus.ObjectiveCompleted), this, nameof(OnObjectiveCompleted));
		_eventBus.Connect(nameof(EventBus.ObjectiveFailed), this, nameof(OnObjectiveFailed));
		_eventBus.Connect(nameof(EventBus.CountdownEnded), this, nameof(OnCountdownEnded));
		_eventBus.Connect(nameof(EventBus.PlayerChanged), this, nameof(OnPlayerChanged));
		_eventBus.Connect(nameof(EventBus.CoinCollected), this, nameof(OnCoinCollected));
	}
	
	void OnPlayerChanged(PlayerController player)
	{
		_player = player;
	}
	
	public PlayerController GetPlayer()
	{
		return _player;
	}
	
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
		if (objectiveFailedCount == Mathf.Max(6 - currentRoom, 1))
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
	
	public int GetCurrentDifficulty()
	{
		return objectiveCompletedCount + objectiveFailedCount;
	}
	
	public int OnCoinCollected(SimpleCoin coin)
	{
		coinCount += coin.value;
		_eventBus.ChangeCoinCount(coinCount);
		return coinCount;
	}
}
