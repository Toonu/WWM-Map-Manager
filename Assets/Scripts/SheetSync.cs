using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Threading.Tasks;

public class SheetSync : MonoBehaviour {
	public ApplicationController generalPopup;
	private IList<IList<object>> sheetUnits = new List<IList<object>>();
	private IList<IList<object>> sheetBases = new List<IList<object>>();
	private SheetReader ss;
	internal string passwordA;
	internal string passwordB;
	internal string passwordAdmin;
	internal int pointsA = 0;
	internal int pointsB = 0;
	private UnitManager manager;
	private EquipmentManager eqManager;
	private ApplicationController aC; 
	private int basesLength = 0;
	private int unitsLength = 0;
	public GameObject equipmentTemplate;

	private void Awake() {
		ss = GetComponent<SheetReader>();
		manager = GameObject.FindWithTag("Units").GetComponent<UnitManager>();
		eqManager = GameObject.FindWithTag("Equipment").GetComponent<EquipmentManager>();
		aC = GameObject.FindWithTag("GameController").GetComponent<ApplicationController>();
	}


	public void SaveSheet() {
		if (manager.bases.Count > basesLength) {
			basesLength = manager.bases.Count;
		}
		if (manager.aerialUnits.Count + manager.groundUnits.Count + manager.navalUnits.Count > unitsLength) {
			unitsLength = manager.aerialUnits.Count + manager.groundUnits.Count + manager.navalUnits.Count;
		}

		foreach (Base b in manager.bases) {
			sheetBases.Add(new List<object> {b.identification.text.ToString(), b.transform.position.x, b.transform.position.y, b.baseType.ToString(), EnumUtil.ConvertBoolToInt(b.sideB)});
		}

		ss.SetSheetRange(sheetBases, $"Bases!A2:E{basesLength+1}");


		foreach (GroundUnit unit in manager.groundUnits) {
			if (unit != null) {
				sheetUnits.Add(new List<object> {
				unit.transform.position.x, unit.transform.position.y, 0,
				(int)unit.specialization, unit.name,
				(int)unit.UnitTier,
				EnumUtil.ConvertBoolToInt(unit.SideB),
				(int)unit.movementModifier,
				(int)unit.transportModifier,
				unit.unitEquipment.Count == 0 ? "" : string.Join("\n", unit.unitEquipment.Select(equipment => $"{equipment.equipmentName}:{equipment.amount}")) });
			}
		}
		foreach (AerialUnit unit in manager.aerialUnits) {
			if (unit != null) {
				sheetUnits.Add(new List<object> {
				unit.transform.position.x, unit.transform.position.y, 1,
				(int)unit.specialization,
				unit.name,
				(int)unit.UnitTier,
				EnumUtil.ConvertBoolToInt(unit.SideB), 0, 0,
				string.Join("\n", unit.unitEquipment.Select(equipment => $"{equipment.equipmentName}:{equipment.amount}")) });
			}
		}
		foreach (NavalUnit unit in manager.navalUnits) {
			if (unit != null) {
				sheetUnits.Add(new List<object> {
				unit.transform.position.x, unit.transform.position.y, 2,
				(int) unit.specialization,
				unit.name,
				(int) unit.UnitTier,
				EnumUtil.ConvertBoolToInt(unit.SideB), 0, 0,
				string.Join("\n", unit.unitEquipment.Select(equipment => $"{equipment.equipmentName}:{equipment.amount}")) });
			}
		}

		ss.SetSheetRange(sheetUnits, $"Units!A2:J{unitsLength + 1}");

		generalPopup.PopUp("Saved!");
	}

	public async Task LoadSheet() {
		try {
			IList<IList<object>> units = await ss.GetSheetRangeAsync("Units!A2:J");
			IList<IList<object>> bases = await ss.GetSheetRangeAsync("Bases!A2:E");
			IList<IList<object>> sheetConfiguration = await ss.GetSheetRangeAsync("Configuration!C2:C");
			IList<IList<object>> equipmentData = await ss.GetSheetRangeAsync("Configuration!E2:K");

			foreach (Unit item in manager.groundUnits) {
				manager.Despawn(item.gameObject);
			}
			foreach (Unit item in manager.aerialUnits) {
				manager.Despawn(item.gameObject);
			}
			foreach (Unit item in manager.navalUnits) {
				manager.Despawn(item.gameObject);
			}
			foreach (Base b in manager.bases) {
				manager.Despawn(b.gameObject);
			}

			/* TODO
			 * Swap unit constructor attributes to change them on the assigned unit directly instead. Removing intermediary.
			 * Merge unit and equipment menu within the equipment-central logic
			 * Move unit specialization to Unit
			 * Implement IPointerClickHandler to handle all context menus
			 * Remove editing menu by mergining it with the spawning menu since they are very similar, also may merge UnitConstructor and UnitConstructor classes while at it, hide and rename buttons per usage.
			 * Delete units and bases when reloading the sheet second and other times
			 * Spawned unit Icon determined based on the equipment
			 * But also keep unit icon editor
			 * Add debug logs everywhere
			 * Spotting system/For of War
			 * Turn system
			 * Airborne and marine units cost more points
			 * Keep movement range in sheet due to non finished turns
			 * Base spawning menu
			 * Team points - removal on spawning new units and so on
			 * Fog of War
			 * Turns
			 * Drawing things by user, lines eg
			 * Zooming out merging of units, 
			 * artillery and dice rolls
			 * extend changeTier and changeIcon etc methods on Unit/Specialized Unit to work based on equipment
			 * Remove unit option with no equipment. Always require equipment - overhaul the spawn menu 
			 * to take equipment as the main thing and changing icon, size echelon and others based on that
			 * Equipment-centric unit creation
			 * 
			 * 
			 * Terrain based movement
			 * if (Physics.Raycast(ray, out hit))
             {
                 hit.collider.renderer.material.color = Color.red;
                 //Debug.Log(hit);
             }
			 */


			passwordA = PasswordManager.HashPassword(sheetConfiguration[0][0].ToString());
			passwordB = PasswordManager.HashPassword(sheetConfiguration[1][0].ToString());
			passwordAdmin = PasswordManager.HashPassword(sheetConfiguration[2][0].ToString());
			pointsA = Convert.ToInt16(sheetConfiguration[3][0].ToString());
			pointsB = Convert.ToInt16(sheetConfiguration[4][0].ToString());
			if (ApplicationController.applicationVersion != sheetConfiguration[5][0].ToString()) {
				throw new InvalidProgramException("Wrong game version! Please update your application");
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
				}
				else {
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
						manager.SpawnBase(
							bases[i][0].ToString(),
							new Vector3(Convert.ToSingle(bases[i][1], enGbCulture),
							Convert.ToSingle(bases[i][2], enGbCulture), -0.1f),
							(BaseType)Enum.Parse(typeof(BaseType), bases[i][3].ToString()),
							EnumUtil.ConvertIntToBool(Convert.ToInt16(bases[i][4])));
						basesLength++;
					}
				}
			}


