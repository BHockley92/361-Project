using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public float CamSpeed = 1.00f;
	public int GUIsize = 25;
	public bool enabled_camera = false;
	
	// magic numbers for now
	// but change to reflect map dimensions
	public int boardX = 20;
	public int boardY = 20;
	
	public int middleX = 20;
	public int middleY = -45;
	
	void Update () {
		Rect recdown = new Rect (0, 0, Screen.width, GUIsize);
		Rect recup = new Rect (0, Screen.height-GUIsize, Screen.width, GUIsize);
		Rect recleft = new Rect (0, 0, GUIsize, Screen.height);
		Rect recright = new Rect (Screen.width-GUIsize, 0, GUIsize, Screen.height);
		
		//Camera panning handlers
		if (enabled_camera && (recdown.Contains(Input.mousePosition) || Input.GetKey(KeyCode.DownArrow))) {
			transform.Translate(0, 0, -CamSpeed, Space.World);
		}
		if (enabled_camera && (recup.Contains(Input.mousePosition)|| Input.GetKey(KeyCode.UpArrow))) {
			transform.Translate(0, 0, CamSpeed, Space.World);
		}
		if (enabled_camera && (recleft.Contains(Input.mousePosition)|| Input.GetKey(KeyCode.LeftArrow))) {
			transform.Translate(-CamSpeed, 0, 0, Space.World);
		}
		if (enabled_camera && (recright.Contains(Input.mousePosition)|| Input.GetKey(KeyCode.RightArrow))) {
			transform.Translate(CamSpeed, 0, 0, Space.World);
		}
	}
}
