using UnityEngine;
using System.Collections;

class HostPlayer : Player
{
	protected PhotonPlayer 	currentPlayer = PhotonNetwork.player;
	protected int			currentPlayerIndex = 0;
	
	// Use this for initialization
	protected override void basicStart () 
	{
		isMyTurn = true;
	}

	[RPC] void endTurn(PhotonPlayer playerEnded)
	{
		// Get next player
		currentPlayerIndex = (currentPlayerIndex++) % PhotonNetwork.playerList.Length;
		currentPlayer = PhotonNetwork.playerList[currentPlayerIndex];

		// Tell him it is his turn
		photonView.RPC("startTurn", currentPlayer, null);
	}
}