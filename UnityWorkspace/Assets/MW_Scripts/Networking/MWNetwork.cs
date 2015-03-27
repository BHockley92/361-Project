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
    GAME_ALREADY_STARTED,
    NOT_HOST,
    ROOM_NOT_CREATED,
    BAD_PLAYER_COUNT
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
     * USE THIS GOD DAMN FUNCTION TO INSTANTIATE ALL OBJECTS THAT NEED TO BE SERIALIZED OVER THE NETWORK.
     * Otherwise they will NOT appear on other players' machine.
     * Make sure a fucking PhotonView component is attached to the prefabs as well.
     */
    public GameObject instantiate(GameObject obj, Vector3 pos, Quaternion rot)
    {
        return (GameObject) PhotonNetwork.Instantiate(obj.name, pos, rot, 0);
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
    public MWNetworkResponse startGame()
	{
		// Make sure starting conditions are satisfied:
        // 1.   A room has been joined
        if (PhotonNetwork.room == null)
        {
            Debug.Log("Cannot start game: a room has not yet been created");
            return MWNetworkResponse.ROOM_NOT_CREATED;
        }
        // 2.   You are the host
        if (!PhotonNetwork.isMasterClient)
        {
            Debug.Log("Cannot start game: you are not the host.");
            return MWNetworkResponse.NOT_HOST;
        }
		// 3. 	Game is not already in play.
		if ((bool) PhotonNetwork.room.customProperties["gameStarted"])
		{
			Debug.Log("Cannot start game: it has already started.");
            return MWNetworkResponse.GAME_ALREADY_STARTED;
		}
		// 4.	2 <= number of players <= maximum players
		if (PhotonNetwork.playerList.Length < 2 
			|| PhotonNetwork.room.playerCount > PhotonNetwork.room.maxPlayers)
		{
            Debug.Log("Cannot start game: must be between 2 and " + PhotonNetwork.room.maxPlayers + " players.");
            return MWNetworkResponse.BAD_PLAYER_COUNT;
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
			mwPlayers.Add((AbstractPlayer)roomProps[player]);
        }
		
		return mwPlayers;
	}
	
	/*
	 * Call the function at the end of your turn.
	 * Lets other players know your turn has ended by updating their game state.
	 */
	public void turnEnded(string gameState)
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
	 * Called when "turnEnded" is called on another player.
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
    }
}