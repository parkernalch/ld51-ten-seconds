using Godot;
using System;

public class SceneTrigger : Area2D
{
	[Export]String targetScene = "MainMenu";
	SceneManager _sceneManager;
	
	public override void _Ready()
	{
		_sceneManager = GetNode<SceneManager>("/root/SceneManager");
		this.Connect("body_entered", this, nameof(OnBodyEntered));
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
			}
		}
	}
}
