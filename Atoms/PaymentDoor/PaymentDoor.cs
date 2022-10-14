using Godot;
using System;

public class PaymentDoor : Area2D
{
	[Export]int toll = 10;
	StaticBody2D doorBlocker;
	public override void _Ready()
	{
		doorBlocker = GetNode<StaticBody2D>("DoorBlocker");
		Connect("body_entered", this, nameof(OnBodyEntered));
	}
	
	public void OnBodyEntered(PlayerController pc)
	{
		if (pc != null && pc is PlayerController)
		{ 
			if( pc.PayToll(toll)) doorBlocker.QueueFree();
		}
		
	}
}
