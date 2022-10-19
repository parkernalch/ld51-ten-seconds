using System.Text.Json;
using JamToolkit.Util;

namespace TenSeconds.Data
{
	public class SaveState
	{
		private const string SaveStateFilePath = "user://save_state.json";

		/// <summary>
		/// The number of times you've played the game
		/// </summary>
		public int GamesPlayed { get; set; }

		/// <summary>
		/// The number of coins you have picked up for all time(across all games played)
		/// </summary>
		public int TotalCoinsPickedUp { get; set; }

		/// <summary>
		/// The number of coins you have dropped up for all time(across all games played)
		/// </summary>
		public int TotalCoinsDropped { get; set; }

		/// <summary>
		/// The most coins held in a single game
		/// </summary>
		public int MostCoinsHeld { get; set; }

		public int FurthestRoom { get; set; }

		public static SaveState Load()
		{
			var json = Files.ReadAllText(SaveStateFilePath);
			if (json == "")
			{
				var state = new SaveState();
				state.Save();
				return state;
			}

			return JsonSerializer.Deserialize<SaveState>(json);
		}



		public void Save()
		{
			var json = JsonSerializer.Serialize(this);
			Files.WriteAllText(SaveStateFilePath, json);
		}
	}
}
