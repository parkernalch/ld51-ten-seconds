using Godot;
using System;
using JamToolkit.Util;

public class PaymentDoor : Area2D
{
	[Export]int toll = 10;
	Label _label;
	Tween _tween;
	CollisionShape2D doorBlocker;
	Sprite doorSprite;
	public override void _Ready()
	{
		_tween = new Tween();
		toll = GetNode<GameManager>("/root/GameManager").CurrentRoom * 2;
		AddChild(_tween);
		_label = GetNode<Label>("Label");
		doorBlocker = GetNode<CollisionShape2D>("DoorBlocker/CollisionShape2D");
		doorSprite = GetNode<Sprite>("DoorBlocker/Sprite");
		this.SafeConnect("body_entered", this, nameof(OnBodyEntered));
		SetCoinCount(toll);
	}

	protected override void Dispose(bool disposing)
	{
		this.SafeDisconnect("body_entered", this, nameof(OnBodyEntered));
		base.Dispose(disposing);
	}

	public void OnBodyEntered(Node2D body)
	{
		if (body.HasMethod("PayToll"))
		{
			PlayerController player = body as PlayerController; 
			if(player.PayToll(toll)) {
				_tween.InterpolateMethod(this, nameof(SetCoinCount), toll, 0, 0.25f);
				_tween.Start();
				CallDeferred(nameof(OpenDoor));
			}
		}
	}
	
	void SetCoinCount(int coins)
	{
		_label.Text = coins.ToString();
	}

	void OpenDoor()
	{
		doorBlocker.Disabled = true;
		doorSprite.Visible = false;
		this.SafeDisconnect("body_entered", this, nameof(OnBodyEntered));
	}
}
