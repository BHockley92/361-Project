using UnityEngine;
using System.Collections;

public class InGameMenu : MonoBehaviour {

	public CanvasGroup menu;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.Escape)) {
			menu.alpha = 1;
			menu.blocksRaycasts = true;
			menu.interactable = true;
		}
		
	}
}
