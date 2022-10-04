using Godot;
using System;

public class PlayableMenu : Node2D
{	
	GameManager _gameManager;

	public override void _Ready()
	{
		_gameManager = GetNode<GameManager>("/root/GameManager");
	}
}
