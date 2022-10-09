using Godot;
using System;

public class Rune : Sprite
{
	[Export]Texture arrowTexture;
	[Export]Texture keyTexture;
	[Export]Texture coinTexture;
	
	EventBus _eventBus;
	
	Color white = new Color(1, 1, 1, 1);
	Color transparent = new Color(0, 0, 0, 0);
	
	Godot.Collections.Array<Sprite> successPips = new Godot.Collections.Array<Sprite>();
	Godot.Collections.Array<Sprite> failurePips = new Godot.Collections.Array<Sprite>();
	int successes = 0;
	
	public override void _Ready()
	{
		_eventBus = GetNode<EventBus>("/root/EventBus");
		foreach (Node pip in GetNode("SuccessPips").GetChildren())
		{
			successPips.Add(pip as Sprite);
		}
		foreach (Node pip in GetNode("FailurePips").GetChildren())
		{
			failurePips.Add(pip as Sprite);
		}
		ClearPips();
		_eventBus.Connect(nameof(EventBus.ObjectiveCountChanged), this, nameof(UpdatePips));
	}
	
	public void SetObjectiveIcon(Objective.OBJECTIVE_TYPE type)
	{
		if (type == Objective.OBJECTIVE_TYPE.COLLECT)
		{
			Texture = coinTexture;
		} else if (type == Objective.OBJECTIVE_TYPE.DODGE)
		{
			Texture = arrowTexture;
		} else 
		{
			Texture = keyTexture;
		}
	}
	
	void ClearPips()
	{
		foreach(Sprite pip in successPips)
		{
			pip.SelfModulate = transparent;
		}
		foreach(Sprite pip in failurePips)
		{
			pip.SelfModulate = transparent;
		}
	}
	
	void UpdatePips(int success, int failure)
	{
		for(int i=0; i<failurePips.Count; i++)
		{
			if (i < failure)
			{
				failurePips[i].SelfModulate = white;
			} else
			{
				failurePips[i].SelfModulate = transparent;
			}
		}
		for(int i=0; i<successPips.Count; i++)
		{
			if (i < success)
			{
				successPips[i].SelfModulate = white;
			} else
			{
				successPips[i].SelfModulate = transparent;
			}
		}
	}
}
