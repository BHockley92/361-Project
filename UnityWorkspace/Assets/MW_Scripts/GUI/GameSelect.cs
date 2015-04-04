using UnityEngine;
using System.Collections;

public class GameSelect : MonoBehaviour {
	//Default to first in list
	public int START_SELECT = 0;
	public string[] GAMES = new string[] {"radio1", "radio2", "radio3"};

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		GUILayout.BeginVertical("Box");
		START_SELECT = GUILayout.SelectionGrid(START_SELECT, GAMES, 1);
		if (GUILayout.Button("Start"))
			Debug.Log("You chose " + GAMES[START_SELECT]);
		
		GUILayout.EndVertical();
	}
}
