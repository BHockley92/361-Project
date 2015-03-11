using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

// TODO Chat?
// TODO Matchmaker

/*
 * This is a utility class for networking within the Medieval Warfare game.
 */
public class MWNetwork
{
	private static string 	version;

	// The constructor is private, as this is a utility class.
	private MWNetwork() 
	{
		version 		= "1.0";	// TODO verify if valid version number
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
	public static bool isMyTurn()
	{
		return (bool) PhotonNetwork.player.customProperties["isMyTurn"];
	}
	
	/*
	 * Creates a room on the network with the given name.
	 */
	public static void createRoom(string roomName)
	{
		PhotonNetwork.CreateRoom(roomName, new RoomOptions() {maxPlayers = 2}, null);
		
		Hashtable roomProps = new Hashtable();
		roomProps.Add("gameStarted", false);
		PhotonNetwork.room.SetCustomProperties(roomProps);
	}
	
	/*
	 * Joins a room on the network with the given name.
	 */
	public static void joinRoom(string roomName)
	{
		PhotonNetwork.JoinRoom(roomName);
	}
	
	/*
	 * Makes the player join a room.
	 * Initializes all player properties for the beginning of a game.
	 * Currently, master client is automatically player 1 and other players turns
	 * are ordered according to when they joined.
	 */
	public static void startGame()
	{
		// Make sure starting conditions are satisfied:
		// 1.	Game is not already in play.
		if ((bool) PhotonNetwork.room.customProperties["gameStarted"])
		{
			Debug.Log("Cannot start game: it has already started.");
			return;
		}
		// 2.	2 <= number of players <= maximum players
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
	 * Ends your turn, starting the next player's turn.
	 */
	public static void endTurn()
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
	public static string[] getRoomList()
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
	public static string[] getPlayerList()
	{
		PhotonPlayer[] players = PhotonNetwork.playerList;
		
		string[] playerNames = new string[players.Length];
		
		for (int i = 0; i < players.Length; i++)
		{
			playerNames[i] = players[i].name;
		}
		
		return playerNames;
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
}