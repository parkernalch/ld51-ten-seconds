using Godot;
using System;

public class SceneTrigger : Area2D
{
	[Export]String targetScene = "MainMenu";
	[Export]bool startActive = true;
	SceneManager _sceneManager;
	EventBus _eventBus;
	
	public override void _Ready()
	{
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_sceneManager = GetNode<SceneManager>("/root/SceneManager");
		_eventBus.Connect(nameof(EventBus.LevelCompleted), this, nameof(Activate));
		if (startActive) Activate();
	}
	
	void Activate()
	{
		this.Connect("body_entered", this, nameof(OnBodyEntered));		
	}
	
	void Deactivate()
	{
		this.Disconnect("body_entered", this, nameof(OnBodyEntered));
	}
	
	void OnBodyEntered(Node2D body)
	{
		if (body is PlayerController)
		{
			if (targetScene == "MainMenu")
			{
				_sceneManager.GoToMainMenu();
			} else if (targetScene == "Credits")
			{
				_sceneManager.GoToCredits();
			} else if (targetScene == "Options")
			{
				_sceneManager.GoToOptions();
			} else if (targetScene == "StartGame")
			{
				_sceneManager.GoToGame();
			} else if (targetScene == "NextLevel")
			{
				_sceneManager.GoToNextLevel();
			}
		}
	}
}
