using Godot;
using System;

public class ObjectiveArea : Area2D
{
	[Export]public bool isSuccessArea = true;
	EventBus _eventBus;
	
	private bool isPlayerInArea = false;
	
	public override void _Ready()
	{
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_eventBus.Connect("CountdownEnded", this, nameof(_OnCountdownEnded));
		this.Connect("body_entered", this, nameof(_OnPlayerEntered));
		this.Connect("body_exited", this, nameof(_OnPlayerExited));
	}
	
	private void _OnPlayerEntered(Node2D body)
	{
		if (body is PlayerController)
		{
			isPlayerInArea = true;
		}
	}
	
	private void _OnPlayerExited(Node2D body)
	{
		if (body is PlayerController)
		{
			isPlayerInArea = false;
		}
	}
	
	private void _OnCountdownEnded()
	{
		if (isSuccessArea && isPlayerInArea)
		{
			_eventBus.CompleteObjective();
		} else if (!isSuccessArea && isPlayerInArea)
		{
			_eventBus.FailObjective();
		}
	}
}
