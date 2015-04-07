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
			GUILayout.BeginArea(new Rect(370, 235, 650, 500));
			GUILayout.BeginScrollView(SCROLLPOS, false, true);
			GUILayout.BeginVertical(GUILayout.Width(625));
			SELECTION = GUILayout.SelectionGrid(SELECTION, GAMES, 1);
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}
	}

	public string getSelected() {
		return GAMES[SELECTION];
	}
}