			//Units

			if (units != null) {
				for (int i = 0; i < units.Count; i++) {
					if (units[i].Count > 8) {
						List<Equipment> equip = new List<Equipment>();

						Unit newUnit = manager.SpawnUnit(
							new Vector3(Convert.ToSingle(units[i][0], enGbCulture), Convert.ToSingle(units[i][1], enGbCulture), -0.1f),
							(UnitTier)Enum.Parse(typeof(UnitTier), units[i][5].ToString()), //Tier
							units[i][4].ToString(), //Spec.
							equip, //Equipment blank list
							EnumUtil.ConvertIntToBool(Convert.ToInt16(units[i][6])), //Side
							Convert.ToInt16(units[i][3]), //Side
							(GroundMovementType)Enum.Parse(typeof(GroundMovementType), units[i][7].ToString()), //Ground movement type
							(GroundTransportType)Enum.Parse(typeof(GroundTransportType), units[i][8].ToString()), //Ground transport type
							Convert.ToInt16(units[i][2])); //Domain
						if (units[i].Count > 9) {
							string[] lines = units[i][9].ToString().Split('\n');

							for (int j = 0; j < lines.Length; j++) {
								string[] word = lines[j].Split(':');
								switch (Convert.ToInt16(units[i][2])) {
									case 0:
										foreach (Equipment equipment in EquipmentManager.eqGround) {
											if (equipment.equipmentName == word[0]) {
												Equipment newEquipment = CreateEquipment(word, equipment);
												equip.Add(newEquipment);
												break;
											}
										}
										foreach (Equipment equipment in EquipmentManager.eqGroundB) {
											if (equipment.equipmentName == word[0]) {
												Equipment newEquipment = CreateEquipment(word, equipment);
												equip.Add(newEquipment);
												break;
											}
										}
										break;
									case 1:
										foreach (Equipment equipment in EquipmentManager.eqAerial) {
											if (equipment.equipmentName == word[0]) {
												Equipment newEquipment = CreateEquipment(word, equipment);
												equip.Add(newEquipment);
												break;
											}
										}
										foreach (Equipment equipment in EquipmentManager.eqAerialB) {
											if (equipment.equipmentName == word[0]) {
												Equipment newEquipment = CreateEquipment(word, equipment);
												equip.Add(newEquipment);
												break;
											}
										}
										break;
									default:
										foreach (Equipment equipment in EquipmentManager.eqNaval) {
											if (equipment.equipmentName == word[0]) {
												Equipment newEquipment = CreateEquipment(word, equipment);
												equip.Add(newEquipment);
												break;
											}
										}
										foreach (Equipment equipment in EquipmentManager.eqNavalB) {
											if (equipment.equipmentName == word[0]) {
												Equipment newEquipment = CreateEquipment(word, equipment);
												equip.Add(newEquipment);
												break;
											}
										}
										break;
								}
								newUnit.AddEquipment(equip);
							}
						}
						unitsLength++;
					}
				}
			}

			Equipment CreateEquipment(string[] word, Equipment equipment) {
				GameObject newEquipmentObject = Instantiate(equipmentTemplate, GameObject.FindWithTag("ServerSync").transform);
				Equipment newEquipment = newEquipmentObject.AddComponent<Equipment>();
				newEquipment.Initiate(equipment.equipmentName, Convert.ToInt16(word[1]), equipment.movementRange, equipment.sightRange, equipment.weaponRange, equipment.cost, equipment.side, equipment.domain);
				return newEquipment;
			}
		}
		catch (Exception e) {
			Debug.LogException(e);
			await aC.generalPopup.PopUpAsync("Error! " + e.Message + e, 30);
			Application.Quit();
			#if UNITY_EDITOR
			EditorApplication.ExitPlaymode();
			#endif
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

	/// <summary>
	/// Prints data in 2D array of lists.
	/// </summary>
	/// <param name="list">List<List<object>> 2D list"</param>
	private void PrintData(List<List<object>> list) {
		for (int i = 0; i < list.Count; i++) {
			IList<object> row = sheetUnits[i];
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
