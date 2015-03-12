using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using UnityEngine.UI;

public class GUILogic : MonoBehaviour {

	public Matchmaking mm;
	public MW_Game GAME;
	public InputField USERNAME;
	public InputField PASSWORD;
	public MW_Player PLAYER;

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
		//Some bs for now to just make players a thing
		XDocument file = XDocument.Load ("player_list.xml");
		foreach (XElement player in file.Root.Elements ("player")) {
			if (player.Attribute ("name").Value == USERNAME.text && player.Attribute ("password").Value == PASSWORD.text) {
				PLAYER = new MW_Player();
				PLAYER.setAttribute(player.Attribute ("name").Value);
			}
		}
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

	//When it becomes current players turn, enable the endturn button
	public void BeginTurn() {
		//TODO
		//Enable the end turn button
	}

	//Ends current turn and goes to next player
	public void EndTurn() {
		GAME.EndTurn ();
		//TODO
		//Disable end turn button
	}

	//The ready or start button depending on host or player
	public void Ready_Start() {
		MedievalWarfare mw = new MedievalWarfare ();
		//TODO
		//Need to get a list of players objects, not just strings
		//GAME = mw.newGame (300, 300, 10, new GameLogic());
	}

	//The leave or disband button depending on host or player
	public void Leave_Disband() {
		mm.LeaveRoom();
	}
}
