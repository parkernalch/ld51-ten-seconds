using Godot;
using System;

public class GameManager : Node
{
	
	int objectiveCompletedCount;
	int objectiveFailedCount;
	
	int objectivesPerRoom = 5;
	
	public override void _Ready()
	{
	}
	
	private void OnCountdownTimeout()
	{
		if (this.IsCurrentObjectiveComplete())
		{
			objectiveCompletedCount++;
		} else 
		{
			objectiveFailedCount++;
		}
		if ( objectiveCompletedCount + objectiveFailedCount % objectivesPerRoom == 0)
		{
			// Change Level Layout every X objectives
			NextLevel();
		}
		this.TriggerObjectiveStart();
	}
	
	private void StartGame()
	{
		objectiveCompletedCount = 0;
		objectiveFailedCount = 0;
	}
	
	private void NextLevel()
	{
		// load next layout
	}
	
	private void TriggerObjectiveStart()
	{
		
	}
	
	private bool IsCurrentObjectiveComplete()
	{
		return true;
	}
	
	public int GetCurrentDifficulty()
	{
		return objectiveCompletedCount + objectiveFailedCount;
	}
}
