using System;
using Godot;
using System.Collections.Generic;
using JamToolkit.Util;
using TenSeconds.Data;

public class GameManager : Node
{
	int objectiveCompletedCount;
	int objectiveFailedCount;

	public List<int> RoomsVisitedInRun = new List<int>();
	public int PreviousRoom;
	public int CurrentRoom
	{
		get => _currentRoom;
		set
		{
			PreviousRoom = _currentRoom;
			_currentRoom = Math.Max(value, 0);
			Scores.FurthestRoom = Math.Max(Scores.FurthestRoom, _currentRoom);
		}
	}

	public int coinCount;

	int objectivesPerRoom = 5;

	EventBus _eventBus;
	SceneManager _sceneManager;
	PlayerController _player;
	private int _currentRoom;
	public SaveState Scores { get; private set; }
	private const float SaveInterval = 2.0f;
	private float _timeUntilSave = SaveInterval;

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
		Scores = SaveState.Load();
		GD.Randomize();
	}

	public override void _Process(float delta)
	{
		_timeUntilSave -= SaveInterval;
		if (_timeUntilSave < 0)
		{
			Scores.Save();
			_timeUntilSave += SaveInterval;
		}
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
		if(RoomsVisitedInRun.Contains(roomIndex)) return;
		RoomsVisitedInRun.Add(roomIndex);
	}

	private void OnCountdownEnded()
	{
		// TODO: Get Next Objective
		// _sceneManager.StartObjective(objective);
	}

	public void StartGame()
	{
		objectiveCompletedCount = 0;
		objectiveFailedCount = 0;
		RoomsVisitedInRun.Clear();
	}

	public void NextLevel()
	{
		GD.Print("Next Level");
		// _sceneManager.LoadNextLevel();
	}
	public void PrevLevel()
	{
		GD.Print("Previous Level");
		// _sceneManager.LoadNextLevel();
	}

	public int SetCurrentRoom(int room)
	{
		this.PreviousRoom = this.CurrentRoom;
		this.CurrentRoom = room;
		return this.CurrentRoom;
	}

	public int GetCurrentDifficulty() => objectiveCompletedCount + objectiveFailedCount;

	public int OnCoinCollected(SimpleCoin coin)
	{

		coinCount += coin.value;
		Scores.TotalCoinsPickedUp += coin.value;
		Scores.MostCoinsHeld = Math.Max(coinCount, Scores.MostCoinsHeld);

		_eventBus.ChangeCoinCount(coinCount);
		return coinCount;
	}

	void OnMissileConnected(Projectile p)
	{
		coinCount = Mathf.Max(coinCount - p.Damage, 0);
		Scores.TotalCoinsDropped += p.Damage;
		_eventBus.ChangeCoinCount(coinCount);
	}

	public bool IsFirstVisit()
	{
		if (_currentRoom == 0) return true;
		return !RoomsVisitedInRun.Contains(_currentRoom);
	}
}
