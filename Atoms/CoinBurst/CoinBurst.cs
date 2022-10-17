using Godot;
using System;

public class CoinBurst : CPUParticles2D
{
   
	public override void _Ready()
	{
		
	}
	
	public void DoBurst(int count, Vector2 at)
	{
		Restart();
		GlobalPosition = at;
		Amount = count;
		Emitting = true;
	}
}
