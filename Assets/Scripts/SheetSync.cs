using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SheetSync : MonoBehaviour {
	public Popup generalPopup;
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
			generalPopup.PopUp("Saved!");
			SaveSheet(bottomRight: GetLocation(unitWidth));
		} else {
			generalPopup.PopUp("No Data to Save!", 2.2f);
		}
	}

	public void SaveSheet(string topLeft = "A1", string bottomRight = "Q") {
		ss.SetSheetRange(sheetData, $"Data!{topLeft}:{bottomRight}");
	}

	public void LoadSheet() {
		IList<IList<object>> data = ss.GetSheetRange("Data!A1:K");
		IList<IList<object>> sheetConfiguration = ss.GetSheetRange("Configuration!C1:C");
		IList<IList<object>> equipmentData = ss.GetSheetRange("Configuration!E1:H");
			
		try {
			unitWidth = Convert.ToInt16(sheetConfiguration[1][0]);
			baseWidth = Convert.ToInt16(sheetConfiguration[2][0]);

			passwordA = PasswordManager.HashPassword(sheetConfiguration[3][0].ToString());
			passwordB = PasswordManager.HashPassword(sheetConfiguration[4][0].ToString());
			passwordAdmin = PasswordManager.HashPassword(sheetConfiguration[5][0].ToString());
			Debug.Log(passwordAdmin);
			pointsA = Convert.ToInt16(sheetConfiguration[6][0].ToString());
			pointsB = Convert.ToInt16(sheetConfiguration[7][0].ToString());
		} catch (Exception e) {
			GameObject.FindWithTag("GameController").GetComponent<ApplicationController>().generalPopup.PopUp("Fatal Error! Could not connect to the server! " + e, 30);
		}
		/*
		foreach (IList<object> col in equipmentData) {
			Equipment.equipmentNames.Add(col[0].ToString(), new Equipment(col[0].ToString(), 1, Convert.ToSingle(col[1]), Convert.ToSingle(col[2]), Convert.ToSingle(col[3]), Convert.ToInt16(col[4])));
		}
		*/
		//Create equipment templates and populate the UI with them
		
		

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

		PrintData();
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

	/// <summary>
	/// Returns letter position of a number
	/// </summary>
	/// <param name="col">Column to find</param>
	/// <returns>string</returns>
	private string GetLocation(int col) {
		col++;
		string column = "";
		while (col > 0) {
			int modulo = (col - 1) % 26;
			column = (char)(65 + modulo) + column;
			col = (col - modulo) / 26;
		}
		return column;
	}

	/// <summary>
	/// Returns column number of a letter
	/// </summary>
	/// <param name="col">Column to find</param>
	/// <returns>int</returns>
	private int GetLocation(string col) {
		int result = 0;
		foreach (char c in col) {
			result *= 26;
			result += c - 64;
		}
		return result - 1;
	}
}
