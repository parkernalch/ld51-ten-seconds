using Godot;
using System;

public class SceneTrigger : Area2D
{
	[Export] string targetScene = "MainMenu";
	[Export] bool startActive = true;
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
		if (!(body is PlayerController)) return;

		switch (targetScene)
		{
			case "MainMenu":
				_sceneManager.GoToMainMenu();
				break;
			case "Credits":
				_sceneManager.GoToCredits();
				break;
			case "Options":
				_sceneManager.GoToOptions();
				break;
			case "StartGame":
				_sceneManager.GoToGame();
				break;
			case "NextLevel":
				_sceneManager.GoToNextLevel();
				break;
		}
	}
}
