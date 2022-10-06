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
	
	Godot.Collections.Array<Sprite> pips = new Godot.Collections.Array<Sprite>();
	int successes = 0;
	
	public override void _Ready()
	{
		_eventBus = GetNode<EventBus>("/root/EventBus");
		foreach (Node pip in GetNode("Pips").GetChildren())
		{
			pips.Add(pip as Sprite);
		}
		ClearPips();
		_eventBus.Connect(nameof(EventBus.ObjectiveCountChanged), this, nameof(UpdatePips));
	}
	
	void ClearPips()
	{
		foreach(Sprite pip in pips)
		{
			pip.SelfModulate = transparent;
		}
	}
	
	void UpdatePips(int success, int failure)
	{
		for(int i=0; i<pips.Count; i++)
		{
			if (i < success)
			{
				pips[i].SelfModulate = white;
			} else
			{
				pips[i].SelfModulate = transparent;
			}
		}
	}
}
