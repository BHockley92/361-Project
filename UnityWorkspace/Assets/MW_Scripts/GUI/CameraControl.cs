using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public float CamSpeed = 1.00f;
	public int GUIsize = 25;
	public bool enabled = false;
	
	void Update () {
		Rect recdown = new Rect (0, 0, Screen.width, GUIsize);
		Rect recup = new Rect (0, Screen.height-GUIsize, Screen.width, GUIsize);
		Rect recleft = new Rect (0, 0, GUIsize, Screen.height);
		Rect recright = new Rect (Screen.width-GUIsize, 0, GUIsize, Screen.height);

		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit)) {
				GameObject.Find("GUILogic").GetComponent<GUILogic>().LAST_CLICKED_ON = hit.transform;
			}
		}
		if (enabled && recdown.Contains(Input.mousePosition))
			transform.Translate(0, 0, -CamSpeed, Space.World);
		
		if (enabled && recup.Contains(Input.mousePosition))
			transform.Translate(0, 0, CamSpeed, Space.World);
		
		if (enabled && recleft.Contains(Input.mousePosition))
			transform.Translate(-CamSpeed, 0, 0, Space.World);
		
		if (enabled && recright.Contains(Input.mousePosition))
			transform.Translate(CamSpeed, 0, 0, Space.World);
	}
}
