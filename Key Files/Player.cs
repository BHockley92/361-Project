using UnityEngine;
using System.Collections;

abstract class Player : Photon.MonoBehaviour 
{
	protected bool isMyTurn;
	protected float speed = 10f;

	// Use this for initialization
	void Start () 
	{
		basicStart();
	}
	protected abstract void basicStart();
	
	// Update is called once per frame
	void Update () 
	{
		if (photonView.isMine && isMyTurn)
		{
			InputMovement();
		}
	}

	protected void InputMovement()
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
			Debug.Log("Pressed space");
			isMyTurn = false;
			photonView.RPC("endTurn", PhotonTargets.MasterClient, PhotonNetwork.player);
		}
	}

	[RPC] protected void startTurn()
	{
		isMyTurn = true;
	}
}
