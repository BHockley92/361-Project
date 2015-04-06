using UnityEngine;
using System.Collections;

public class TileClicker : MonoBehaviour
{
	public GUILogic guiLogic;
	
	void Start () {
		guiLogic = GameObject.Find("GUILogic").GetComponent<GUILogic>();
	}
	
	// YOU NEED TO INCLUDE A COLLIDER ON THE
	// GAME OBJECT FOR THIS TO WORK
	void OnMouseOver(){
		if(Input.GetMouseButtonDown(1)){
			Debug.Log (guiLogic.LAST_CLICKED_ON.tag);
			if(guiLogic.LAST_CLICKED_ON.tag.Equals("Unit")) {
				GameObject.Find("GUILogic").GetComponent<GUILogic>().moveUnit(this.transform);
			}
				
		}
	}
}

