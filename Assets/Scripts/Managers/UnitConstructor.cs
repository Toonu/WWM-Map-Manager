using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
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
	internal void SetGroundUnit(GroundUnit newUnit) { 
		if (constructedUnit != null) {
			UnitManager.Instance.Despawn(constructedUnit.gameObject);
		}
		UpdateDomain(0);
		groundUnit = newUnit; 
		constructedUnit = newUnit;
		UpdateOptions();
	}

	public AerialUnit GetAerialUnit() => aerialUnit;
	internal void SetAerialUnit(AerialUnit newUnit) {
		if (constructedUnit != null) {
			UnitManager.Instance.Despawn(constructedUnit.gameObject);
		}
		UpdateDomain(1);
		aerialUnit = newUnit; 
		constructedUnit = newUnit;
		UpdateOptions();
	}

	public NavalUnit GetNavalUnit() => navalUnit;
	internal void SetNavalUnit(NavalUnit newUnit) {
		if (constructedUnit != null) {
			UnitManager.Instance.Despawn(constructedUnit.gameObject);
		}
		UpdateDomain(2);
		navalUnit = newUnit;
		constructedUnit = newUnit;
		UpdateOptions();
	}

	public static bool Editing = false;

	//private int higherUnitIdentifierNumber = 0;
	#endregion
	#region UI Attributes
	private TextMeshProUGUI tierUI;
	private TMP_Dropdown domainUI;
	private TMP_Dropdown movementUI;
	private TMP_Dropdown transportUI;
	private TMP_Dropdown specializationUI;
	private TMP_InputField nameUI;
	private GameObject imageUI;
	private UITextFloatAppender costLabelUI;
	private UITextFloatAppender sightLabelUI;
	private UITextFloatAppender rangeLabelUI;
	#endregion
	#region EquipmentUI Attributes
	private TMP_InputField equipmentAmountUI;
	private TMP_Dropdown equipmentTypeUI;
	private UITextFloatAppender equipmentCostLabelUI;
	private UITextFloatAppender equipmentSightLabelUI;
	private UITextFloatAppender equipmentRangeLabelUI;
	private GameObject equipmentPanelsUI;
	public GameObject equipmentPanel;
	#endregion

	#region UI Updates
	public void Awake() {
		domainUI = transform.GetChild(1).Find("UnitDomain").GetComponent<TMP_Dropdown>();
		specializationUI = transform.GetChild(1).Find("UnitSpecialization").GetComponent<TMP_Dropdown>();
		nameUI = transform.GetChild(1).Find("UnitName").GetComponent<TMP_InputField>();
		movementUI = transform.GetChild(1).Find("MovementType").GetComponent<TMP_Dropdown>();
		transportUI = transform.GetChild(1).Find("TransportType").GetComponent<TMP_Dropdown>();
		tierUI = transform.GetChild(2).GetChild(3).GetComponent<TextMeshProUGUI>();
		imageUI = transform.GetChild(2).GetChild(4).gameObject;
		costLabelUI = transform.GetChild(2).Find("Cost").GetComponent<UITextFloatAppender>();
		sightLabelUI = transform.GetChild(2).Find("Sight").GetComponent<UITextFloatAppender>();
		rangeLabelUI = transform.GetChild(2).Find("Range").GetComponent<UITextFloatAppender>();

		equipmentAmountUI = transform.GetChild(3).Find("EqAmount").GetComponent<TMP_InputField>();
		equipmentCostLabelUI = transform.GetChild(3).Find("EqCost").GetComponent<UITextFloatAppender>();
		equipmentTypeUI = transform.GetChild(3).Find("EqType").GetComponent<TMP_Dropdown>();
		equipmentSightLabelUI = transform.GetChild(3).Find("EqSight").GetComponent<UITextFloatAppender>();
		equipmentRangeLabelUI = transform.GetChild(3).Find("EqRange").GetComponent<UITextFloatAppender>();
		equipmentPanelsUI = transform.GetChild(4).gameObject;
	}
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

		specializationUI.ClearOptions();
		specializationUI.AddOptions(options);

		//Generate new name if creating new unit
		if (!Editing) {
			UnityEngine.Random.Range(0, 100);
            System.Random random = new();
		}
		nameUI.gameObject.SetActive(ApplicationController.isAdmin);
		specializationUI.gameObject.SetActive(ApplicationController.isAdmin);
		movementUI.gameObject.SetActive(ApplicationController.isAdmin);
		transportUI.gameObject.SetActive(ApplicationController.isAdmin);
		domainUI.gameObject.SetActive(ApplicationController.isAdmin);

		//Removal of non-domain attributes and images
		if (unitDomain == 0 && ApplicationController.isAdmin) {
			movementUI.gameObject.SetActive(true);
			enumNames = Enum.GetNames(typeof(GroundMovementType));
			options = new List<TMP_Dropdown.OptionData>();
			for (int i = 0; i < enumNames.Length; i++) {
				options.Add(new TMP_Dropdown.OptionData(enumNames[i], UnitManager.Instance.movementType[i]));
			}
			movementUI.ClearOptions();
			movementUI.AddOptions(options);
			transportUI.gameObject.SetActive(true);
			enumNames = Enum.GetNames(typeof(GroundTransportType));
			options = new List<TMP_Dropdown.OptionData>();
			for (int i = 0; i < enumNames.Length; i++) {
				options.Add(new TMP_Dropdown.OptionData(enumNames[i], UnitManager.Instance.transportType[i]));
			}
			transportUI.ClearOptions();
			transportUI.AddOptions(options);
			imageUI.transform.GetChild(2).gameObject.SetActive(true);
		} else {
			movementUI.gameObject.SetActive(false);
			transportUI.gameObject.SetActive(false);
			imageUI.transform.GetChild(2).gameObject.SetActive(false);
		}
	}
	private void UpdateStatLabels() {
		equipmentCostLabelUI.UpdateText(constructedEquipment.cost * constructedEquipment.Amount);
		equipmentSightLabelUI.UpdateText(constructedEquipment.sightRange);
		equipmentRangeLabelUI.UpdateText(constructedEquipment.movementRange);

		if (constructedUnit.equipmentList.Count != 0) {
			costLabelUI.UpdateText(constructedEquipment.cost * constructedEquipment.Amount + constructedUnit.equipmentList.Sum(e => e.cost));
			float min = constructedUnit.equipmentList.Min(e => e.movementRange);
			float max = constructedUnit.equipmentList.Max(e => e.sightRange);
			sightLabelUI.UpdateText(max < constructedEquipment.sightRange ? constructedEquipment.sightRange : max);
			rangeLabelUI.UpdateText(min > constructedEquipment.movementRange ? constructedEquipment.movementRange : min);
		} else {
			costLabelUI.UpdateText(constructedEquipment.cost * constructedEquipment.Amount);
			sightLabelUI.UpdateText(constructedEquipment.sightRange);
			rangeLabelUI.UpdateText(constructedEquipment.movementRange);
		}
	}
	private void UpdateOptions() {
		//Returns the most numerous armour-traction type.
		if (groundUnit != null) {
			movementUI.SetValueWithoutNotify((int)groundUnit.movementModifier);
			transportUI.SetValueWithoutNotify((int)groundUnit.transportModifier);
		}

		nameUI.text = constructedUnit.name;
		tierUI.text = constructedUnit.GetUnitTierText();
		specializationUI.SetValueWithoutNotify(constructedUnit.GetSpecialization());

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
	}
	#endregion

	#region Modifying Unit Equipment
	public void SetEquipmentAmount(string amount) {
		if (string.IsNullOrEmpty(amount)) {
			ApplicationController.generalPopup.PopUp("Amount must be greater than 0", 5);
			SetEquipmentAmount(1);
		} else {
			SetEquipmentAmount(Convert.ToInt16(amount));
		}
	}
	public void SetEquipmentAmount(int newAmount) {
		constructedEquipment.Amount = newAmount;
		equipmentAmountUI.text = newAmount.ToString();
		Debug.Log("Set equipment amount to " + newAmount);
		UpdateStatLabels();
	}
	public void SetEquipmentType(int type) {
		constructedEquipment = equipmentTemplates[type];
		Debug.Log("Set equipment type to " + constructedEquipment.equipmentName);
	}

	public void RemoveEquipment(Equipment equipment, bool refund = false) {
		if (refund) SheetSync.UpdatePoints(equipment.cost * equipment.Amount);
		Debug.Log("Removing equipment " + equipment);
		constructedUnit.RemoveEquipment(equipment);
		UpdateOptions();
	}
	public void AddEquipment() {
		foreach (Equipment equipment in constructedUnit.equipmentList) {
			if (equipment.equipmentName == constructedEquipment.equipmentName) {
				ApplicationController.generalPopup.GetComponent<UIPopup>().PopUp("Equipment already added", 5);
				return;
			}
		}
		Debug.Log("Adding equipment " + constructedEquipment);
		
		Equipment newEquipment = EquipmentManager.CreateEquipment(constructedEquipment, Convert.ToInt16(equipmentAmountUI.text));
		SheetSync.UpdatePoints(-newEquipment.cost * newEquipment.Amount);
		constructedUnit.AddEquipment(newEquipment);
		AddEquipmentUI(newEquipment);
		UpdateOptions();
	}
	private void AddEquipmentUI(Equipment equipment) {
		GameObject equipmentLabel = Instantiate(equipmentPanel, equipmentPanelsUI.transform);
		equipmentLabel.transform.GetChild(0).GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"[{equipment.Amount,-3}] {equipment.equipmentName}";

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
	public void UpdateSpecialization(int i) {
		constructedUnit.ChangeSpecialization(i);
		specializationUI.SetValueWithoutNotify(i);
		tierUI.text = constructedUnit.GetUnitTierText();
	}
	public void UpdateMovementModifier(int i) {
		movementUI.SetValueWithoutNotify(i);
		if (groundUnit != null) GetGroundUnit().ChangeSpecialization((GroundMovementType)i);
	}
	public void UpdateTransportModifier(int i) {
		transportUI.SetValueWithoutNotify(i);
		if (groundUnit != null) GetGroundUnit().ChangeSpecialization((GroundTransportType)i);
	}
	public void UpdateName(string identification) {
		if (identification != "") {
			constructedUnit.SetName(identification);
			nameUI.text = identification;
		}
	}
	public void UpdatePosition(Vector3 position) {
		position = new Vector3(position.x, position.y, -0.15f);
		constructedUnit.transform.position = position;
		constructedUnit.StartPosition = position;
	}
	public void UpdateAffiliation(bool sideB) {
		constructedUnit.ChangeAffiliation(sideB);
	}
	#endregion

	#region Menu loading and closing
	public void Close() {
		if (constructedUnit.equipmentList.Count == 0) ApplicationController.generalPopup.PopUp("You need at least one vehicle/equipment!");
		else Clean();
	}
	
	public void DespawnUnit() {
		if (!Editing && constructedUnit != null) UnitManager.Instance.Despawn(constructedUnit.gameObject);
		Debug.Log("Unit editor canceling.");
		Clean();
	}

	private void Clean() {
		aerialUnit = null;
		navalUnit = null;
		groundUnit = null;
		constructedUnit = null;
		for (int i = 0; i < equipmentPanelsUI.transform.childCount; i++) Destroy(equipmentPanelsUI.transform.GetChild(i).gameObject);
		gameObject.SetActive(false);
		Debug.Log("Unit editor closed.");
	}

	public void UpdateDomain(int domain) {
		unitDomain = domain;
		domainUI.SetValueWithoutNotify(domain);
		LoadUIOptions();
		Debug.Log("Unit editor domain changed to " + domain);
	}

	public void UpdateUnit(Unit unit) {
		Editing = true;
		if (unit.GetType() == typeof(GroundUnit)) SetGroundUnit((GroundUnit)unit); 
		else if (unit.GetType() == typeof(AerialUnit)) SetAerialUnit((AerialUnit)unit); 
		else SetNavalUnit((NavalUnit)unit);
		foreach (Equipment equipment in unit.equipmentList) AddEquipmentUI(equipment);
		Debug.Log("Unit editor opened.");
	}

	public void UpdateUnit(int domain) {
		Editing = false;
		string newIdentifier = UnitManager.GenerateName(domain);
		if (domain == 0) {
			SetGroundUnit((GroundUnit)UnitManager.Instance.SpawnUnit(constructedUnit != null ? constructedUnit.StartPosition : Vector3.zero, UnitTier.Company, newIdentifier, new List<Equipment>(), false, 0, GroundMovementType.Motorized, GroundTransportType.None, domain));
		} else if (domain == 1) {
			SetAerialUnit((AerialUnit)UnitManager.Instance.SpawnUnit(constructedUnit != null ? constructedUnit.StartPosition : Vector3.zero, UnitTier.Company, newIdentifier, new List<Equipment>(), false, 0, GroundMovementType.Motorized, GroundTransportType.None, domain));
		} else {
			SetNavalUnit((NavalUnit)UnitManager.Instance.SpawnUnit(constructedUnit != null ? constructedUnit.StartPosition : Vector3.zero, UnitTier.Company, newIdentifier, new List<Equipment>(), false, 0, GroundMovementType.Motorized, GroundTransportType.None, domain));
		}
		Debug.Log("Unit editor spawner opened.");
	}
	#endregion
}