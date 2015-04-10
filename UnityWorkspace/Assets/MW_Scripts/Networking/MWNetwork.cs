using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// Photon networking namespaces
using Hashtable = ExitGames.Client.Photon.Hashtable;

// Authentication namespaces
using PlayFab;
using PlayFab.ClientModels;

// TODO Chat?

/*
 * An enum for MWNetwork responses.
 */
public enum MWNetworkResponse
{
	GAME_START_SUCCESS,
	READY_CHECKED,
	PLAYERS_NOT_READY
}

/*
 * This is a utility class for networking within the Medieval Warfare game.
 * This is mostly an enhanced wrapper for Photon's built-in functions to hide the API details from the group.
 */
public class MWNetwork : Photon.MonoBehaviour
{
	// Useful references to GUI elements
	public GUILogic gui;
	public Text GUIplayerList;
	public Text Chat;
	
	// Used for sending game state over network.
	private string     cachedGameState = "";
	private const byte UPDATED_GAME_STATE = 1;
	private const byte CHAT_MESSAGE_SENT = 2;
	private const int  GAME_STATE_SUBSTRINGS = 4;
	private int        gameStateSubstringsReceived = 0;
	
	// Fields for interfacing with Playfab
	private Hashtable statistics = new Hashtable();
	
	// Singleton reference
	private static MWNetwork instance;
	
	private static string version = "1.0";    // The game version.  Will probably never change this.
	
	/* 
	 * Connects to Photon's master server using current version number.
	 */
	void Start()
	{
		instance = this;
		
		Chat = GameObject.Find("Main Camera/Canvas/LobbyMenu/Chat/Chat").GetComponent<Text>();
		
		// Photon initialization
		PhotonNetwork.ConnectUsingSettings(version);
		Debug.Log("Connected to master server!");
		PhotonNetwork.OnEventCall += OnGameStateReceived;
		PhotonNetwork.OnEventCall += OnChatMessageReceived;
		
		// Playfab initialization
		statistics.Add("wins", 0);
		statistics.Add("losses", 0);
	}
	
	/* 
     * Returns the MWNetwork component upon which you can call all functions below.
     */
	public static MWNetwork getInstance()
	{
		return instance;
	}
	
	/*
	 * Sets a maximum number of players that can join a room/game.
	 */
	public void setMaxPlayers(int max)
	{
		PhotonNetwork.room.maxPlayers = max;
	}
	
	/*
	 * Creates a room on the network with the given name.
	 */
	public void hostRoom(string roomName)
	{
		PhotonNetwork.CreateRoom(roomName, new RoomOptions() { maxPlayers = 4 }, null);
	}
	
	/*
	 * Joins a room on the network with the given name.
	 */
	public void joinRoom(string roomName)
	{
		PhotonNetwork.JoinRoom(roomName);
	}
	
	/*
	 * Initializes all player properties for the beginning of a game.
	 * Currently, master client is automatically player 1 and other players turns
	 * are ordered according to when they joined.
     * Returns a response indicating whether the game was started succesfully or not.
	 */
	public MWNetworkResponse ReadyStart()
	{		
		// If not host, ready check
		if (!PhotonNetwork.isMasterClient)
		{
			// Ready check
			Hashtable readyCheck = new Hashtable();
			readyCheck.Add("ready", true);
			PhotonNetwork.SetPlayerCustomProperties(readyCheck);
			
			return MWNetworkResponse.READY_CHECKED;
		}
		
		// If host, make sure all players are ready
		foreach (PhotonPlayer player in PhotonNetwork.playerList)
		{
			if (!player.isMasterClient && (bool)player.customProperties["ready"] == false)
			{
				return MWNetworkResponse.PLAYERS_NOT_READY;
			}
		}
		
		// Make the room impossible to see or join
		PhotonNetwork.room.visible = false;
		PhotonNetwork.room.open = false;
		
		return MWNetworkResponse.GAME_START_SUCCESS;
	}
	
	public string GetLocalPlayerName()
	{
		return PhotonNetwork.playerName;
	}
	
	/*
     * Returns whether or not you have joined a game.
     */ 
	public bool joinedRoom()
	{
		return PhotonNetwork.room != null;
	}
	
