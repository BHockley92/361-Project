using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

// TODO Chat?
// TODO Matchmaker

/*
 * This is a utility class for networking within the Medieval Warfare game.
 */
public class MWNetwork : Photon.MonoBehaviour
{
    // The game version.  Will probably never change this.
    private static string version = "1.0";

    public GUILogic GUIplayer;

    /* 
     * Returns the MWNetwork component upon which you can call all functions below.
     */
    public static MWNetwork getInstance()
    {
        return GameObject.Find("MWNetwork").GetComponent<MWNetwork>();
    }

	/* 
	 * Connects to Photon's master server using current version number.
	 */
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(version);
        Debug.Log("Connected to master server!");
    }
	
	/*
	 * Sets a maximum number of players that can join a room/game.
	 */
    public void setMaxPlayers(int max)
	{
		PhotonNetwork.room.maxPlayers = max;
	}
	
	/*
	 * Returns whether it is this player's turn or not.
	 */
	public bool isMyTurn()
	{
		return (bool) PhotonNetwork.player.customProperties["isMyTurn"];
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
	 */
	public void startGame()
	{
		// Make sure starting conditions are satisfied:
        // 1.   A room has been joined
        if (PhotonNetwork.room == null)
        {
            Debug.Log("Cannot start game: a room has not yet been created");
        }
        // 2.   You are the host
        if (!PhotonNetwork.isMasterClient)
        {
            Debug.Log("Cannot start game: you are not the host.");
        }
		// 2. 	Game is not already in play.
		if ((bool) PhotonNetwork.room.customProperties["gameStarted"])
		{
			Debug.Log("Cannot start game: it has already started.");
			return;
		}
		// 3.	2 <= number of players <= maximum players
		if (PhotonNetwork.playerList.Length < 2 
			|| PhotonNetwork.playerList.Length > PhotonNetwork.room.maxPlayers)
		{
            Debug.Log("Cannot start game: must be between 2 and " + PhotonNetwork.room.maxPlayers + " players.");		// TODO remove if photon checks this already
			return;
		}
		
		// Make the room impossible to see or join
		PhotonNetwork.room.visible = false;
		PhotonNetwork.room.open = false;
		
		// Set player turns (the host is currently automatically player 1)
		Hashtable isMyTurn = new Hashtable();
		foreach (PhotonPlayer player in PhotonNetwork.playerList)
		{
			if (player.isMasterClient)
			{
				isMyTurn["isMyTurn"] = true;
			}
			else {
				isMyTurn["isMyTurn"] = false;
			}
			
			player.SetCustomProperties(isMyTurn);
		}
		
		// Mark the game as having started
		Hashtable roomProps = new Hashtable();
		roomProps.Add("gameStarted", true);
		PhotonNetwork.room.SetCustomProperties(roomProps);
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
    public bool isHost()
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
	 * Ends your turn, starting the next player's turn.
     * TODO change order in which players play?
	 */
	public void endTurn()
	{
		// End your turn
		Hashtable property = new Hashtable();
		property.Add("isMyTurn", false);
		PhotonNetwork.player.SetCustomProperties(property);
		
		// Start next player's turn.
		// TODO make sure there are no race conditions for property.
		property["isMyTurn"] = true;
		PhotonPlayer[] players = PhotonNetwork.playerList;
        PhotonPlayer nextPlayer = players[(System.Array.IndexOf(players, PhotonNetwork.player) + 1) % players.Length];
        nextPlayer.SetCustomProperties(property);
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
	 * Returns an array of player names.
	 */
	public List<AbstractPlayer> getPlayers()
	{
		PhotonPlayer[] players = PhotonNetwork.playerList;
        Hashtable roomProps = PhotonNetwork.room.customProperties;

        List<AbstractPlayer> mw_players = new List<AbstractPlayer>();

		foreach (PhotonPlayer player in players)
        {
            mw_players.Add( (AbstractPlayer)roomProps[player] );
        }
		
		return mw_players;
	}
	
	/*
	 * This function updates the game state on the other players' machines
	 */
    public void gameStateUpdated()
    {
        //LoadBalancingClient.Service();

        /*
         * As it is for the demo, the game state should update automatically.
         */ 
    }

    /*****************************************************************************************************
     * The following functions implement pertinent Photon callbacks to react to certain networking events.
     *****************************************************************************************************/

    void OnJoinedRoom()
    {
        Debug.Log("Connected to Room.");

        // Instantiate a MW_Player game object
        if (GUIplayer.PLAYER != null)
        {
            MW_Player newPlayer = GUIplayer.PLAYER;

            // Map network player to respective MW_Player and add to room properties
            Hashtable newPlayerMap = new Hashtable();
            newPlayerMap.Add(PhotonNetwork.player, newPlayer);
            PhotonNetwork.room.SetCustomProperties(newPlayerMap);
        }
        else
        {
            Debug.Log("Joined room before MW_Player was instantiated in GUILogic.  Player was therefore not added to the room properties.");
        }
    }

    void OnCreatedRoom()
    {
        Hashtable roomProps = new Hashtable();
        roomProps.Add("gameStarted", false);
        PhotonNetwork.room.SetCustomProperties(roomProps);
    }
}