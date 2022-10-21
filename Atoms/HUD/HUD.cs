using Godot;
using System;
using JamToolkit.Util;

public class HUD : CanvasLayer
{
	Label _levelLabel;
	Label _coinLabel;
	EventBus _eventBus;
	GameManager _gm;
	ColorRect _colorRect;
	NinePatchRect _healthBar;

	protected override void Dispose(bool disposing)
	{
		_eventBus.SafeDisconnect(nameof(EventBus.EnteredRoom), this, nameof(OnEnteredRoom));
		_eventBus.SafeDisconnect(nameof(EventBus.CoinCountChanged), this, nameof(OnCoinCountChanged));

		base.Dispose(disposing);
	}

	public override void _Ready()
	{
		_gm = GetNode<GameManager>("/root/GameManager");
		_healthBar = GetNode<NinePatchRect>("HealthBar");
		_colorRect = GetNode<ColorRect>("ColorRect");
		_levelLabel = GetNode<Label>("LevelLabel");
		_coinLabel = GetNode<Label>("HBoxContainer/CoinCountLabel");
		_coinLabel.Text = _gm.coinCount.ToString();
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_eventBus.SafeConnect(nameof(EventBus.EnteredRoom), this, nameof(OnEnteredRoom));
		_eventBus.SafeConnect(nameof(EventBus.CoinCountChanged), this, nameof(OnCoinCountChanged));
		_eventBus.SafeConnect(nameof(EventBus.PlayerDied), this, nameof(OnPlayerDied));
		_eventBus.SafeConnect(nameof(EventBus.PlayerHealthChanged), this, nameof(OnPlayerHealthChanged));
	}
	
	void OnPlayerHealthChanged(float percent)
	{
		(_healthBar.Material as ShaderMaterial).SetShaderParam("fill_amount", percent);
	}
	
	void OnPlayerDied()
	{
		Tween _tween = new Tween();
		AddChild(_tween);
		_tween.InterpolateProperty(_colorRect, "self_modulate", Colors.Transparent, Colors.White, 1f, Tween.TransitionType.Linear, Tween.EaseType.In, 1f);
		_tween.Start();
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
