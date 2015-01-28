using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : Photon.MonoBehaviour 
{
	private const string roomName = "KBH_Room";
	private RoomInfo[] roomsList;

	public GameObject host;
	public GameObject player;

	// Use this for initialization
	void Start () 
	{
		PhotonNetwork.ConnectUsingSettings("1.0");
		Debug.Log("Connected to master server!");
	}

	// Update is called once per frame
	void Update () {

	}

	void OnReceivedRoomListUpdate()
	{
		roomsList = PhotonNetwork.GetRoomList();
	}

	void OnJoinedRoom()
	{
		if (PhotonNetwork.playerList.Length == 1)
			PhotonNetwork.Instantiate(host.name, Vector3.up, Quaternion.identity, 0);
		else
			PhotonNetwork.Instantiate(player.name, Vector3.up, Quaternion.identity, 0);
	}

	// For changing turns
	void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
	{
		PhotonPlayer[] players 	= PhotonNetwork.playerList;
		PhotonPlayer player 	= playerAndUpdatedProps[0] as PhotonPlayer;
		Hashtable props 		= playerAndUpdatedProps[1] as Hashtable;

		if (!((bool) props["isMyTurn"]))
		{
			for (int i = 0; i < players.Length; i++)
			{
				if (player.Equals(players[i]))
				{
					player = players[++i % players.Length];
					props["isMyTurn"] = true;
					player.SetCustomProperties(props);
				}
			}
		}
	}

	void OnGUI()
	{
		if (!PhotonNetwork.connected)
		{
			GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
		}
		else if (PhotonNetwork.room == null)
		{
			// Create Room
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
			{
				PhotonNetwork.CreateRoom(roomName, new RoomOptions() {maxPlayers = 4}, null);
			}
			
			// Join Room
			if (roomsList != null)
			{
				for (int i = 0; i < roomsList.Length; i++)
				{
					if (GUI.Button(new Rect(100, 250 + (110 * i), 250, 100), "Join " + roomsList[i].name))
						PhotonNetwork.JoinRoom(roomsList[i].name);
				}
			}
		}
	}

}
