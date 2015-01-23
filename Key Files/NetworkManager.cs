﻿using UnityEngine;
using System.Collections;

public class NetworkManager : Photon.MonoBehaviour 
{
	private const string roomName = "UserInputUniqueRoomName";
	private RoomInfo[] roomsList;

	public GameObject hostPrefab;
	public GameObject clientPrefab;
	
	// Use this for initialization
	void Start () {
		PhotonNetwork.ConnectUsingSettings("1.0");
	}

	void OnReceivedRoomListUpdate()
	{
		roomsList = PhotonNetwork.GetRoomList();
	}

	void OnJoinedRoom()
	{
		Debug.Log("Connected to Room.");

		// Spawn player
		if (PhotonNetwork.playerList.Length == 1)
			PhotonNetwork.Instantiate(hostPrefab.name, Vector3.up * 3, Quaternion.identity, 0);
		else
			PhotonNetwork.Instantiate(clientPrefab.name, Vector3.up * 3, Quaternion.identity, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
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
				PhotonNetwork.CreateRoom(roomName, new RoomOptions() {maxPlayers = 4}, null);
			
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
