using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitConstructor : MonoBehaviour {
	internal int unitDomain;
	internal Unit constructedUnit;
	internal Equipment constructedEquipment;

	private GroundUnit groundUnit;
	private AerialUnit aerialUnit;
	private NavalUnit navalUnit;
	private List<Equipment> equipmentTemplates = new List<Equipment>();

	public GroundUnit GetGroundUnit() => groundUnit;
	internal void SetGroundUnit(GroundUnit value) { groundUnit = value; constructedUnit = value; }

	public AerialUnit GetAerialUnit() => aerialUnit;
	internal void SetAerialUnit(AerialUnit value) { aerialUnit = value; constructedUnit = value; }

	public NavalUnit GetNavalUnit() => navalUnit;
	internal void SetNavalUnit(NavalUnit value) { navalUnit = value; constructedUnit = value; }

	//private int higherUnitIdentifierNumber = 0;

	#region UI
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
	#region EquipmentUI
	private TMP_InputField equipmentAmountUI;
	private TMP_Dropdown equipmentTypeUI;
	private UITextFloatAppender equipmentCostLabelUI;
	private UITextFloatAppender equipmentSightLabelUI;
	private UITextFloatAppender equipmentRangeLabelUI;
	private GameObject equipmentPanelsUI;
	public GameObject equipmentPanel;
	#endregion

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

	private void OnEnable() {
		UpdateUI();
		UpdateEquipmentUI();
	}

	public void UpdateUI() {
		string[] enumNames;
		List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

		//Specialization domain switch
		switch (unitDomain) {
			case 1:
			enumNames = Enum.GetNames(typeof(AerialSpecialization));
			for (int i = 0; i < enumNames.Length; i++) {
				options.Add(new TMP_Dropdown.OptionData(enumNames[i], UnitManager.Instance.aerialSpecialization[i]));
			}
			specializationUI.value = (int)GetAerialUnit().specialization;
			break;
			case 2:
			enumNames = Enum.GetNames(typeof(NavalSpecialization));
			for (int i = 0; i < enumNames.Length; i++) {
				options.Add(new TMP_Dropdown.OptionData(enumNames[i], UnitManager.Instance.navalSpecialization[i]));
			}
			specializationUI.value = (int)GetNavalUnit().specialization;
			break;
			default:
			enumNames = Enum.GetNames(typeof(GroundSpecialization));
			//HQ assignable only by admin
			for (int i = 0; i < enumNames.Length - (ApplicationController.admin ? 1 : 0); i++) {
				options.Add(new TMP_Dropdown.OptionData(enumNames[i], UnitManager.Instance.groundSpecialization[i]));
			}
			movementUI.value = (int)GetGroundUnit().movementModifier;
			transportUI.value = (int)GetGroundUnit().transportModifier;
			specializationUI.value = (int)GetGroundUnit().specialization;
			break;
		}

		specializationUI.ClearOptions();
		specializationUI.AddOptions(options);

		

		UpdateName(UnitManager.Instance.GetLast().ToString());
		tierUI.text = constructedUnit.GetUnitTierText();


		tierUI.gameObject.SetActive(ApplicationController.admin);
		nameUI.gameObject.SetActive(ApplicationController.admin);
		specializationUI.gameObject.SetActive(ApplicationController.admin);
		movementUI.gameObject.SetActive(ApplicationController.admin);
		transportUI.gameObject.SetActive(ApplicationController.admin);
		domainUI.gameObject.SetActive(ApplicationController.admin);

		//Removal of non-domain attributes
		switch (unitDomain) {
			case 0:
				if (ApplicationController.admin) {
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
				}
				break;
			default:
				movementUI.gameObject.SetActive(false);
				transportUI.gameObject.SetActive(false);
				imageUI.transform.GetChild(2).gameObject.SetActive(false);
				break;
		}	
	}

	#region Eq
	public void UpdateEquipmentUI() {
		equipmentTypeUI.ClearOptions();
		List<string> eqNames = new List<string>();
		if (constructedUnit.SideB) {
			eqNames = EquipmentManager.equipmentHostile[unitDomain].Select(e => e.equipmentName).ToList();
			equipmentTemplates = EquipmentManager.equipmentHostile[unitDomain];
		} else {
			eqNames = EquipmentManager.equipmentFriendly[unitDomain].Select(e => e.equipmentName).ToList();
			equipmentTemplates = EquipmentManager.equipmentFriendly[unitDomain];
		}
		equipmentTypeUI.AddOptions(eqNames);
		SetEquipmentType(0);
	}
	private void UpdateEquipmentStatisticsUI() {
		equipmentCostLabelUI.UpdateText(constructedEquipment.cost * constructedEquipment.Amount);
		equipmentSightLabelUI.UpdateText(constructedEquipment.sightRange);
		equipmentRangeLabelUI.UpdateText(constructedEquipment.movementRange);

		if (constructedUnit.equipmentList.Count != 0) {
			costLabelUI.UpdateText(constructedEquipment.cost * constructedEquipment.Amount + constructedUnit.equipmentList.Sum(e => e.cost));
			float min = constructedUnit.equipmentList.Min(e => e.movementRange);
			float max = constructedUnit.equipmentList.Max(e => e.sightRange);
			sightLabelUI.UpdateText(max < constructedEquipment.sightRange ? constructedEquipment.sightRange : max);
			rangeLabelUI.UpdateText(min > constructedEquipment.movementRange ? constructedEquipment.movementRange : min);

			if (unitDomain != 2) {
				//Returns the most numerous unit type in the Unit
				UpdateSpecialization(constructedUnit.equipmentList
				.GroupBy(equipment => equipment.specialization)
				.OrderByDescending(group => group.Count())
				.FirstOrDefault()?.Key ?? 0);
			}

			//Returns the most numerous armour-traction type.
			UpdateMovementModifier(constructedUnit.equipmentList
			.GroupBy(equipment => equipment.movement)
			.OrderByDescending(group => group.Count())
			.FirstOrDefault()?.Key ?? 0);

			int vehicles = constructedUnit.equipmentList.Count;

			int echelon = EnumUtil.GetUnitTier(unitDomain, vehicles);

			constructedUnit.SetUnitTier(echelon);

		} else {
			costLabelUI.UpdateText(constructedEquipment.cost * constructedEquipment.Amount);
			sightLabelUI.UpdateText(constructedEquipment.sightRange);
			rangeLabelUI.UpdateText(constructedEquipment.movementRange);
		}
	}

	public void SetEquipmentAmount(int newAmount) {
		constructedEquipment.Amount = newAmount;
		equipmentAmountUI.text = newAmount.ToString();
		UpdateEquipmentStatisticsUI();
	}
	public void SetEquipmentAmount(string amount) {
		if (string.IsNullOrEmpty(amount)) {
			ApplicationController.generalPopup.PopUp("Amount must be greater than 0", 5);
			SetEquipmentAmount(10);
		} else { 
			SetEquipmentAmount(Convert.ToInt16(amount)); 
		}
	}
	public void SetEquipmentType(int type) {
		constructedEquipment = equipmentTemplates[type];
		SetEquipmentAmount(constructedEquipment.Amount); 
	}

	public void RemoveEquipment(Equipment equipment, bool refund = false) {
		if (refund) SheetSync.UpdatePoints(equipment.cost * equipment.Amount);
		constructedUnit.RemoveEquipment(equipment);
	}

	public void AddEquipment() {
		foreach (Equipment equipment in constructedUnit.equipmentList) {
			if (equipment.equipmentName == constructedEquipment.equipmentName) {
				ApplicationController.generalPopup.GetComponent<UIPopup>().PopUp("Equipment already added", 5);
				return;
			}
		}
		
		Equipment newEquipment = EquipmentManager.CreateEquipment(constructedEquipment, Convert.ToInt16(equipmentAmountUI.text));
		SheetSync.UpdatePoints(-newEquipment.cost * newEquipment.Amount);
		constructedUnit.AddEquipment(newEquipment);
		AddEquipmentUI(newEquipment);
	}

	private void AddEquipmentUI(Equipment equipment) {
		GameObject equipmentLabel = Instantiate(equipmentPanel, equipmentPanelsUI.transform);
		equipmentLabel.transform.GetChild(0).GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"[{equipment.Amount,-3}] {equipment.equipmentName}";

		Button sellButton = equipmentLabel.transform.GetChild(1).GetComponent<Button>();
		Button deleteButton = equipmentLabel.transform.GetChild(2).GetComponent<Button>();

		if (!ApplicationController.admin) {
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

	public void UpdateSpecialization(int i) {
		constructedUnit.ChangeSpecialization(i);
		if (navalUnit != null) {
			constructedUnit.SetUnitTier(i + 5);
			tierUI.text = constructedUnit.GetUnitTierText();
		}
	}
	public void UpdateMovementModifier(int i) {
		GroundMovementType movementModifier = (GroundMovementType)i;
		GetGroundUnit().ChangeSpecialization(movementModifier);
	}
	public void UpdateTransportModifier(int i) {
		GroundTransportType transportModifier = (GroundTransportType)i;
		GetGroundUnit().ChangeSpecialization(transportModifier);
	}
	public void UpdateName(string identification) {
		if (identification != "") {
			constructedUnit.SetName(identification);
			nameUI.text = identification;
		}
	}

	public void UpdateDomain(int domain) {
		unitDomain = domain;
		domainUI.SetValueWithoutNotify(domain);
		if (constructedUnit != null) {
			DespawnUnit();
		}
		UpdateUnit();
		OnEnable();
	}

	public void UpdatePosition(Vector3 position) {
		position = new Vector3(position.x, position.y, -0.15f);
		constructedUnit.transform.position = position;
		constructedUnit.StartPosition = position;
	}
	public void UpdateAffiliation(bool sideB) {
		constructedUnit.ChangeAffiliation(sideB);
	}
	public void Close() {
		if (constructedUnit.equipmentList.Count == 0) {
			ApplicationController.generalPopup.PopUp("You need at least one vehicle/equipment!");
			return;
		}
		Clean();
	}
	private void Clean() {
		aerialUnit = null;
		navalUnit = null;
		groundUnit = null;
		constructedUnit = null;
		for (int i = 0; i < equipmentPanelsUI.transform.childCount; i++) {
			Destroy(equipmentPanelsUI.transform.GetChild(i).gameObject);
		}
		gameObject.SetActive(false);
		Debug.Log("Unit editor closed.");
	}

	public void DespawnUnit() {
		UnitManager.Instance.Despawn(constructedUnit.gameObject);
		Debug.Log("Unit editor canceling.");
		Clean();
	}


	public void UpdateUnit(Unit unit) {
		constructedUnit = unit;
		if (unit.GetType() == typeof(GroundUnit)) {
			SetGroundUnit((GroundUnit)unit);
			unitDomain = 0;
		} else if (unit.GetType() == typeof(AerialUnit)) {
			SetAerialUnit((AerialUnit)unit);
			unitDomain = 1;
		} else {
			SetNavalUnit((NavalUnit)unit);
			unitDomain = 2;
		}
		foreach (Equipment equipment in unit.equipmentList) {
			AddEquipmentUI(equipment);
		}
		Debug.Log("Unit editor opened.");
	}

	public void UpdateUnit() {
		switch (unitDomain) {
			case 1:
			SetAerialUnit((AerialUnit)UnitManager.Instance.SpawnUnit(Vector3.zero, UnitTier.Company, "1", new List<Equipment>(), false, 0, GroundMovementType.Motorized, GroundTransportType.None, unitDomain));
			break;
			case 2:
			SetNavalUnit((NavalUnit)UnitManager.Instance.SpawnUnit(Vector3.zero, UnitTier.Company, "1", new List<Equipment>(), false, 0, GroundMovementType.Motorized, GroundTransportType.None, unitDomain));
			break;
			default:
			SetGroundUnit((GroundUnit)UnitManager.Instance.SpawnUnit(Vector3.zero, UnitTier.Company, "1", new List<Equipment>(), false, 0, GroundMovementType.Motorized, GroundTransportType.None, unitDomain));
			break;
		}
		Debug.Log("Unit editor opened.");
	}
}