using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SheetSync : MonoBehaviour {
	public UIPopup generalPopup;
	private SheetReader ss;
	internal static string passwordA;
	internal static string passwordB;
	internal static string passwordAdmin;
	internal static float pointsA = 0;
	internal static float pointsB = 0;
	internal static int basesLength = 0;
	internal static int unitsLength = 0;
	internal static int Turn {get { return turn; } set {
			turn = value;
			turnLabelUI.UpdateText(turn);
		} }
	private static int turn = 0;
	private static string turnPwd;
	private static TextMeshProUGUI pointsLabel;
	private static UITextFloatAppender turnLabelUI;

	private void Awake() {
		ss = GetComponent<SheetReader>();
		pointsLabel = transform.parent.Find("UI/BottomPanel/Points").GetComponent<TextMeshProUGUI>();
		turnLabelUI = transform.parent.Find("UI/BottomPanel/Turn").GetComponent<UITextFloatAppender>();
	}

	public void SaveSheet() {
		SaveBases();
		SaveUnits();
		SaveConfiguration();
		generalPopup.PopUp("Saved!");
	}

	private void SaveConfiguration() {
		IList<IList<object>> teamPoints = new List<IList<object>> {
			new List<object> { turnPwd },
			new List<object> { pointsA },
			new List<object> { pointsB },
			new List<object> { turn }
		};

		ss.SetSheetRange(teamPoints, $"Configuration!C6:C9");
	}
	private void SaveTurnPassword() {
		ss.SetSheetRange(new List<IList<object>> { new List<object> { turnPwd } }, $"Configuration!C6:C6");
	}
	private void SaveBases() {
		IList<IList<object>> sheetBases = new List<IList<object>>();
		if (UnitManager.Instance.bases.Count > basesLength) {
			basesLength = UnitManager.Instance.bases.Count;
		}
		foreach (Base b in UnitManager.Instance.bases) {
			sheetBases.Add(new List<object> { b.name, b.transform.position.x, b.transform.position.y, b.BaseType.ToString(), EnumUtil.ConvertBoolToInt(b.SideB) });
		}
		ss.SetSheetRange(sheetBases, $"Bases!A2:E{basesLength + 1}");
	}
	private void SaveUnits() {
		IList<IList<object>> sheetUnits = new List<IList<object>>();
		
		if (UnitManager.Instance.aerialUnits.Count + UnitManager.Instance.groundUnits.Count + UnitManager.Instance.navalUnits.Count > unitsLength) {
			unitsLength = UnitManager.Instance.aerialUnits.Count + UnitManager.Instance.groundUnits.Count + UnitManager.Instance.navalUnits.Count;
		}
		foreach (GroundUnit unit in UnitManager.Instance.groundUnits) {
			if (unit != null) {
				sheetUnits.Add(new List<object> {0,
				unit.transform.position.x, unit.transform.position.y,
				(int)unit.GetUnitTier(),
				unit.name,
				EnumUtil.ConvertBoolToInt(unit.SideB),
				(int)unit.specialization,
				(int)unit.protectionType,
				(int)unit.transportType,
				unit.StartPosition.x, unit.StartPosition.y,
				unit.equipmentList.Count == 0 ? "" : string.Join("\n", unit.equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.Amount}")),
				EnumUtil.GetCorpsInt(unit.parentTextUI.text) });
			}
		}
		foreach (AerialUnit unit in UnitManager.Instance.aerialUnits) {
			if (unit != null) {
				sheetUnits.Add(new List<object> {1,
				unit.transform.position.x, unit.transform.position.y,
				(int)unit.GetUnitTier(),
				unit.name,
				EnumUtil.ConvertBoolToInt(unit.SideB),
				(int)unit.specialization,
				0,
				0,
				unit.StartPosition.x, unit.StartPosition.y,
				unit.equipmentList.Count == 0 ? "" : string.Join("\n", unit.equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.Amount}")),
				EnumUtil.GetCorpsInt(unit.parentTextUI.text)});
			}
		}
		foreach (NavalUnit unit in UnitManager.Instance.navalUnits) {
			if (unit != null) {
				sheetUnits.Add(new List<object> {2,
				unit.transform.position.x, unit.transform.position.y,
				(int)unit.GetUnitTier(),
				unit.name,
				EnumUtil.ConvertBoolToInt(unit.SideB),
				(int)unit.specialization,
				0,
				0,
				unit.StartPosition.x, unit.StartPosition.y,
				unit.equipmentList.Count == 0 ? "" : string.Join("\n", unit.equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.Amount}")),
				EnumUtil.GetCorpsInt(unit.parentTextUI.text)});
			}
		}

		ss.SetSheetRange(sheetUnits, $"Units!A2:M{unitsLength + 1}");
	}

	public async void LoadSheet() {
		GameObject loading = ApplicationController.Instance.transform.Find("UI/Loading").gameObject;
		loading.SetActive(true);
		await LoadSheetAsync();
		loading.SetActive(false);
	}

	public async Task<bool> LoadSheetAsync() {
		IList<IList<object>> units = await ss.GetSheetRangeAsync("Units!A2:M");
		IList<IList<object>> bases = await ss.GetSheetRangeAsync("Bases!A2:E");
		IList<IList<object>> sheetConfiguration = await ss.GetSheetRangeAsync("Configuration!C2:C");
		IList<IList<object>> equipmentData = await ss.GetSheetRangeAsync("Configuration!E2:N");

		if (bases == null || units == null || sheetConfiguration == null || equipmentData == null) {
			throw new ApplicationException("Sever connection failed!");
		}


		UnitManager.Instance.groundUnits.Clear();
		UnitManager.Instance.aerialUnits.Clear();
		UnitManager.Instance.navalUnits.Clear();
		UnitManager.Instance.bases.Clear();
		EquipmentManager.equipmentFriendly[0].Clear();
		EquipmentManager.equipmentFriendly[1].Clear();
		EquipmentManager.equipmentFriendly[2].Clear();
		EquipmentManager.equipmentHostile[0].Clear();
		EquipmentManager.equipmentHostile[1].Clear();
		EquipmentManager.equipmentHostile[2].Clear();

		for (int i = 2; i < UnitManager.Instance.transform.childCount; i++) {
			Destroy(UnitManager.Instance.transform.GetChild(i).gameObject);
		}
		Transform bas = UnitManager.Instance.transform.GetChild(0);

		for (int i = 0; i < bas.childCount; i++) {
			Destroy(bas.GetChild(i).gameObject);
		}

		for (int i = 0; i < EquipmentManager.Instance.transform.childCount; i++) {
			Destroy(EquipmentManager.Instance.transform.GetChild(i).gameObject);
		}

		Transform templates = ApplicationController.Instance.transform.Find("Templates").transform;
		for (int i = 0; i < templates.childCount; i++) {
			Destroy(templates.GetChild(i).gameObject);
		}

		/* TODO
		  User drawings using LineRenderer system or sprites for better symbols?
		  Weather system using area box or circle around transform affecting range of moveables
		  Terrain based spawning and movement - bases now spawning in the sea and units movable only on land or sea or both for air.
		  Generating higher echelons on zoom out on centered position between smaller unit with sum of their equipment.
		*/
		//Administration
		passwordA = PasswordManager.HashPassword(sheetConfiguration[1][0].ToString());
		passwordB = PasswordManager.HashPassword(sheetConfiguration[2][0].ToString());
		passwordAdmin = PasswordManager.HashPassword(sheetConfiguration[3][0].ToString());
		pointsA = Convert.ToSingle(sheetConfiguration[5][0], ApplicationController.culture);
		pointsB = Convert.ToSingle(sheetConfiguration[6][0], ApplicationController.culture);
		UpdatePoints(0);
		if (ApplicationController.applicationVersion != sheetConfiguration[0][0].ToString()) {
			throw new InvalidProgramException("Wrong game version! Please update your application");
		}
		turn = Convert.ToInt16(sheetConfiguration[7][0]);
		turnPwd = sheetConfiguration[4][0].ToString();

		//Movables
		EquipmentManager.CreateTemplates(equipmentData);
		UnitManager.Instance.SpawnBases(bases);
		UnitManager.Instance.SpawnUnits(units);

		Debug.Log("Server data loaded!");
		return true;
	}

	public static void UpdatePoints(float addition) {
		if (ApplicationController.isSideB) {
			pointsB += addition;
			pointsLabel.text = $"Pts:{pointsB}";
		} else {
			pointsA += addition;
			pointsLabel.text = $"Pts:{pointsA}";
		}
	}

	public void FinishTurn() {
		string pass = ApplicationController.Instance.transform.Find("UI/PopupWarningTurn/Password").GetComponent<TMP_InputField>().text;
		if (turnPwd == pass) {
			turnPwd = PasswordManager.HashPassword(pass);
			SaveTurnPassword();
			Turn++;
			UnitManager.Instance.CalculatePositions();
			UnitManager.Instance.CalculateSpotting();
		} else {
			ApplicationController.generalPopup.PopUp("Wrong password! Ask the GM for the current one!");
		}
	}
}
