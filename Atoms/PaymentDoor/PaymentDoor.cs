using Godot;
using System;
using JamToolkit.Util;

public class PaymentDoor : Area2D
{
	[Export]int toll = 10;
	CollisionShape2D doorBlocker;
	Sprite doorSprite;
	public override void _Ready()
	{
		doorBlocker = GetNode<CollisionShape2D>("DoorBlocker/CollisionShape2D");
		doorSprite = GetNode<Sprite>("DoorBlocker/Sprite");
		this.SafeConnect("body_entered", this, nameof(OnBodyEntered));
	}

	protected override void Dispose(bool disposing)
	{
		this.SafeDisconnect("body_entered", this, nameof(OnBodyEntered));
		base.Dispose(disposing);
	}

	public void OnBodyEntered(PlayerController pc)
	{
		if (pc is PlayerController controller)
		{
			if( controller.PayToll(toll)) {
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
