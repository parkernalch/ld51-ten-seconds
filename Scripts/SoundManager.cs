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
	AudioStreamPlayer actionMusic;
	AudioStreamPlayer menuMusic;
	Tween _tween;
	private AudioStreamPlayer _backgroundTrack;
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
		_tween = new Tween();
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_eventBus.SafeConnect(nameof(EventBus.CoinCountChanged), this, nameof(OnCoinCountChanged));
		iceSound = GetNode<AudioStreamPlayer>("IceSfx");
		webSound = GetNode<AudioStreamPlayer>("WebSfx");
		cannonballSound = GetNode<AudioStreamPlayer>("CannonballSfx");
		hurtSound = GetNode<AudioStreamPlayer>("HurtSfx");
		dashSound = GetNode<AudioStreamPlayer>("DashSfx");
		coinSound = GetNode<AudioStreamPlayer>("CoinSfx");
		actionMusic = GetNode<AudioStreamPlayer>("ActionMusic");
		menuMusic = GetNode<AudioStreamPlayer>("MenuMusic");
	}
	
	public void PlayMenuMusic()
	{
		if (menuMusic.Playing) return;
		menuMusic.VolumeDb = -10;
		menuMusic.Play();
		if (actionMusic.Playing)
		{
			_tween.InterpolateProperty(
				actionMusic,
				"volume_db",
				actionMusic.VolumeDb,
				-10,
				3f
			);
		}
		_tween.InterpolateProperty(
			menuMusic,
			"volume_db",
			menuMusic.VolumeDb,
			0,
			3f
		);
		_tween.Start();
		_backgroundTrack = menuMusic;
	}
	
	public void PlayGameMusic()
	{
		if (actionMusic.Playing) return;
		actionMusic.VolumeDb = -10;
		actionMusic.Play();
		if (menuMusic.Playing)
		{
			_tween.InterpolateProperty(
				menuMusic,
				"volume_db",
				menuMusic.VolumeDb,
				-10,
				3f
			);
		}
		_tween.InterpolateProperty(
			actionMusic,
			"volume_db",
			actionMusic.VolumeDb,
			0,
			3f
		);
		_tween.Start();
		_backgroundTrack = actionMusic;
	}
	
	public void DuckBackingTrack(float target, float duration)
	{
		_tween.StopAll();
		_tween.InterpolateProperty(
			_backgroundTrack,
			"volume_db",
			_backgroundTrack.VolumeDb,
			target,
			duration
		);
		_tween.Start();
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
