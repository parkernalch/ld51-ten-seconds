using Godot;
using System;

public class Coin : Area2D
{
	public override void _Ready()
	{
		Connect("body_entered", this, nameof(OnCoinBodyEntered));
	}

	private void OnCoinBodyEntered(Node body)
	{
		QueueFree();
	}

}
