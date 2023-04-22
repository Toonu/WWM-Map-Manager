using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SheetSync : MonoBehaviour {
	public UIPopup generalPopup;
	private readonly IList<IList<object>> sheetUnits = new List<IList<object>>();
	private readonly IList<IList<object>> sheetBases = new List<IList<object>>();
	private SheetReader ss;
	internal static string passwordA;
	internal static string passwordB;
	internal static string passwordAdmin;
	internal static float pointsA = 0;
	internal static float pointsB = 0;
	internal static int basesLength = 0;
	internal static int unitsLength = 0;
	private static TextMeshProUGUI pointsLabel;

	private void Awake() {
		ss = GetComponent<SheetReader>();
		pointsLabel = transform.parent.Find("UI/BottomPanel/Points").GetComponent<TextMeshProUGUI>();
	}


	public void SaveSheet() {
		//TODO No saving for now
		return;
		if (UnitManager.Instance.bases.Count > basesLength) {
			basesLength = UnitManager.Instance.bases.Count;
		}
		if (UnitManager.Instance.aerialUnits.Count + UnitManager.Instance.groundUnits.Count + UnitManager.Instance.navalUnits.Count > unitsLength) {
			unitsLength = UnitManager.Instance.aerialUnits.Count + UnitManager.Instance.groundUnits.Count + UnitManager.Instance.navalUnits.Count;
		}

		foreach (Base b in UnitManager.Instance.bases) {
			sheetBases.Add(new List<object> { b.name, b.transform.position.x, b.transform.position.y, b.BaseType.ToString(), EnumUtil.ConvertBoolToInt(b.SideB) });
		}

		ss.SetSheetRange(sheetBases, $"Bases!A2:E{basesLength + 1}");


		foreach (GroundUnit unit in UnitManager.Instance.groundUnits) {
			if (unit != null) {
				sheetUnits.Add(new List<object> {
				unit.transform.position.x, unit.transform.position.y, 0,
				(int)unit.specialization, unit.name,
				(int)unit.GetUnitTier(),
				EnumUtil.ConvertBoolToInt(unit.SideB),
				(int)unit.movementModifier,
				(int)unit.transportModifier,
				unit.equipmentList.Count == 0 ? "" : string.Join("\n", unit.equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.Amount}")) });
			}
		}
		foreach (AerialUnit unit in UnitManager.Instance.aerialUnits) {
			if (unit != null) {
				sheetUnits.Add(new List<object> {
				unit.transform.position.x, unit.transform.position.y, 1,
				(int)unit.specialization,
				unit.name,
				(int)unit.GetUnitTier(),
				EnumUtil.ConvertBoolToInt(unit.SideB), 0, 0,
				string.Join("\n", unit.equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.Amount}")) });
			}
		}
		foreach (NavalUnit unit in UnitManager.Instance.navalUnits) {
			if (unit != null) {
				sheetUnits.Add(new List<object> {
				unit.transform.position.x, unit.transform.position.y, 2,
				(int) unit.specialization,
				unit.name,
				(int) unit.GetUnitTier(),
				EnumUtil.ConvertBoolToInt(unit.SideB), 0, 0,
				string.Join("\n", unit.equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.Amount}")) });
			}
		}

		ss.SetSheetRange(sheetUnits, $"Units!A2:J{unitsLength + 1}");

		generalPopup.PopUp("Saved!");
	}

	public async void LoadSheet() {
		GameObject loading = ApplicationController.Instance.transform.Find("UI/Loading").gameObject;
		loading.SetActive(true);
		await LoadSheetAsync();
		loading.SetActive(false);
	}

	public async Task<bool> LoadSheetAsync() {
		IList<IList<object>> units = await ss.GetSheetRangeAsync("Units!A2:J");
		IList<IList<object>> bases = await ss.GetSheetRangeAsync("Bases!A2:E");
		IList<IList<object>> sheetConfiguration = await ss.GetSheetRangeAsync("Configuration!C2:C");
		IList<IList<object>> equipmentData = await ss.GetSheetRangeAsync("Configuration!E2:N");

		if (bases == null || units == null || sheetConfiguration == null || equipmentData == null) {
			throw new ApplicationException("Sever connection failed!");
		}

		foreach (Unit item in UnitManager.Instance.groundUnits.ToList()) {
			if (item != null) {
				UnitManager.Instance.Despawn(item.gameObject);
			}
		}
		foreach (Unit item in UnitManager.Instance.aerialUnits.ToList()) {
			if (item != null) {
				UnitManager.Instance.Despawn(item.gameObject);
			}
		}
		foreach (Unit item in UnitManager.Instance.navalUnits.ToList()) {
			if (item != null) {
				UnitManager.Instance.Despawn(item.gameObject);
			}
		}
		foreach (Base b in UnitManager.Instance.bases.ToList()) {
			if (b != null) {
				UnitManager.Instance.Despawn(b.gameObject);
			}
		}
		//TODO Handle equipment deletion

		/* TODO
		* Turn system
		* Keep movement range in sheet due to non finished turns
		* Transport type of unit based on equipment
		* Team points
		* Fog of War
		* Drawing things by user, lines eg
		* Zooming out generates HQ units centered on average of positions of small units with their equipment.
		* artillery and dice rolls
		* Terrain based movement and base spawning
		* Weather system
		* When spot unit, cannot reset back to original position - spot only when right next to enemy, otherwise spot at the end of the turn
		*/
		//Administration
		passwordA = PasswordManager.HashPassword(sheetConfiguration[0][0].ToString());
		passwordB = PasswordManager.HashPassword(sheetConfiguration[1][0].ToString());
		passwordAdmin = PasswordManager.HashPassword(sheetConfiguration[2][0].ToString());
		pointsA = Convert.ToSingle(sheetConfiguration[3][0].ToString());
		pointsB = Convert.ToSingle(sheetConfiguration[4][0].ToString());
		UpdatePoints(0);
		if (ApplicationController.applicationVersion != sheetConfiguration[5][0].ToString()) {
			throw new InvalidProgramException("Wrong game version! Please update your application");
		}

		//Movables
		EquipmentManager.CreateTemplates(equipmentData);
		UnitManager.Instance.SpawnBases(bases);
		UnitManager.Instance.SpawnUnits(units);

		Debug.Log("Server data loaded!");
		return true;
	}

	public string GetData(int x, int y) {
		return sheetUnits[x][y].ToString();
	}

	public void SetData(int x, int y, string data) {
		sheetUnits[x][y] = data;
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
}
