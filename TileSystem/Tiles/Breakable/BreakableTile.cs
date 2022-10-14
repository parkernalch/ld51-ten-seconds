using Godot;
using System;

public class BreakableTile : Area2D
{
	[Export]Godot.Collections.Array<Texture> crackTextures = new Godot.Collections.Array<Texture>();
	int currentCrackTextureIndex;
	bool isBroken;
	Sprite _sprite;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite>("Sprite");
		currentCrackTextureIndex = 0;
		Connect("body_exited", this, nameof(Crack));
	}
	
	void Crack(PlayerController pc)
	{
		if (!(pc is PlayerController)) return;
		currentCrackTextureIndex += 1;
		GD.Print("Crack Level ", currentCrackTextureIndex);
		if(currentCrackTextureIndex >= crackTextures.Count)
		{
			Break();
		} else
		{
			_sprite.Texture = crackTextures[currentCrackTextureIndex];
		}

	}
	
	void Break()
	{
		Disconnect("body_exited", this, nameof(Crack));
		Connect("body_entered", this, nameof(Drop));
	}
	
	void Drop(PlayerController pc)
	{
		if (pc != null)
		{
			pc.QueueFree();
		}
	}
}
