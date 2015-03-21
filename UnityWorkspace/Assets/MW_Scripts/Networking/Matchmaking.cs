using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


/// <summary>
/// This script automatically connects to Photon (using the settings file), 
/// tries to join a random room and creates one if none was found (which is ok).
/// </summary>
public class Matchmaking : Photon.MonoBehaviour
{
	//public CanvasGroup menu;
	
	public GameObject lobbyPlayersTextGO;
	public GUILogic GUILogic;
	
	private byte version = 1;
	
	private bool host = false;
	
	public bool error = false;
	
	public bool inRoom = false;
	
	public void Connect() {
		//TODO: set the player name from login credentials
		PhotonNetwork.playerName = GUILogic.PLAYER.username;
		PhotonNetwork.ConnectUsingSettings(""+version);
		Debug.Log ("Connected.");
	}
	
	public void JoinGame () {
		Connect();
	}
	
	public void HostGame() {
		host = true;
		Connect();
	}
	
	public void LeaveRoom(){
		PhotonNetwork.LeaveRoom();
		PhotonNetwork.LeaveLobby();
	}
	
	public IList<String> ListPlayers() {
		PhotonPlayer[] players = PhotonNetwork.playerList;
		List<String> pnames = new List<String>();
		foreach(PhotonPlayer p in players) {
			pnames.Add(p.name);
		}
		return pnames;
	}
	
	// to react to events "connected" and (expected) error "failed to join random room", we implement some methods. PhotonNetworkingMessage lists all available methods!
	
	public virtual void OnConnectedToMaster()
	{
	}
	
	public virtual void OnPhotonRandomJoinFailed()
	{
		Debug.Log ("Tried to connect to a room but no rooms!");
	}
	
	// the following methods are implemented to give you some context. re-implement them as needed.
	
	public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		Debug.LogError("Cause: " + cause);
	}
	
	public void OnJoinedRoom()
	{
		inRoom = true;
	}
	
	public void OnJoinedLobby()
	{
		if(host) {
		//TODO: better room settings
			PhotonNetwork.CreateRoom("DemoRoom", new RoomOptions() { maxPlayers = 4 }, null);
		}
		else {
//			RoomInfo[] rooms = PhotonNetwork.GetRoomList();
			//TODO: get room info and display it in gui
			
			//TODO: this is a temp thing for the demo: join any room
			PhotonNetwork.JoinRandomRoom();
		}
		// put your own name on the player list
		OnPhotonPlayerConnected(null);
	}
	
	public void OnPhotonPlayerConnected(PhotonPlayer newPlayer) {
		Debug.Log ("Player amount in room changed.");
		UnityEngine.UI.Text text = lobbyPlayersTextGO.GetComponentInChildren<UnityEngine.UI.Text>();
		string players = "";
		foreach(string s in ListPlayers())
			players += s + "\n";
		text.text = players;
	}
	
	public void OnPhotonPlayerDisconnected() {
		OnPhotonPlayerConnected(null);
	}
}
