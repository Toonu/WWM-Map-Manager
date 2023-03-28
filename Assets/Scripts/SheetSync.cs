using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;

public class SheetSync : MonoBehaviour {
	public Popup generalPopup;
	private IList<IList<object>> sheetData = new List<IList<object>>();
	private IList<IList<object>> sheetDataBases = new List<IList<object>>();
	private SheetReader ss;
	internal string passwordA;
	internal string passwordB;
	internal string passwordAdmin;
	internal int pointsA = 0;
	internal int pointsB = 0;
	private UnitManager manager;
	private EquipmentManager eqManager;

	private void Awake() {
		ss = GetComponent<SheetReader>();
		manager = GameObject.FindWithTag("Units").GetComponent<UnitManager>();
		eqManager = GameObject.FindWithTag("Equipment").GetComponent<EquipmentManager>();
	}


	public void SaveSheet() {
		foreach (Base b in manager.bases) {
			sheetDataBases.Add(new List<object> {b.identification.text.ToString(), b.transform.position.x, b.transform.position.y, b.baseType.ToString(), EnumUtil.ConvertBoolToInt(b.sideB)});
		}

		ss.SetSheetRange(sheetDataBases, $"Bases!A2:E{sheetDataBases.Count+1}");
		generalPopup.PopUp("Saved!");
	}

	public void LoadSheet() {
		IList<IList<object>> data = ss.GetSheetRange("Data!A2:K");
		IList<IList<object>> bases = ss.GetSheetRange("Bases!A2:E");
		IList<IList<object>> sheetConfiguration = ss.GetSheetRange("Configuration!C2:C");
		IList<IList<object>> equipmentData = ss.GetSheetRange("Configuration!E2:K");

		try {
			passwordA = PasswordManager.HashPassword(sheetConfiguration[0][0].ToString());
			passwordB = PasswordManager.HashPassword(sheetConfiguration[1][0].ToString());
			passwordAdmin = PasswordManager.HashPassword(sheetConfiguration[2][0].ToString());
			pointsA = Convert.ToInt16(sheetConfiguration[3][0].ToString());
			pointsB = Convert.ToInt16(sheetConfiguration[4][0].ToString());
		} catch (Exception e) {
			GameObject.FindWithTag("GameController").GetComponent<ApplicationController>().generalPopup.PopUp("Fatal Error! Could not connect to the server! " + e, 30);
		}


		//Equipment

		
		CultureInfo enGbCulture = new CultureInfo("en-GB");

		eqManager.equipmentNames.Clear();

		GameObject templates = Instantiate(new GameObject("Templates"), transform);

		foreach (IList<object> col in equipmentData) {
			if (Convert.ToInt16(col[6]) == 3) {
				continue;
			}
			GameObject temp = Instantiate(UnitManager.Instance.equipmentTemplate, templates.transform);
			Equipment eq = temp.AddComponent<Equipment>();
			eq.Initiate(col[0].ToString(), 1, Convert.ToSingle(col[1], enGbCulture), Convert.ToSingle(col[2], enGbCulture), Convert.ToSingle(col[3], enGbCulture), Convert.ToInt16(col[4]), Convert.ToInt16(col[5]), Convert.ToInt16(col[6]));
			temp.name = $"Template: {eq.equipmentName}";
			if (eq.side == 0) {
				switch (eq.domain) {
					case 0:
					EquipmentManager.eqGround.Add(eq);
					break;
					case 1:
					EquipmentManager.eqAerial.Add(eq);
					break;
					case 2:
					EquipmentManager.eqNaval.Add(eq);
					break;
				}
			} else {
				switch (eq.domain) {
					case 0:
					EquipmentManager.eqGroundB.Add(eq);
					break;
					case 1:
					EquipmentManager.eqAerialB.Add(eq);
					break;
					case 2:
					EquipmentManager.eqNavalB.Add(eq);
					break;
				}
			}
		}

		//Bases

		if (bases != null) {
			for (int i = 0; i < bases.Count; i++) {
				if (bases[i].Count == 5) {
					manager.SpawnBase(bases[i][0].ToString(), new Vector3(Convert.ToSingle(bases[i][1], enGbCulture), Convert.ToSingle(bases[i][2], enGbCulture), -1), (BaseType)Enum.Parse(typeof(BaseType), bases[i][3].ToString()), EnumUtil.ConvertIntToBool(Convert.ToInt16(bases[i][4])));
				}
			}
		}
		

		//Data

		//PrintData();
	}

	public string GetData(int x, int y) {
		return sheetData[x][y].ToString();
	}

	public void SetData(int x, int y, string data) {
		sheetData[x][y] = data;
	}

	private void PrintData(List<List<object>> list) {
		for (int i = 0; i < list.Count; i++) {
			IList<object> row = sheetData[i];
			for (int j = 0; j < row.Count; j++) {
				if (row[j].ToString() != "") {
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
