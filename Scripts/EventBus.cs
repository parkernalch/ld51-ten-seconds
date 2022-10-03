using Godot;
using System;

public class EventBus : Node
{
	[Signal]
	public delegate void CoinCollected(Coin coin);

	[Signal]
	public delegate void KeyObtained(Node key);

	[Signal]
	public delegate void ChestOpened(Node chest);

	[Signal]
	public delegate void MissileConnected(Projectile missile);
}
