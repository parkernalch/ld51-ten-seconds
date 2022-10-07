using Godot;
using System;

public class GameManager : Node
{
	
	int objectiveCompletedCount;
	int objectiveFailedCount;
	
	public int currentRoom = 0;
	
	int objectivesPerRoom = 5;
	
	EventBus _eventBus;
	SceneManager _sceneManager;
	
	public override void _Ready()
	{
		_sceneManager = GetNode<SceneManager>("/root/SceneManager");
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_eventBus.Connect(nameof(EventBus.EnteredRoom), this, nameof(OnEnteredRoom));
		_eventBus.Connect(nameof(EventBus.ObjectiveCompleted), this, nameof(OnObjectiveCompleted));
		_eventBus.Connect(nameof(EventBus.ObjectiveFailed), this, nameof(OnObjectiveFailed));
		_eventBus.Connect(nameof(EventBus.CountdownEnded), this, nameof(OnCountdownEnded));
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
		currentRoom++;
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
	
	private void NextLevel()
	{
		// _sceneManager.LoadNextLevel();
	}
	
	public int GetCurrentDifficulty()
	{
		return objectiveCompletedCount + objectiveFailedCount;
	}
}
