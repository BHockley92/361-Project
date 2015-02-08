using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkPlayer : Photon.MonoBehaviour
{
	private float speed = 10f;
	private Hashtable properties = new Hashtable();
	
	// Use this for initialization
	void Start () 
	{
		if (photonView.isMine)
		{
			properties.Add("isMyTurn", false);
			PhotonNetwork.player.SetCustomProperties(properties);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (photonView.isMine && (bool) PhotonNetwork.player.customProperties["isMyTurn"])
		{
			InputMovement();
		}
	}
	
	private void InputMovement()
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
			properties["isMyTurn"] = false;
			PhotonNetwork.player.SetCustomProperties(properties);
		}
	}
}