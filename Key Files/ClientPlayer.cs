using UnityEngine;
using System.Collections;

class ClientPlayer : Player
{	
	// Use this for initialization
	protected override void basicStart () 
	{
		isMyTurn = false;
	}
}