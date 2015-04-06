using UnityEngine;
using System.Collections;

public class Clicker : MonoBehaviour
{	
	public GUILogic guiLogic;
	
	void Start () {
		guiLogic = GameObject.Find("GUILogic").GetComponent<GUILogic>();
	}
	
	// YOU NEED TO INCLUDE A COLLIDER ON THE
	// GAME OBJECT FOR THIS TO WORK
	void OnMouseOver(){
		if(Input.GetMouseButtonDown(0)){
			guiLogic.LAST_CLICKED_ON = this.transform;
		}
	}
}