	/*
    * Returns whether or not you have started the game.
    */ 
	public bool gameStarted()
	{
		return (bool) PhotonNetwork.room.customProperties["gameStarted"];
	}
	
	/*
     * Returns whether or not you are the host
     */
	public bool amHost()
	{
		return PhotonNetwork.isMasterClient;
	}
	
	/*
     * Returns whether or not you are connected to Photon's master server.
     */ 
	public bool connected()
	{
		return PhotonNetwork.connected;
	}
	
	/*
	 * Returns an array of room names.
	 */
	public string[] getRooms()
	{
		RoomInfo[] rooms = PhotonNetwork.GetRoomList();
		
		string[] roomNames = new string[rooms.Length];
		
		for (int i = 0; i < rooms.Length; i++)
		{
			roomNames[i] = rooms[i].name;
		}
		
		return roomNames;
	}
	
	/*
	 * Returns a list of AbstractPlayers in the room. 
	 */
	public List<AbstractPlayer> getPlayers()
	{
		PhotonPlayer[] players = PhotonNetwork.playerList;
		
		List<AbstractPlayer> mwPlayers = new List<AbstractPlayer>();
		
		foreach (PhotonPlayer player in players)
		{
			MW_Player mwPlayer = new MW_Player();
			mwPlayer.setAttribute(player.name);
			
			mwPlayers.Add(mwPlayer);
		}
		
		return mwPlayers;
	}
	
	
	/***************************************************************************************************
	 * CHAT
	 ***************************************************************************************************/
	
	public void SendChatMessage(string message)
	{
		if (!PhotonNetwork.RaiseEvent(CHAT_MESSAGE_SENT,
		                              message,
		                              true,
		                              new RaiseEventOptions() { Receivers = ExitGames.Client.Photon.Lite.ReceiverGroup.All }))
		{
			Debug.Log("Error sending chat message over network.");
		}
	}
	
	public void OnChatMessageReceived(byte eventCode, object content, int senderId)
	{
		if (eventCode == CHAT_MESSAGE_SENT)
		{
			Chat.text += PhotonPlayer.Find(senderId).name + " says: " + (string) content + '\n';
		}
	}
	
	/***************************************************************************************************
	 * GAME STATE NETWORK PERSISTENCE
	 ***************************************************************************************************/
	
	/*
	 * This function shares your game state to other machines.
	 * These machines will in turn update their game state.
	 * TODO If necessary, dynamically choose the number of substrings to send
	 */
	public void ShareGameState(string gameState)
	{
		string[] gameStateSubstrings = new string[GAME_STATE_SUBSTRINGS];
		int gameStateSubtringLength = Mathf.FloorToInt(gameState.Length / GAME_STATE_SUBSTRINGS);
		
		Debug.Log("Sharing game state of length " + gameState.Length);
		
		// Divide game state string into substrings
		for (int i = 0; i < GAME_STATE_SUBSTRINGS; i++)
		{
			if (i != GAME_STATE_SUBSTRINGS - 1)
			{
				gameStateSubstrings[i] = gameState.Substring(i * gameStateSubtringLength, gameStateSubtringLength);
			}
			else {
				gameStateSubstrings[i] = gameState.Substring(i * gameStateSubtringLength);
			}
		}
		// Send each substring over the network
		for (int i = 0; i < GAME_STATE_SUBSTRINGS; i++)
		{			
			if (!PhotonNetwork.RaiseEvent(UPDATED_GAME_STATE,
			                              gameStateSubstrings[i],
			                              true,
			                              new RaiseEventOptions() { Receivers = ExitGames.Client.Photon.Lite.ReceiverGroup.All }))
			{
				Debug.Log("Error sending game state over network.");
			}
		}
	}
	
	/*
	 * Called when "ShareGameState" is called on another player.
	 * Delegates the deserialization task.
	 */
	public void OnGameStateReceived(byte eventCode, object content, int senderId)
	{
		if (eventCode == UPDATED_GAME_STATE)
		{
			cachedGameState += (string) content;
			gameStateSubstringsReceived++;
			
			if (gameStateSubstringsReceived == GAME_STATE_SUBSTRINGS)
			{
				Debug.Log("Sending game state string to GUILogic.  String length: " + cachedGameState.Length);
				
				/*Write the string to a file. TESTING
				StreamWriter file = new StreamWriter("Kevin_update.txt");
				file.WriteLine((string)cachedGameState);
				
				file.Flush();
				*/
				
				gui.UpdateGameState(cachedGameState, senderId);
				
				
				
				// Reinitialize game state string cache
				cachedGameState = "";
				gameStateSubstringsReceived = 0;
			}
		}
	}
	
