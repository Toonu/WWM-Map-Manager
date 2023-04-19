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
	private static int basesLength = 0;
	private static int unitsLength = 0;
	private static TextMeshProUGUI pointsLabel;

	private void Awake() {
		ss = GetComponent<SheetReader>();
		pointsLabel = transform.parent.Find("UI/BottomPanel/Points").GetComponent<TextMeshProUGUI>();
	}


	public void SaveSheet() {
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

	public async Task LoadSheet() {
		IList<IList<object>> units = await ss.GetSheetRangeAsync("Units!A2:J");
		IList<IList<object>> bases = await ss.GetSheetRangeAsync("Bases!A2:E");
		IList<IList<object>> sheetConfiguration = await ss.GetSheetRangeAsync("Configuration!C2:C");
		IList<IList<object>> equipmentData = await ss.GetSheetRangeAsync("Configuration!E2:N");

		if (bases == null || units == null || sheetConfiguration == null || equipmentData == null) {
			throw new ApplicationException("Sever connection failed!");
		}

		foreach (Unit item in UnitManager.Instance.groundUnits) {
			UnitManager.Instance.Despawn(item.gameObject);
		}
		foreach (Unit item in UnitManager.Instance.aerialUnits) {
			UnitManager.Instance.Despawn(item.gameObject);
		}
		foreach (Unit item in UnitManager.Instance.navalUnits) {
			UnitManager.Instance.Despawn(item.gameObject);
		}
		foreach (Base b in UnitManager.Instance.bases) {
			UnitManager.Instance.Despawn(b.gameObject);
		}

		/* TODO
		 * Check admin menus, since in build I could see there were movement and transport icons present on naval unit and also other issues.
		* Equipment-centric unit creation and modification.
		* Add debug logs everywhere
		* Turn system
		* Airborne and marine units cost more points
		* Keep movement range in sheet due to non finished turns
		* Base spawning menu
		* Team points - removal on spawning new units and so on
		* Fog of War
		* Drawing things by user, lines eg
		* Zooming out merging of units, 
		* artillery and dice rolls
		* 
		* Terrain based movement and base spawning
		* if (Physics.Raycast(ray, out hit))
        {
            hit.collider.renderer.material.color = Color.red;
            //Debug.Log(hit);
        }
		*/

		passwordA = PasswordManager.HashPassword(sheetConfiguration[0][0].ToString());
		passwordB = PasswordManager.HashPassword(sheetConfiguration[1][0].ToString());
		passwordAdmin = PasswordManager.HashPassword(sheetConfiguration[2][0].ToString());
		pointsA = Convert.ToSingle(sheetConfiguration[3][0].ToString());
		pointsB = Convert.ToSingle(sheetConfiguration[4][0].ToString());
		UpdatePoints(0);
		if (ApplicationController.applicationVersion != sheetConfiguration[5][0].ToString()) {
			throw new InvalidProgramException("Wrong game version! Please update your application");
		}

		//Equipment
		EquipmentManager.CreateTemplates(equipmentData);
			
		//Bases
		for (int i = 0; i < bases.Count; i++) {
			if (bases[i].Count == 5) {
				UnitManager.Instance.SpawnBase(bases[i][0].ToString(), new Vector3(Convert.ToSingle(bases[i][1], ApplicationController.culture), Convert.ToSingle(bases[i][2], ApplicationController.culture), -0.1f),	(BaseType)Enum.Parse(typeof(BaseType), bases[i][3].ToString()),	EnumUtil.ConvertIntToBool(Convert.ToInt16(bases[i][4])));
				basesLength++;
			}
		}

		//Units
		for (int i = 0; i < units.Count; i++) {
			if (units[i].Count > 8) {
				int domain = Convert.ToInt16(units[i][2]);
				Unit newUnit = UnitManager.Instance.SpawnUnit(
					new Vector3(Convert.ToSingle(units[i][0], ApplicationController.culture), Convert.ToSingle(units[i][1], ApplicationController.culture), -0.1f),
					(UnitTier)Enum.Parse(typeof(UnitTier), units[i][5].ToString()), //Tier
					units[i][4].ToString(), //Spec.
					new List<Equipment>(), //Equipment blank list
					EnumUtil.ConvertIntToBool(Convert.ToInt16(units[i][6])), //Side
					Convert.ToInt16(units[i][3]), //Side
					(GroundMovementType)Enum.Parse(typeof(GroundMovementType), units[i][7].ToString()), //Ground movement type
					(GroundTransportType)Enum.Parse(typeof(GroundTransportType), units[i][8].ToString()), //Ground transport type
					domain); //Domain
				if (units[i].Count > 9) {
					string[] lines = units[i][9].ToString().Split('\n');

					for (int j = 0; j < lines.Length; j++) {
						string[] word = lines[j].Split(':');

						if (!newUnit.SideB) {
							EquipmentManager.equipmentHostile[domain].ForEach(delegate (Equipment equipment) {
								if (equipment.equipmentName == word[0]) {
									newUnit.AddEquipment(EquipmentManager.CreateEquipment(equipment, Convert.ToInt16(word[1])));
									return;
								}
							});
						} else {
							EquipmentManager.equipmentFriendly[domain].ForEach(delegate (Equipment equipment) {
								if (equipment.equipmentName == word[0]) {
									newUnit.AddEquipment(EquipmentManager.CreateEquipment(equipment, Convert.ToInt16(word[1])));
									return;
								}
							});
						}
					}
				}
				unitsLength++;
			}
		}

		Debug.Log("Server data loaded!");
		return;
	}

	public string GetData(int x, int y) {
		return sheetUnits[x][y].ToString();
	}

	public void SetData(int x, int y, string data) {
		sheetUnits[x][y] = data;
	}

	public static void UpdatePoints(float addition) {
		if (ApplicationController.sideB) {
			pointsB += addition;
			pointsLabel.text = $"Pts:{pointsB}";
		} else {
			pointsA += addition;
			pointsLabel.text = $"Pts:{pointsA}";
		}
		
	}
}
