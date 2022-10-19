using Godot;
using System;
using JamToolkit.Util;

public class OptionsScene : Node2D
{

	public override void _Ready()
	{
		var gameManager = GetNode<GameManager>("/root/GameManager");
		var scores = gameManager.Scores;


		SetLabelText("Furthest Room:", nameof(scores.FurthestRoom), scores.FurthestRoom);
		SetLabelText("Games Played:", nameof(scores.GamesPlayed), scores.GamesPlayed);
		SetLabelText("Most Coins Held:", nameof(scores.MostCoinsHeld), scores.MostCoinsHeld);
		SetLabelText("Total Coins Dropped:", nameof(scores.TotalCoinsDropped), scores.TotalCoinsDropped);
		SetLabelText("Total Coins Picked Up:", nameof(scores.TotalCoinsPickedUp), scores.TotalCoinsPickedUp);
	}

	private void SetLabelText(string label, string labelName, int score)
	{
		var labelLabel = this.FindChildByName("Labels").FindChildByName<Label>(labelName);
		labelLabel.Text = label;

		var valueLabel = this.FindChildByName("Values").FindChildByName<Label>(labelName);
		valueLabel.Text = score.ToString("N0");
	}
}
