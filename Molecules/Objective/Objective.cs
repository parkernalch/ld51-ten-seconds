using Godot;
using System;
using JamToolkit.Util;

public class Objective : Node2D
{
	public enum OBJECTIVE_TYPE {
		COLLECT,
		DODGE,
		NONE
	};
	[Export]public OBJECTIVE_TYPE objectiveType;
	EventBus _eventBus;

	Godot.Collections.Array<ObjectiveObject> objects = new Godot.Collections.Array<ObjectiveObject>();
	bool isActive = false;

	public override void _Ready()
	{
		_eventBus = GetNode<EventBus>("/root/EventBus");
		foreach(ObjectiveObject obj in GetChildren())
		{
			if (obj != null)
			{
				obj.Disable();
				objects.Add(obj);
			}
		}
	}

	public void Activate()
	{
		foreach(ObjectiveObject obj in objects)
		{
			obj.Enable();
			GD.Print("Enabled object ", obj.Name);
			obj.SafeConnect(nameof(ObjectiveObject.StateResolved), this, nameof(OnStateResolved));
			GD.Print("Connected StateResolved for ", obj.Name);
		}
		_eventBus.SafeConnect(nameof(EventBus.CountdownEnded), this, nameof(ObjectiveTimerEnded));
		isActive = true;
	}

	public void Deactivate()
	{
		if (!isActive) return;
		isActive = false;
		Visible = false;
		foreach(ObjectiveObject obj in objects)
		{
			obj.SafeDisconnect(nameof(ObjectiveObject.StateResolved), this, nameof(OnStateResolved));
			obj.Disable();
		}
		_eventBus.SafeDisconnect(nameof(EventBus.CountdownEnded), this, nameof(ObjectiveTimerEnded));
	}

	void OnStateResolved(ObjectiveObject obj)
	{
		if (obj.IsFailed() && isActive)
		{
			_eventBus.FailObjective();
			Deactivate();
		}
	}

	public void CheckState()
	{
		foreach(ObjectiveObject obj in objects)
		{
			if (!obj.IsSucceeded())
			{
				_eventBus.FailObjective();
				return;
			}
		}
		_eventBus.CompleteObjective();
	}

	async void ObjectiveTimerEnded()
	{
		CheckState();
		await ToSignal(GetTree(), "idle_frame");
		QueueFree();
	}
}
