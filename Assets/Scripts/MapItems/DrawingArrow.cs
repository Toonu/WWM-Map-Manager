using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Mesh2DColliderMaker))]
public class DrawingArrow : MonoBehaviour, IDrawing {
	private List<Vector3> verticesList;
	private List<int> trianglesList;
	private Mesh mesh;
	private Mesh2DColliderMaker meshColliderMaker;

	internal bool isExtending = false;
	bool IDrawing.IsExtending { get { return isExtending; } set { isExtending = value; } }
	private bool isFinished = false;
	bool IDrawing.IsFinished { get { return isFinished; } set { isFinished = value; meshColliderMaker.CreatePolygon2DColliderPoints(); } }
	private bool isSideB;
	bool IDrawing.IsSideB { get { return isSideB; } set { isSideB = value; } }
	int i;
	int IDrawing.I { get { return i; } set { i = value; } }
	public List<Vector3> VerticesList { get { return verticesList; } set { verticesList = value; mesh.vertices = verticesList.ToArray(); } }
	public List<int> TrianglesList { get { return trianglesList; } set { trianglesList = value; mesh.triangles = trianglesList.ToArray(); } }

	void IDrawing.Initialize(List<Vector3> verticesList, List<int> trianglesList) {
		Start();
		isFinished = true;
		VerticesList = verticesList;
		TrianglesList = trianglesList;
		mesh.vertices = verticesList.ToArray();
		mesh.triangles = trianglesList.ToArray();
		meshColliderMaker.CreatePolygon2DColliderPoints();
	}
	void Start() {
		mesh = GetComponent<MeshFilter>().mesh;
		meshColliderMaker = GetComponent<Mesh2DColliderMaker>();
		verticesList ??= new List<Vector3>();
		trianglesList ??= new List<int>();
	}

	void Update() {
		if (!isFinished) {
			verticesList.Clear();
			trianglesList.Clear();
			Vector3 endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			endPoint = transform.InverseTransformPoint(endPoint);
			endPoint.z = transform.position.z;
			float stemWidth = Mathf.Log(Camera.main.orthographicSize + 100) - 4.557f;
			float distance = Vector3.Distance(transform.position, endPoint);

			Vector3 perpendicularVector = new(-endPoint.normalized.y, endPoint.normalized.x, 0f);
			Vector3 normalVector = 0.3f * distance * new Vector3(-endPoint.normalized.x, -endPoint.normalized.y, 0f);

			verticesList.Add(perpendicularVector * stemWidth);
			verticesList.Add(-perpendicularVector * stemWidth);
			verticesList.Add(endPoint + (perpendicularVector * stemWidth) + normalVector);
			verticesList.Add(endPoint - (perpendicularVector * stemWidth) + normalVector);

			trianglesList.Add(0);
			trianglesList.Add(1);
			trianglesList.Add(3);
			trianglesList.Add(0);
			trianglesList.Add(3);
			trianglesList.Add(2);
			
			verticesList.Add(endPoint + (2 * stemWidth * perpendicularVector) + normalVector);
			verticesList.Add(endPoint - (2 * stemWidth * perpendicularVector) + normalVector);
			verticesList.Add(endPoint);
			trianglesList.Add(4);
			trianglesList.Add(6);
			trianglesList.Add(5);
			
			for (int j = 0; j < verticesList.Count; j++) {
				Vector3 n = new(verticesList[j].x, verticesList[j].y, transform.position.z);
				verticesList[j] = n;
			}

			mesh.vertices = verticesList.ToArray();
			mesh.triangles = trianglesList.ToArray();
		}
	}
}