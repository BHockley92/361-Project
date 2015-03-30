using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

// TODO Chat?
// TODO Matchmaker

// TODO Call Authenticate upon login.
// TODO Set player turns. How? Should MW_Player implement the Equals() method?
// TODO Make sure ONLY host spawns map via the instantiate method below

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
	// Event codes
	private const byte UPDATED_GAME_STATE = 1;

	// Useful references to GUI elements
	public GUILogic gui;
	public Text GUIplayerList;
    
	private static MWNetwork instance;
	private static string version = "1.0";    // The game version.  Will probably never change this.

	/* 
	 * Connects to Photon's master server using current version number.
	 */
    void Start()
    {
    	instance = this;
    
        PhotonNetwork.ConnectUsingSettings(version);
        Debug.Log("Connected to master server!");
        
        PhotonNetwork.OnEventCall += this.OnGameStateReceived;
    }

    /* 
     * Returns the MWNetwork component upon which you can call all functions below.
     */
    public static MWNetwork getInstance()
    {
		return instance;
	}
	
	/*
	 * Attempts to authenticate the player.
	 * Returns whether or not the player autheticated successfully
	 * TODO return an error code?
	 */
	public bool Authenticate(string username, string password)
	{
		// TODO Check username/password in database (playfab?)
		PhotonNetwork.playerName = gui.PLAYER.username;
		
		return true;
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
		// Ready check
		Hashtable readyCheck = new Hashtable();
		readyCheck.Add("ready", true);
		PhotonNetwork.SetPlayerCustomProperties(readyCheck);
	
		// If not host, do nothing else
		if (!PhotonNetwork.isMasterClient)
		{
			return MWNetworkResponse.READY_CHECKED;
		}
		
		// If host, make sure all players are ready
		foreach (PhotonPlayer player in PhotonNetwork.playerList)
		{
			if ((bool)player.customProperties["ready"] == false)
			{
				return MWNetworkResponse.PLAYERS_NOT_READY;
			}
		}
		
		// Make the room impossible to see or join
		PhotonNetwork.room.visible = false;
		PhotonNetwork.room.open = false;
		
		// Mark the game as having started
		Hashtable roomProps = new Hashtable();
		roomProps.Add("gameStarted", true);
		PhotonNetwork.room.SetCustomProperties(roomProps);

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
	 * FIXME Associate MW_Player to a network player.
	 */
	public List<AbstractPlayer> getPlayers()
	{
		PhotonPlayer[] players = PhotonNetwork.playerList;
        Hashtable roomProps = PhotonNetwork.room.customProperties;

        List<AbstractPlayer> mwPlayers = new List<AbstractPlayer>();

		foreach (PhotonPlayer player in players)
		{
			MW_Player mwPlayer = new MW_Player();
			mwPlayer.setAttribute(player.name);
			
			mwPlayers.Add(mwPlayer);
		}
		
		return mwPlayers;
	}
	
	/*
	 * This function shares your game state to other machines.
	 * These machines will in turn update their game state.
	 */
	public void ShareGameState(string gameState)
	{
		if (!PhotonNetwork.RaiseEvent(UPDATED_GAME_STATE,
								      gameState,
								      true,
								      new RaiseEventOptions() { Receivers = ExitGames.Client.Photon.Lite.ReceiverGroup.Others }))
		{
			Debug.Log("Error sending game state over network.");
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
			gui.UpdateGameState((string)content, senderId);
		}
	}

    /*****************************************************************************************************
     * The following functions implement pertinent Photon callbacks to react to certain networking events.
     *****************************************************************************************************/

    void OnJoinedRoom()
    {
    	// Set player custom properties
    	Hashtable readyCheck = new Hashtable();
    	readyCheck.Add("ready", false);
    	PhotonNetwork.SetPlayerCustomProperties(readyCheck);
    
        // TODO Find a way to get MW_Player synced over network.
        if (gui.PLAYER != null)
        {            
			// Update player list on GUI
			string playerNames = "";
			foreach (PhotonPlayer player in PhotonNetwork.playerList)
			{
				playerNames += player.name + '\n';
			}
			GUIplayerList.text = playerNames;
        }
        else { // FIXME this should probably be an exception but will probably remove it eventually anyways...
            Debug.Log("Joined room before MW_Player was instantiated in GUILogic.\n"
                      + "Player was therefore not added to the room properties.\n"
                      + "Probable cause: MW_Player not instantiated.");
        }
    }

    void OnCreatedRoom()
    {
        Hashtable roomProps = new Hashtable();
        roomProps.Add("gameStarted", false);
        PhotonNetwork.room.SetCustomProperties(roomProps);
		Debug.Log ("room created");
    }
}