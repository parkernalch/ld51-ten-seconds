using Godot;

public class SceneManager : Node
{
	EventBus _eventBus;
	GameManager _gameManager;
	PackedScene gameScene = ResourceLoader.Load<PackedScene>("res://Scenes/main_scene.tscn");
	PackedScene transitionScene = ResourceLoader.Load<PackedScene>("res://Scenes/FloorTransition/FloorTransition.tscn");
	PackedScene mainMenuScene = ResourceLoader.Load<PackedScene>("res://Scenes/primary/PlayableMenu/PlayableMenu.tscn");
	PackedScene creditsScene = ResourceLoader.Load<PackedScene>("res://Scenes/primary/CreditsScene/CreditsScene.tscn");
	PackedScene optionsScene = ResourceLoader.Load<PackedScene>("res://Scenes/primary/OptionsScene/OptionsScene.tscn");
	public override void _Ready()
	{
		_gameManager = GetNode<GameManager>("/root/GameManager");
		_eventBus = GetNode<EventBus>("/root/EventBus");
	}

	public void GoToMainMenu()
	{
		_gameManager.SetCurrentRoom(0);
		_gameManager.RoomsVisitedInRun.Clear();
		GetTree().ChangeSceneTo(mainMenuScene);
	}

	public void GoToGame()
	{
		_gameManager.SetCurrentRoom(1);
		_gameManager.StartGame();
		_gameManager.Scores.GamesPlayed++;
		_gameManager.coinCount = 0;
		GetTree().ChangeSceneTo(gameScene);
	}

	public void GoToNextLevel()
	{
		_eventBus.LeaveRoom(_gameManager.CurrentRoom);
		_gameManager.SetCurrentRoom(_gameManager.CurrentRoom + 1);
		GetTree().ChangeSceneTo(transitionScene);
	}

	public void FinishTransition()
	{
		GetTree().ChangeSceneTo(gameScene);
	}

	public void GoToPreviousLevel()
	{
		if (_gameManager.CurrentRoom <= 1)
		{
			// TODO: may want to change this or at least play a fall animation or "woo woo woooooo" noise 💀
			GoToMainMenu();
			return;
		}

		_gameManager.CurrentRoom--;
		GetTree().ChangeSceneTo(transitionScene);
		// GetTree().ChangeSceneTo(gameScene);
	}

	public void GoToOptions() => GetTree().ChangeSceneTo(optionsScene);

	public void GoToCredits() => GetTree().ChangeSceneTo(creditsScene);

	public void GoToGameOver()
	{
		// TODO: add GameOver scene or update Credits Scene to have a GameOver mode?
		GoToMainMenu();
	}
}
