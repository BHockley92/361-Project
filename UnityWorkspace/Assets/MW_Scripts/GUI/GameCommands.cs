using UnityEngine;
using System.Collections;

public class GameCommands : MonoBehaviour {

	public Matchmaking mm;

	//Exit to desktop/quit button
	public void ExitApp() {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

	//Authenticates the user to view stats
	public void Authenticate() {

	}

	//Loads lobby info of selected game
	public void JoinGame() {
		mm.JoinGame();
		if (mm.error){};
		//TODO: some kind of error reaction
	}

	//Populate popup with available maps
	public void NewGame() {

	}

	//Populate popup with saved maps
	public void LoadGame() {

	}

	//Create a lobby and populate with information
	public void HostGame() {
		mm.HostGame();
	}

	//Sends message in input to all players
	public void SendMessage() {

	}

	//Terminates the current game and sends to main menu
	public void EndGame() {

	}

	//Saves the current state of the game and informs all players
	public void SaveGame() {

	}

	//Ends current turn and goes to next player
	public void EndTurn() {

	}

	//The ready or start button depending on host or player
	public void Ready_Start() {
		//MedievalWarfare mw = new MedievalWarfare ();
		//MW_Game currentGame = mw.newGame ();

	}

	//The leave or disband button depending on host or player
	public void Leave_Disband() {
		mm.LeaveRoom();
	}
}
