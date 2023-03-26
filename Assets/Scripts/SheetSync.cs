using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SheetSync : MonoBehaviour {
	public TextMeshProUGUI savedPopup;
	private IList<IList<object>> sheetData = new List<IList<object>>();
	private SheetReader ss;
	private int unitWidth;
	private int baseWidth;
	internal string passwordA;
	internal string passwordB;
	internal string passwordAdmin;
	internal int pointsA = 0;
	internal int pointsB = 0;

	private void Awake() {
		ss = GetComponent<SheetReader>();
	}

	public void SaveSheet() {
		if (unitWidth != 0) {
			Debug.Log($"SheetLength:{unitWidth}");
			SaveSheet(bottomRight: GetLocation(unitWidth));
		} else {
			savedPopup.text = "No data to save";
		}
	}

	public void SaveSheet(string topLeft = "A1", string bottomRight = "Q") {
		ss.SetSheetRange(sheetData, $"Data!{topLeft}:{bottomRight}");
	}

	public void LoadSheet() {
		IList<IList<object>> data = ss.GetSheetRange("Data!A1:K");
		IList<IList<object>> sheetConfiguration = ss.GetSheetRange("Configuration!C1:C");

		unitWidth = Convert.ToInt16(sheetConfiguration[1][0]);
		baseWidth = Convert.ToInt16(sheetConfiguration[2][0]);

		passwordA = PasswordManager.HashPassword(sheetConfiguration[3][0].ToString());
		passwordB = PasswordManager.HashPassword(sheetConfiguration[4][0].ToString());
		passwordAdmin = PasswordManager.HashPassword(sheetConfiguration[5][0].ToString());
		pointsA = Convert.ToInt16(sheetConfiguration[6][0].ToString());
		pointsB = Convert.ToInt16(sheetConfiguration[7][0].ToString());

		for (int i = 0; i < data.Count; i++) {
			sheetData.Add(new List<object>());
			for (int j = 0; j < unitWidth; j++) {
				sheetData[i].Add("");
			}
		}

		for (int i = 0; i < data.Count; i++) {
			IList<object> row = data[i];

			for (int j = 0; j < row.Count; j++) {
				sheetData[i][j] = row[j];
			}
		}
		SetData(5, 5, "Test");

		//PrintData();
	}

	public string GetData(int x, int y) {
		return sheetData[x][y].ToString();
	}

	public void SetData(int x, int y, string data) {
		sheetData[x][y] = data;
	}

	private void PrintData(bool showEmpty = false) {
		for (int i = 0; i < sheetData.Count; i++) {
			IList<object> row = sheetData[i];
			for (int j = 0; j < row.Count; j++) {
				if (showEmpty) {
					Debug.Log(i + ":" + j + " " + row[j].ToString());
				} else if (row[j].ToString() != "") {
					Debug.Log(i + ":" + j + " " + row[j].ToString());
				}
			}
		}
	}

	private string GetLocation(int x) {
		x++;
		string column = "";
		while (x > 0) {
			int modulo = (x - 1) % 26;
			column = (char)(65 + modulo) + column;
			x = (x - modulo) / 26; // calculate the new x value
		}
		return column;
	}

	private int GetLocation(string location) {
		int result = 0;
		foreach (char c in location) {
			result *= 26;
			result += c - 64;
		}
		return result - 1;
	}
}
