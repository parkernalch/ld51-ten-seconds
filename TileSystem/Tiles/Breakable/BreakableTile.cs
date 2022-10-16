using Godot;
using System;

public class BreakableTile : Area2D
{
	[Export]Godot.Collections.Array<Vector2> crackIndices = new Godot.Collections.Array<Vector2>();
	int currentCrackTextureIndex;
	bool isBroken;
	ShaderMaterial _shaderMaterial;
	Sprite _sprite;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite>("Sprite");
		_shaderMaterial = _sprite.Material as ShaderMaterial;
		currentCrackTextureIndex = 0;
		Connect("body_exited", this, nameof(OnBodyExited));
	}
	
	void OnBodyExited(PlayerController pc)
	{
		if (!(pc is PlayerController)) return;
		Crack();
	}
	
	public void Crack()
	{
		currentCrackTextureIndex += 1;
		GD.Print("Crack Level ", currentCrackTextureIndex);
		if(currentCrackTextureIndex >= crackIndices.Count)
		{
			Break();
		} else
		{
			_shaderMaterial.SetShaderParam("position", crackIndices[currentCrackTextureIndex]);
			// _sprite.Texture = crackIndices[currentCrackTextureIndex];
		}

	}
	
	void Break()
	{
		Disconnect("body_exited", this, nameof(OnBodyExited));
		Connect("body_entered", this, nameof(OnBodyEntered));
	}
	
	void OnBodyEntered(PlayerController pc)
	{
		if (!(pc is PlayerController)) return;
		if (pc.IsVulnerable()) Drop(pc);
	}
	
	void Drop(PlayerController pc)
	{
		if (pc != null)
		{
			pc.QueueFree();
			GetNode<SceneManager>("/root/SceneManager").GoToPreviousLevel();
		}
	}
}
