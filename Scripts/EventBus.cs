using Godot;
using System;

public class EventBus : Node
{
	private bool _disposed = false;

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		_disposed = true;
	}

	[Signal]
	public delegate void CoinCreated(SimpleCoin coin);

	public void CreateCoin(SimpleCoin coin)
	{
		if (_disposed) return;
		this.EmitSignal(nameof(EventBus.CoinCreated), coin);
	}

	[Signal]
	public delegate void CoinCollected(SimpleCoin coin);

	public void CollectCoin(SimpleCoin coin)
	{
		if (_disposed) return;
		this.EmitSignal(nameof(EventBus.CoinCollected), coin);
	}

	[Signal]
	public delegate void KeyObtained(Node key);
	public void ObtainKey(Node key)
	{
		if (_disposed) return;
		this.EmitSignal(nameof(EventBus.KeyObtained), key);
	}

	[Signal]
	public delegate void ChestOpened(Node chest);
	public void OpenChest(Node chest)
	{
		if (_disposed) return;
		this.EmitSignal(nameof(EventBus.ChestOpened), chest);
	}

	[Signal]
	public delegate void MissileConnected(Projectile missile);

	public void ConnectMissile(Projectile projectile)
	{
		if (_disposed) return;
		this.EmitSignal(nameof(EventBus.MissileConnected), projectile);
	}

	[Signal]
	public delegate void ObjectiveCompleted();
	public void CompleteObjective()
	{
		if (_disposed) return;
		this.EmitSignal(nameof(EventBus.ObjectiveCompleted));
	}

	[Signal]
	public delegate void ObjectiveFailed();
	public void FailObjective()
	{
		if (_disposed) return;
		this.EmitSignal(nameof(EventBus.ObjectiveFailed));
	}

	[Signal]
	public delegate void CountdownEnded();
	public void EndCountdown()
	{
		if (_disposed) return;
		this.EmitSignal(nameof(EventBus.CountdownEnded));
	}

	[Signal]
	public delegate void LevelCompleted();
	public void CompleteLevel()
	{
		if (_disposed) return;
		this.EmitSignal(nameof(EventBus.LevelCompleted));
	}

	[Signal]
	public delegate void LevelFailed();
	public void FailLevel()
	{
		if (_disposed) return;
		this.EmitSignal(nameof(EventBus.LevelFailed));
	}

	[Signal]
	public delegate void ObjectiveCountChanged(int successful, int failed);
	public void ChangeObjectiveCount(int success, int fail)
	{
		if (_disposed) return;
		this.EmitSignal(nameof(ObjectiveCountChanged), success, fail);
	}

	[Signal]
	public delegate void EnteredRoom(int roomIndex);
	public void EnterRoom(int roomIndex)
	{
		if (_disposed) return;
		this.EmitSignal(nameof(EnteredRoom), roomIndex);
	}

	[Signal]
	public delegate void PlayerChanged(PlayerController player);
	public void LoadPlayer(PlayerController player)
	{
		if (_disposed) return;
		this.EmitSignal(nameof(PlayerChanged), player);
	}

	[Signal]
	public delegate void CoinCountChanged(int newValue);
	public void ChangeCoinCount(int newCount)
	{
		if (_disposed) return;
		this.EmitSignal(nameof(CoinCountChanged), newCount);
	}

	[Signal]
	public delegate void LobbedProjectileImpacted(Vector2 impactPoint, int radius);
	public void NotifyProjectileImpact(Vector2 impactPoint, int radius)
	{
		if (_disposed) return;
		this.EmitSignal(nameof(LobbedProjectileImpacted), impactPoint, radius);
	}
}
