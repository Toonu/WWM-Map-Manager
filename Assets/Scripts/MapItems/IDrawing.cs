using System.Collections.Generic;
using UnityEngine;

internal interface IDrawing {
	public bool IsFinished { get; internal set; }
	public bool IsExtending { get; internal set; }
	public bool IsSideB { get; internal set; }
	public int I { get; set; }
	public List<Vector3> VerticesList { get; set; }
	public List<int> TrianglesList { get; set; }

	void Initialize(List<Vector3> verticesList, List<int> trianglesList);
}