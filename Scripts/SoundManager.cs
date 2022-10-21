using Godot;
using System;
using JamToolkit.Util;

public class SoundManager : Node
{
	EventBus _eventBus;
	AudioStreamPlayer iceSound;
	AudioStreamPlayer webSound;
	AudioStreamPlayer cannonballSound;
	AudioStreamPlayer hurtSound;
	AudioStreamPlayer dashSound;
	AudioStreamPlayer coinSound;
	public enum SOUND_TYPE{
		ICE,
		WEB,
		CANNONBALL,
		HURT,
		DASH,
		COIN
	};
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_eventBus.SafeConnect(nameof(EventBus.CoinCountChanged), this, nameof(OnCoinCountChanged));
		iceSound = GetNode<AudioStreamPlayer>("IceSfx");
		webSound = GetNode<AudioStreamPlayer>("WebSfx");
		cannonballSound = GetNode<AudioStreamPlayer>("CannonballSfx");
		hurtSound = GetNode<AudioStreamPlayer>("HurtSfx");
		dashSound = GetNode<AudioStreamPlayer>("DashSfx");
		coinSound = GetNode<AudioStreamPlayer>("CoinSfx");
	}
	
	void OnCoinCountChanged(int newCount)
	{
		PlaySound(SOUND_TYPE.COIN);
	}
	
	public void PlaySound(SOUND_TYPE soundType)
	{
		switch (soundType)
		{
			case SOUND_TYPE.ICE:
				iceSound.Play();
			break;
			case SOUND_TYPE.WEB:
				webSound.Play();
			break;
			case SOUND_TYPE.CANNONBALL:
				cannonballSound.Play();
			break;
			case SOUND_TYPE.COIN:
				coinSound.Play();
			break;
			case SOUND_TYPE.HURT:
				hurtSound.Play();
			break;
			case SOUND_TYPE.DASH:
				dashSound.Play();
			break;
		}	

	}

}
