using Godot;
using System;

public class PaymentDoor : Area2D
{
	[Export]int toll = 10;
	CollisionShape2D doorBlocker;
	Sprite doorSprite;
	public override void _Ready()
	{
		doorBlocker = GetNode<CollisionShape2D>("DoorBlocker/CollisionShape2D");
		doorSprite = GetNode<Sprite>("DoorBlocker/Sprite");
		Connect("body_entered", this, nameof(OnBodyEntered));
	}
	
	public void OnBodyEntered(PlayerController pc)
	{
		if (pc != null && pc is PlayerController)
		{ 
			if( pc.PayToll(toll)) {
				CallDeferred(nameof(OpenDoor));
			}
		}
	}
	
	void OpenDoor()
	{
		doorBlocker.Disabled = true;
		doorSprite.Visible = false;
	}
}
