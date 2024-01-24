using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Circle : MonoBehaviour {
	[Range(0.1f, 100f)]
	public float radius = 1.0f;     //Circle radius
	[Range(3, 256)]
	public int numSegments = 128;   //Circle segments

	/// <summary>
	/// Method keeps redrawing the circle based on its radius and segment attributes.
	public void FixedUpdate() {
		//Gets Component on start and sets the amount of segments.
		LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
		lineRenderer.positionCount = numSegments + 1;
		lineRenderer.useWorldSpace = false;

		float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
		float theta = 0f;

		//Draws the circle, segment by segment.
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