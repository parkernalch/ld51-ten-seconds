using Godot;
using System;

public class Player : Node2D
{
	public override void _Process(float delta) => Position = this.GetViewport().GetMousePosition();
}
