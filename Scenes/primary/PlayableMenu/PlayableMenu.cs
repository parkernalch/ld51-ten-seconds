using Godot;
using System;

public class PlayableMenu : Node2D
{	
	GameManager _gameManager;
	PlayerController player;

	public override void _Ready()
	{
		_gameManager = GetNode<GameManager>("/root/GameManager");
		GetNode<EventBus>("/root/EventBus").LoadPlayer(GetNode<PlayerController>("PlayerController"));
	}
}
