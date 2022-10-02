using Godot;
using System;

public class EventBus : Node
{
	[Signal]
	delegate void CoinCollected(Coin coin);
	[Signal]
	delegate void KeyObtained(Node key);
	[Signal]
	delegate void ChestOpened(Node chest);
	[Signal]
	delegate void MissileCnnectedo(Projectile missile);
}
