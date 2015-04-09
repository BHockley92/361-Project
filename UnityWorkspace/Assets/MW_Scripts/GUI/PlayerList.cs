using UnityEngine;
using System.Collections;

public class PlayerList : MonoBehaviour {
	//Default to first in list
	public string[] PLAYERS;
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
			GUILayout.BeginArea(new Rect(5, 50, 815, 224));
			GUILayout.BeginVertical(GUILayout.Width(815));
			foreach(string current_name in PLAYERS) {
				GUILayout.Button(current_name, GUILayout.Height(35));
				GUILayout.Space(10);
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}
}
