using Godot;
using System;

public class EventBus : Node
{
	[Signal]
	public delegate void CoinCreated(Coin coin);

	public void CreateCoin(Coin coin) => this.EmitSignal(nameof(EventBus.CoinCreated), coin);

	[Signal]
	public delegate void CoinCollected(Coin coin);

	public void CollectCoin(Coin coin) => this.EmitSignal(nameof(EventBus.CoinCollected), coin);

	[Signal]
	public delegate void KeyObtained(Node key);
	public void ObtainKey(Node key) => this.EmitSignal(nameof(EventBus.KeyObtained), key);

	[Signal]
	public delegate void ChestOpened(Node chest);
	public void OpenChest(Node chest) => this.EmitSignal(nameof(EventBus.ChestOpened), chest);

	[Signal]
	public delegate void MissileConnected(Projectile missile);

	public void ConnectMissile(Projectile projectile) => this.EmitSignal(nameof(EventBus.MissileConnected), projectile);

	[Signal]
	public delegate void ObjectiveCompleted();
	public void CompleteObjective() => this.EmitSignal(nameof(EventBus.ObjectiveCompleted));

	[Signal]
	public delegate void ObjectiveFailed();
	public void FailObjective() => this.EmitSignal(nameof(EventBus.ObjectiveFailed));

	[Signal]
	public delegate void CountdownEnded();
	public void EndCountdown() => this.EmitSignal(nameof(EventBus.CountdownEnded));
}
