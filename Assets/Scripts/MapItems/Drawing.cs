using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Mesh2DColliderMaker))]
public class Drawing : MonoBehaviour, IDrawing {
	private List<Vector3> verticesList;
	private List<int> trianglesList;
	private Mesh mesh;
	internal int i = 0;
	private int segment = 0;
	private float stemWidth;
	private Mesh2DColliderMaker meshColliderMaker;

	private bool isExtending = false;
	bool IDrawing.IsExtending { get { return isExtending; } set { isExtending = value; if (!value) StopExtending(); } }
	private bool isFinished = false;
	bool IDrawing.IsFinished { get { return isFinished; } set { isFinished = value; } }
	private bool isSideB;
	bool IDrawing.IsSideB { get { return isSideB; } set { isSideB = value; } }
	int IDrawing.I { get { return i; } set { i = value; segment++; } }
	public List<Vector3> VerticesList { get { return verticesList; } set { verticesList = value; mesh.vertices = verticesList.ToArray(); } }
	public List<int> TrianglesList { get { return trianglesList; } set { trianglesList = value; mesh.triangles = trianglesList.ToArray(); } }

	void IDrawing.Initialize(List<Vector3> verticesList, List<int> trianglesList) {
		Start();
		isFinished = true;
		isExtending = false;
		i = 0;
		VerticesList = verticesList;
		TrianglesList = trianglesList;
		mesh.vertices = verticesList.ToArray();
		mesh.triangles = trianglesList.ToArray();
		meshColliderMaker.CreatePolygon2DColliderPoints();
	}
	void Start() {
		meshColliderMaker = GetComponent<Mesh2DColliderMaker>();
		mesh = GetComponent<MeshFilter>().mesh;
		verticesList ??= new List<Vector3>();
		trianglesList ??= new List<int>();
	}

	internal void StopExtending() {
		if (meshColliderMaker != null) {
			meshColliderMaker.CreatePolygon2DColliderPoints();
		}
		isExtending = false;
		i = 0;
	}

	void Update() {
		if (!isFinished && !isExtending) {
			verticesList.Clear();
			trianglesList.Clear();
			Vector3 endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			endPoint = transform.InverseTransformPoint(endPoint);
			endPoint.z = transform.position.z;
			stemWidth = Mathf.Log(Camera.main.orthographicSize + 100) - 4.557f;

			Vector3 perpendicularVector = new(-endPoint.normalized.y, endPoint.normalized.x, 0f);

			verticesList.Add(perpendicularVector * stemWidth);
			verticesList.Add(-perpendicularVector * stemWidth);
			verticesList.Add(endPoint - (perpendicularVector * stemWidth));
			verticesList.Add(endPoint + (perpendicularVector * stemWidth));

			trianglesList.Add(1);
			trianglesList.Add(0);
			trianglesList.Add(3);
			trianglesList.Add(1);
			trianglesList.Add(3);
			trianglesList.Add(2);

			for (int j = 0; j < verticesList.Count; j++) {
				Vector3 n = new(verticesList[j].x, verticesList[j].y, transform.position.z);
				verticesList[j] = n;
			}

			mesh.vertices = verticesList.ToArray();
			mesh.triangles = trianglesList.ToArray();
		} else if (isExtending) {
			Vector3 endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			endPoint = transform.InverseTransformPoint(endPoint);
			endPoint.z = transform.position.z;

			Vector3 perpendicularVector = new(-endPoint.y, -endPoint.x, 0f);
			perpendicularVector.Normalize();

			//Remove the new segment vertices and paths so new ones can be added
			if (i > 0) {
				verticesList.RemoveRange(verticesList.Count - 2, 2);
				trianglesList.RemoveRange(trianglesList.Count - 6, 6);
			}
			i++;

			verticesList.Add(endPoint + (perpendicularVector * stemWidth));
			verticesList.Add(endPoint - (perpendicularVector * stemWidth));

			if (segment % 2 == 0) {
				trianglesList.Add(verticesList.Count - 3);
				trianglesList.Add(verticesList.Count - 4);
				trianglesList.Add(verticesList.Count - 2);

				trianglesList.Add(verticesList.Count - 3);
				trianglesList.Add(verticesList.Count - 1);
				trianglesList.Add(verticesList.Count - 2);
			} else {
				trianglesList.Add(verticesList.Count - 3);
				trianglesList.Add(verticesList.Count - 1);
				trianglesList.Add(verticesList.Count - 2);

				trianglesList.Add(verticesList.Count - 3);
				trianglesList.Add(verticesList.Count - 4);
				trianglesList.Add(verticesList.Count - 2);
			}
			

			for (int j = 0; j < verticesList.Count; j++) {
				Vector3 n = new(verticesList[j].x, verticesList[j].y, transform.position.z);
				verticesList[j] = n;
			}

			mesh.vertices = verticesList.ToArray();
			mesh.triangles = trianglesList.ToArray();
		}
	}
}