using Godot;
using System;
using JamToolkit.Util;

public class SceneManager : Node
{
	PackedScene gameScene = ResourceLoader.Load<PackedScene>("res://Scenes/main_scene.tscn");
	PackedScene mainMenuScene = ResourceLoader.Load<PackedScene>("res://Scenes/primary/PlayableMenu/PlayableMenu.tscn");
	PackedScene creditsScene = ResourceLoader.Load<PackedScene>("res://Scenes/primary/CreditsScene/CreditsScene.tscn");
	PackedScene optionsScene = ResourceLoader.Load<PackedScene>("res://Scenes/primary/OptionsScene/OptionsScene.tscn");

	public override void _Ready()
	{
	}

	public void GoToMainMenu()
	{
		GetTree().ChangeSceneTo(mainMenuScene);
	}

	public void GoToGame()
	{
		var gameManager = GetNode<GameManager>("/root/GameManager");
		gameManager.CurrentRoom = 1;
		GetTree().ChangeSceneTo(gameScene);
	}

	public void GoToNextLevel()
	{
		var gameManager = GetNode<GameManager>("/root/GameManager");
		gameManager.CurrentRoom++;
		GetTree().ChangeSceneTo(gameScene);
	}

	public void GoToPreviousLevel()
	{
		var gameManager = GetNode<GameManager>("/root/GameManager");

		if (gameManager.CurrentRoom <= 1)
		{
			// TODO: may want to change this or at least play a fall animation or "woo woo woooooo" noise 💀
			GoToMainMenu();
			return;
		}

		gameManager.CurrentRoom--;
		GetTree().ChangeSceneTo(gameScene);
	}

	public void GoToOptions()
	{
		GetTree().ChangeSceneTo(optionsScene);
	}

	public void GoToCredits()
	{
		GetTree().ChangeSceneTo(creditsScene);
	}
}
