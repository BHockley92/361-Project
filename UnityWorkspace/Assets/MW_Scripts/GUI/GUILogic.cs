using UnityEngine;
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
	public InputField CHAT;
	private Dictionary<Vector2,AbstractTile> BOARD_TILES = new Dictionary<Vector2,AbstractTile>();
	private SerializeGame SERIALIZER = new SerializeGame();
	private XmlDocument LOADED_GAME;
	private bool FROM_LOADED = false;
	private UNIT_ACTION BUTTON_CLICKED_ON = UNIT_ACTION.NONE;
	
	private Vector3 VILLAGE_OFFSET = new Vector3(0.0f,0.5f,-0.7f);
	private Vector3 UNIT_OFFSET = new Vector3(0.0f,0.5f,-0.71f);

	private enum UNIT_ACTION {
		NONE,
		BUILD_ROAD,
		COMBINE_UNIT,
		CULTIVATE_MEADOW
	};

	//Exit to desktop/quit button
	public void ExitApp() {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

	public void OnGUI() {
		GameObject[] VillageButtons = GameObject.FindGameObjectsWithTag("ForStructures");
		GameObject[] UnitButtons = GameObject.FindGameObjectsWithTag("ForUnits");
		if(LAST_CLICKED_ON != null && LAST_CLICKED_ON.tag.Equals("Village")) {
			//Show Village Buttons
			foreach(GameObject button in VillageButtons) {
				button.SetActive (true);
			}
			//Hide Unit Buttons
			foreach(GameObject button in UnitButtons) {
				button.SetActive (false);
			}
			AbstractTile building_tile;
			Vector3 tilepos = LAST_CLICKED_ON.position - VILLAGE_OFFSET;
			BOARD_TILES.TryGetValue(new Vector2(tilepos.x, tilepos.z), out building_tile);
			if(!building_tile.myVillage.myPlayer.username.Equals(NETWORK.GetLocalPlayerName())) {
				return;
			}
			else {
				GameObject.Find("Gold").GetComponent<Text>().text = "Gold: " + building_tile.myVillage.gold;
				GameObject.Find("Wood").GetComponent<Text>().text = "Wood: " + building_tile.myVillage.gold;
				GameObject.Find("Pop").GetComponent<Text>().text = "Population: " + building_tile.myVillage.supportedUnits;
			}
		}
		else if(LAST_CLICKED_ON != null && LAST_CLICKED_ON.tag.Equals ("Unit")) {
			//Show Unit Buttons
			foreach(GameObject button in VillageButtons) {
				button.SetActive (true);
			}
			//Hide Village Buttons
			foreach(GameObject button in VillageButtons) {
				button.SetActive (false);
			}
		}
	}

	//Authenticates the user to view stats
	public void Authenticate() {
		PLAYER = new MW_Player();
		Debug.Log ("Authenticate Player has been called"); //only called once why?
		PLAYER.setAttribute(USERNAME.text);
		NETWORK.Authenticate(USERNAME.text, PASSWORD.text);
	}

	public void ListRooms() {
		GameObject.Find ("Lobbies").GetComponent<GameSelect>().GAMES = NETWORK.getRooms();
	}

	//Loads lobby info of selected game
	public void JoinGame() {
		string room_name = GameObject.Find ("Lobbies").GetComponent<GameSelect>().getSelected();
		NETWORK.joinRoom(room_name);	// temporary fix for single room instances
		GameObject.Find ("GameName").GetComponent<Text>().text = room_name;
	}

	//Populate popup with available maps
	public void NewGame() {
		//TODO: Do we have presets or are we just doing a random no matter what?
	}

	//Populate popup with saved maps
	public void LoadGame() {
		Debug.Log ("Load Game called");
		FROM_LOADED = true;
		GameObject.Find("SavedGames").GetComponent<GameSelect>().GAMES = Directory.GetFiles("saves");
	}

	//Create a lobby and populate with information
	public void HostGame() {
		//Will grab room name from selected GUI object
		if(FROM_LOADED) {
			LOADED_GAME = new XmlDocument();
			LOADED_GAME.Load (GameObject.Find("SavedGames").GetComponent<GameSelect>().getSelected());
		}
		Debug.Log ("Host game called");
		string room_name = GameObject.Find("RoomName").GetComponentsInChildren<Text>()[1].text;
		GameObject.Find ("LobbyName").GetComponent<Text>().text = room_name;
		NETWORK.hostRoom(room_name);
	}

	//Sends message in input to all players
	public void SendMessage() {
		NETWORK.SendChatMessage(GameObject.Find("Main Camera/Canvas/LobbyMenu/PlayerChat").GetComponent<InputField>().text);
	}

	//Terminates the current game and sends to main menu
	public void EndGame() {
		//TODO: How do we want to handle when a player ends the game? Does it do it differently if one is the host or not
	}

	//Saves the current state of the game and informs all players
	public void SaveGame() {
		if(GameObject.Find ("SaveName").GetComponentsInChildren<Text>()[1].text != "") {
			//We want to save a new file with the above as the name
		}
		//Otherwise they want to overwrite so find the file, delete it and save the game with the same name
		else {
			GameObject.Find ("SavedGames").GetComponent<GameSelect>().getSelected();
		}
		SERIALIZER.saveGameState(GAME);
		//Fake that the game is saving
		StartCoroutine(FakeSaving ());
	}

	//Will fake that the game is saving
	private IEnumerator FakeSaving() {
		yield return new WaitForSeconds(2);
		CanvasGroup saving_text = GameObject.Find ("Saving").GetComponent<CanvasGroup>();
		saving_text.alpha = 0;
		saving_text.interactable = false;
		saving_text.blocksRaycasts = false;
	}

	public void UpdateGameState(string gameState, int senderId) {
		XmlDocument state = new XmlDocument();
		state.LoadXml(gameState); 
		GAME.gameBoard = SERIALIZER.loadGameState(state, GAME);
		Debug.Log ("Received a state");
		Text end_turn = GameObject.Find("ButtonEndTurn").GetComponentsInChildren<Text>()[0]; // not sure if this is ben's intent
		if (GAME.turnOf.username.Equals(NETWORK.GetLocalPlayerName())) {
			//TODO: Covey it is your turn to the user (Need error message thingy)
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
		GAME.myGameLogic.beginTurn(GAME.turnOf,GAME);
		ENDTURN.enabled = true;
	}

	//Ends current turn and goes to next player
	public void EndTurn() {
		//Clear old selection
		Destroy(GameObject.Find("SelectionArrow(Clone)"));
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
			//Add generated board to GAME
			if(LOADED_GAME != null) {
				GAME = SERIALIZER.loadGame(LOADED_GAME);
			}
			else {
				GAME = mw.newGame (NETWORK.getPlayers());
				Debug.Log ("Random map");
				GAME.gameBoard = new Board(20,20,2);
				
				// ALL THE ASSIGNMENT STUFF SHOULD BE DONE IN THE
				// BACKEND YOU RETARDS

				// I PUT IT HERE WHILE I DEALT WITH OTHER SHIT! FUCK YOU!
				
				//each tile that's not water gets a village owned by a random player ie: set up tileOwner
				//during BFS remove all except 1 of the villages that have more than 3 tiles
				foreach (Tile t in GAME.gameBoard.board) {
					if(t.myType != LandType.Sea){
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
		else if (response == MWNetworkResponse.READY_CHECKED)
		{
			MedievalWarfare mw = new MedievalWarfare ();
			GAME = mw.newGame (NETWORK.getPlayers());
		}
		else {
			//Check the other possible problems
			Debug.Log ("failed to start: " + response.ToString());
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
			// set it up here because we change the colour below
		
			Color playerColour = new Color(1,1,1);
			//set colour as fn of player
			if(current.myVillage != null) {
				switch(GAME.participants.IndexOf(current.myVillage.myPlayer)) {
					case 0:
						playerColour = new Color(0.5f,0.0f,0.0f);
						break;
					case 1:
						playerColour = new Color(0.0f,0.5f,0.0f);
						break;
					case 2:
						playerColour = new Color(0.0f,0.0f,0.5f);
						break;
					case 3:
						playerColour = new Color(0.0f,0.5f,0.5f);
						break;
					default:
						Debug.Log("Too many players!");
						break;
				}
			}
			
			instantiated_tile.GetComponent<HexTile>().MeshSetup(playerColour);
			instantiated_tile.AddComponent<BoxCollider>();
			instantiated_tile.AddComponent(typeof(TileClicker));
			instantiated_tile.tag = "Tile";
			
			
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
					
					// TODO: do structures even need a clicker?
					//instantiated_structure.AddComponent<BoxCollider>();
					//instantiated_structure.AddComponent(typeof(Clicker));
					instantiated_structure.tag = "Structure";
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
				GameObject instantiated_village = (GameObject)GameObject.Instantiate(village, pos + VILLAGE_OFFSET, Quaternion.identity);
				
				if(current.myVillage.myPlayer.username.Equals(NETWORK.GetLocalPlayerName())) {
					instantiated_village.AddComponent<BoxCollider>();
					instantiated_village.AddComponent(typeof(Clicker));
				}
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
				GameObject instantiated_unit = (GameObject)GameObject.Instantiate(unit, pos + UNIT_OFFSET, Quaternion.identity);
				
				if(current.myVillage.myPlayer.username.Equals(NETWORK.GetLocalPlayerName())) {
					instantiated_unit.AddComponent<BoxCollider>();
					instantiated_unit.AddComponent(typeof(Clicker));
				}
				instantiated_unit.tag = "Unit";
				
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
		Vector3 tilepos = LAST_CLICKED_ON.position - VILLAGE_OFFSET;
		BOARD_TILES.TryGetValue(new Vector2(tilepos.x, tilepos.z), out building_tile);
		if(!building_tile.myVillage.myPlayer.username.Equals(NETWORK.GetLocalPlayerName())) {
			//TODO: Show error and stop
			return;
		}
		VillageType new_type = VillageType.Fort;
		switch(building_tile.myVillage.myType) {
			case VillageType.Hovel: new_type = VillageType.Town; break;
			case VillageType.Town: new_type = VillageType.Fort; break;
			case VillageType.Fort: new_type = VillageType.Castle; break; 
			default: break;
		}
		if(GAME.myGameLogic.upgradeVillage(building_tile.myVillage, new_type)) {
			//Create new village
			GameObject upgraded_village = (GameObject)Resources.Load("building"+new_type.ToString().ToLower());
			GameObject new_village = (GameObject)GameObject.Instantiate(upgraded_village,new Vector3(building_tile.gamePosition.x, 0, building_tile.gamePosition.y), Quaternion.identity);
			
			// don't need to check for player ownership: is upgrade so of course it's player-owned
			new_village.AddComponent<BoxCollider>();
			new_village.AddComponent(typeof(Clicker));
			new_village.tag = "Village";
			//Set as child
			new_village.transform.parent = GameObject.Find ("map").transform;
			//Destroy old village
			Object.Destroy(LAST_CLICKED_ON.gameObject);
			//Set new last clicked on
			LAST_CLICKED_ON = upgraded_village.transform;
		}
		else {
			//Show error
			GameObject error = GameObject.Find ("Error");
			error.GetComponent<Text>().text = "You cannot upgrade that village";
			error.GetComponent<Text>().enabled = true;
			StartCoroutine(DelayError (error));
		}
	}

	public void UpgradeUnit() {
		if(LAST_CLICKED_ON == null || !LAST_CLICKED_ON.tag.Equals("Unit")) {
			//TODO: They need to have selected a unit to upgrade so we should be hiding buttons until certain conditions are met
			return;
		}

		//Finds the tile associated with the unit
		AbstractTile unit_tile;
		Vector3 tilepos = LAST_CLICKED_ON.position - UNIT_OFFSET;
		BOARD_TILES.TryGetValue(new Vector2(tilepos.x, tilepos.z), out unit_tile);
		UnitType new_type = UnitType.Knight;
		Debug.Log ((unit_tile != null).ToString ());
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
			
			// don't need to check for player ownership: is upgrade so of course it's player-owned
			new_unit.AddComponent<BoxCollider>();
			new_unit.AddComponent(typeof(Clicker));
			new_unit.tag = "Unit";
			//Set as child
			new_unit.transform.parent = GameObject.Find("map").transform;
			//Remove the old one
			GameObject.Destroy(LAST_CLICKED_ON.gameObject);
			//Set new last clicked on
			LAST_CLICKED_ON = upgraded_unit.transform;
		}
		else {
			GameObject error = GameObject.Find ("Error");
			error.GetComponent<Text>().text = "You cannot upgrade that unit";
			error.GetComponent<Text>().enabled = true;
			StartCoroutine(DelayError (error));
		}
	}

	public void HireVillager() {
		if(LAST_CLICKED_ON == null || !LAST_CLICKED_ON.tag.Equals("Village")) {
			//TODO: They need to have selected a village to hire the unit so we should be hiding buttons until certain conditions are met
			return;
		}
		//Finds the tile associated with the village
		AbstractTile building_tile;
		Vector3 tilepos = LAST_CLICKED_ON.position - VILLAGE_OFFSET;
		BOARD_TILES.TryGetValue (new Vector2 (tilepos.x, tilepos.z), out building_tile);
		if (!building_tile.myVillage.myPlayer.username.Equals (NETWORK.GetLocalPlayerName ())) {
			//TODO: Show error and stop
			return;
		}
		//check if unit doesn't exist on tile already
		if (building_tile.occupyingUnit == null) {
			AbstractUnit new_villager = new Unit (building_tile.myVillage, building_tile);

			Tile hired_tile = GAME.myGameLogic.hireVillager (new_villager, building_tile.myVillage, building_tile) as Tile;
			
			//Load new unit if return true
			if (hired_tile != null) {
				//Debug.Log ("Unit Hired: at " + hired_tile.boardPosition.x + ", " + hired_tile.boardPosition.y);
				GameObject new_unit = (GameObject)Resources.Load ("unitpeasant");
				Vector3 unit_location = new Vector3(hired_tile.boardPosition.x, 0.1f, hired_tile.boardPosition.y) + UNIT_OFFSET;
				GameObject hired_villager = (GameObject)GameObject.Instantiate (new_unit, unit_location, Quaternion.identity);
		
				// don't need to check for player ownership: is purchase so of course it's player-owned
				hired_villager.AddComponent<BoxCollider> ();
				hired_villager.AddComponent (typeof(Clicker));
				hired_villager.tag = "Unit";
				//Set as child
				hired_villager.transform.parent = GameObject.Find ("map").transform;

				//Set new last clicked on
				LAST_CLICKED_ON = new_unit.transform;
			} else {
				//remove this new villager made because hireVillager failed
				new_villager = null;

			}
		} 
		else {
			GameObject error = GameObject.Find ("Error");
			error.GetComponent<Text>().text = "That village does not have enough resources";
			error.GetComponent<Text>().enabled = true;
			StartCoroutine(DelayError (error));
			Debug.Log("Unit exists at this location already");
		}
	}

	public void BuildRoad() {
		BUTTON_CLICKED_ON = UNIT_ACTION.BUILD_ROAD;
	}
	
	public void CombineUnit() {
		BUTTON_CLICKED_ON = UNIT_ACTION.COMBINE_UNIT;
	}
	
	public void CultivateMeadow() {
		BUTTON_CLICKED_ON = UNIT_ACTION.CULTIVATE_MEADOW;
	}

	public void moveUnit(Transform tile) {
		if(LAST_CLICKED_ON == null || !LAST_CLICKED_ON.tag.Equals("Unit")) {
			
			return;
		}
		AbstractTile dest_tile;
		BOARD_TILES.TryGetValue(new Vector2(tile.position.x, tile.position.z), out dest_tile);
		AbstractTile unit_tile;
		Vector3 tilepos = LAST_CLICKED_ON.position - UNIT_OFFSET;
		BOARD_TILES.TryGetValue(new Vector2(tilepos.x, tilepos.z), out unit_tile);
		if(unit_tile.occupyingUnit.currentAction != ActionType.ReadyForOrders) {
			GameObject error = GameObject.Find ("Error");
			error.GetComponent<Text>().text = "That unit has already been moved this turn";
			error.GetComponent<Text>().enabled = true;
			StartCoroutine(DelayError (error));
			return;
		}
//		Debug.Log ("Dest tile positon: " + tile.position.x.ToString () + " , " + tile.position.z.ToString ());
		Debug.Log ("Dest board position: " + dest_tile.boardPosition.x.ToString () + " , " + dest_tile.boardPosition.y.ToString ());
//		Debug.Log ("Unit tile positon: " + tilepos.x.ToString () + " , " + tilepos.z.ToString ());
//		Debug.Log ("Unit board position: " + unit_tile.boardPosition.x.ToString () + " , " + unit_tile.boardPosition.y.ToString ());
		bool movedUnit = GAME.myGameLogic.moveUnit(unit_tile.occupyingUnit,dest_tile);
		if (movedUnit) {
			LAST_CLICKED_ON.position = new Vector3 (tile.position.x, 0.1f, tile.position.z) + UNIT_OFFSET;
			Clicker.MoveSelectionArrow (LAST_CLICKED_ON.position);
		}
		else {
			GameObject error = GameObject.Find ("Error");
			error.GetComponent<Text>().text = "Some Error Message Here";
			error.GetComponent<Text>().enabled = true;
			StartCoroutine(DelayError (error));
		}
	}

	//Will fake that the game is saving
	private IEnumerator DelayError(GameObject error) {
		yield return new WaitForSeconds(3);
		error.GetComponent<Text>().text = "";
		error.GetComponent<Text>().enabled = false;
	}

	//The leave or disband button depending on host or player
	public void Leave_Disband() {
		//TODO: Need callback that tells everyone to call leave_disband if they aren't the host
	}
}
