using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerListTextGenerator : Photon.MonoBehaviour 
{
    private Text playerListUI;
    private MWNetwork network;

	// Use this for initialization
	void Start () 
    {
        playerListUI = GetComponent<Text>();
        network = MWNetwork.getInstance();
	}
	
	// Update is called once per frame
	void OnJoinedRoom () 
    {
        List<AbstractPlayer> players = network.getPlayers();
        string playerNames = "";

        foreach (AbstractPlayer player in players)
        {
            playerNames += player.username + '\n';
        }

        playerListUI.text = playerNames;
	}
}
