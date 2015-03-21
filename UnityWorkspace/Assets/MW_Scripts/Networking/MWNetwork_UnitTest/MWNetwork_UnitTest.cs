using UnityEngine;
using System.Collections;

public class MWNetwork_UnitTest : Photon.MonoBehaviour 
{
    private MWNetwork network;
    public GameObject player;

    // Use this for initialization
    void Start()
    {
        network = MWNetwork.getInstance();
    }

    void OnGUI()
    {
        if (network != null && network.connected() )
        {
            if (!network.joinedRoom())
            {
                // Host a game
                if (GUI.Button(new Rect(10, 10, 150, 100), "Host Game"))
                {
                    network.hostRoom("Test Room");
                }

                // Join a game
                string[] rooms = network.getRooms();
                if (rooms.Length != 0)
                {
                    for (int i = 0; i < rooms.Length; i++)
                    {
                        if (GUI.Button(new Rect(100, 100, 250, 100), "Join " + rooms[i]))
                        {
                            network.joinRoom(rooms[i]);
                        }
                    }
                }
            }
            // This start game button is available to whoever is hosting the game
            else if (network.joinedRoom() && network.amHost() && !network.gameStarted())
            {
                if (GUI.Button(new Rect(100, 100, 250, 100), "Start Game"))
                {
                    network.startGame();
                }
            }
        }
    }
}
