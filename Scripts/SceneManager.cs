using Godot;
using System;

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
		GetTree().ChangeSceneTo(gameScene);
	}
	
	public void GoToNextLevel()
	{
		GetNode<GameManager>("/root/GameManager").currentRoom++;
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
