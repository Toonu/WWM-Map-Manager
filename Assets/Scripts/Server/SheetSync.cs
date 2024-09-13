using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SheetSync : MonoBehaviour {
	private SheetReader ss;         //Sheet connection

	#region Static Internal Variables

	internal static string passwordA;           //Team A password
	internal static string passwordB;           //Team B password
	internal static string passwordAdmin;       //Admin password
	internal static float pointsA = 0;          //Team A points
	internal static float pointsB = 0;          //Team B points
	internal static bool controllerA = false;   //Team A controller
	internal static bool controllerB = false;   //Team B controller
	internal static int basesLength = 0;        //Amount of bases in the sheet
	internal static int unitsLength = 0;        //Amount of bases in the sheet
	internal static int drawingsLength = 0;     //Amount of drawings in the sheet
	internal static int maximalSupply = 0;      //Maximal supply of units
	internal static string turn = "A";          //Turn number
	private static string turnPwd;              //Turn password for finishing turn
	private static ContextMenu contextMenu;

	#endregion

	/// <summary>
	/// Method for loading Components on startup.
	/// </summary>
	private void Awake() {
		ss = GetComponent<SheetReader>();
		contextMenu = transform.parent.GetChild(transform.parent.childCount - 1).gameObject.GetComponent<ContextMenu>();
	}

	#region Saving To Server

	/// <summary>
	/// Method for saving data to the sheet.
	/// </summary>
	public void SaveSheet() {
		SaveBases();
		SaveUnits();
		SaveDrawings();
		_ = SaveConfiguration();
		ApplicationController.generalPopup.PopUp("Server data saved!");
	}

	/// <summary>
	/// Method for saving the configuration to the sheet.
	/// </summary>
	public async Task SaveConfiguration() {
		//Take all configuration data from the application and save it to the sheet.
		if (passwordAdmin == null) return; //Prevents sending corrupted data to the sheet.
		IList<IList<object>> teamPoints;
		if (ApplicationController.isSideB) {
			teamPoints = new List<IList<object>> {
			new List<object> { turn },
			new List<object> { turnPwd },
			new List<object> { pointsB },
			new List<object> { EnumUtil.ConvertBoolToInt(ApplicationController.isController) }
			};
			await ss.SetSheetRange(teamPoints, $"Configuration!C8:C11");
		} else {
			teamPoints = new List<IList<object>> {
			new List<object> { pointsA },
			new List<object> { EnumUtil.ConvertBoolToInt(ApplicationController.isController) },
			new List<object> { turn },
			new List<object> { turnPwd }
			};
			await ss.SetSheetRange(teamPoints, $"Configuration!C6:C9");
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
			sheetBases.Add(new List<object> { b.name, b.transform.position.x, b.transform.position.y, b.BaseType.ToString(), EnumUtil.ConvertBoolToInt(b.SideB), EnumUtil.ConvertBoolToInt(b.IsGhost) });
		}
		_ = ss.SetSheetRange(sheetBases, $"Bases!B2:G{basesLength + 1}");
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
				EnumUtil.ConvertBoolToInt(unit.IsGhost)});
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
				EnumUtil.ConvertBoolToInt(unit.IsGhost)
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
				EnumUtil.ConvertBoolToInt(unit.IsGhost)});
			}
		}

		_ = ss.SetSheetRange(sheetUnits, $"Units!B2:O{unitsLength + 1}");
	}

	public void SaveDrawings() {
		IList<IList<object>> sheetDrawings = DrawingManager.Instance.ParseDrawings();
		_ = ss.SetSheetRange(sheetDrawings, $"Drawings!B2:E{drawingsLength + 1}");
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

		//Only equipment and configuration data is needed for the application to run. Map can work with no units, bases or drawings.
		IList<IList<object>> equipmentData = await ss.GetSheetRangeAsync("Configuration!E2:Q") ?? throw new ApplicationException("Sever connection failed or equipment is corrupted!");
		IList<IList<object>> units = await ss.GetSheetRangeAsync("Units!B2:O");
		IList<IList<object>> bases = await ss.GetSheetRangeAsync("Bases!B2:G");
		IList<IList<object>> drawings = await ss.GetSheetRangeAsync("Drawings!B2:F");

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
		DrawingManager.Instance.drawings.Clear();
		DrawingManager.Instance.objects.Clear();

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
		for (int i = 1; i < ClickMapController.Instance.mapObject.transform.childCount; i++) {
			Destroy(ClickMapController.Instance.mapObject.transform.GetChild(i).gameObject);
		}

		//Spawning units from the sheet data.
		EquipmentManager.CreateTemplates(equipmentData);
		UnitManager.Instance.SpawnBases(bases);
		UnitManager.Instance.SpawnUnits(units);
		DrawingManager.Instance.CreateDrawings(drawings);

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

		maximalSupply = Convert.ToInt32(sheetConfiguration[10][0], ApplicationController.culture);

		//Update labels on the bottom panel of UI.
		contextMenu.UpdateControllerButtons();

		return true;
	}

	#endregion

	#region ControllerSync

	/// <summary>
	/// Update the team points and turn every time period continuously.
	/// </summary>
	public async void StartUpdateLoop() {
		while (true) {
			//Non controller pulls, controller pushes updates.
			if (!ApplicationController.isController) {
				await LoadSheetAsync();
				if (ApplicationController.isDebug) {
					Debug.Log("Pulling sheet data.");
				}
				await Task.Delay(10000);
			} else if (ApplicationController.isController) {
				//Saving just configuration because unit movements and handled in Unit OnDragEnd and unit or base editing and soft resets in IClickController and Constructors.
				_ = SaveConfiguration();
				if (ApplicationController.isDebug) {
					Debug.Log("Pushing sheet data.");
				}
				await Task.Delay(14500);
			}
		}
	}

	public async Task CheckController() {
		if (ApplicationController.isController) {
			ApplicationController.isController = false;
			if (ApplicationController.isDebug) Debug.Log("Control yielded!");
			_ = SaveConfiguration();
		} else {
			await LoadSheetConfigurationData();
			bool isCurrentController = (ApplicationController.isSideB && !controllerB) || (!ApplicationController.isSideB && !controllerA);
			ApplicationController.isController = isCurrentController;
			if (ApplicationController.isDebug) Debug.Log(isCurrentController ? "Control taken!" : "Control not available!");
			//Save onto server only if user becomes controller or yields a controller so everyone can apply for it again.
		}

		contextMenu.UpdateControllerButtons();
	}

	#endregion


	#region Configuration attribute updating

	/// <summary>
	/// Method adds or removes points from a team side.
	/// </summary>
	/// <param name="addition">Float point addition or reduction if negative.</param>
	/// <exception cref="ApplicationException">Exception thrown when in negative amount of points.</exception>
	public static bool UpdatePoints(float addition) {
		pointsA += ApplicationController.isSideB ? 0 : addition;
		pointsB += ApplicationController.isSideB ? addition : 0;
		if (pointsA < 0 || pointsB < 0) {
			ApplicationController.generalPopup.PopUp("Not enough points!", 5);
			return false;
		}
		contextMenu.UpdateControllerButtons();
		return true;
	}

	/// <summary>
	/// Method starts the turn with proper password input.
	/// </summary>
	public void FinishTurn() {
		string pass = ApplicationController.generalPopup.inputField.text;
		if (turnPwd == pass) {
			//Takes old password and hashes it to the new one which is then saved to the sheet.
			turnPwd = PasswordManager.HashPassword(pass);
			turn = (turn == "A") ? "B" : "A";
			//Turn-ending mass-calculations.
			UnitManager.Instance.ResetStartPositions();
			UnitManager.Instance.CalculateSpotting();
			SaveSheet();
		} else {
			ApplicationController.generalPopup.PopUp("Wrong password! Ask the GM for the current one!", 2, true);
		}
		contextMenu.UpdateControllerButtons();
	}

	#endregion
}
