using UnityEngine;
using UnityEngine.EventSystems;

// PointerEventData.pointerPressRaycast example

// A cube which can be moved by the mouse or track bar at runtime.
// Up/down and left/right are supported.
//
// Create a 3D project and add a Cube. Position the Cube at the origin. Add a
// PhysicsRaycaster component to the Main Camera. Also, add an Empty GameObject
// to the Hierarchy. Apply an EventSystem component and a StandaloneInputModule
// component to this Empty GameObject. Next create this script and assign it to
// the Cube. Now, run the Game. The Cube can be moved up/down and left/right.

public class Example : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	private Vector3 newPosition;
	private Vector3 delta;
	private Vector3 startPosition;

	void Awake() {
		// Move the Cube away from the origin.
		startPosition = Vector3.zero;
		transform.position = startPosition;

		newPosition = Vector3.zero;
		delta = Vector2.zero;
	}

	public void OnBeginDrag(PointerEventData eventData) {
		// Obtain the position of the hit GameObject.
		delta = eventData.pointerPressRaycast.worldPosition;
		delta -= transform.position;
	}

	public void OnDrag(PointerEventData eventData) {
		newPosition = eventData.pointerCurrentRaycast.worldPosition - delta;
		transform.position = newPosition;
	}

	public void OnEndDrag(PointerEventData eventData) {
		transform.position = startPosition;
	}
}