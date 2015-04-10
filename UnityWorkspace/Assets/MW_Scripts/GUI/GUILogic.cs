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
	public bool BUILD_TOWER = false;
	private Dictionary<Vector2,AbstractTile> BOARD_TILES = new Dictionary<Vector2,AbstractTile>();
	private SerializeGame SERIALIZER = new SerializeGame();
	private XmlDocument LOADED_GAME;
	private bool FROM_LOADED = false;
	private UNIT_ACTION BUTTON_CLICKED_ON = UNIT_ACTION.NONE;
	private GameObject[] VillageButtons;
	private GameObject[] UnitButtons;
	
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

	public void Start() {
		VillageButtons = GameObject.FindGameObjectsWithTag("ForStructures");
		foreach(GameObject button in VillageButtons) {
			button.AddComponent<CanvasGroup>();
			button.GetComponent<CanvasGroup>().alpha = 0;
			button.GetComponent<CanvasGroup>().interactable = false;
			button.GetComponent<CanvasGroup>().blocksRaycasts = false;
		}
		UnitButtons = GameObject.FindGameObjectsWithTag("ForUnits");
		foreach(GameObject button in UnitButtons) {
			button.AddComponent<CanvasGroup>();
			button.GetComponent<CanvasGroup>().alpha = 0;
			button.GetComponent<CanvasGroup>().interactable = false;
			button.GetComponent<CanvasGroup>().blocksRaycasts = false;
		}
	}

	public void OnGUI() {
		//Only show the button when clicking on things if it's the players turn.
		if (GAME != null && GAME.turnOf.username.Equals(NETWORK.GetLocalPlayerName())) {
			if(LAST_CLICKED_ON != null && LAST_CLICKED_ON.tag.Equals("Village")) {
				//Show Village Buttons
				foreach(GameObject button in VillageButtons) {
					button.GetComponent<CanvasGroup>().alpha = 1;
					button.GetComponent<CanvasGroup>().interactable = true;
					button.GetComponent<CanvasGroup>().blocksRaycasts = true;
				}
				//Hide Unit Buttons
				foreach(GameObject button in UnitButtons) {
					button.GetComponent<CanvasGroup>().alpha = 0;
					button.GetComponent<CanvasGroup>().interactable = false;
					button.GetComponent<CanvasGroup>().blocksRaycasts = false;
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
				foreach(GameObject button in UnitButtons) {
					button.GetComponent<CanvasGroup>().alpha = 1;
					button.GetComponent<CanvasGroup>().interactable = true;
					button.GetComponent<CanvasGroup>().blocksRaycasts = true;
				}
				//Hide Village Buttons
				foreach(GameObject button in VillageButtons) {
					button.GetComponent<CanvasGroup>().alpha = 0;
					button.GetComponent<CanvasGroup>().interactable = false;
					button.GetComponent<CanvasGroup>().blocksRaycasts = false;
				}
			}
		}
	}

	//Authenticates the user to view stats
	public void Authenticate() {
		//Create the player
		PLAYER = new MW_Player();
		PLAYER.setAttribute(USERNAME.text);
		NETWORK.Authenticate(USERNAME.text, PASSWORD.text);
	}

	public void HandleLogin(bool result) {
		//If we succeed
		if(result) {
			//Continue with the next menus
			CanvasGroup play_menu = GameObject.Find ("PlayMenu").GetComponent<CanvasGroup>();
			play_menu.alpha = 1;
			play_menu.blocksRaycasts = true;
			play_menu.interactable = true;
			CanvasGroup main_menu = GameObject.Find ("MainMenu").GetComponent<CanvasGroup>();
			main_menu.alpha = 0;
			main_menu.blocksRaycasts = false;
			main_menu.interactable = false;
			CanvasGroup authentication = GameObject.Find ("Main Camera/Canvas/MainMenu/Popup").GetComponent<CanvasGroup>();
			authentication.alpha = 0;
			authentication.interactable = false;
			authentication.blocksRaycasts = false;
		}
		else {
			CanvasGroup failed = GameObject.Find ("Failure").GetComponent<CanvasGroup>();
			failed.alpha = 1;
			failed.interactable = true;
			failed.blocksRaycasts = true;
		}
	}

	public void ListRooms() {
		GameObject.Find ("Lobbies").GetComponent<GameSelect>().GAMES = NETWORK.getRooms();
	}

	//Loads lobby info of selected game
	public void JoinGame() {
		string room_name = GameObject.Find ("Lobbies").GetComponent<GameSelect>().getSelected();
		NETWORK.joinRoom(room_name);	// temporary fix for single room instances
		GameObject.Find ("LobbyName").GetComponent<Text>().text = room_name;
	}

	public void HandleJoinRoom(bool result) {
		if(result) {
			//Continue with the next menus
			CanvasGroup lobby_menu = GameObject.Find ("LobbyMenu").GetComponent<CanvasGroup>();
			lobby_menu.alpha = 1;
			lobby_menu.blocksRaycasts = true;
			lobby_menu.interactable = true;
			CanvasGroup play_menu = GameObject.Find ("PlayMenu").GetComponent<CanvasGroup>();
			play_menu.alpha = 0;
			play_menu.blocksRaycasts = false;
			play_menu.interactable = false;
			CanvasGroup room_join = GameObject.Find ("Main Camera/Canvas/PlayMenu/Popup").GetComponent<CanvasGroup>();
			room_join.alpha = 0;
			room_join.interactable = false;
			room_join.blocksRaycasts = false;
			GameObject.Find ("menu_background").GetComponent<SpriteRenderer>().enabled = false;
		}
		else {
			//Show the error popup
			GameObject.Find ("LobbyName").GetComponent<Text>().text = "";
			CanvasGroup join_fail = GameObject.Find ("JoinFailure").GetComponent<CanvasGroup>();
			join_fail.alpha = 1;
			join_fail.interactable = true;
			join_fail.blocksRaycasts = true;
		}
	}

	//Populate popup with available maps
	public void NewGame() {
		FROM_LOADED = false;
		GameObject.Find("SavedGamesMenu").GetComponent<GameSelect>().GAMES = Directory.GetFiles("premade");
	}

	//Populate popup with saved maps
	public void LoadGame() {
		FROM_LOADED = true;
		GameObject.Find("SavedGamesMenu").GetComponent<GameSelect>().GAMES = Directory.GetFiles("saves");
	}

	public void ResetList() {
		GameObject.Find("SavedGamesMenu").GetComponent<GameSelect>().GAMES = new List<string>().ToArray();
	}

	//Create a lobby and populate with information
	public void HostGame() {
		//Will grab room name from selected GUI object
		if(FROM_LOADED) {
			LOADED_GAME = new XmlDocument();
			LOADED_GAME.Load (GameObject.Find("SavedGamesMenu").GetComponent<GameSelect>().getSelected());
		}
		Debug.Log ("Host game called");
		string room_name = GameObject.Find("RoomName").GetComponentsInChildren<Text>()[1].text;
		GameObject.Find ("LobbyName").GetComponent<Text>().text = room_name;
		NETWORK.hostRoom(room_name);
		ResetList ();
	}

	public void HandleCreateRoom(bool result) {
		if(result) {
			//Continue with the next menus
			CanvasGroup lobby_menu = GameObject.Find ("LobbyMenu").GetComponent<CanvasGroup>();
			lobby_menu.alpha = 1;
			lobby_menu.blocksRaycasts = true;
			lobby_menu.interactable = true;
			CanvasGroup host_menu = GameObject.Find ("HostMenu").GetComponent<CanvasGroup>();
			host_menu.alpha = 0;
			host_menu.blocksRaycasts = false;
			host_menu.interactable = false;
			CanvasGroup room_creation = GameObject.Find ("Main Camera/Canvas/HostMenu/Popup").GetComponent<CanvasGroup>();
			room_creation.alpha = 0;
			room_creation.interactable = false;
			room_creation.blocksRaycasts = false;
			GameObject.Find ("menu_background").GetComponent<SpriteRenderer>().enabled = false;
		}
		else {
			//Show the error popup
			GameObject.Find ("LobbyName").GetComponent<Text>().text = "";
			CanvasGroup host_fail = GameObject.Find ("HostFailure").GetComponent<CanvasGroup>();
			host_fail.alpha = 1;
			host_fail.interactable = true;
			host_fail.blocksRaycasts = true;
		}
	}

	//Sends message in input to all players
	public void SendMessage() {
		NETWORK.SendChatMessage(GameObject.Find("Main Camera/Canvas/LobbyMenu/PlayerChat").GetComponent<InputField>().text);
	}

	//Terminates the current game and sends to main menu
	public void EndGame() {
		NETWORK.EndGame();
	}

	
	public void forceQuit() {
		//Clear the map
		GameObject map = GameObject.Find ("map");
		List<GameObject> children = new List<GameObject>();
		foreach(Transform child in map.transform) {
			children.Add (child.gameObject);
		}
		children.ForEach(child => Destroy (child));
		//Remove the old tiles since I'm loading in the new ones
		BOARD_TILES.Clear();
		CanvasGroup main_menu = GameObject.Find ("MainMenu").GetComponent<CanvasGroup>();
		main_menu.alpha = 1;
		main_menu.blocksRaycasts = true;
		main_menu.interactable = true;
		CanvasGroup in_game_menu = GameObject.Find ("Main Camera/Canvas/GameGUI/Popup").GetComponent<CanvasGroup>();
		in_game_menu.alpha = 0;
		in_game_menu.blocksRaycasts = false;
		in_game_menu.interactable = false;
		CanvasGroup game_gui = GameObject.Find ("GameGUI").GetComponent<CanvasGroup>();
		game_gui.alpha = 0;
		game_gui.interactable = false;
		game_gui.blocksRaycasts = false;
		GameObject.Find ("menu_background").GetComponent<SpriteRenderer>().enabled = true;
	}

	//Saves the current state of the game and informs all players
	public void SaveGame() {
		XmlDocument game = SERIALIZER.saveGameState(GAME);
		if(GameObject.Find ("SaveName").GetComponentsInChildren<Text>()[1].text != "") {
			game.Save (@"saves\" + GameObject.Find ("SaveName").GetComponentsInChildren<Text>()[1].text);
		}
		//Otherwise they want to overwrite so find the file, delete it and save the game with the same name
		else {
			string selected_game = GameObject.Find ("SavedGamesInGame").GetComponent<GameSelect>().getSelected();
			if(File.Exists(selected_game)) {
				File.Delete(selected_game);
				game.Save (GameObject.Find ("SavedGamesInGame").GetComponent<GameSelect>().getSelected());
			}
			else {
				//Should never occur since we only load in files that exist but save it anyway
				game.Save (GameObject.Find ("SavedGamesInGame").GetComponent<GameSelect>().getSelected());
			}
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
		GAME = SERIALIZER.loadGame(state);
		Debug.Log ("Received a state");
		if (GAME != null) Debug.Log(NETWORK.GetLocalPlayerName() + " is notified that it is " + GAME.turnOf.username + "'s turn.");				Text end_turn = GameObject.Find("ButtonEndTurn").GetComponentsInChildren<Text>()[0]; // not sure if this is ben's intent
		if (GAME.turnOf.username.Equals(NETWORK.GetLocalPlayerName())) {
			end_turn.text = "End Turn";
			Debug.Log ("My turn");
			BeginTurn ();
		}
		else {
			end_turn.text = "Waiting for " + GAME.turnOf.username;
			Debug.Log ("Other player's turn");
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
		ENDTURN.interactable = true;
	}

	//Ends current turn and goes to next player
	public void EndTurn() {
		//Clear old selection
		Destroy(GameObject.Find("SelectionArrow(Clone)"));
		ENDTURN.interactable = false;
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
				GAME.gameBoard = new Board(20,20,3);
				
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
			Debug.Log("Ready checked.");
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
			float x = current.boardPosition.y + current.boardPosition.x * 2;
			float y = current.boardPosition.y + 0.5f * current.boardPosition.y;
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
			//Spawn all the shit
			instantiated_tile.GetComponent<HexTile>().InstantiateTile();
		
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
			//Add collider and click script
			instantiated_tile.AddComponent<BoxCollider>();
			instantiated_tile.AddComponent(typeof(TileClicker));
			//Add a border
			GameObject border = Instantiate(tile, instantiated_tile.transform.position, Quaternion.identity) as GameObject;
			//Make border a child of the tile so it doesn't spawn billions of them
			border.transform.parent = instantiated_tile.transform;
			//Setup components of border
			MeshFilter  meshFilter = border.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = border.AddComponent<MeshRenderer>();
			meshFilter.mesh = instantiated_tile.GetComponent<MeshFilter>().mesh;
			meshRenderer.material.mainTexture = (Texture)Resources.Load ("border-white");
			meshRenderer.material.shader = Shader.Find ("Transparent/Diffuse");
			meshRenderer.material.color = playerColour;
			//Add tag
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
					GameObject instantiated_structure = (GameObject)GameObject.Instantiate(structure, pos + VILLAGE_OFFSET, Quaternion.identity);
					
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
		if(LAST_CLICKED_ON == null || !LAST_CLICKED_ON.tag.Equals("Village") || !GAME.turnOf.username.Equals(NETWORK.GetLocalPlayerName())) {
			//TODO: They need to have selected a village to upgrade so we should be hiding buttons until certain conditions are met
			return;
		}
		//Finds the tile associated with the building
		AbstractTile building_tile;
		Vector3 tilepos = LAST_CLICKED_ON.position - VILLAGE_OFFSET;
		BOARD_TILES.TryGetValue(new Vector2(tilepos.x, tilepos.z), out building_tile);
		if(!building_tile.myVillage.myPlayer.username.Equals(NETWORK.GetLocalPlayerName())) {
			//TODO: Show error and stop
			//Technically it shouldn't be possible to even see this
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
			GameObject new_village = (GameObject)GameObject.Instantiate(upgraded_village,new Vector3(building_tile.gamePosition.x, 0, building_tile.gamePosition.y) + VILLAGE_OFFSET, Quaternion.identity);
			
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
		if(LAST_CLICKED_ON == null || !LAST_CLICKED_ON.tag.Equals("Unit") || !GAME.turnOf.username.Equals(NETWORK.GetLocalPlayerName())) {
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
			GameObject new_unit = (GameObject)GameObject.Instantiate(upgraded_unit,LAST_CLICKED_ON.position + UNIT_OFFSET, Quaternion.identity);
			
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
		if(LAST_CLICKED_ON == null || !LAST_CLICKED_ON.tag.Equals("Village") || !GAME.turnOf.username.Equals(NETWORK.GetLocalPlayerName())) {
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

			Tile hired_tile = GAME.myGameLogic.hireVillager (new_villager, building_tile.myVillage) as Tile;
			
			//Load new unit if return true
			if (hired_tile != null) {
				//Debug.Log ("Unit Hired: at " + hired_tile.boardPosition.x + ", " + hired_tile.boardPosition.y);
				GameObject new_unit = (GameObject)Resources.Load ("unitpeasant");
				Vector3 unit_location = new Vector3(hired_tile.gamePosition.x, 0.1f, hired_tile.gamePosition.y) + UNIT_OFFSET;
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
		if(GAME.turnOf.username.Equals(NETWORK.GetLocalPlayerName())) {
			BUTTON_CLICKED_ON = UNIT_ACTION.BUILD_ROAD;
		}
	}
	
	public void CombineUnit() {
		if(GAME.turnOf.username.Equals(NETWORK.GetLocalPlayerName())) {
			BUTTON_CLICKED_ON = UNIT_ACTION.COMBINE_UNIT;
		}
	}
	
	public void CultivateMeadow() {
		if(GAME.turnOf.username.Equals(NETWORK.GetLocalPlayerName())) {
			BUTTON_CLICKED_ON = UNIT_ACTION.CULTIVATE_MEADOW;
		}
	}

	public void BuildTower() {
		if(GAME.turnOf.username.Equals(NETWORK.GetLocalPlayerName())) {
			BUILD_TOWER = true;
		}
	}

	public void buildTower(Transform build_spot) {
		BUILD_TOWER = false;
		AbstractTile dest_tile;
		BOARD_TILES.TryGetValue(new Vector2(build_spot.position.x, build_spot.position.z), out dest_tile);
		AbstractTile village_tile;
		Vector3 tilepos = LAST_CLICKED_ON.position - UNIT_OFFSET;
		BOARD_TILES.TryGetValue(new Vector2(tilepos.x, tilepos.z), out village_tile);
		if (!village_tile.myVillage.controlledRegion.Contains(dest_tile)) {
			GameObject error = GameObject.Find ("Error");
			error.GetComponent<Text>().text = "You do not control that territory";
			error.GetComponent<Text>().enabled = true;
			StartCoroutine(DelayError (error));
			return;
		}
		if(GAME.myGameLogic.buildTower (dest_tile)) {
			GameObject tower_object = (GameObject)Resources.Load("structureTower");
			GameObject tower = (GameObject)GameObject.Instantiate(tower_object ,new Vector3(dest_tile.gamePosition.x, 0, dest_tile.gamePosition.y) + VILLAGE_OFFSET, Quaternion.identity);
			//Tower is the players thing
			tower.AddComponent<BoxCollider>();
			tower.AddComponent(typeof(Clicker));
			tower.transform.parent = GameObject.Find ("map").transform;
			LAST_CLICKED_ON = tower.transform;
		}
		else {
			GameObject error = GameObject.Find ("Error");
			error.GetComponent<Text>().text = "Cannot build tower on enemy territory";
			error.GetComponent<Text>().enabled = true;
			StartCoroutine(DelayError (error));
		}
	}

	public void BuildCannon() {
		if(LAST_CLICKED_ON == null || !LAST_CLICKED_ON.tag.Equals("Village") || !GAME.turnOf.username.Equals(NETWORK.GetLocalPlayerName())) {
			//TODO: They need to have selected a village to hire the unit so we should be hiding buttons until certain conditions are met
			return;
		}
		AbstractTile building_tile;
		Vector3 tilepos = LAST_CLICKED_ON.position - VILLAGE_OFFSET;
		BOARD_TILES.TryGetValue (new Vector2 (tilepos.x, tilepos.z), out building_tile);
		if (!building_tile.myVillage.myPlayer.username.Equals (NETWORK.GetLocalPlayerName ())) {
			//TODO: Show error and stop
			return;
		}
		AbstractTile hired_tile = GAME.myGameLogic.buildCannon(building_tile ,building_tile.myVillage);
		if(hired_tile != null) {
			GameObject new_unit = (GameObject)Resources.Load ("unitcannon");
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
		}
		else {
			GameObject error = GameObject.Find ("Error");
			error.GetComponent<Text>().text = "That village does not have enough resources";
			error.GetComponent<Text>().enabled = true;
			StartCoroutine(DelayError (error));
		}
	}

	public void moveUnit(Transform tile) {
		if(LAST_CLICKED_ON == null || !LAST_CLICKED_ON.tag.Equals("Unit") || !GAME.turnOf.username.Equals(NETWORK.GetLocalPlayerName())) {
			return;
		}
		AbstractTile dest_tile;
		BOARD_TILES.TryGetValue(new Vector2(tile.position.x, tile.position.z), out dest_tile);
		AbstractTile unit_tile;
		Vector3 tilepos = LAST_CLICKED_ON.position - UNIT_OFFSET;
		BOARD_TILES.TryGetValue(new Vector2(tilepos.x, tilepos.z), out unit_tile);
		if(unit_tile.occupyingUnit.currentAction != ActionType.ReadyForOrders) {
			GameObject error = GameObject.Find ("Error");
			error.GetComponent<Text>().text = "That unit is unable to perform another action this turn";
			error.GetComponent<Text>().enabled = true;
			StartCoroutine(DelayError (error));
			return;
		}
//		Debug.Log ("Dest tile positon: " + tile.position.x.ToString () + " , " + tile.position.z.ToString ());
		Debug.Log ("Dest board position: " + dest_tile.boardPosition.x.ToString () + " , " + dest_tile.boardPosition.y.ToString ());
//		Debug.Log ("Unit tile positon: " + tilepos.x.ToString () + " , " + tilepos.z.ToString ());
//		Debug.Log ("Unit board position: " + unit_tile.boardPosition.x.ToString () + " , " + unit_tile.boardPosition.y.ToString ());
		if (GAME.myGameLogic.moveUnit(unit_tile.occupyingUnit,dest_tile)) {
			//Make sure the unit's current action is changed if it wasn't changed in moveUnit
			if(dest_tile.occupyingUnit.currentAction == ActionType.ReadyForOrders) {
				switch(BUTTON_CLICKED_ON) {
					case UNIT_ACTION.BUILD_ROAD:{
						if(dest_tile.occupyingUnit.myType != UnitType.Peasant){
							GameObject error = GameObject.Find ("Error");
							error.GetComponent<Text>().text = "Only peasants can build roads.";
							error.GetComponent<Text>().enabled = true;
							StartCoroutine(DelayError (error));
							return;
						}
						else{
							dest_tile.occupyingUnit.currentAction = ActionType.BuildingRoad;
							break;
						}
						
					}
					case UNIT_ACTION.COMBINE_UNIT: {
														
						dest_tile.occupyingUnit.currentAction = ActionType.UpgradingCombining; 
						break;
					}
					case UNIT_ACTION.CULTIVATE_MEADOW: {
					//only peasants will cultivate a meadow
						if(dest_tile.occupyingUnit.myType != UnitType.Peasant){
							GameObject error = GameObject.Find ("Error");
							error.GetComponent<Text>().text = "Only peasants can cultivate meadows.";
							error.GetComponent<Text>().enabled = true;
							StartCoroutine(DelayError (error));
							return;
						}
						else{
							dest_tile.occupyingUnit.currentAction = ActionType.StartCultivating; 
							break;
						}
					}
					default: dest_tile.occupyingUnit.currentAction = ActionType.Moved; break;
				}
			}
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
		NETWORK.EndGame();
	}
}
