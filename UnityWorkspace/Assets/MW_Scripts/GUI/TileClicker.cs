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
			if(guiLogic.LAST_CLICKED_ON.tag.Equals("Unit")) {
				GameObject.Find("GUILogic").GetComponent<GUILogic>().moveUnit(this.transform);
			}
			else if(guiLogic.LAST_CLICKED_ON.tag.Equals("Structure") && guiLogic.BUILD_TOWER) {
				GameObject.Find("GUILogic").GetComponent<GUILogic>().buildTower(this.transform);
			}
		}
	}
}

