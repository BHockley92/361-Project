using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using GameEnums;
using System.Xml;

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
	private XmlDocument LOADED_GAME;

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

	}

	//Populate popup with saved maps
	public void LoadGame() {

	}

	//Create a lobby and populate with information
	public void HostGame() {
		//Will grab room name from selected GUI object
		NETWORK.hostRoom("demo");
		//From game object, if object has path, than load the XML
		//if(.path != null) { LOADED_GAME.Load(.path); }
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

	public void UpdateGameState(string gameState, int senderId) {
		XmlDocument state = new XmlDocument();
		state.LoadXml (gameState);
		GAME.gameBoard = SERIALIZER.loadGameState(state, GAME);
		//TODO: kevin
		BeginTurn ();
		//Always do this
		visualizeBoard();
	}

	//When it becomes current players turn, enable the endturn button
	public void BeginTurn() {
		GAME.myGameLogic.beginTurn(PLAYER,GAME);
		ENDTURN.enabled = true;
	}

	//Ends current turn and goes to next player
	public void EndTurn() {
		ENDTURN.enabled = false;
		//Update game state with transitions
		GAME.EndTurn ();
		//Serialize the state of the game now
		XmlDocument state = SERIALIZER.saveGameState(GAME,PLAYER);
		//Push the serialize state over the network
		NETWORK.turnEnded(state.OuterXml);
	}

	//The ready or start button depending on host or player
	public void Ready_Start() {
		if(NETWORK.startGame() == MWNetworkResponse.GAME_START_SUCCESS) {
			MedievalWarfare mw = new MedievalWarfare ();
			//Get width,height, and water boarder from game UI stuff or xml if exists otherwise have defaults
			GAME = mw.newGame (NETWORK.getPlayers());
			//Add generated board to GAME
			if(LOADED_GAME != null) {
				GAME.gameBoard = SERIALIZER.loadGameState(LOADED_GAME, GAME);
			}
			else {
				GAME.gameBoard = new Board(20,20,2);
				//each tile that's not water gets a village owned by a random player ie: set up tileOwner
				//during BFS remove all except 1 of the villages that have more than 3 tiles
				foreach (Tile t in GAME.gameBoard.board) {
					int randomPlayer = Random.Range (0, NETWORK.getPlayers().Count); //i still think it's -1 need to check
					List<AbstractTile> myTile = new List<AbstractTile>();
					myTile.Add(t);
					//region consists of the single tile it occupies
					t.myVillage = new Village (myTile, NETWORK.getPlayers() [randomPlayer]);
				}
				mw.assignRegions (GAME.gameBoard);
			}
			//Start the game
			XmlDocument state = SERIALIZER.saveGameState(GAME,PLAYER);
			//TODO: Kevin?
			NETWORK.turnEnded(state.OuterXml);
			visualizeBoard();
		}
		else {
			//Check the other possible problems
		}
	}

	//TODO: Nick
	private void visualizeBoard() {
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

			// I tried... -Nick
			Vector3 pos = new Vector3(x, 0.1f, y); // based on the above position

			// building -- no resources that match this
			if( current.occupyingStructure.myType != null)
			{
				switch(current.occupyingStructure.myType)
				{
				case StructureType.Road:
					break;
				case StructureType.Tombstone:
					break;
				case StructureType.Tower:
					break;
				}
			}

			// village
			if(current.myVillage != null && current.myVillage.location == current)
			{
				GameObject village = null;
				switch(current.myVillage.myType)
				{
				case VillageType.Hovel:
					village = (GameObject)Resources.Load("buildinghovel");
					break;
				case VillageType.Town:
					village = (GameObject)Resources.Load("buildingtown");
					break;
				case VillageType.Fort:
					village = (GameObject)Resources.Load("buildingfort");
					break;
				case VillageType.Castle:
					village = (GameObject)Resources.Load("buildingcastle");
					break;
				}
				GameObject.Instantiate(village, pos, Quaternion.identity);
			}

			// unit
			if( current.occupyingUnit != null)
			{
				GameObject unit = null;
				if( !current.occupyingUnit.isCannon)
				{
					switch(current.occupyingUnit.myType)
					{
					case UnitType.Peasant:
						unit = (GameObject)Resources.Load("unitPeasant");
						break;
					case UnitType.Infantry:
						unit = (GameObject)Resources.Load("unitInfantry");
						break;
					case UnitType.Soldier:
						unit = (GameObject)Resources.Load("unitSoldier");
						break;
					case UnitType.Knight:
						unit = (GameObject)Resources.Load("unitKnight");
						break;
					}
				}
				else
				{
					unit = (GameObject)Resources.Load("unitCannon");
				}
				GameObject.Instantiate(unit, pos, Quaternion.identity);
			}
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
		if(GAME.myGameLogic.upgradeVillage(building_tile.myVillage, new_type)) {
			//Create new village
			GameObject upgraded_village = (GameObject)Resources.Load("building"+new_type.ToString().ToLower());
			GameObject.Instantiate(upgraded_village,new Vector3(building_tile.gamePosition.x, 0, building_tile.gamePosition.y), Quaternion.identity);
			//Destroy old village
			Object.Destroy(LAST_CLICKED_ON);
			//Set new last clicked on
			LAST_CLICKED_ON = upgraded_village.transform;
		}
		else {
			//Show error
		}
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
		if(GAME.myGameLogic.upgradeUnit(unit_tile.occupyingUnit, new_type)) {
			//Generate the new unit
			GameObject upgraded_unit = (GameObject)Resources.Load("unit"+new_type.ToString().ToLower());
			GameObject.Instantiate(upgraded_unit,LAST_CLICKED_ON.position, Quaternion.identity);
			//Remove the old one
			GameObject.Destroy(LAST_CLICKED_ON);
			//Set new last clicked on
			LAST_CLICKED_ON = upgraded_unit.transform;
		}
		else {
			//Show error
		}
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
