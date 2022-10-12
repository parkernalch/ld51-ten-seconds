using Godot;
using System;

public class TileSystem : TileMap
{
	Godot.Collections.Dictionary<int, PackedScene> tileLookup = new Godot.Collections.Dictionary<int, PackedScene>();
	int halfCell;
	 
	public override void _Ready()
	{
		halfCell = (int)(CellSize.x * 0.5f);
		// Floor
		tileLookup.Add(0, ResourceLoader.Load<PackedScene>("res://TileSystem/Tiles/Floor/FloorTile.tscn"));
		// Web
		tileLookup.Add(1, ResourceLoader.Load<PackedScene>("res://TileSystem/Tiles/Web/WebTile.tscn"));
		// Void
		tileLookup.Add(2, ResourceLoader.Load<PackedScene>("res://TileSystem/Tiles/Void/VoidTile.tscn"));
		// Ice
		tileLookup.Add(3, ResourceLoader.Load<PackedScene>("res://TileSystem/Tiles/Ice/IceTile.tscn"));
		
		ReplaceTilesWithScenes();
	}

	void ReplaceTilesWithScenes()
	{
		Godot.Collections.Array usedTiles = GetUsedCells();
		Vector2 offsetVector = new Vector2(halfCell, halfCell);
		foreach(Vector2 tilePosition in usedTiles)
		{
			int cell = GetCell((int)tilePosition.x, (int)tilePosition.y);
			Node2D scene = tileLookup[cell].Instance<Node2D>();
			AddChild(scene);
			scene.GlobalPosition = MapToWorld(tilePosition) + offsetVector;
			SetCell((int)tilePosition.x, (int)tilePosition.y, -1);
		}
	}
}
