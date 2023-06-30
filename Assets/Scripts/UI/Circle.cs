using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Circle : MonoBehaviour {
	[Range(0.1f, 100f)]
	public float radius = 1.0f;

	[Range(3, 256)]
	public int numSegments = 128;

	/// <summary>
	/// Method keeps redrawing the circle based on its radius and segment attributes.
	public void FixedUpdate() {
		LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
		lineRenderer.positionCount = numSegments + 1;
		lineRenderer.useWorldSpace = false;

		float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
		float theta = 0f;

		for (int i = 0; i < numSegments + 1; i++) {
			float x = radius * Mathf.Cos(theta);
			float y = radius * Mathf.Sin(theta);
			Vector3 pos = new(x, y, 0);
			lineRenderer.SetPosition(i, pos);
			theta += deltaTheta;
		}
	}


	/// <summary>
	/// Method clears the circle lines.
	/// </summary>
	public void Clear() {
		gameObject.GetComponent<LineRenderer>().positionCount = 0;
	}
}