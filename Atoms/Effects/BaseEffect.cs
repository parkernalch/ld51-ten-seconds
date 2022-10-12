using Godot;
using System;
using JamToolkit.Util;

public class BaseEffect : Node2D
{
	public PlayerController controller;

	public override void _Ready()
    {
		controller = this.FindSingleton<PlayerController>();
    }
    
    public virtual void Attach() { }
    public virtual void Remove() {}

}
