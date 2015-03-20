using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	void Start () {

	}
	
	
	void Update () {	
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit)) {
				GameObject.Find("GUILogic").GetComponent<GUILogic>().LAST_CLICKED_ON = hit.transform;
			}
		}
	}
}
