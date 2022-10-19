using Godot;
using System;
using JamToolkit.Util;

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
		this.SafeConnect("body_exited", this, nameof(OnBodyExited));
	}

	void OnBodyExited(PlayerController pc)
	{
		if (pc == null) return;
		Crack();
	}

	public void Crack()
	{
		currentCrackTextureIndex += 1;
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
		this.SafeDisconnect("body_exited", this, nameof(OnBodyExited));
		this.SafeConnect("body_entered", this, nameof(OnBodyEntered));
	}

	void OnBodyEntered(PlayerController pc)
	{
		if (pc == null) return;
		if (pc.IsVulnerable())
			Drop(pc);
	}

	async void Drop(PlayerController pc)
	{
		if (pc == null) return;

		PlayerController.DropLocation = pc.GlobalPosition;
		pc.Fall();
		// pc.QueueFree();

		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		GetNode<SceneManager>("/root/SceneManager").GoToPreviousLevel();
	}
}
