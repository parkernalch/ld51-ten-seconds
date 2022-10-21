using Godot;
using System;
using JamToolkit.Util;

public class TileSystem : TileMap
{
	EventBus _eventBus;
	GameManager _gm;
	Node2D _cachedTileContainer;
	Godot.Collections.Dictionary<int, PackedScene> tileLookup = new Godot.Collections.Dictionary<int, PackedScene>();
	int halfCell;
	Godot.Collections.Dictionary<Vector2, Node2D> instancedTiles = new Godot.Collections.Dictionary<Vector2, Node2D>();
	Node _currentScene;

	public override void _Ready()
	{
		_cachedTileContainer = GetNode<Node2D>("CachedTiles");
		_currentScene = GetTree().CurrentScene;
		_eventBus = GetNode<EventBus>("/root/EventBus");
		_gm = GetNode<GameManager>("/root/GameManager");
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
		if (_gm.IsFirstVisit())
		{
			ReplaceTilesWithScenes();
		} else {
			LoadTiles(_gm.CurrentRoom);
		}
		_eventBus.SafeConnect(nameof(EventBus.LobbedProjectileImpacted), this, nameof(BreakTile));
		_eventBus.SafeConnect(nameof(EventBus.LeftRoom), this, nameof(SaveTiles));
	}
	
	void SaveTiles(int room)
	{
		PackedScene _packedScene = new PackedScene();
		_packedScene.Pack(_cachedTileContainer);
		ResourceSaver.Save($"res://floor_{_gm.CurrentRoom}.tscn", _packedScene);
	}
	
	void LoadTiles(int room)
	{
		var scene = GD.Load<PackedScene>($"res://floor_{_gm.CurrentRoom}.tscn");
		var myScene = scene.Instance();
		if (myScene.GetParent() != null)
		{
			myScene.GetParent().RemoveChild(myScene);
		}
		Clear();
		CallDeferred(nameof(SafeAddChild), myScene);
	}
	
	async void SafeAddChild(Node2D scene)
	{
		_cachedTileContainer.Owner = null;
		_cachedTileContainer.QueueFree();
		await ToSignal(GetTree(), "idle_frame");
		AddChild(scene);
		_cachedTileContainer = scene;
		foreach(BaseTile tile in _cachedTileContainer.GetChildren())
		{
			instancedTiles[tile.Coordinates] = tile;
		}
	}

	void ReplaceTilesWithScenes()
	{
		Godot.Collections.Array usedTiles = GetUsedCells();
		Vector2 offsetVector = new Vector2(halfCell, halfCell);
		foreach(Vector2 tilePosition in usedTiles)
		{
			int cell = GetCell((int)tilePosition.x, (int)tilePosition.y);
			BaseTile scene = tileLookup[cell].Instance<BaseTile>();
			_cachedTileContainer.AddChild(scene);
			scene.Owner = _cachedTileContainer;
			scene.GlobalPosition = MapToWorld(tilePosition) + offsetVector;
			SetCell((int)tilePosition.x, (int)tilePosition.y, -1);
			scene.Coordinates = tilePosition;
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
				BaseTile scene = tileLookup[2].Instance<BaseTile>();
				_cachedTileContainer.AddChild(scene);
				scene.Owner = _cachedTileContainer;
				scene.GlobalPosition = MapToWorld(mapPosition) + new Vector2(halfCell, halfCell);
				cell.QueueFree();
				scene.Coordinates = mapPosition;
				(scene as BreakableTile).Crack();
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
