using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using UnityEngine.UI;
using GameEnums;

public class GUILogic : MonoBehaviour {

	public MWNetwork NETWORK;
	public MW_Game GAME;
	public InputField USERNAME;
	public InputField PASSWORD;
	public MW_Player PLAYER;
	public Button ENDTURN;

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
				Debug.Log (PLAYER.username);
			}
		}
	}

	//Loads lobby info of selected game
	public void JoinGame() {
		NETWORK.joinRoom("demo");
	}

	//Populate popup with available maps
	public void NewGame() {

	}

	//Populate popup with saved maps
	public void LoadGame() {

	}

	//Create a lobby and populate with information
	public void HostGame() {
		NETWORK.hostRoom("demo");
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
		if (NETWORK.isMyTurn ()) {
			ENDTURN.enabled = true;
		}
	}

	//Ends current turn and goes to next player
	public void EndTurn() {
		ENDTURN.enabled = false;
		GAME.EndTurn ();
		NETWORK.endTurn ();
	}

	//The ready or start button depending on host or player
	public void Ready_Start() {
		MedievalWarfare mw = new MedievalWarfare ();
		GAME = mw.newGame (NETWORK.getPlayers());
		Camera.main.GetComponent<CameraControl> ().GAME = GAME;
		NETWORK.startGame ();
	}

	public void UpgradeBuilding() {
		GameObject closestTile = null;
		float minDistance = Mathf.Infinity;
		foreach(GameObject current in GameObject.FindObjectsOfType<GameObject>()) {
			if(current.name.Contains("tile") && Mathf.Abs(Camera.main.GetComponent<CameraControl>().TARGET.transform.position.magnitude - current.transform.position.magnitude) < minDistance) {
				minDistance = Mathf.Abs(Camera.main.GetComponent<CameraControl>().TARGET.transform.position.magnitude - current.transform.position.magnitude);
				closestTile = current;
			}
		}
		foreach (Tile current in GAME.gameBoard.board) {
			if(current.boardPosition == new Vector2(closestTile.transform.position.x, closestTile.transform.position.z)) {
				int village_type = (int)current.occupyingStructure.myType;
				VillageType newType = 0;
				switch(village_type) {
					case 0: newType = VillageType.Town;break;
					case 1: newType = VillageType.Fort;break;
					case 2: newType = VillageType.Fort;break;
				}
				GAME.myGameLogic.upgradeVillage (current.myVillage,newType);
			}
		}
	}

	public void UpgradeUnit() {
		GameObject closestTile = null;
		float minDistance = Mathf.Infinity;
		foreach(GameObject current in GameObject.FindObjectsOfType<GameObject>()) {
			if(current.name.Contains("tile") && Mathf.Abs(Camera.main.GetComponent<CameraControl>().TARGET.transform.position.magnitude - current.transform.position.magnitude) < minDistance) {
				minDistance = Mathf.Abs(Camera.main.GetComponent<CameraControl>().TARGET.transform.position.magnitude - current.transform.position.magnitude);
				closestTile = current;
			}
		}
		foreach (Tile current in GAME.gameBoard.board) {
			if(current.boardPosition == new Vector2(closestTile.transform.position.x, closestTile.transform.position.z)) {
				int unit_type = (int)current.occupyingUnit.myType;
				UnitType newType = 0;
				switch(unit_type) {
					case 0: newType = UnitType.Infantry;break;
					case 1: newType = UnitType.Soldier;break;
					case 2: newType = UnitType.Knight;break;
					case 3: newType = UnitType.Knight;break;
				}
				GAME.myGameLogic.upgradeUnit (current.occupyingUnit,newType);
			}
		}
	}

	public void HireVillager() {
		GameObject closestTile = null;
		float minDistance = Mathf.Infinity;
		foreach(GameObject current in GameObject.FindObjectsOfType<GameObject>()) {
			if(current.name.Contains("tile") && Mathf.Abs(Camera.main.GetComponent<CameraControl>().TARGET.transform.position.magnitude - current.transform.position.magnitude) < minDistance) {
				minDistance = Mathf.Abs(Camera.main.GetComponent<CameraControl>().TARGET.transform.position.magnitude - current.transform.position.magnitude);
				closestTile = current;
			}
		}
		foreach (Tile current in GAME.gameBoard.board) {
			if (current.boardPosition == new Vector2 (closestTile.transform.position.x, closestTile.transform.position.z)) {
				Unit newUnit = new Unit(current.myVillage, current);
				newUnit.myType = UnitType.Peasant;
				GAME.myGameLogic.hireVillager(newUnit,current.myVillage,current);
			}
		}
	}

	//The leave or disband button depending on host or player
	public void Leave_Disband() {
	}
}
