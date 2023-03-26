using System;
using UnityEngine;

namespace Assets.Scripts {
	public class CameraController : MonoBehaviour {
		public float speed = 20;
		public Camera cam;
		public GameObject map;
		public ApplicationController settings;
		public float maxZoom = 5;
		public float minZoom = 20;
		public float sensitivity = 1;
		private Vector3 mapMin;
		private Vector3 mapMax;

		private void Start() {
			Texture2D loadedMap = new Texture2D(2, 2);
			byte[] bytes = System.IO.File.ReadAllBytes("./Map.png");
			loadedMap.LoadImage(bytes);
			map.GetComponent<Renderer>().material.mainTexture = loadedMap;
			mapMin = map.GetComponent<Renderer>().bounds.min;
			mapMax = map.GetComponent<Renderer>().bounds.max;
			//Debug.Log(mapMin + " " + mapMax);
		}

		void LateUpdate() {
			//mouseScrollDelta goes from -1 to 1.
			ZoomCamera(Input.mouseScrollDelta.y);

			//Moving camera when holding MMB
			if (Input.GetMouseButton(2)) {
				float specificSpeed = speed * Time.deltaTime * cam.orthographicSize;
				Vector3 newPosition = transform.position + new Vector3(-Input.GetAxis("Mouse X") * specificSpeed,
					-Input.GetAxis("Mouse Y") * specificSpeed, 0);

				MoveCamera(newPosition);
			}

			if (Input.GetKey(KeyCode.UpArrow) ||
			    Input.GetKey(KeyCode.DownArrow) ||
			    Input.GetKey(KeyCode.LeftArrow) ||
			    Input.GetKey(KeyCode.RightArrow))
				MoveCameraByKey();
		}

		private void MoveCamera(Vector3 newPosition) {
			newPosition.x = Mathf.Clamp(newPosition.x, mapMin.x, mapMax.x);
			newPosition.y = Mathf.Clamp(newPosition.y, mapMin.y, mapMax.y);
			transform.position = newPosition;
		}


		private void MoveCameraByKey() {
			float specificSpeed = speed * Time.deltaTime * cam.orthographicSize / 30;
			Vector3 newPosition = transform.position +
			                      new Vector3((Convert.ToInt32(Input.GetKey(KeyCode.RightArrow))
			                                   - Convert.ToInt32(Input.GetKey(KeyCode.LeftArrow))) * specificSpeed,
				                      (Convert.ToInt32(Input.GetKey(KeyCode.UpArrow))
				                       - Convert.ToInt32(Input.GetKey(KeyCode.DownArrow))) * specificSpeed, 0);
			MoveCamera(newPosition);
		}

		private void ZoomCamera(float scrollDeltaY) {
			float newSize = cam.orthographicSize - scrollDeltaY * sensitivity;
			newSize = Mathf.Clamp(newSize, minZoom, maxZoom);
			cam.orthographicSize = newSize;
		}
	}
}