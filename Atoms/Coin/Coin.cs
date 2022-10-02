using Godot;
using System;

public class Coin : Area2D
{
	EventBus _eventBus;
	
	public override void _Ready()
	{
		_eventBus = GetNode<EventBus>("/root/EventBus");
		Connect("body_entered", this, nameof(OnCoinBodyEntered));
	}

	private void OnCoinBodyEntered(Node body)
	{
		_eventBus.EmitSignal("coin_collected", this);
		QueueFree();
	}

}
