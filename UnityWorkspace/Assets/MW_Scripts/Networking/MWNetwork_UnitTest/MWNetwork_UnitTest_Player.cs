using UnityEngine;
using System.Collections;

public class MWNetwork_UnitTest_Player : Photon.MonoBehaviour 
{
    MWNetwork network;

    void Start()
    {
        network = MWNetwork.getInstance();
    }

    void Update()
    {
        if (network.gameStarted() && network.isMyTurn() && photonView.isMine)
            InputMovement();
    }

    void InputMovement()
    {
        if (Input.GetKey(KeyCode.W))
            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + Vector3.forward * 10 * Time.deltaTime);

        if (Input.GetKey(KeyCode.S))
            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position - Vector3.forward * 10 * Time.deltaTime);

        if (Input.GetKey(KeyCode.D))
            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + Vector3.right * 10 * Time.deltaTime);

        if (Input.GetKey(KeyCode.A))
            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position - Vector3.right * 10 * Time.deltaTime);
    }

    void OnGUI()
    {
        if (network != null && network.gameStarted() && network.isMyTurn())
        {
            if (GUI.Button(new Rect(100, 100, 250, 100), "End Turn"))
            {
                network.endTurn();
            }
        }
    }
}
