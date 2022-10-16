using Godot;
using System;

public class EventBus : Node
{
	[Signal]
	public delegate void CoinCreated(SimpleCoin coin);

	public void CreateCoin(SimpleCoin coin) => this.EmitSignal(nameof(EventBus.CoinCreated), coin);

	[Signal]
	public delegate void CoinCollected(SimpleCoin coin);

	public void CollectCoin(SimpleCoin coin) => this.EmitSignal(nameof(EventBus.CoinCollected), coin);

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

	[Signal]
	public delegate void LevelCompleted();
	public void CompleteLevel() => this.EmitSignal(nameof(EventBus.LevelCompleted));
	
	[Signal]
	public delegate void LevelFailed();
	public void FailLevel() => this.EmitSignal(nameof(EventBus.LevelFailed));
	
	[Signal]
	public delegate void ObjectiveCountChanged(int successful, int failed);
	public void ChangeObjectiveCount(int success, int fail) => this.EmitSignal(nameof(ObjectiveCountChanged), success, fail);

	[Signal]
	public delegate void EnteredRoom(int roomIndex);
	public void EnterRoom(int roomIndex) => this.EmitSignal(nameof(EnteredRoom), roomIndex);
	
	[Signal]
	public delegate void PlayerChanged(PlayerController player);
	public void LoadPlayer(PlayerController player) => this.EmitSignal(nameof(PlayerChanged), player);
	
	[Signal]
	public delegate void CoinCountChanged(int newValue);
	public void ChangeCoinCount(int newCount) => this.EmitSignal(nameof(CoinCountChanged), newCount);
	
	[Signal]
	public delegate void LobbedProjectileImpacted(Vector2 impactPoint);
	public void NotifyProjectileImpact(Vector2 impactPoint) => this.EmitSignal(nameof(LobbedProjectileImpacted), impactPoint);
}
