using Godot;
using System;

public class BaseTile : Node2D
{
	public enum BREAK_LEVEL {
		UNBROKEN,
		CHIPPED,
		CRACKED,
		BROKEN
	};

	[Export] public Vector2 Coordinates;

	// public BREAK_LEVEL BreakLevel = BREAK_LEVEL.UNBROKEN;
}
