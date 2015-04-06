using UnityEngine;
using System.Collections;

public class GameSelect : MonoBehaviour {
	//Default to first in list
	public string[] GAMES;
	private Vector2 ListScrollPos;
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
			GUILayout.BeginArea(new Rect(370, 231.5f, 655, 509.4f));
			GUILayout.BeginScrollView(ListScrollPos, false, true);
			GUILayout.BeginVertical(GUILayout.Width(625));
			foreach(string current in GAMES)
			{
				GUILayout.Button(current, GUILayout.Height (40));
				GUILayout.Space (5);
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}
	}
}
