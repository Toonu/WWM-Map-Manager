using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitConstructor : MonoBehaviour {
	#region General Attributes
	internal int unitDomain;
	internal Unit constructedUnit;
	internal Equipment constructedEquipment;

	private GroundUnit groundUnit;
	private AerialUnit aerialUnit;
	private NavalUnit navalUnit;
	private List<Equipment> equipmentTemplates = new();

	public GroundUnit GetGroundUnit() => groundUnit;
	/// <summary>
	/// When new unit is assigned, clear old unit and update UI.
	/// </summary>
	/// <param name="newUnit">New unit to assign.</param>
	internal void SetGroundUnit(GroundUnit newUnit) {
		if (constructedUnit != null) {
			UnitManager.Instance.Despawn(constructedUnit.gameObject, false);
			ClearEquipment();
		}
		UpdateDomain(0);
		groundUnit = newUnit;
		constructedUnit = newUnit;
		UpdateOptions();
	}

	public AerialUnit GetAerialUnit() => aerialUnit;
	/// <summary>
	/// When new unit is assigned, clear old unit and update UI.
	/// </summary>
	/// <param name="newUnit">New unit to assign.</param>
	internal void SetAerialUnit(AerialUnit newUnit) {
		if (constructedUnit != null) {
			UnitManager.Instance.Despawn(constructedUnit.gameObject, false);
			ClearEquipment();
		}
		UpdateDomain(1);
		aerialUnit = newUnit;
		constructedUnit = newUnit;
		UpdateOptions();
	}

	public NavalUnit GetNavalUnit() => navalUnit;
	/// <summary>
	/// When new unit is assigned, clear old unit and update UI.
	/// </summary>
	/// <param name="newUnit">New unit to assign.</param>
	internal void SetNavalUnit(NavalUnit newUnit) {
		if (constructedUnit != null) {
			UnitManager.Instance.Despawn(constructedUnit.gameObject, false);
			ClearEquipment();
		}
		UpdateDomain(2);
		navalUnit = newUnit;
		constructedUnit = newUnit;
		UpdateOptions();
	}

	public static bool Editing = false;

	//private int higherUnitIdentifierNumber = 0;
	#endregion
	#region UnitUI Attributes
	private TextMeshProUGUI unitIconTierLabelUI;
	private TMP_Dropdown unitDomainUI;
	private TMP_Dropdown unitMovementTypeUI;
	private TMP_Dropdown unitTransportTypeUI;
	private TMP_Dropdown unitSpecializationUI;
	private TMP_InputField unitNameUI;
	private TMP_InputField unitAltitudeUI;
	private GameObject unitIconImageUI;
	private UILabelTextAppender unitCostLabelUI;
	private UILabelTextAppender unitSightLabelUI;
	private UILabelTextAppender unitRangeLabelUI;
	private UILabelTextAppender unitCostSupplyLabelUI;
	private Button finishButton;
	private Button cancelButton;
	#endregion
	#region EquipmentUI Attributes
	private TMP_InputField equipmentAmountUI;
	private TMP_Dropdown equipmentTypeUI;
	private UILabelTextAppender equipmentCostLabelUI;
	private UILabelTextAppender equipmentSightLabelUI;
	private UILabelTextAppender equipmentRangeLabelUI;
	private UILabelTextAppender equipmentCostSupplyLabelUI;
	private GameObject equipmentListUI;
	private Button equipmentBuyButton;
	private Button equipmentAddButton;
	public GameObject equipmentBarTemplateUI;
	#endregion

	#region UI Updates
	/// <summary>
	/// Method sets up Component references.
	/// </summary>
	public void Awake() {
		unitDomainUI = transform.GetChild(1).Find("UnitDomain").GetComponent<TMP_Dropdown>();
		unitSpecializationUI = transform.GetChild(1).Find("UnitSpecialization").GetComponent<TMP_Dropdown>();
		unitMovementTypeUI = transform.GetChild(1).Find("MovementType").GetComponent<TMP_Dropdown>();
		unitTransportTypeUI = transform.GetChild(1).Find("TransportType").GetComponent<TMP_Dropdown>();
		unitNameUI = transform.GetChild(1).Find("UnitName").GetComponent<TMP_InputField>();
		unitAltitudeUI = transform.GetChild(1).Find("Altitude").GetComponent<TMP_InputField>();

		unitCostLabelUI = transform.GetChild(2).GetChild(0).GetComponent<UILabelTextAppender>();
		unitSightLabelUI = transform.GetChild(2).GetChild(1).GetComponent<UILabelTextAppender>();
		unitRangeLabelUI = transform.GetChild(2).GetChild(2).GetComponent<UILabelTextAppender>();
		unitCostSupplyLabelUI = transform.GetChild(2).GetChild(3).GetComponent<UILabelTextAppender>();
		unitIconTierLabelUI = transform.GetChild(2).GetChild(4).GetComponent<TextMeshProUGUI>();
		unitIconImageUI = transform.GetChild(2).GetChild(5).gameObject;
		finishButton = transform.GetChild(2).GetChild(7).GetComponent<Button>();
		cancelButton = transform.GetChild(2).GetChild(9).GetComponent<Button>();

		equipmentCostLabelUI = transform.GetChild(3).GetChild(0).GetComponent<UILabelTextAppender>();
		equipmentSightLabelUI = transform.GetChild(3).GetChild(1).GetComponent<UILabelTextAppender>();
		equipmentRangeLabelUI = transform.GetChild(3).GetChild(2).GetComponent<UILabelTextAppender>();
		equipmentCostSupplyLabelUI = transform.GetChild(3).GetChild(3).GetComponent<UILabelTextAppender>();
		equipmentTypeUI = transform.GetChild(3).GetChild(4).GetComponent<TMP_Dropdown>();
		equipmentAmountUI = transform.GetChild(3).GetChild(5).GetComponent<TMP_InputField>();
		equipmentBuyButton = transform.GetChild(3).GetChild(6).GetComponent<Button>();
		equipmentAddButton = transform.GetChild(3).GetChild(7).GetComponent<Button>();

		equipmentListUI = transform.GetChild(4).gameObject;
		
		finishButton.onClick.RemoveAllListeners();
		cancelButton.onClick.RemoveAllListeners();
		unitDomainUI.onValueChanged.RemoveAllListeners();
		unitSpecializationUI.onValueChanged.RemoveAllListeners();
		unitMovementTypeUI.onValueChanged.RemoveAllListeners();
		unitTransportTypeUI.onValueChanged.RemoveAllListeners();
		unitNameUI.onValueChanged.RemoveAllListeners();
		equipmentBuyButton.onClick.RemoveAllListeners();
		equipmentAddButton.onClick.RemoveAllListeners();
		
		finishButton.onClick.AddListener(Close);
		cancelButton.onClick.AddListener(DespawnUnit);
		equipmentBuyButton.onClick.AddListener(BuyEquipment);
		equipmentAddButton.onClick.AddListener(AddEquipment);

		unitDomainUI.onValueChanged.AddListener(delegate { UpdateDomain(unitDomainUI.value); });
		unitSpecializationUI.onValueChanged.AddListener(delegate { UpdateSpecialization(unitSpecializationUI.value); });
		unitMovementTypeUI.onValueChanged.AddListener(delegate { UpdateProtection(unitMovementTypeUI.value); });
		unitTransportTypeUI.onValueChanged.AddListener(delegate { UpdateTransportation(unitTransportTypeUI.value); });
		unitNameUI.onValueChanged.AddListener(delegate { UpdateName(unitNameUI.text); });
		unitCostSupplyLabelUI.UpdateOrigin($"Supply: {SheetSync.maximalSupply}/");
	}
	/// <summary>
	/// Method sets up Full UI based on domain and other attributes.
	/// </summary>
	private void LoadUIOptions() {
		string[] enumNames;
		List<TMP_Dropdown.OptionData> options = new();

		//Specialization domain switch
		switch (unitDomain) {
			case 1:
				enumNames = Enum.GetNames(typeof(AerialSpecialization));
				for (int i = 0; i < enumNames.Length; i++) {
					options.Add(new TMP_Dropdown.OptionData(enumNames[i], UnitManager.Instance.aerialSpecialization[i]));
				}
				UpdateAltitude("1000");
				break;
			case 2:
				enumNames = Enum.GetNames(typeof(NavalSpecialization));
				for (int i = 0; i < enumNames.Length; i++) {
					options.Add(new TMP_Dropdown.OptionData(enumNames[i], UnitManager.Instance.navalSpecialization[i]));
				}
				break;
			default:
				enumNames = Enum.GetNames(typeof(GroundSpecialization));
				//HQ assignable only by isAdmin
				for (int i = 0; i < enumNames.Length - (ApplicationController.isAdmin ? 1 : 0); i++) {
					options.Add(new TMP_Dropdown.OptionData(enumNames[i], UnitManager.Instance.groundSpecialization[i]));
				}
				break;
		}

		unitSpecializationUI.ClearOptions();
		unitSpecializationUI.AddOptions(options);

		//Generate new name if creating new unit
		if (!Editing) {
			UnityEngine.Random.Range(0, 100);
			_ = new System.Random();
			finishButton.gameObject.SetActive(true);
		} else {
			finishButton.gameObject.SetActive(false);
		}
		unitNameUI.gameObject.SetActive(ApplicationController.isAdmin);
		unitSpecializationUI.gameObject.SetActive(ApplicationController.isAdmin);
		unitMovementTypeUI.gameObject.SetActive(ApplicationController.isAdmin);
		unitTransportTypeUI.gameObject.SetActive(ApplicationController.isAdmin);
		unitDomainUI.gameObject.SetActive(ApplicationController.isAdmin);

		//Removal of non-domain attributes and images
		if (unitDomain == 0 && ApplicationController.isAdmin) {
			unitMovementTypeUI.gameObject.SetActive(true);
			enumNames = Enum.GetNames(typeof(GroundProtectionType));
			options = new List<TMP_Dropdown.OptionData>();
			for (int i = 0; i < enumNames.Length; i++) {
				options.Add(new TMP_Dropdown.OptionData(enumNames[i], UnitManager.Instance.protectionType[i]));
			}
			unitMovementTypeUI.ClearOptions();
			unitMovementTypeUI.AddOptions(options);
			unitTransportTypeUI.gameObject.SetActive(true);
			enumNames = Enum.GetNames(typeof(GroundTransportType));
			options = new List<TMP_Dropdown.OptionData>();
			for (int i = 0; i < enumNames.Length; i++) {
				options.Add(new TMP_Dropdown.OptionData(enumNames[i], UnitManager.Instance.transportType[i]));
			}
			unitTransportTypeUI.ClearOptions();
			unitTransportTypeUI.AddOptions(options);
			unitIconImageUI.transform.GetChild(2).gameObject.SetActive(true);
			equipmentBuyButton.gameObject.SetActive(true);
		} else {
			unitMovementTypeUI.gameObject.SetActive(false);
			unitTransportTypeUI.gameObject.SetActive(false);
			unitIconImageUI.transform.GetChild(2).gameObject.SetActive(false);
			equipmentBuyButton.gameObject.SetActive(false);
		}
	}
	/// <summary>
	/// Method updates UI cost attribute labels.
	/// </summary>
	private void UpdateStatLabels() {
		equipmentCostLabelUI.UpdateText(constructedEquipment.cost * constructedEquipment.Amount);
		equipmentSightLabelUI.UpdateText(constructedEquipment.sightRange);
		equipmentRangeLabelUI.UpdateText(constructedEquipment.movementRange);
		equipmentCostSupplyLabelUI.UpdateText(constructedEquipment.costSupply * constructedEquipment.Amount);
		equipmentAmountUI.SetTextWithoutNotify(constructedEquipment.Amount.ToString());
		
		if (constructedUnit.equipmentList.Count != 0) {
			unitCostLabelUI.UpdateText(constructedUnit.equipmentList.Sum(e => e.cost * e.Amount));
			unitCostSupplyLabelUI.UpdateText(constructedUnit.equipmentList.Sum(e => e.costSupply * e.Amount));
			float min = constructedUnit.equipmentList.Min(e => e.movementRange);
			float max = constructedUnit.equipmentList.Max(e => e.sightRange);
			unitSightLabelUI.UpdateText(max < constructedEquipment.sightRange ? constructedEquipment.sightRange : max);
			unitRangeLabelUI.UpdateText(min > constructedEquipment.movementRange ? constructedEquipment.movementRange : min);
		} else {
			unitCostLabelUI.UpdateText(constructedEquipment.cost * constructedEquipment.Amount);
			unitCostSupplyLabelUI.UpdateText(constructedEquipment.costSupply * constructedEquipment.Amount);
			unitSightLabelUI.UpdateText(constructedEquipment.sightRange);
			unitRangeLabelUI.UpdateText(constructedEquipment.movementRange);
		}
	}
	/// <summary>
	/// Method updates UI options based on unit attributes after new unit is assigned or its equipment modified.
	/// </summary>
	private void UpdateOptions() {
		//Returns the most numerous armour-traction type.
		if (groundUnit != null) {
			unitMovementTypeUI.SetValueWithoutNotify((int)groundUnit.protectionType);
			unitTransportTypeUI.SetValueWithoutNotify((int)groundUnit.transportType);
		}

		unitNameUI.text = constructedUnit.name;
		unitIconTierLabelUI.text = constructedUnit.GetUnitTierText();
		unitSpecializationUI.SetValueWithoutNotify(constructedUnit.GetSpecialization());

		//Equipment list and set to 0.
		equipmentTypeUI.ClearOptions();
		List<string> eqNames = new();

		if (constructedUnit.SideB) {
			eqNames = EquipmentManager.equipmentFriendly[unitDomain].Select(e => e.equipmentName).ToList();
			equipmentTemplates = EquipmentManager.equipmentFriendly[unitDomain];
		} else {
			eqNames = EquipmentManager.equipmentHostile[unitDomain].Select(e => e.equipmentName).ToList();
			equipmentTemplates = EquipmentManager.equipmentHostile[unitDomain];
		}

		equipmentTypeUI.AddOptions(eqNames);
		SetEquipmentType(0);
		SetEquipmentAmount(1);
	}
	#endregion

	#region Modifying Unit Equipment
	/// <summary>
	/// Method sets requested equipment amount.
	/// </summary>
	/// <param name="amount">int in a string</param>
	public void SetEquipmentAmount(string amount) {
		if (string.IsNullOrEmpty(amount)) {
			ApplicationController.generalPopup.PopUp("Amount must be greater than 0", 5);
			SetEquipmentAmount(1);
		} else if (int.TryParse(amount, out int result)) {
			SetEquipmentAmount(result);
		}
	}
	/// <summary>
	/// Method sets requested equipment amount.
	/// </summary>
	/// <param name="newAmount">int amount</param>
	public void SetEquipmentAmount(int newAmount) { 
		if (newAmount > 0) {
			constructedEquipment.Amount = newAmount;
			equipmentAmountUI.text = newAmount.ToString();
			if (ApplicationController.isDebug) Debug.Log("Set equipment amount to " + newAmount);
			UpdateStatLabels();
		}
	}
	/// <summary>
	/// Method sets requested equipment type.
	/// </summary>
	/// <param name="type"></param>
	public void SetEquipmentType(int type) {
		constructedEquipment = equipmentTemplates[type];
		UpdateStatLabels();
		if (ApplicationController.isDebug) Debug.Log("Set equipment type to " + constructedEquipment.equipmentName);
	}

	/// <summary>
	/// Method removes equipment from unit and refunds it.
	/// </summary>
	/// <param name="equipment"></param>
	/// <param name="refund"></param>
	public void RemoveEquipment(Equipment equipment, bool refund = false) {
		if (refund) if (SheetSync.UpdatePoints(equipment.cost * equipment.Amount)) return;
		if (ApplicationController.isDebug) Debug.Log("Removing equipment " + equipment);
		constructedUnit.RemoveEquipment(equipment);
		UpdateOptions();
	}
	/// <summary>
	/// Method adds equipment to unit if it is not already present and pays for it.
	/// </summary>
	public void BuyEquipment() { AddEquipment(true); }
	/// <summary>
	/// Method adds equipment to unit if it is not already present.
	/// </summary>
	public void AddEquipment() { AddEquipment(false); }
	private void AddEquipment(bool refund) {
		foreach (Equipment equipment in constructedUnit.equipmentList) {
			if (equipment.equipmentName == constructedEquipment.equipmentName) {
				ApplicationController.generalPopup.GetComponent<UIPopup>().PopUp("Equipment already added", 5);
				return;
			}
		}
		if ((constructedEquipment.Amount * constructedEquipment.costSupply) + constructedUnit.equipmentList.Sum(e => e.costSupply * e.Amount) > SheetSync.maximalSupply) {
			ApplicationController.generalPopup.GetComponent<UIPopup>().PopUp("Unit would exceed maximal supply limit!", 5);
			return;
		}
		if (constructedEquipment.Amount * constructedEquipment.cost + constructedUnit.equipmentList.Sum(e => e.cost * e.Amount) > (ApplicationController.isSideB ? SheetSync.pointsB : SheetSync.pointsA)) {
			ApplicationController.generalPopup.GetComponent<UIPopup>().PopUp("Unit cost would exceed your faction points limit!", 5);
			return;
		}

		if (ApplicationController.isDebug) Debug.Log("Adding equipment " + constructedEquipment);

		Equipment newEquipment = EquipmentManager.CreateEquipment(constructedEquipment, Convert.ToInt16(equipmentAmountUI.text));
		if (refund) SheetSync.UpdatePoints(-newEquipment.cost * newEquipment.Amount);
		constructedUnit.AddEquipment(newEquipment);
		AddEquipmentUI(newEquipment);
		UpdateOptions();
	}
	/// <summary>
	/// Method creates new equipment UI element.
	/// </summary>
	/// <param name="equipment"></param>
	private void AddEquipmentUI(Equipment equipment) {
		GameObject equipmentLabel = Instantiate(equipmentBarTemplateUI, equipmentListUI.transform);
		equipmentLabel.transform.GetChild(0).GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"[A:{equipment.Amount,-3}][P:{equipment.Amount * equipment.cost}] {equipment.equipmentName}";

		Button sellButton = equipmentLabel.transform.GetChild(1).GetComponent<Button>();
		Button deleteButton = equipmentLabel.transform.GetChild(2).GetComponent<Button>();

		if (!ApplicationController.isAdmin) {
			Destroy(deleteButton.gameObject);
		}

		deleteButton.onClick.AddListener(() => {
			//No vehicle-less units.
			if (constructedUnit.equipmentList.Count > 1) {
				RemoveEquipment(equipment);
				Destroy(equipmentLabel);
			} else {
				ApplicationController.generalPopup.PopUp("Cannot remove last vehicle!");
			}
		});

		sellButton.onClick.AddListener(() => {
			if (constructedUnit.equipmentList.Count > 1) {
				RemoveEquipment(equipment, true);
				Destroy(equipmentLabel);
			} else {
				ApplicationController.generalPopup.PopUp("Cannot remove last vehicle!");
			}
		});
	}
	#endregion

	#region Modifying Unit
	/// <summary>
	/// Updates unit specialization and respective UI elements.
	/// </summary>
	/// <param name="i"></param>
	public void UpdateSpecialization(int i) {
		constructedUnit.ChangeSpecialization(i);
		unitSpecializationUI.SetValueWithoutNotify(i);
		unitIconTierLabelUI.text = constructedUnit.GetUnitTierText();
		if (ApplicationController.isDebug) Debug.Log("Changed constructed unit specialization to " + i);
	}
	/// <summary>
	/// Updates unit movement-protection and respective UI elements.
	/// </summary>
	/// <param name="i"></param>
	public void UpdateProtection(int i) {
		unitMovementTypeUI.SetValueWithoutNotify(i);
		if (groundUnit != null) GetGroundUnit().ChangeSpecialization((GroundProtectionType)i);
		if (ApplicationController.isDebug) Debug.Log("Changed constructed unit movement to " + (GroundProtectionType)i);
	}
	/// <summary>
	/// Updates unit transportation type and respective UI elements.
	/// </summary>
	/// <param name="i"></param>
	public void UpdateTransportation(int i) {
		unitTransportTypeUI.SetValueWithoutNotify(i);
		if (groundUnit != null) GetGroundUnit().ChangeSpecialization((GroundTransportType)i);
		if (ApplicationController.isDebug) Debug.Log("Changed constructed unit transport to " + (GroundTransportType)i);
	}
	/// <summary>
	/// Updates unit name and respective UI elements.
	/// </summary>
	/// <param name="identification"></param>
	public void UpdateName(string identification) {
		if (identification != "") {
			constructedUnit.SetName(identification);
			unitNameUI.text = identification;
		}
	}
	/// <summary>
	/// Updates unit position and startPosition.
	/// </summary>
	/// <param name="position"></param>
	public void UpdatePosition(Vector3 position) {
		position = new Vector3(position.x, position.y, -0.15f);
		constructedUnit.transform.position = position;
		constructedUnit.StartPosition = position;
		if (ApplicationController.isDebug) Debug.Log("Changed constructed unit position to " + position);
	}
	/// <summary>
	/// Updates unit affiliation to either side and respective UI elements.
	/// </summary>
	/// <param name="sideB"></param>
	public void UpdateAffiliation(bool sideB) {
		constructedUnit.ChangeAffiliation(sideB);
		if (ApplicationController.isDebug) Debug.Log("Changed constructed unit affiliation to B " + sideB);
	}
	/// <summary>
	/// Updates unit altitude and respective UI elements.
	/// </summary>
	/// <param name="altitude"></param>
	public void UpdateAltitude(string altitude) {
		if (aerialUnit != null) aerialUnit.altitude = int.Parse(altitude);
		unitAltitudeUI.SetTextWithoutNotify(altitude);
		if (ApplicationController.isDebug) Debug.Log("Changed constructed unit altitude set to " + altitude);
	}
	#endregion

	#region Menu loading and closing
	/// <summary>
	/// Method closes menu, modifying or spawning the unit as a result.
	/// </summary>
	public void Close() {
		if (constructedUnit.equipmentList.Count == 0) ApplicationController.generalPopup.PopUp("You need at least one vehicle/equipment!");
		else Clean();
	}

	/// <summary>
	/// Method closes menu, destroying the unit.
	/// </summary>
	public void DespawnUnit() {
		if (!Editing && constructedUnit != null) UnitManager.Instance.Despawn(constructedUnit.gameObject, true);
		if (ApplicationController.isDebug) Debug.Log("Unit editor canceling.");
		Clean();
	}

	/// <summary>
	/// Method cleans all unit data and closes the menu.
	/// </summary>
	private void Clean() {
		aerialUnit = null;
		navalUnit = null;
		groundUnit = null;
		constructedUnit = null;
		ClearEquipment();
		gameObject.SetActive(false);
		if (ApplicationController.isDebug) Debug.Log("Unit editor closed.");
		//Saving any edits or spawns to the server.
		if (ApplicationController.isController) {
			ApplicationController.Instance.server.SaveUnits();
		}
	}

	/// <summary>
	/// Method clears all equipment UI elements Objects.
	/// </summary>
	private void ClearEquipment() {
		for (int i = 0; i < equipmentListUI.transform.childCount; i++) Destroy(equipmentListUI.transform.GetChild(i).gameObject);
	}

	/// <summary>
	/// Method updateds the whole Menu UI domain and reloads the UI elements.
	/// </summary>
	/// <param name="domain"></param>
	public void UpdateDomain(int domain) {
		unitDomain = domain;
		unitDomainUI.SetValueWithoutNotify(domain);
		LoadUIOptions();
		if (ApplicationController.isDebug) Debug.Log("Unit editor domain changed to " + domain);
	}

	/// <summary>
	/// Method updates unit to edit.
	/// </summary>
	/// <param name="unit"></param>
	public void UpdateUnit(Unit unit) {
		Editing = true;
		if (unit.GetType() == typeof(GroundUnit)) SetGroundUnit((GroundUnit)unit);
		else if (unit.GetType() == typeof(AerialUnit)) SetAerialUnit((AerialUnit)unit);
		else SetNavalUnit((NavalUnit)unit);
		foreach (Equipment equipment in unit.equipmentList) AddEquipmentUI(equipment);
		if (ApplicationController.isDebug) Debug.Log("Unit editor opened.");
	}

	/// <summary>
	/// Method creates unit to edit.
	/// </summary>
	/// <param name="unit"></param>
	public void UpdateUnit(int domain) {
		Editing = false;
		string newIdentifier = UnitManager.GenerateName(domain);
		if (domain == 0) {
			SetGroundUnit((GroundUnit)UnitManager.Instance.SpawnUnit(constructedUnit != null ? constructedUnit.transform.position : Vector3.zero, UnitTier.Company, newIdentifier, new List<Equipment>(), false, 0, GroundProtectionType.Motorized, GroundTransportType.None, domain));
		} else if (domain == 1) {
			SetAerialUnit((AerialUnit)UnitManager.Instance.SpawnUnit(constructedUnit != null ? constructedUnit.transform.position : Vector3.zero, UnitTier.Company, newIdentifier, new List<Equipment>(), false, 0, GroundProtectionType.Motorized, GroundTransportType.None, domain));

		} else {
			SetNavalUnit((NavalUnit)UnitManager.Instance.SpawnUnit(constructedUnit != null ? constructedUnit.transform.position : Vector3.zero, UnitTier.Company, newIdentifier, new List<Equipment>(), false, 0, GroundProtectionType.Motorized, GroundTransportType.None, domain));
		}
		SetEquipmentAmount(1);
		if (ApplicationController.isDebug) Debug.Log("Unit editor spawner opened.");
	}
	#endregion
}