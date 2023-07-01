using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SheetSync : MonoBehaviour {
	public UIPopup generalPopup;	//General Popup UI to show messages
	private SheetReader ss;         //Sheet connection
	public TextMeshProUGUI controllerLabelTextUI = null;
	public Button controllerSaveButtonUI;
	public Button controllerTurnButtonUI;

	#region Static Internal Variables

	internal static string passwordA;			//Team A password
	internal static string passwordB;			//Team B password
	internal static string passwordAdmin;		//Admin password
	internal static float pointsA = 0;			//Team A points
	internal static float pointsB = 0;			//Team B points
	internal static bool controllerA = false;	//Team A controller
	internal static bool controllerB = false;	//Team B controller
	internal static int basesLength = 0;		//Amount of bases in the sheet
	internal static int unitsLength = 0;        //Amount of bases in the sheet
	private static string turn = "A";			//Turn number
	private static string turnPwd;				//Turn password for finishing turn
	private static UITextFloatAppender pointsLabel;	//Team points UI element
	private static UITextFloatAppender turnLabelUI; //Turn label UI element

	#endregion

	/// <summary>
	/// Method for loading Components on startup.
	/// </summary>
	private void Awake() {
		ss = GetComponent<SheetReader>();
		pointsLabel = transform.parent.Find("UI/BottomPanel/Points").GetComponent<UITextFloatAppender>();
		turnLabelUI = transform.parent.Find("UI/BottomPanel/Turn").GetComponent<UITextFloatAppender>();
	}

	#region SavingToServer

	/// <summary>
	/// Method for saving data to the sheet.
	/// </summary>
	public void SaveSheet() {
		SaveBases();
		SaveUnits();
		SaveConfiguration();
		generalPopup.PopUp("Saved!");
	}

	/// <summary>
	/// Method for saving the configuration to the sheet.
	/// </summary>
	public void SaveConfiguration() {
		//Take all configuration data from the application and save it to the sheet.
		IList<IList<object>> teamPoints;
		if (ApplicationController.isSideB) {
			teamPoints = new List<IList<object>> {
			new List<object> { turn },
			new List<object> { turnPwd },
			new List<object> { pointsB },
			new List<object> { EnumUtil.ConvertBoolToInt(ApplicationController.isController) }
			};
			ss.SetSheetRange(teamPoints, $"Configuration!C8:C11");
		} else {
			teamPoints = new List<IList<object>> {
			new List<object> { pointsA },
			new List<object> { EnumUtil.ConvertBoolToInt(ApplicationController.isController) },
			new List<object> { turn },
			new List<object> { turnPwd }
			};
			ss.SetSheetRange(teamPoints, $"Configuration!C6:C9");
		}
	}

	/// <summary>
	/// Method for saving bases to the sheet.
	/// </summary>
	public void SaveBases() {
		IList<IList<object>> sheetBases = new List<IList<object>>();
		//Ensure that all bases in the sheet are removed and therefore duplicates averted. If move bases in app than sheet, rewrites the amount.
		if (UnitManager.Instance.bases.Count > basesLength) {
			basesLength = UnitManager.Instance.bases.Count;
		}
		foreach (Base b in UnitManager.Instance.bases) {
			sheetBases.Add(new List<object> { b.name, b.transform.position.x, b.transform.position.y, b.BaseType.ToString(), EnumUtil.ConvertBoolToInt(b.SideB), EnumUtil.ConvertBoolToInt(b.isGhost) });
		}
		ss.SetSheetRange(sheetBases, $"Bases!A2:F{basesLength + 1}");
	}
	/// <summary>
	/// Method for saving units to the sheet.
	/// </summary>
	public void SaveUnits() {
		IList<IList<object>> sheetUnits = new List<IList<object>>();
		//Avert duplicates by comparing amount of units in the sheet to the amount of units in the application and using the larger number.
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
				EnumUtil.GetCorpsInt(unit.parentTextUI.text),
				EnumUtil.ConvertBoolToInt(unit.isGhost)});
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
				EnumUtil.GetCorpsInt(unit.parentTextUI.text),
				EnumUtil.ConvertBoolToInt(unit.isGhost)
				});
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
				EnumUtil.GetCorpsInt(unit.parentTextUI.text),
				EnumUtil.ConvertBoolToInt(unit.isGhost)});
			}
		}

		ss.SetSheetRange(sheetUnits, $"Units!A2:N{unitsLength + 1}");
	}

	#endregion

	#region Loading from server

	/// <summary>
	/// Method for asynchronous loading of the sheet.
	/// </summary>
	public async void LoadSheet() {
		GameObject loading = ApplicationController.Instance.transform.Find("UI/Loading").gameObject;
		loading.SetActive(true);
		await LoadSheetAsync();
		loading.SetActive(false);
	}

	/// <summary>
	/// Method loads units, bases and equipment from the sheet.
	/// </summary>
	/// <returns>true</returns>
	/// <exception cref="ApplicationException">Thrown if there is problem with pulling the data from the sheet.</exception>
	public async Task<bool> LoadSheetAsync() {
		//Get all data from the sheet.
		await LoadSheetConfigurationData();
		IList<IList<object>> units = await ss.GetSheetRangeAsync("Units!A2:N") ?? throw new ApplicationException("Sever connection failed or units are corrupted!");
		IList<IList<object>> bases = await ss.GetSheetRangeAsync("Bases!A2:F") ?? throw new ApplicationException("Sever connection failed or bases are corrupted!");
		IList<IList<object>> equipmentData = await ss.GetSheetRangeAsync("Configuration!E2:N") ?? throw new ApplicationException("Sever connection failed or equipment is corrupted!");

		//Reset all lists and dictionaries.
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

		//Destroy all units, bases, equipment and template.
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

		//Spawning units from the sheet data.
		EquipmentManager.CreateTemplates(equipmentData);
		UnitManager.Instance.SpawnBases(bases);
		UnitManager.Instance.SpawnUnits(units);

		Debug.Log("Server data loaded!");
		return true;
	}

	/// <summary>
	/// Method loads the main variables of the program from the sheet.
	/// </summary>
	/// <returns>true</returns>
	/// <exception cref="ApplicationException">Thrown if there is problem with pulling the data from sheet.</exception>
	/// <exception cref="InvalidProgramException">Thrown if user is using wrong game version.</exception>
	private async Task<bool> LoadSheetConfigurationData() {
		IList<IList<object>> sheetConfiguration = await ss.GetSheetRangeAsync("Configuration!C2:C") ?? throw new ApplicationException("Sever connection failed or configuration is corrupted!");

		if (ApplicationController.applicationVersion != sheetConfiguration[0][0].ToString()) {
			throw new InvalidProgramException("Wrong game version! Please update your application");
		}
		if (sheetConfiguration.Any(e => e.Count == 0)) {
			throw new InvalidProgramException("There was error loading the basic configuration data! Maybe you was yielding and taking control too quickly!");
		}

		//Load configuration variables
		passwordA = PasswordManager.HashPassword(sheetConfiguration[1][0].ToString());
		passwordB = PasswordManager.HashPassword(sheetConfiguration[2][0].ToString());
		passwordAdmin = PasswordManager.HashPassword(sheetConfiguration[3][0].ToString());

		pointsA = Convert.ToSingle(sheetConfiguration[4][0], ApplicationController.culture);
		controllerA = EnumUtil.ConvertIntToBool(Convert.ToInt16(sheetConfiguration[5][0].ToString()));

		turn = sheetConfiguration[6][0].ToString();
		turnPwd = sheetConfiguration[7][0].ToString();

		pointsB = Convert.ToSingle(sheetConfiguration[8][0], ApplicationController.culture);
		controllerB = EnumUtil.ConvertIntToBool(Convert.ToInt16(sheetConfiguration[9][0].ToString()));

		//Update labels on the bottom panel of UI.
		UpdateConfigurationLabels();

		return true;
	}

	#endregion

	/*
	TODO User drawings using LineRenderer system or sprites for better symbols?
	TODO Weather system using area box or circle around transform affecting range of moveables
	TODO Terrain based spawning and movement - bases now spawning in the sea and units movable only on land or sea or both for air.
	*/

	#region ControllerSync

	/// <summary>
	/// Update the team points and turn every time period continuously.
	/// </summary>
	public async void StartUpdateLoop() {
		while (true) {
			//Non controller pulls, controller pushes updates.
			if (!ApplicationController.isController) {
				await LoadSheetConfigurationData();
				if (ApplicationController.isDebug) {
					Debug.Log("Pulling sheet data.");
				}
				await Task.Delay(30000);
			} else {
				//Saving just configuration because unit movements and handled in Unit OnDragEnd.
				SaveConfiguration();
				if (ApplicationController.isDebug) {
					Debug.Log("Pushing sheet data.");
				}
				await Task.Delay(14500);
				//Save unit spawned or despawned?
			}
		}
	}

	public async void CheckController(TextMeshProUGUI buttonLabelUI = null) {
		if (buttonLabelUI == null) { buttonLabelUI = controllerLabelTextUI; }
		if (ApplicationController.isController) {
			ApplicationController.isController = false;
			if (ApplicationController.isDebug) {
				Debug.Log("Control yielded!");
			}
			SaveConfiguration();
			buttonLabelUI.text = "  Take control";
			controllerSaveButtonUI.interactable = false;
			controllerTurnButtonUI.interactable = false;
		}
		await LoadSheetConfigurationData();
		//I am side B and Side B has no controller > I become controller.
		if ((ApplicationController.isSideB && !controllerB) || (!ApplicationController.isSideB && !controllerA)) {
			ApplicationController.isController = true;
			if (ApplicationController.isDebug) {
				Debug.Log("Control taken!");
			}
			SaveConfiguration();
			buttonLabelUI.text = "  Yield control";
			controllerSaveButtonUI.interactable = true;
			controllerTurnButtonUI.interactable = true;
		}
	}

	#endregion


	#region Configuration attribute updating

	/// <summary>
	/// Method adds or removes points from a team side.
	/// </summary>
	/// <param name="addition">Float point addition or reduction if negative.</param>
	public static void UpdatePoints(float addition) {
		pointsA += ApplicationController.isSideB ? 0 : addition;
		pointsB += ApplicationController.isSideB ? addition : 0;
		UpdateConfigurationLabels();
	}

	/// <summary>
	/// Method updates the UI bottom panel labels.
	/// </summary>
	private static void UpdateConfigurationLabels() {
		if (ApplicationController.isSideB) {
			pointsLabel.UpdateText(pointsB);
		} else {
			pointsLabel.UpdateText(pointsA);
		}
		turnLabelUI.UpdateText(turn);
	}

	/// <summary>
	/// Method starts the turn with proper password input.
	/// </summary>
	public void FinishTurn() {
		string pass = ApplicationController.Instance.transform.Find("UI/PopupWarningTurn/Password").GetComponent<TMP_InputField>().text;
		if (turnPwd == pass) {
			//Takes old password and hashes it to the new one which is then saved to the sheet.
			turnPwd = PasswordManager.HashPassword(pass);
			turn = (turn == "A") ? "B" : "A";
			//Turn-ending mass-calculations.
			UnitManager.Instance.ResetStartPositions();
			UnitManager.Instance.CalculateSpotting();
			SaveSheet();
		} else {
			ApplicationController.generalPopup.PopUp("Wrong password! Ask the GM for the current one!");
		}
		UpdateConfigurationLabels();
	}

	#endregion
}
