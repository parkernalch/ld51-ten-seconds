using Godot;
using System;
using JamToolkit.Util;

public class BreakableTile : BaseTile
{
	[Export]Godot.Collections.Array<Vector2> crackIndices = new Godot.Collections.Array<Vector2>();
	[Export]public BREAK_LEVEL BreakLevel { get; set;}
	bool isBroken;
	ShaderMaterial _shaderMaterial;
	Sprite _sprite;
	Node2D _parent;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite>("Sprite");
		_parent = (Node2D)GetParent();
		Owner = _parent;
		_shaderMaterial = _sprite.Material as ShaderMaterial;
		if (BreakLevel == BREAK_LEVEL.BROKEN)
		{
			Break();
		}
		SetShaderPerBreakLevel();
		this.SafeConnect("body_exited", this, nameof(OnBodyExited));
	}
	
	void SetShaderPerBreakLevel()
	{
		Vector2 v2 = new Vector2(0, 0);
		switch (BreakLevel)
		{
			case BREAK_LEVEL.UNBROKEN:
				v2 = crackIndices[0];
				break;
			case BREAK_LEVEL.CHIPPED:
				v2 = crackIndices[1];
				break;
			case BREAK_LEVEL.CRACKED:
				v2 = crackIndices[2];
				break;
			case BREAK_LEVEL.BROKEN:
				v2 = crackIndices[3];
				break;
		}
		_shaderMaterial.SetShaderParam("position", v2);
	}

	void OnBodyExited(PlayerController pc)
	{
		if (pc == null) return;
		Crack();
	}

	public void Crack()
	{
		if (BreakLevel == BREAK_LEVEL.UNBROKEN)
		{
			BreakLevel = BREAK_LEVEL.CHIPPED;
		}
		else if (BreakLevel == BREAK_LEVEL.CHIPPED)
		{
			BreakLevel = BREAK_LEVEL.CRACKED;
		} else if (BreakLevel == BREAK_LEVEL.CRACKED)
		{
			BreakLevel = BREAK_LEVEL.BROKEN;
			CallDeferred(nameof(Break));
		}
		SetShaderPerBreakLevel();
	}

	void Break()
	{
		this.SafeDisconnect("body_exited", this, nameof(OnBodyExited));
		CollisionShape2D _cs = GetNode<CollisionShape2D>("CollisionShape2D");
		CircleShape2D _circle = new CircleShape2D();
		_circle.Radius = 6;
		_cs.Shape = _circle;
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

		pc.GlobalPosition = GlobalPosition;
		PlayerController.DropLocation = pc.GlobalPosition;
		pc.Fall();
		// pc.QueueFree();

		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		GetNode<SceneManager>("/root/SceneManager").GoToPreviousLevel();
	}
}
