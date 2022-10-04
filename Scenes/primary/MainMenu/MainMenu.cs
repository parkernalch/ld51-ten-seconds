using Godot;
using System;

public class MainMenu : Control
{
	Panel creditsPanel;
	Panel optionsPanel;
	
	Button playButton;
	Button creditsButton;
	Button optionsButton;
	
	public override void _Ready()
	{
		playButton = this.GetNode<Button>("ButtonContainer/PlayButton");
		creditsButton = this.GetNode<Button>("ButtonContainer/CreditsButton");
		optionsButton = this.GetNode<Button>("ButtonContainer/OptionsButton");
		
		playButton.Connect("pressed", this, nameof(OnPlayButtonPressed));
		creditsButton.Connect("pressed", this, nameof(OnCreditsButtonPressed));
		optionsButton.Connect("pressed", this, nameof(OnOptionsButtonPressed));
		this.GetNode<Button>("CreditsPanel/BackButton")
			.Connect("pressed", this, nameof(OnBackButtonPressed));
		this.GetNode<Button>("OptionsPanel/BackButton")
			.Connect("pressed", this, nameof(OnBackButtonPressed));
	
		creditsPanel = this.GetNode<Panel>("CreditsPanel");
		optionsPanel = this.GetNode<Panel>("OptionsPanel");
	}
	
	private void OnBackButtonPressed()
	{
		creditsPanel.Visible = false;
		optionsPanel.Visible = false;
	}
	
	private void OnPlayButtonPressed()
	{
		this.GetNode<SceneManager>("/root/SceneManager").GoToGame();
	}
	
	private void OnCreditsButtonPressed()
	{
		creditsPanel.Visible = true;
		optionsPanel.Visible = false;
	}
	
	private void OnOptionsButtonPressed()
	{
		optionsPanel.Visible = true;
		creditsPanel.Visible = false;
	}
}
