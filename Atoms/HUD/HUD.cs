using Godot;
using System;
using JamToolkit.Util;

public class HUD : CanvasLayer
{
	Label _levelLabel;
	Label _coinLabel;
	EventBus _eventBus;
	GameManager _gm;

	protected override void Dispose(bool disposing)
	{
		_eventBus.SafeDisconnect(nameof(EventBus.EnteredRoom), this, nameof(OnEnteredRoom));
		_eventBus.SafeDisconnect(nameof(EventBus.CoinCountChanged), this, nameof(OnCoinCountChanged));

		base.Dispose(disposing);
	}

	public override void _Ready()
	{
		_gm = GetNode<GameManager>("/root/GameManager");
		_levelLabel = GetNode<Label>("LevelLabel");
		_coinLabel = GetNode<Label>("HBoxContainer/CoinCountLabel");
		_coinLabel.Text = _gm.coinCount.ToString();
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_eventBus.SafeConnect(nameof(EventBus.EnteredRoom), this, nameof(OnEnteredRoom));
		_eventBus.SafeConnect(nameof(EventBus.CoinCountChanged), this, nameof(OnCoinCountChanged));
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
