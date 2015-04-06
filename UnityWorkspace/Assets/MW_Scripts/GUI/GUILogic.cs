﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using GameEnums;
using System.Xml;
using System.IO;

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

	public void Update() {
		//TODO: Should be changing what buttons we display based on what LAST_CLICKED_ON is
	}

	//Authenticates the user to view stats
	public void Authenticate() {
		PLAYER = new MW_Player();
		Debug.Log ("Authenticate Player has been called"); //only called once why?
		PLAYER.setAttribute(USERNAME.text);
		NETWORK.Authenticate(USERNAME.text, PASSWORD.text);
	}

	public void ListRooms() {
		//TODO: From the popup, make the "buttons" text this
		string[] rooms = NETWORK.getRooms();
	}

	//Loads lobby info of selected game
	public void JoinGame() {
		//TODO: From the popup with the list of rooms, grab the room name and join it
		NETWORK.joinRoom("demo");
	}

	//Populate popup with available maps
	public void NewGame() {
		//TODO: Do we have presets or are we just doing a random no matter what?
	}

	//Populate popup with saved maps
	public void LoadGame() {
		//TODO: Find the popup to populate, set the "buttons" text to this
		string[] files = Directory.GetFiles("/saves");
	}

	//Create a lobby and populate with information
	public void HostGame() {
		//Will grab room name from selected GUI object
		Debug.Log ("Host game called");
		NETWORK.hostRoom("demo");
	}

	//Sends message in input to all players
	public void SendMessage() {
		//TODO: Chat api (may scrap?)
	}

	//Terminates the current game and sends to main menu
	public void EndGame() {
		//TODO: How do we want to handle when a player ends the game? Does it do it differently if one is the host or not
	}

	//Saves the current state of the game and informs all players
	public void SaveGame() {
		SERIALIZER.saveGameState(GAME);
		//TODO: Show a window that says saving is happening, delay like 5 seconds and then go away
	}

	public void UpdateGameState(string gameState, int senderId) {
		XmlDocument state = new XmlDocument();
		state.LoadXml(gameState); 
		GAME.gameBoard = SERIALIZER.loadGameState(state, GAME);
		Debug.Log ("Received a state");
		//TODO: Test this works
		Text end_turn = GameObject.Find("ButtonEndTurn").GetComponentsInChildren<Text>()[0]; // not sure if this is ben's intent
		if (GAME.turnOf.username.Equals(NETWORK.GetLocalPlayerName())) {
			//TODO: Covey it is your turn to the user (Need error message thingy)
			Debug.Log(GAME.turnOf.username + " " + (GAME.turnOf.myVillages != null).ToString());
			end_turn.text = "End Turn";
			BeginTurn ();
		}
		else {
			end_turn.text = "Waiting for " + GAME.turnOf.username;
		}
		//Always do this
		Debug.Log ("turned camera on");
		Camera.main.GetComponent<CameraControl>().enabled_camera = true;
		Debug.Log ("Showing the board");
		visualizeBoard();
	}

	//When it becomes current players turn, enable the endturn button
	public void BeginTurn() {
		Debug.Log ("my turn");
		Debug.Log(GAME.turnOf.username + " " + (GAME.turnOf.myVillages != null).ToString());
		GAME.myGameLogic.beginTurn(GAME.turnOf,GAME);
		ENDTURN.enabled = true;
	}

	//Ends current turn and goes to next player
	public void EndTurn() {
		ENDTURN.enabled = false;
		//Update game state with transitions
		GAME.EndTurn ();
		//Serialize the state of the game now
		XmlDocument state = SERIALIZER.saveGameState(GAME);
		//Push the serialize state over the network
		NETWORK.ShareGameState(state.OuterXml);
	}

	//The ready or start button depending on host or player
	public void Ready_Start() {
		MWNetworkResponse response = NETWORK.ReadyStart();
		if(response == MWNetworkResponse.GAME_START_SUCCESS) {
			MedievalWarfare mw = new MedievalWarfare ();
			//Get width,height, and water boarder from game UI stuff or xml if exists otherwise have defaults
			GAME = mw.newGame (NETWORK.getPlayers());
			//Add generated board to GAME
			if(LOADED_GAME != null) {
				GAME.gameBoard = SERIALIZER.loadGameState(LOADED_GAME, GAME);
			}
			else {
				Debug.Log ("Random map");
				GAME.gameBoard = new Board(20,20,2);
				
				// ALL THE ASSIGNMENT STUFF SHOULD BE DONE IN THE
				// BACKEND YOU RETARDS
				
				//each tile that's not water gets a village owned by a random player ie: set up tileOwner
				//during BFS remove all except 1 of the villages that have more than 3 tiles
				foreach (Tile t in GAME.gameBoard.board) {
					if(t.myType != LandType.Sea){ //attempt to fix error
						int randomPlayer = Random.Range (0, GAME.participants.Count); 
						List<AbstractTile> myTile = new List<AbstractTile>();
						myTile.Add(t);
						//region consists of the single tile it occupies
						t.myVillage = new Village (myTile, GAME.participants[randomPlayer]);
					}
				}
				mw.assignRegions (GAME.gameBoard);
				
				// remove all special land types from village tiles
				foreach (Tile t in GAME.gameBoard.board) {
					if(t.myVillage != null && t.myVillage.location == t) {
						t.myType = LandType.Grass;
					}
				}
				
				Debug.Log ("Assigned villagers to players");
			}
			//Start the game
			XmlDocument state = SERIALIZER.saveGameState(GAME); //TODO: Emily debug saveGameState
			Debug.Log ("board saved");
			NETWORK.ShareGameState(state.OuterXml);
			Debug.Log ("pushed board");
		}
		else {
			//Check the other possible problems
			Debug.Log ("failed to start");
		}
	}

	private void visualizeBoard() {
		//Clear the old map
		GameObject map = GameObject.Find ("map");
		List<GameObject> children = new List<GameObject>();
		foreach(Transform child in map.transform) {
			children.Add (child.gameObject);
		}
		children.ForEach(child => Destroy (child));
		//Remove the old tiles since I'm loading in the new ones
		BOARD_TILES.Clear();
		//Load in the current map
		foreach(Tile current in GAME.gameBoard.board) {
			//Calculate it's game coordinates
			float x = 0;
			float y = 0;
			if(current.boardPosition.y%2==0) {
				x = 2*current.boardPosition.x;
				y = (2*current.boardPosition.y)-(current.boardPosition.y/2.0f);
			}
			else {
				x = 2*current.boardPosition.x+1;
				y = current.boardPosition.y + (current.boardPosition.y/2.0f);
			}
			//Update the tile object
			current.gamePosition = new Vector2(x,y);
			//Store it for easier lookup later
			BOARD_TILES.Add(current.gamePosition,current);
			//Making the tile
			GameObject tile = null;
			//Find the type of terrain to spawn
			switch(current.myType) {
				case LandType.Grass: tile = (GameObject)Resources.Load("tileGrass"); break;
				case LandType.Meadow: tile = (GameObject)Resources.Load("tileMeadow"); break;
				case LandType.Sea: tile = (GameObject)Resources.Load("tileWater"); break;
				case LandType.Tree: tile = (GameObject)Resources.Load("tileForest"); break;
			}
		
			
			//Create the game representation of the tile
			Vector3 pos = new Vector3(x, 0.1f, y); // based on the above position
			GameObject instantiated_tile = (GameObject)GameObject.Instantiate(tile, pos, Quaternion.identity);
			//Set as child
			instantiated_tile.transform.parent = map.transform;

			if(current.occupyingStructure.myType != StructureType.NONE) {
				GameObject structure = null;
				switch(current.occupyingStructure.myType) {
					case StructureType.Road: instantiated_tile.GetComponent<Renderer>().material.mainTexture = (Texture2D)Resources.Load("Tiles/road"); break;
					case StructureType.Tombstone: structure = (GameObject)Resources.Load("structureTombstone"); break;
					case StructureType.Tower: structure = (GameObject)Resources.Load("structureTower"); break;
					case StructureType.NONE: break;
				}
				if(structure != null) {
					//TODO: Add to position to make sure the object appears naturally
					GameObject instantiated_structure = (GameObject)GameObject.Instantiate(structure, pos, Quaternion.identity);
					//Set as child
					instantiated_structure.transform.parent = map.transform;
				}
			}

			// village
			if(current.myVillage != null && current.myVillage.location == current) {
				GameObject village = null;
				switch(current.myVillage.myType) {
					case VillageType.Hovel: village = (GameObject)Resources.Load("buildinghovel"); break;
					case VillageType.Town: village = (GameObject)Resources.Load("buildingtown"); break;
					case VillageType.Fort: village = (GameObject)Resources.Load("buildingfort"); break;
					case VillageType.Castle: village = (GameObject)Resources.Load("buildingcastle"); break;
				}
				GameObject instantiated_village = (GameObject)GameObject.Instantiate(village, pos + new Vector3(0,0.5f,-0.3f), Quaternion.identity);
				instantiated_village.AddComponent<BoxCollider2D>();
				instantiated_village.AddComponent(typeof(Clicker));
				instantiated_village.tag = "Village";
				//Set as child
				instantiated_village.transform.parent = map.transform;
			}

			// unit
			if( current.occupyingUnit != null) {
				GameObject unit = null;
				if(!current.occupyingUnit.isCannon) {
					switch(current.occupyingUnit.myType) {
						case UnitType.Peasant: unit = (GameObject)Resources.Load("unitPeasant"); break;
						case UnitType.Infantry: unit = (GameObject)Resources.Load("unitInfantry"); break;
						case UnitType.Soldier: unit = (GameObject)Resources.Load("unitSoldier"); break;
						case UnitType.Knight: unit = (GameObject)Resources.Load("unitKnight"); break;
					}
				}
				else {
					unit = (GameObject)Resources.Load("unitCannon");
				}
				//TODO: Add to position to make sure the object appears naturally
				GameObject instantiated_unit = (GameObject)GameObject.Instantiate(unit, pos, Quaternion.identity);
				//Set as child
				instantiated_unit.transform.parent = map.transform;
			}
		}
	}

	public void UpgradeVillage() {
		if(LAST_CLICKED_ON == null || !LAST_CLICKED_ON.tag.Equals("Village")) {
			//TODO: They need to have selected a village to upgrade so we should be hiding buttons until certain conditions are met
			return;
		}
		//Finds the tile associated with the building
		AbstractTile building_tile;
		BOARD_TILES.TryGetValue(new Vector2(LAST_CLICKED_ON.position.x, LAST_CLICKED_ON.position.z), out building_tile);
		if(!building_tile.myVillage.myPlayer.username.Equals(NETWORK.GetLocalPlayerName())) {
			//TODO: Show error and stop
			return;
		}
		VillageType new_type = VillageType.Fort;
		switch(building_tile.myVillage.myType) {
			case VillageType.Hovel: new_type = VillageType.Town; break;
			case VillageType.Town: new_type = VillageType.Fort; break;
			default: break;
		}
		if(GAME.myGameLogic.upgradeVillage(building_tile.myVillage, new_type)) {
			//Create new village
			GameObject upgraded_village = (GameObject)Resources.Load("building"+new_type.ToString().ToLower());
			GameObject new_village = (GameObject)GameObject.Instantiate(upgraded_village,new Vector3(building_tile.gamePosition.x, 0, building_tile.gamePosition.y), Quaternion.identity);
			//Set as child
			new_village.transform.parent = GameObject.Find ("map").transform;
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
		if(LAST_CLICKED_ON == null) {
			//TODO: They need to have selected a unit to upgrade so we should be hiding buttons until certain conditions are met
			return;
		}
		//Finds the tile associated with the unit
		AbstractTile unit_tile;
		BOARD_TILES.TryGetValue(new Vector2(LAST_CLICKED_ON.position.x, LAST_CLICKED_ON.position.z), out unit_tile);
		UnitType new_type = UnitType.Knight;
		if(!unit_tile.myVillage.myPlayer.username.Equals(NETWORK.GetLocalPlayerName())) {
			//TODO: Show error and stop
			return;
		}
		switch(unit_tile.occupyingUnit.myType) {
			case UnitType.Peasant: new_type = UnitType.Infantry; break;
			case UnitType.Infantry: new_type = UnitType.Soldier; break;
			case UnitType.Soldier: new_type = UnitType.Knight; break;
			default: break;
		}
		if(GAME.myGameLogic.upgradeUnit(unit_tile.occupyingUnit, new_type)) {
			//Generate the new unit
			GameObject upgraded_unit = (GameObject)Resources.Load("unit"+new_type.ToString().ToLower());
			GameObject new_unit = (GameObject)GameObject.Instantiate(upgraded_unit,LAST_CLICKED_ON.position, Quaternion.identity);
			//Set as child
			new_unit.transform.parent = GameObject.Find("map").transform;
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
		if(LAST_CLICKED_ON == null) {
			//TODO: They need to have selected a village to hire the unit so we should be hiding buttons until certain conditions are met
			return;
		}
		//Finds the tile associated with the village
		AbstractTile building_tile;
		BOARD_TILES.TryGetValue(new Vector2(LAST_CLICKED_ON.position.x, LAST_CLICKED_ON.position.z+0.3f), out building_tile);
		if(!building_tile.myVillage.myPlayer.username.Equals(NETWORK.GetLocalPlayerName())) {
			//TODO: Show error and stop
			return;
		}
		AbstractUnit new_villager = new Unit(building_tile.myVillage, building_tile);
		//TODO: Needs to return a bool so I can handle any issues
		GAME.myGameLogic.hireVillager(new_villager, building_tile.myVillage, building_tile);
		//Load new unit
		GameObject new_unit = (GameObject)Resources.Load("peasent");
		GameObject hired_villager = (GameObject)GameObject.Instantiate(new_unit ,LAST_CLICKED_ON.position + new Vector3(0,0,0), Quaternion.identity);
		//Set as child
		hired_villager.transform.parent = GameObject.Find("map").transform;
		//Destroy current unit
		Object.Destroy(LAST_CLICKED_ON);
		//Set new last clicked on
		LAST_CLICKED_ON = new_unit.transform;
	}

	public void moveUnit(Transform tile) {
		AbstractTile dest_tile;
		BOARD_TILES.TryGetValue(new Vector2(tile.position.x, tile.position.z), out dest_tile);
		AbstractTile unit_tile;
		BOARD_TILES.TryGetValue(new Vector2(LAST_CLICKED_ON.position.x, LAST_CLICKED_ON.position.z), out unit_tile);
		GAME.myGameLogic.moveUnit(unit_tile.occupyingUnit,dest_tile);
		LAST_CLICKED_ON.position = new Vector3(tile.position.x, 0, tile.position.z);
	}

	//The leave or disband button depending on host or player
	public void Leave_Disband() {
		//TODO: Need callback that tells everyone to call leave_disband if they aren't the host
	}
}