	/***************************************************************************************************
	 * PLAYFAB AUTHENTICATION & STATS RETRIEVAL
	 ***************************************************************************************************/
	
	/*
	 * Attempts to authenticate the player.
	 */
	public void Authenticate(string username, string password)
	{
		LoginWithPlayFabRequest request = new LoginWithPlayFabRequest();
		
		request.Username = username;
		request.Password = password;
		request.TitleId = PlayFabData.TitleId;
		PlayFabClientAPI.LoginWithPlayFab(request, OnLoginResult, OnLoginError);
	} 
	
	/*
	 * A callback for the local player's successful login.
	 */
	private void OnLoginResult(LoginResult result)
	{
		PhotonNetwork.playerName = gui.PLAYER.username;
		PlayFabData.AuthKey = PlayFabClientAPI.AuthKey;
		Debug.Log("Login success!");
		
		UpdateLocalPlayerStatistics();
	}
	
	/*
	 * A callback for the local player's unsuccessful login.
	 */
	private void OnLoginError(PlayFabError error)
	{
		Debug.Log("Login error: " + error.ErrorMessage);
	}
	
	/* 
	 * Request to update the local player's statistics from Playfab.
	 */
	private void UpdateLocalPlayerStatistics()
	{
		GetUserDataRequest request = new GetUserDataRequest ();
		if (PlayFabData.AuthKey != null)
			PlayFabClientAPI.GetUserData (request, OnStatisticsReceived, OnStatisticsError);
		else 
			Debug.Log("AuthKey null.");
	}
	
	/*
	 * A callback for successfully downloading the local player's statistics.
	 */
	private void OnStatisticsReceived(GetUserDataResult result)
	{
		statistics["wins"] = result.Data["wins"].Value;
		statistics["losses"] = result.Data["losses"].Value;
		
		Debug.Log("Statistics downloaded successfully: " +
		          statistics["wins"].ToString() + " wins, " +
		          statistics["losses"].ToString() + " losses.");
	}
	
	/*
	 * A callback for unsuccessfully downloading the local player's statistics.
	 */
	private void OnStatisticsError(PlayFabError error)
	{
		Debug.Log("Statistics retreival error: " + error.ErrorMessage);
	}
	
	/***************************************************************************************************
     * PHOTON CALLBACKS
     ***************************************************************************************************/
	
	void OnJoinedRoom()
	{
		// Set player custom properties
		Hashtable readyCheck = new Hashtable();
		readyCheck.Add("ready", false);
		PhotonNetwork.SetPlayerCustomProperties(readyCheck);
		
		UpdateGUIPlayerList();
	}
	
	
	void OnPhotonPlayerConnected()
	{
		UpdateGUIPlayerList();
	}
	
	
	void OnCreatedRoom()
	{
		Hashtable roomProps = new Hashtable();
		roomProps.Add("gameStarted", false);
		PhotonNetwork.room.SetCustomProperties(roomProps);
		
		Debug.Log ("Room created.");
	}
	
	void OnPhotonCreateRoomFailed()
	{
		Debug.Log("Could not create room, probably because room name is already taken.");
	}
	
	void OnPhotonJoinRoomFailed()
	{
		Debug.Log("Could not join room, possibly because the room is full.");
	}
	
	void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		Debug.Log("Could not connect to server: " + cause.ToString());
	}
	
	void OnDisconnectedFromPhoton()
	{
		Debug.Log("Disconnected from Photon server.");
	}
	
	void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{
		Debug.Log(otherPlayer.name + " left the room.");
	}
	
	// Update player list on GUI
	private void UpdateGUIPlayerList()
	{
		string playerNames = "";
		foreach (PhotonPlayer player in PhotonNetwork.playerList)
		{
			playerNames += player.name + '\n';
		}
		GUIplayerList.text = playerNames;
	}
}