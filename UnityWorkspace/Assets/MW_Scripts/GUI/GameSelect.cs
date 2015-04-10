using UnityEngine;
using System.Collections;

public class GameSelect : MonoBehaviour {
	//Default to first in list
	public string[] GAMES;
	private Vector2 SCROLLPOS;
	private int SELECTION = 0;
	// Use this for initialization
	void Start () {
	
	}
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI() {
		CanvasGroup show_menu = this.transform.parent.GetComponentInParent<CanvasGroup>();
		//Only display this menu if we're showing it
		if(show_menu.alpha == 1 && show_menu.interactable && show_menu.blocksRaycasts) {
			GUILayout.BeginArea(new Rect(Screen.width*0.2875f, Screen.height*0.2875f, Screen.width*0.425f, Screen.height*0.41f));
			GUILayout.BeginScrollView(SCROLLPOS, false, true);
			GUILayout.BeginVertical(GUILayout.Width(625));
			SELECTION = GUILayout.SelectionGrid(SELECTION, GAMES, 1);
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}
	}

	public string getSelected() {
		if(GAMES.Length > 0) {
			return GAMES[SELECTION];
		}
		else {
			return null;
		}
	}
}
