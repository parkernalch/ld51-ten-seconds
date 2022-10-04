using Godot;
using System;

public class Countdown : CenterContainer
{
	float intervalTime = 10f;
	float elapsedTime = 0f;
	
	Label _label;
	Tween _tween;
	
	Random _random;
	
	EventBus _eventBus;
	
	public override void _Ready()
	{
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_random = new Random();
		_label = GetNode<Label>("Label");
		_tween = GetNode<Tween>("Tween");
		elapsedTime = intervalTime;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		elapsedTime -= delta;
		UpdateLabelText();
		if (elapsedTime <= 0)
		{
			Timeout();
		}
	}
	
	void UpdateLabelText()
	{
		elapsedTime = Mathf.Max(elapsedTime, 0);
		int seconds = Mathf.FloorToInt(elapsedTime);
		int milliseconds = Mathf.FloorToInt((elapsedTime - (float)seconds) * 1000f);
		_label.Text = seconds.ToString().PadZeros(2) + "." + milliseconds.ToString().PadZeros(3); 
	}
	
	void Timeout()
	{
		var nxt = _random.Next();
		GD.Print("Timeout", nxt);
		_eventBus.EndCountdown();
		if (nxt % 2 == 0)
		{
			FlashFailure();
			_eventBus.FailObjective();
		} else 
		{
			FlashSuccess();
			_eventBus.CompleteObjective();
		}
		elapsedTime = intervalTime;
	}
	
	void FlashSuccess()
	{
		GD.Print("Success!");
		_tween.StopAll();
		_tween.InterpolateProperty(_label, "self_modulate", new Color(1, 1, 1, 1), new Color(0, 1, 0, 1), 0.2f);
		_tween.InterpolateProperty(_label, "self_modulate", new Color(0, 1, 0, 1), new Color(1, 1, 1, 1), 0.2f, 0, 0, 0.2f);	
		_tween.Start();
	}
	
	void FlashFailure()
	{
		GD.Print("Failure");
		_tween.StopAll();
		_tween.InterpolateProperty(_label, "self_modulate", new Color(1, 1, 1, 1), new Color(1, 0, 0, 1), 0.2f);
		_tween.InterpolateProperty(_label, "self_modulate", new Color(1, 0, 0, 1), new Color(1, 1, 1, 1), 0.2f, 0, 0, 0.2f);	
		_tween.Start();
	}
}
