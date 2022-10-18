using Godot;
using System;

public class HUD : CanvasLayer
{
	Label _levelLabel;
	Label _coinLabel;
	EventBus _eventBus;
	GameManager _gm;

	protected override void Dispose(bool disposing)
	{
		_eventBus.Disconnect(nameof(EventBus.EnteredRoom), this, nameof(OnEnteredRoom));
		_eventBus.Disconnect(nameof(EventBus.CoinCountChanged), this, nameof(OnCoinCountChanged));

		base.Dispose(disposing);
	}

	public override void _Ready()
	{
		_gm = GetNode<GameManager>("/root/GameManager");
		_levelLabel = GetNode<Label>("LevelLabel");
		_coinLabel = GetNode<Label>("HBoxContainer/CoinCountLabel");
		_coinLabel.Text = _gm.coinCount.ToString();
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_eventBus.Connect(nameof(EventBus.EnteredRoom), this, nameof(OnEnteredRoom));
		_eventBus.Connect(nameof(EventBus.CoinCountChanged), this, nameof(OnCoinCountChanged));
	}

	void OnEnteredRoom(int room)
	{
		int currentLevel = GetNode<GameManager>("/root/GameManager").CurrentRoom;
		_levelLabel.Text = "lv " + currentLevel.ToString();
	}

	void OnCoinCountChanged(int newCount)
	{
		_coinLabel.Text = newCount.ToString();
	}
}
