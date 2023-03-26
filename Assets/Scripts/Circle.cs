using UnityEngine;

namespace Assets.Scripts {
	[RequireComponent(typeof(LineRenderer))]
	public class Circle : MonoBehaviour {
		[Range(0.1f, 100f)] public float radius = 1.0f;

		[Range(3, 256)] public int numSegments = 128;

		void Start() {
		}

		public void Update() {
			LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
			lineRenderer.positionCount = numSegments + 1;
			lineRenderer.useWorldSpace = false;

			float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
			float theta = 0f;

			for (int i = 0; i < numSegments + 1; i++) {
				float x = radius * Mathf.Cos(theta);
				float y = radius * Mathf.Sin(theta);
				Vector3 pos = new Vector3(x, y, 0);
				lineRenderer.SetPosition(i, pos);
				theta += deltaTheta;
			}
		}

		public void Clear() {
			gameObject.GetComponent<LineRenderer>().positionCount = 0;
		}
	}
}