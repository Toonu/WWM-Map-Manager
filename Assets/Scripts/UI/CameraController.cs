using System;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public float speed = 20;		//Camera speed
	internal Camera cam;			//Current camera
	public GameObject map;			//Game map
	public float maxZoom = 5;		//Maximal zoom level
	public float minZoom = 20;		//Minimal zoom level
	public float sensitivity = 1;	//Zoom and movement sensitivity
	private Vector3 mapMin;			//Map bottom left corner
	private Vector3 mapMax;			//Map top right corner

	/// <summary>
	/// Method gets the camera component at startup.
	/// </summary>
	private void Awake() {
		cam = GetComponent<Camera>();
	}

	/// <summary>
	/// Method loads map from files and sets the program boundaries resolution.
	/// </summary>
	private void Start() {
		Texture2D loadedMap = new(2, 2);
		byte[] bytes = System.IO.File.ReadAllBytes(Application.dataPath + "/Map.png");
		loadedMap.LoadImage(bytes);
		//Creating new Image sprite from the loaded map and getting its bounds.
		Sprite newSprite = Sprite.Create(loadedMap, new Rect(0, 0, loadedMap.width, loadedMap.height), new Vector2(0.5f, 0.5f));
		map.GetComponent<SpriteRenderer>().sprite = newSprite;
		mapMin = map.GetComponent<SpriteRenderer>().bounds.min;
		mapMax = map.GetComponent<SpriteRenderer>().bounds.max;
	}

	/// <summary>
	/// Method updates camera position and zoom.
	/// </summary>
	void LateUpdate() {
		//mouseScrollDelta goes from -1 to 1.
		if (Input.mouseScrollDelta.y != 0) {
			ZoomCamera(Input.mouseScrollDelta.y);
		}

		//Moving camera when holding MMB
		if (Input.GetMouseButton(2)) {
			//Calculating the movement amount.
			float specificSpeed = speed * Time.deltaTime * cam.orthographicSize;
			Vector3 newPosition = transform.position + new Vector3(-Input.GetAxis("Mouse X") * specificSpeed, -Input.GetAxis("Mouse Y") * specificSpeed, 0);
			MoveCamera(newPosition);
		}

		//Moving camera when holding arrow keys
		if (Input.GetKey(KeyCode.UpArrow) ||
			Input.GetKey(KeyCode.DownArrow) ||
			Input.GetKey(KeyCode.LeftArrow) ||
			Input.GetKey(KeyCode.RightArrow))
			MoveCamera();
	}

	/// <summary>
	/// Method moves the camera and clamps it to bounds.
	/// </summary>
	/// <param name="newPosition">New position.</param>
	private void MoveCamera(Vector3 newPosition) {
		//Moving camera
		newPosition.x = Mathf.Clamp(newPosition.x, mapMin.x, mapMax.x);
		newPosition.y = Mathf.Clamp(newPosition.y, mapMin.y, mapMax.y);
		transform.position = newPosition;
	}

	/// <summary>
	/// Method moves the camera by key.
	/// </summary>
	private void MoveCamera() {
		//Calculating the movement amount.
		float specificSpeed = speed * Time.deltaTime * cam.orthographicSize / 30;
		Vector3 newPosition = transform.position +
			new Vector3((Convert.ToInt32(Input.GetKey(KeyCode.RightArrow))
			- Convert.ToInt32(Input.GetKey(KeyCode.LeftArrow))) * specificSpeed,
			(Convert.ToInt32(Input.GetKey(KeyCode.UpArrow))
			- Convert.ToInt32(Input.GetKey(KeyCode.DownArrow))) * specificSpeed, 0);
		MoveCamera(newPosition);
	}


	/// <summary>
	/// Method zooms the camera and clamps it to bounds.
	/// </summary>
	/// <param name="scrollDeltaY">Amount of scrolling.</param>
	private void ZoomCamera(float scrollDeltaY) {
		//Calculating the zoom amount.
		float newSize = cam.orthographicSize - scrollDeltaY * sensitivity;
		newSize = Mathf.Clamp(newSize, minZoom, maxZoom);
		//Adjusting Unit size when zooming.
		if (cam.orthographicSize > 1.5 && UnitManager.Instance.higherEchelons.Count == 0) {
			UnitManager.GroupUnits();
		} else if (cam.orthographicSize <= 1.5 && UnitManager.Instance.higherEchelons.Count > 0) {
			UnitManager.Instance.UnGroupUnits();
		}
		cam.orthographicSize = newSize;
	}
}
