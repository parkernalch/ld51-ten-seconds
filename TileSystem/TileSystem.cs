using Godot;
using System;

public class TileSystem : TileMap
{
	EventBus _eventBus;
	Godot.Collections.Dictionary<int, PackedScene> tileLookup = new Godot.Collections.Dictionary<int, PackedScene>();
	int halfCell;
	Godot.Collections.Dictionary<Vector2, Node2D> instancedTiles = new Godot.Collections.Dictionary<Vector2, Node2D>();
	
	 
	public override void _Ready()
	{
		_eventBus = GetNode<EventBus>("/root/EventBus");
		halfCell = (int)(CellSize.x * 0.5f);
		// Ice
		tileLookup.Add(0, ResourceLoader.Load<PackedScene>("res://TileSystem/Tiles/Ice/IceTile.tscn"));
		// Web
		tileLookup.Add(1, ResourceLoader.Load<PackedScene>("res://TileSystem/Tiles/Web/WebTile.tscn"));
		// Cracker
		tileLookup.Add(2, ResourceLoader.Load<PackedScene>("res://TileSystem/Tiles/Breakable/BreakableTile.tscn"));
		// Void
		tileLookup.Add(3, ResourceLoader.Load<PackedScene>("res://TileSystem/Tiles/Void/VoidTile.tscn"));
		// Floor
		tileLookup.Add(4, ResourceLoader.Load<PackedScene>("res://TileSystem/Tiles/Floor/FloorTile.tscn"));
		ReplaceTilesWithScenes();
		_eventBus.Connect(nameof(EventBus.LobbedProjectileImpacted), this, nameof(BreakTile));
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
			instancedTiles[tilePosition] = scene;
		}
	}
	
	void BreakTileByMapPosition(Vector2 mapPosition)
	{
		if (!instancedTiles.Keys.Contains(mapPosition)) return;
		Node2D cell = instancedTiles[mapPosition];
		if (cell != null)
		{
			if (cell is BreakableTile)
			{
				(cell as BreakableTile).Crack();
			} else 
			{
				Node2D scene = tileLookup[2].Instance<Node2D>();
				AddChild(scene);
				scene.GlobalPosition = MapToWorld(mapPosition) + new Vector2(halfCell, halfCell);
				cell.QueueFree();
				instancedTiles[mapPosition] = scene;
			}
		}

	}
	
	void BreakTile(Vector2 globalBreakPosition, int displacement)
	{
		Vector2 localPos = this.ToLocal(globalBreakPosition);
		Vector2 mapPos = WorldToMap(localPos);
		BreakTileByMapPosition(mapPos);
		if (displacement > 0)
		{
			BreakTileByMapPosition(new Vector2(mapPos.x - 1, mapPos.y));
			BreakTileByMapPosition(new Vector2(mapPos.x + 1, mapPos.y));
			BreakTileByMapPosition(new Vector2(mapPos.x, mapPos.y - 1));
			BreakTileByMapPosition(new Vector2(mapPos.x, mapPos.y + 1));
		}
	}
}
