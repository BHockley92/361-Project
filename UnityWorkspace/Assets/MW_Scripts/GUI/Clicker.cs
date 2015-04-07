using UnityEngine;
using System.Collections;

// only attach this to >local player's< units/villages
public class Clicker : MonoBehaviour
{	
	public GUILogic guiLogic;
	
	private Vector3 arrow_v = new Vector3(0.0f,0.5f,0.0f);
	
	void Start () {
		guiLogic = GameObject.Find("GUILogic").GetComponent<GUILogic>();
	}
	
	// YOU NEED TO INCLUDE A COLLIDER ON THE
	// GAME OBJECT FOR THIS TO WORK
	void OnMouseOver(){
		if(Input.GetMouseButtonDown(0)){
			// destroy previous "selected" effect
			Destroy(GameObject.Find("SelectionArrow(Clone)"));
			GameObject sa = Instantiate(Resources.Load("SelectionArrow")) as GameObject;
			sa.transform.position = this.transform.position + arrow_v;
			guiLogic.LAST_CLICKED_ON = this.transform;

		}
	}
}

