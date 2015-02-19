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
	private static String 	version;
	private ArrayList		players;
	private bool 			gameStarted;

	// The constructor is private, as this is a utility class.
	private MWNetwork() 
	{
		version 		= "1.0";	// TODO verify if valid version number
		players			= new ArrayList;
		gameStarted 	= false;
	};
	
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
	public static void createRoom(String roomName)
	{
		PhotonNetwork.CreateRoom(roomName, new RoomOptions() {maxPlayers = 2}, null);
		
		Hashtable roomProps = new Hashtable();
		roomProps.Add("gameStarted", false);
		PhotonNetwork.room.SetCustomProperties(roomProps);
	}
	
	/*
	 * Joins a room on the network with the given name.
	 */
	public static void joinRoom(String roomName)
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
		if (PhotonNetwork.playerList.length < 2 
			|| PhotonNetwork.playerList.length > PhotonNetwork.room.maxPlayers)
		{
			Debug.Log("Cannot start game: must be between 2 and " + maxPlayers + " players.");
			return;
		}
		
		// Make the room impossible to see or join
		PhotonNetwork.room.visible = false;
		PhotonNetwork.room.open = false;
		
		// Set player turns
		Hashtable isMyTurn = new Hashtable();
		for (PhotonPlayer player in PhotonNetwork.playerList)
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
		roomProps.Add("gameStarted", false);
		PhotonNetwork.room.SetCustomProperties(roomProps);
	}
	
	/*
	 * Marks you as having started your turn.
	 */
	public static void beginTurn()
	{
		property = new Hashtable();
		property.Add("isMyTurn") = true;
		PhotonNetwork.player.SetCustomProperties(property);
	}

	/*
	 * Ends your turn, starting the next player's turn.
	 */
	public static void endTurn()
	{
		// End your turn
		property = new Hashtable();
		property.Add("isMyTurn") = false;
		PhotonNetwork.player.SetCustomProperties(property);
		
		// Start next player's turn.
		// TODO make sure there are no race conditions for property.
		// TODO check if game over before setting beginning of turn?
		property["isMyTurn"] = true;	
		GetNextFor(PhotonNetwork.player).SetCustomProperties(property);
	}
	
	/*
	 * Displays the room list for a lobby at the given position (x,y).
	 */
	public static void displayRoomList(int x, int y)
	{
		roomsList = PhotonNetwork.GetRoomList();
		
		if (roomsList != null)
			{
				foreach (RoomInfo room in roomsList)
				{
					if (GUI.Button(new Rect(x, y + (110 * i), 250, 100), roomsList.name))
						PhotonNetwork.JoinRoom(roomsList.name);
				}
			}
	}
	
	/*
	 * 
	 */
	void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
	{
		// Retrieve player and properties from the passed parameter
		PhotonPlayer 	player 	= playerAndUpdatedProps[0] as PhotonPlayer;
		Hashtable 		props 	= playerAndUpdatedProps[1] as Hashtable;
		
		// 
	}
	
	/*
	 * This function announces to other players that your game state has changed.
	 */
	public void gameStateChanged()
	{
		photonView.RPC("updateGameState", PhotonTargets.Others, gameState);
	}
	
	/*
	 * Updates the state of the game with the one provided.
	 */
	[RPC]
	private void updateGameState(GameState gameState)
	{
		
	}
}