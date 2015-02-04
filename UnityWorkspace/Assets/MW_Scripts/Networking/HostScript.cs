﻿using UnityEngine;
using System.Collections;

public class HostScript : Photon.MonoBehaviour 
{
	public float speed = 10f;
	volatile bool isMyTurn = true;

	// Use this for initialization
	void Start () 
	{
		isMyTurn = true;
	}
	
	// Update is called once per frame
	void Update()
	{
		if (photonView.isMine && isMyTurn)
		{
			InputMovement();
		}
	}
	
	void InputMovement()
	{
		if (Input.GetKey(KeyCode.W))
			rigidbody.MovePosition(rigidbody.position + Vector3.forward * speed * Time.deltaTime);
		
		if (Input.GetKey(KeyCode.S))
			rigidbody.MovePosition(rigidbody.position - Vector3.forward * speed * Time.deltaTime);
		
		if (Input.GetKey(KeyCode.D))
			rigidbody.MovePosition(rigidbody.position + Vector3.right * speed * Time.deltaTime);
		
		if (Input.GetKey(KeyCode.A))
			rigidbody.MovePosition(rigidbody.position - Vector3.right * speed * Time.deltaTime);
		
		if (Input.GetKey(KeyCode.Space))
		{
			photonView.RPC("nextTurn", PhotonTargets.All, PhotonNetwork.player.ID);
			isMyTurn = false;
			Debug.Log ("Space Pressed: Is it my turn? " + isMyTurn);
		}
	}

	[RPC] void nextTurn(int id)
	{
		isMyTurn = true;
		Debug.Log("Turn start");
		if(photonView.isMine)
		{
			Debug.Log("Still mine.");
		}
		else{ Debug.Log("not mine");}
	}
}
