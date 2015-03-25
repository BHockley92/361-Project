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
	private SerializeGame SERIALIZER = new SerializeGame();

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
		//
	}

	//Populate popup with saved maps
	public void LoadGame() {
		//
	}

	//Create a lobby and populate with information
	public void HostGame() {
		NETWORK.hostRoom("demo");
	}

	//Sends message in input to all players
	public void SendMessage() {
		//Chat api
	}

	//Terminates the current game and sends to main menu
	public void EndGame() {
		//How do we want to handle when a player ends the game?
	}

	//Saves the current state of the game and informs all players
	public void SaveGame() {
		SERIALIZER.saveGameState(GAME,PLAYER);
	}

	//When it becomes current players turn, enable the endturn button
	public void BeginTurn() {
		GAME.myGameLogic.beginTurn(PLAYER,GAME);
		//If my turn then enable button
		ENDTURN.enabled = true;
		//Otherwise, keep it disabled but update text
	}

	private void UpdateGameState() {

	}

	//Ends current turn and goes to next player
	public void EndTurn() {
		ENDTURN.enabled = false;
		GAME.EndTurn ();
		//Serialize the state of the game now
		SERIALIZER.saveGameState(GAME,PLAYER);
		//Push the serialize state over the network (with next player?)
	}

	//The ready or start button depending on host or player
	public void Ready_Start() {
		//If not host, dont do shit
		MedievalWarfare mw = new MedievalWarfare ();
		GAME = mw.newGame (NETWORK.getPlayers());
		visualizeMap();
		NETWORK.startGame(mw);
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
			GameObject.Instantiate(tile, new Vector3(x, 0.1f, y), Quaternion.identity);
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
		//Create new village
		GameObject upgraded_village = (GameObject)Resources.Load("building"+new_type.ToString().ToLower());
		GameObject.Instantiate(upgraded_village,new Vector3(building_tile.gamePosition.x, 0, building_tile.gamePosition.y), Quaternion.identity);
		//Destroy old village
		Object.Destroy(LAST_CLICKED_ON);
		//Set new last clicked on
		LAST_CLICKED_ON = upgraded_village.transform;
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
		//Generate the new unit
		GameObject upgraded_unit = (GameObject)Resources.Load("unit"+new_type.ToString().ToLower());
		GameObject.Instantiate(upgraded_unit,LAST_CLICKED_ON.position, Quaternion.identity);
		//Remove the old one
		GameObject.Destroy(LAST_CLICKED_ON);
		//Set new last clicked on
		LAST_CLICKED_ON = upgraded_unit.transform;

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
		GameObject.Instantiate(new_unit ,LAST_CLICKED_ON.position + new Vector3(0,0,0), Quaternion.identity);
		//Destroy current unit
		Object.Destroy(LAST_CLICKED_ON);
		//Set new last clicked on
		LAST_CLICKED_ON = new_unit.transform;
	}

	//The leave or disband button depending on host or player
	public void Leave_Disband() {
		//TODO: Need callback that tells everyone to call leave_disband if they aren't the host
	}
}
