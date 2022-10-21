using Godot;
using System;
using System.Linq;
using JamToolkit.Util;
using Microsoft.Win32;

public class TileSystem : TileMap
{
	EventBus _eventBus;
	GameManager _gm;
	Node2D _cachedTileContainer;
	Godot.Collections.Dictionary<TileType, PackedScene> tileLookup = new Godot.Collections.Dictionary<TileType, PackedScene>();
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
		tileLookup.Add(TileType.Ice, ResourceLoader.Load<PackedScene>("res://TileSystem/Tiles/Ice/IceTile.tscn"));
		// Web
		tileLookup.Add(TileType.Web, ResourceLoader.Load<PackedScene>("res://TileSystem/Tiles/Web/WebTile.tscn"));
		// Cracker
		tileLookup.Add(TileType.Cracker, ResourceLoader.Load<PackedScene>("res://TileSystem/Tiles/Breakable/BreakableTile.tscn"));
		// Void
		tileLookup.Add(TileType.Void, ResourceLoader.Load<PackedScene>("res://TileSystem/Tiles/Void/VoidTile.tscn"));
		// Floor
		tileLookup.Add(TileType.Floor, ResourceLoader.Load<PackedScene>("res://TileSystem/Tiles/Floor/FloorTile.tscn"));
		if (_gm.IsFirstVisit())
		{
			ReplaceTilesWithScenes();
		} else {
			LoadTiles(_gm.CurrentRoom);
		}
		_eventBus.SafeConnect(nameof(EventBus.LobbedProjectileImpacted), this, nameof(OnProjectileImpact));
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
		foreach(var tile in _cachedTileContainer.GetChildren().OfType<BaseTile>())
		{
			instancedTiles[tile.Coordinates] = tile;
		}
	}

	public void TrackNode(Node2D node)
	{
		var cachedTiles = GetNode<Node2D>("CachedTiles");
		cachedTiles.AddChild(node);
		node.Owner = cachedTiles;
	}

	void ReplaceTilesWithScenes()
	{
		Godot.Collections.Array usedTiles = GetUsedCells();
		Vector2 offsetVector = new Vector2(halfCell, halfCell);
		foreach(Vector2 tilePosition in usedTiles)
		{
			var cell = (TileType)GetCell((int)tilePosition.x, (int)tilePosition.y);
			BaseTile scene = tileLookup[cell].Instance<BaseTile>();
			_cachedTileContainer.AddChild(scene);
			scene.Owner = _cachedTileContainer;
			scene.GlobalPosition = MapToWorld(tilePosition) + offsetVector;
			SetCell((int)tilePosition.x, (int)tilePosition.y, -1);
			scene.Coordinates = tilePosition;
			instancedTiles[tilePosition] = scene;
		}
	}

	private void InstanceTile(Vector2 mapPosition, TileType tileType)
	{
		var scene = tileLookup[tileType].Instance<BaseTile>();
		_cachedTileContainer.AddChild(scene);
		scene.Owner = _cachedTileContainer;
		scene.GlobalPosition = MapToWorld(mapPosition) + new Vector2(halfCell, halfCell);
		scene.Coordinates = mapPosition;
		if (scene is BreakableTile tile)
		{
			tile.Crack();
		}
		instancedTiles[mapPosition] = scene;
	}

	void OnProjectileImpact(ProjectileType projectileType, Vector2 globalBreakPosition, int displacement)
	{
		Vector2 localPos = this.ToLocal(globalBreakPosition);
		Vector2 mapPos = WorldToMap(localPos);

		switch (projectileType)
		{
			case ProjectileType.Cannon:
				HitTileSpread(TileType.Cracker, mapPos, displacement);
				break;

			case ProjectileType.Ice:
				HitTileSpread(TileType.Ice, mapPos, displacement, 0.35f); // 35% probability
				break;

			case ProjectileType.Web:
				HitTileSpread(TileType.Web, mapPos, displacement, 0.2f); // 20% probability
				break;
		}
	}

	private void HitTileSpread(TileType tileType, Vector2 mapPos, int displacement, float probability = 1.0f)
	{
		HitTile(tileType, mapPos);

		if (displacement <= 0) return;

		if (ProbabilityTest(probability))
			HitTile(tileType, new Vector2(mapPos.x - 1, mapPos.y));

		if (ProbabilityTest(probability))
			HitTile(tileType, new Vector2(mapPos.x + 1, mapPos.y));

		if (ProbabilityTest(probability))
			HitTile(tileType, new Vector2(mapPos.x, mapPos.y - 1));

		if (ProbabilityTest(probability))
			HitTile(tileType, new Vector2(mapPos.x, mapPos.y + 1));
	}

	bool ProbabilityTest(float probability)
	{
		if (probability == 1.0f) return true;

		return GD.Randf() <= probability;
	}

	void HitTile(TileType tileType, Vector2 mapPosition)
	{
		if (!instancedTiles.TryGetValue(mapPosition, out var cell))
			return;

		if (cell == null)
			return;

		if (cell is BreakableTile tile)
		{
			tile.Crack();
			return;
		}

		// is there a better way to do this?

		// only replace floor tiles
		if(!cell.Filename.Contains("FloorTile"))
			return;

		InstanceTile(mapPosition, tileType);
		cell.QueueFree();
	}
}
