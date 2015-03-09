using UnityEngine;
using System.Collections;

public class GameCommands : MonoBehaviour {

	//Exit to desktop/quit button
	public void ExitApp() {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

	//Loads lobby info of selected game
	public void JoinGame() {

	}

	//Populate popup with available maps
	public void NewGame() {

	}

	//Populate popup with saved maps
	public void LoadGame() {

	}

	//Create a lobby and populate with information
	public void HostGame() {

	}

	//Sends message in input to all players
	public void SendMessage() {

	}

	//Terminates the current game and sends to main menu
	public void EndGame() {

	}

	//Saves the current state of the game in XML format and informs all players
	public void SaveGame() {

	}

	//Ends current turn and goes to next player
	public void EndTurn() {

	}

	//The ready or start button depending on host or player
	public void Ready_Start() {

	}

	//The leave or disband button depending on host or player
	public void Leave_Disband() {

	}
}
