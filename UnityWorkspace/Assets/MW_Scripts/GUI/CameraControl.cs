﻿using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public float SCROLL_SPEED = 15;
	public float SCROLL_EDGE = 0.1f;
	
	public float PAN_SPEED = 10;
	
	public Vector2 ZOOM_RANGE = new Vector2( -10, 100 );
	public float CUR_ZOOM = 0;
	public float ZOOM_Z_SPEED = 1;
	public float ZOOM_ROT = 1;
	public Vector2 ZOOM_ANGLE_RANGE = new Vector2( 20, 70 );
	
	public float ROT_SPEED = 10;
	
	private Vector3 INIT_POS;

	public GameObject TARGET;

	void Start () {
		INIT_POS = transform.position;      
	}
	
	
	void Update () {

		//Updates TARGET to whatever the user clicked on
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray,out hit,100)) {
				//Only update to useful things
				if(hit.transform.gameObject.name.Contains("unit") || hit.transform.gameObject.name.Contains("building")) {
					TARGET = hit.transform.gameObject;
				}
			}
		}
		// panning     
		if ( Input.GetMouseButton( 0 ) ) {
			transform.Translate(Vector3.right * Time.deltaTime * PAN_SPEED * (Input.mousePosition.x - Screen.width * 0.5f) / (Screen.width * 0.5f), Space.World);
			transform.Translate(Vector3.forward * Time.deltaTime * PAN_SPEED * (Input.mousePosition.y - Screen.height * 0.5f) / (Screen.height * 0.5f), Space.World);
		}
		
		else {
			if ( Input.GetKey("d") ) {             
				transform.Translate(Vector3.right * Time.deltaTime * PAN_SPEED, Space.Self );   
			}
			else if ( Input.GetKey("a") ) {            
				transform.Translate(Vector3.right * Time.deltaTime * -PAN_SPEED, Space.Self );              
			}
			
			if ( Input.GetKey("w") || Input.mousePosition.y >= Screen.height * (1 - SCROLL_EDGE) ) {            
				transform.Translate(Vector3.forward * Time.deltaTime * PAN_SPEED, Space.Self );             
			}
			else if ( Input.GetKey("s") || Input.mousePosition.y <= Screen.height * SCROLL_EDGE ) {         
				transform.Translate(Vector3.forward * Time.deltaTime * -PAN_SPEED, Space.Self );            
			}  
		}
		
		// zoom in/out
		CUR_ZOOM -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 1000 * ZOOM_Z_SPEED;
		
		CUR_ZOOM = Mathf.Clamp( CUR_ZOOM, ZOOM_RANGE.x, ZOOM_RANGE.y );
		
		transform.position = new Vector3( transform.position.x, transform.position.y - (transform.position.y - (INIT_POS.y + CUR_ZOOM)) * 0.1f, transform.position.z );
	}
}
