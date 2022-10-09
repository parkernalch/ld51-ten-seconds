using Godot;
using System;
using System.Collections.Generic;

public class Trail : Line2D
{
	[Export]int length = 10;
	[Export]int width = 4;
	[Export]public Color color = new Color(1, 1, 1, 1);
	List<Vector2> _points = new List<Vector2>();
	Node2D parent;
	public override void _Ready()
	{
		this.DefaultColor = color;
		this.Width = width;
		parent = GetParent<Node2D>();
	}
	public override void _PhysicsProcess(float delta)
	{
		this.GlobalPosition = Vector2.Zero;
		this.GlobalRotation = 0;
		_points.Add(parent.GlobalPosition );
		if (_points.Count > length)
		{
			_points.RemoveAt(0);
		}
		this.Points = _points.ToArray();
	}
}
