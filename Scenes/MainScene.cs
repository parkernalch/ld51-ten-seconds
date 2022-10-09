using Godot;
using System;
using JamToolkit.Util;

/// <summary>
/// TODO: this just sets up a "Player" that will be replaced with the real player and creates missiles every 10s
/// </summary>
public class MainScene : Node2D
{
	EventBus _eventBus;
	Rune _rune;
	private float time = 0;
	private PlayerController _player;
	private const float Interval = 5f;
	private int _level = 1;
	Godot.Collections.Array<Objective> _objectives = new Godot.Collections.Array<Objective>();
	Objective _currentObjective;
	// If GameManager.currentLevel > 0, load increasingly difficult Obstacles
	public override void _Ready()
	{
		_rune = GetNode<Rune>("Rune");
		_eventBus = GetNode<EventBus>("/root/EventBus");
		foreach(Objective o in GetNode("Objectives").GetChildren())
		{
			o.Deactivate();
			_objectives.Add(o);
		}
		GameManager _gameManager = GetNode<GameManager>("/root/GameManager");
		_player = (PlayerController)this.FindNode("PlayerController");
		_eventBus.LoadPlayer(_player);
		_eventBus.Connect(nameof(EventBus.CountdownEnded), this, nameof(LoadNextObjective));
		_eventBus.EnterRoom(_gameManager.currentRoom);
		LoadNextObjective();
	}
	
	void LoadNextObjective()
	{
		if (_currentObjective != null &&_currentObjective.HasMethod(nameof(Objective.Deactivate)))
		{
			_currentObjective.Deactivate();
		}
		if (_objectives.Count == 0)
		{
			GetNode<SceneManager>("/root/SceneManager").GoToMainMenu();
			return;
		}
		_currentObjective = _objectives[0];
		_objectives.RemoveAt(0);
		_currentObjective.Activate();
		_rune.SetObjectiveIcon(_currentObjective.objectiveType);
	}
	
	public PlayerController GetPlayer() => _player;
}
