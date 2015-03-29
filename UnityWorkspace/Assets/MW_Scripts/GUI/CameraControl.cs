using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public bool disablePanning = false;
	public bool disableSelect = false;
	public bool disableZoom = false;
	
	public float maximumZoom = 1f;
	public float minimumZoom = 20f;
	public Color selectColor = Color.green;
	public float selectLineWidth = 2f;
	
	public float lookDamper = 5f;
	public string selectionObjectName = "RTS Selection";
	
	private readonly string[] INPUT_MOUSE_BUTTONS = {"Mouse Look", "Mouse Select"};
	private bool ready;
	private bool[] isDragging = new bool[2];
	private Vector3 selectStartPosition;
	private Texture2D pixel;
	private GameObject selection;

	void Start () {
		try {
			startupChecks();
			ready = true;
		} catch (UnityException exception) {
			ready = false;
			throw exception;
		}
		setPixel(selectColor);
	}

	private void startupChecks() {
		if (!Camera.main) {
			throw new MissingComponentException("RTS Camera must be attached to a camera.");
		}
		try {
			Input.GetAxis(INPUT_MOUSE_BUTTONS[0]);
			Input.GetAxis(INPUT_MOUSE_BUTTONS[1]);
		} catch (UnityException) {
			throw new UnassignedReferenceException("Inputs " + INPUT_MOUSE_BUTTONS[0] + " and " +
			                                       INPUT_MOUSE_BUTTONS[1] + " must be defined.");
		}
	}

	private void setPixel(Color color) {
		pixel = new Texture2D(1, 1);
		pixel.SetPixel(0, 0, color);
		pixel.Apply();
	}
	
	void Update () {	
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit)) {
				GameObject.Find("GUILogic").GetComponent<GUILogic>().LAST_CLICKED_ON = hit.transform;
			}
		}
		if (!ready) { return; }
		updateLook();
		updateZoom();
	}

	private bool isClicking(int index) {
		return Input.GetAxis(INPUT_MOUSE_BUTTONS[index]) == 1f;
	}


	private Vector2 getMouseMovement() {
		return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
	}

	private void updateZoom() {
		if (disableZoom) { return; }
		var newSize = Camera.main.orthographicSize - Input.GetAxis("Mouse ScrollWheel");
		newSize = Mathf.Clamp(newSize, maximumZoom, minimumZoom);
		Camera.main.orthographicSize = newSize;
	}

	private void updateLook() {
		if (!isDragging[0] || disablePanning) { return; }
		var newPosition = transform.position;
		var mousePosition = getMouseMovement();
		newPosition.x = newPosition.x - (mousePosition.x * Camera.main.orthographicSize / lookDamper);
		newPosition.y = newPosition.y - (mousePosition.y * Camera.main.orthographicSize / lookDamper);
		transform.position = newPosition;
	}
}
