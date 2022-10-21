using Godot;
using System;

public class FloorTransition : CanvasLayer
{
	Label _prevLabel;
	Label _nextLabel;
	AnimationPlayer _anim;
	GameManager _gm;
	SceneManager _sceneManager;
	public override void _Ready()
	{
		_sceneManager = GetNode<SceneManager>("/root/SceneManager");
		_anim = GetNode<AnimationPlayer>("AnimationPlayer");
		_prevLabel = GetNode<Label>("ColorRect/PreviousContainer/PreviousLevelLabel");
		_nextLabel = GetNode<Label>("ColorRect/NextContainer/NextLevelLabel");
		_gm = GetNode<GameManager>("/root/GameManager");
		_anim.Play("RESET");

		StartAnimation();
	}
	
	async void StartAnimation()
	{
		await ToSignal(GetTree().CreateTimer(2f), "timeout");
		if (_gm.PreviousRoom > _gm.CurrentRoom)
		{
			_prevLabel.Text = _gm.CurrentRoom.ToString();
			_nextLabel.Text = _gm.PreviousRoom.ToString();
			_anim.PlayBackwards("transition");
		} else {
			_prevLabel.Text = _gm.PreviousRoom.ToString();
			_nextLabel.Text = _gm.CurrentRoom.ToString();
			_anim.Play("transition");
		}
		await ToSignal(_anim, "animation_finished");
		_sceneManager.FinishTransition();
	}

}
