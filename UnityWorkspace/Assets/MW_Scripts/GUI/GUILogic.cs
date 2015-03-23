using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using GameEnums;

public class GUILogic : MonoBehaviour {

	public MWNetwork NETWORK;
	public MW_Game GAME;
	public InputField USERNAME;
	public InputField PASSWORD;
	public MW_Player PLAYER;
	public Button ENDTURN;
	public Transform LAST_CLICKED_ON { get; set;}
	private Dictionary<Vector2,AbstractTile> BOARD_TILES = new Dictionary<Vector2,AbstractTile>();

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
		
		PLAYER = new MW_Player();
		PLAYER.setAttribute(USERNAME.text);
		
		NETWORK.Authenticate(USERNAME.text, PASSWORD.text);
	}

	//Loads lobby info of selected game
	public void JoinGame() {
		NETWORK.joinRoom("demo");
	}

	//Populate popup with available maps
	public void NewGame() {
		//Discuss with kevin
	}

	//Populate popup with saved maps
	public void LoadGame() {
		//discuss with alex
	}

	//Create a lobby and populate with information
	public void HostGame() {
		NETWORK.hostRoom("demo");
	}

	//Sends message in input to all players
	public void SendMessage() {
		//Discuss with kevin
	}

	//Terminates the current game and sends to main menu
	public void EndGame() {
		//Discuss with Kevin
	}

	//Saves the current state of the game and informs all players
	public void SaveGame() {
		//Discuss with Alex?
	}

	//When it becomes current players turn, enable the endturn button
	public void BeginTurn() {
		//Need a call back or some sort of notication. Busy waiting doesn't work
		//Callback should include the game state so I can pass it in
		//GAME.myGameLogic.beginTurn(,PLAYER);
		//Update GUI
	}

	//Ends current turn and goes to next player
	public void EndTurn() {
		ENDTURN.enabled = false;
		GAME.EndTurn ();
	}

	//The ready or start button depending on host or player
	public void Ready_Start() {
		MedievalWarfare mw = new MedievalWarfare ();
		GAME = mw.newGame (NETWORK.getPlayers());
		visualizeMap();
		NETWORK.startGame(mw);	// TODO React to network's response.
	}

	private void visualizeMap() {
		foreach(Tile current in GAME.gameBoard.board) {
			GameObject tile = null;
			//Find the type of terrain to spawn
			switch(current.myType) {
				case LandType.Grass: tile = (GameObject)Resources.Load("tileGrass"); break;
				case LandType.Meadow: tile = (GameObject)Resources.Load("tileMeadow"); break;
				case LandType.Sea: tile = (GameObject)Resources.Load("tileWater"); break;
				case LandType.Tree: tile = (GameObject)Resources.Load("tileForest"); break;
			}
			//Calculate it's game coordinates
			float x = 0;
			float y = 0;
			if(current.boardPosition.y%2==0) {
				x = 2*current.boardPosition.x;
				y = (2*current.boardPosition.y)-(current.boardPosition.y/2);
			}
			else {
				x = 2*current.boardPosition.x+1;
				y = current.boardPosition.y + current.boardPosition.y/2.0f;
			}
			//Update the tile object
			current.gamePosition = new Vector2(x,y);
			//Store it for easier lookup
			BOARD_TILES.Add(current.gamePosition,current);
			//Create the game representation of the tile
			NETWORK.instantiate(tile, new Vector3(x, 0.1f, y), Quaternion.identity);
		}
	}

	public void UpgradeVillage() {
		//Finds the tile associated with the building
		AbstractTile building_tile;
		BOARD_TILES.TryGetValue(new Vector2(LAST_CLICKED_ON.position.x, LAST_CLICKED_ON.position.z), out building_tile);
		VillageType new_type = VillageType.Fort;
		switch(building_tile.myVillage.myType) {
			case VillageType.Hovel: new_type = VillageType.Town; break;
			case VillageType.Town: new_type = VillageType.Fort; break;
			default: break;
		}
		GAME.myGameLogic.upgradeVillage(building_tile.myVillage, new_type);

		//Name associated or am I doing switch statements
	}

	public void UpgradeUnit() {
		//Finds the tile associated with the unit
		AbstractTile unit_tile;
		BOARD_TILES.TryGetValue(new Vector2(LAST_CLICKED_ON.position.x, LAST_CLICKED_ON.position.z), out unit_tile);
		UnitType new_type = UnitType.Knight;
		switch(unit_tile.occupyingUnit.myType) {
			case UnitType.Peasant: new_type = UnitType.Infantry; break;
			case UnitType.Infantry: new_type = UnitType.Soldier; break;
			case UnitType.Soldier: new_type = UnitType.Knight; break;
			default: break;
		}
		GAME.myGameLogic.upgradeUnit(unit_tile.occupyingUnit, new_type);

		//Name associated or am I doing switch statements?
	}

	public void HireVillager() {
		//Finds the tile associated with the village
		AbstractTile building_tile;
		BOARD_TILES.TryGetValue(new Vector2(LAST_CLICKED_ON.position.x, LAST_CLICKED_ON.position.z), out building_tile);
		//...Doesn't have to be a villager?
		AbstractUnit new_villager = new Unit(building_tile.myVillage, building_tile);
		GAME.myGameLogic.hireVillager(new_villager, building_tile.myVillage, building_tile);
		
		//Load new unit
		GameObject new_unit = (GameObject)Resources.Load("peasent");
		NETWORK.instantiate(new_unit ,LAST_CLICKED_ON.position + new Vector3(0,0,0), Quaternion.identity);
		//Destroy current unit
		Object.Destroy(LAST_CLICKED_ON);
		//Set new last clicked on
		LAST_CLICKED_ON = new_unit.transform;
	}

	//The leave or disband button depending on host or player
	public void Leave_Disband() {
		//TODO: Discuss with kevin on how this should work
	}
}
