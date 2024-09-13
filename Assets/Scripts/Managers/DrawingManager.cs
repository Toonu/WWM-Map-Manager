using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawingManager : MonoBehaviour {
	private static DrawingManager instance;
	public static DrawingManager Instance => instance;
	public DrawingArrow sampleArrow;                //Arrow prefab
	public Drawing sampleLine;                      //Line prefab

	internal List<IDrawing> drawings = new();
	internal List<GameObject> objects = new();

	private void Start() {
		instance = this;
	}

	public void DeleteDrawing(GameObject drawing) {
		IDrawing drawingID = drawing.GetComponent<IDrawing>();
		drawings.Remove(drawingID);
		objects.Remove(drawing);
		Destroy(drawing);
	}

	internal DrawingArrow SpawnArrow(Vector3 position, bool fromServer) {
		return (DrawingArrow)Setup(Instantiate(sampleArrow.gameObject, transform.parent), position, fromServer);
	}

	internal Drawing Spawn(Vector3 position, bool fromServer) {
		return (Drawing)Setup(Instantiate(sampleLine.gameObject, transform.parent), position, fromServer);
	}

	private IDrawing Setup(GameObject newDrawing, Vector3 position, bool fromServer) {
		IDrawing drawing = newDrawing.GetComponent<IDrawing>();
		drawings.Add(drawing);
		objects.Add(newDrawing);
		drawing.IsSideB = ApplicationController.isSideB;

		newDrawing.transform.position = position;
		return drawing;
	}

	public IList<IList<object>> ParseDrawings() {
		List<IList<object>> data = new();
		for (int i = 0; i < drawings.Count; i++) {
			data.Add(new List<object> { objects[i].transform.position.x, objects[i].transform.position.y, EnumUtil.ConvertBoolToInt(drawings[i].IsSideB), ParseDrawing(drawings[i]) });
		}
		return data;
	}

	private string ParseDrawing(IDrawing drawing) {
		string data = "";
		foreach (Vector3 v in drawing.VerticesList) {
			data += $"{v.x}|{v.y}|{v.z}|";
		}
		data += ";";
		foreach (int t in drawing.TrianglesList) {
			data += $"{t}|";
		}
		return data;
	}

	private void ParseDrawing(string data, ref IDrawing drawing) {
		string[] dataSplit = data.Split(";");
		string[] vertices = dataSplit[0].Split("|");
		string[] triangles = dataSplit[1].Split("|");
		List<Vector3> verticesList = new();
		List<int> trianglesList = new();
		for (int i = 0; i + 2 < vertices.Length; i += 3) {
			verticesList.Add(new Vector3(Convert.ToSingle(vertices[i]), Convert.ToSingle(vertices[i+1]), Convert.ToSingle(vertices[i+2])));
		}
		for (int i = 0; i < triangles.Length - 1; i++) {
			trianglesList.Add(Convert.ToInt32(triangles[i]));
		}
		
		drawing.Initialize(verticesList, trianglesList);
	}

	public void CreateDrawings(IList<IList<object>> data) {
		if (data == null) return;
		foreach (List<object> col in data) {
			if (col.Count > 0) {
				IDrawing drawing;
				
				if (col[3].ToString().Split(";")[0].Split("|").Length == 22) {
					drawing = SpawnArrow(new(Convert.ToSingle(col[0], ApplicationController.culture), Convert.ToSingle(col[1], ApplicationController.culture), -0.1f), true);
				} else {
					drawing = Spawn(new(Convert.ToSingle(col[0], ApplicationController.culture), Convert.ToSingle(col[1], ApplicationController.culture), -0.1f), true);
				}

				
				drawing.IsSideB = EnumUtil.ConvertIntToBool(Convert.ToInt16(col[2]));
				ParseDrawing(col[3].ToString(), ref drawing);
				SheetSync.drawingsLength++;
			}
		}
	}
}