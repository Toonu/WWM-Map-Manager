using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitConstructor : MonoBehaviour {
	internal int unitDomain;
	internal Unit constructedUnit;

	private GroundUnit groundUnit;
	private AerialUnit aerialUnit;
	private NavalUnit navalUnit;

	public GroundUnit GetGroundUnit() => groundUnit;
	internal void SetGroundUnit(GroundUnit value) { groundUnit = value; constructedUnit = value; }

	public AerialUnit GetAerialUnit() => aerialUnit;
	internal void SetAerialUnit(AerialUnit value) { aerialUnit = value; constructedUnit = value; }

	public NavalUnit GetNavalUnit() => navalUnit;
	internal void SetNavalUnit(NavalUnit value) { navalUnit = value; constructedUnit = value; }

	internal List<Equipment> unitEquipment = new List<Equipment>();
	//private int higherUnitIdentifierNumber = 0;

	private TextMeshProUGUI tierUI;
	private TMP_Dropdown domainUI;
	private TMP_Dropdown movementUI;
	private TMP_Dropdown transportUI;
	private TMP_Dropdown specializationUI;
	private TMP_InputField nameUI;
	private GameObject imageUI;
	private EquipmentManager equipmentManager;


	public void Awake() {
		domainUI = transform.GetChild(1).Find("UnitDomain").GetComponent<TMP_Dropdown>();
		specializationUI = transform.GetChild(1).Find("UnitSpecialization").GetComponent<TMP_Dropdown>();
		nameUI = transform.GetChild(1).Find("UnitName").GetComponent<TMP_InputField>();
		movementUI = transform.GetChild(1).Find("MovementType").GetComponent<TMP_Dropdown>();
		transportUI = transform.GetChild(1).Find("TransportType").GetComponent<TMP_Dropdown>();
		tierUI = transform.GetChild(2).GetChild(3).GetComponent<TextMeshProUGUI>();
		imageUI = transform.GetChild(2).GetChild(4).gameObject;

		equipmentManager = transform.parent.GetComponent<EquipmentManager>();
	}

	private void OnEnable() {
		UpdateUI();
		equipmentManager.AddEquipmentList(unitEquipment);
		equipmentManager.UpdateUI();
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

		nameUI.text = (UnitManager.Instance.GetLast() + 1).ToString();
		tierUI.text = constructedUnit.GetUnitTierText();


		tierUI.gameObject.SetActive(ApplicationController.admin);
		nameUI.gameObject.SetActive(ApplicationController.admin);
		specializationUI.gameObject.SetActive(ApplicationController.admin);
		movementUI.gameObject.SetActive(ApplicationController.admin);
		transportUI.gameObject.SetActive(ApplicationController.admin);
		domainUI.gameObject.SetActive(ApplicationController.admin);
	}

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
		}
	}

	public void UpdateDomain(int domain) {
		unitDomain = domain;
		domainUI.SetValueWithoutNotify(domain);
		DespawnUnit();
		UpdateUnit();
		OnEnable();
	}

	public void UpdatePosition(Vector3 position) {
		constructedUnit.transform.position = position;
	}
	public void UpdateAffiliation(bool sideB) {
		constructedUnit.ChangeAffiliation(sideB);
	}
	public void Close() {
		unitEquipment.Clear();
		aerialUnit = null;
		navalUnit = null;
		groundUnit = null;
		constructedUnit = null;
	}
	public void DespawnUnit() {
		if (constructedUnit != null) {
			UnitManager.Instance.Despawn(constructedUnit.gameObject);
		}
		Close();
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
	}

	public void UpdateUnit() {
		switch (unitDomain) {
			case 1:
			SetAerialUnit((AerialUnit)UnitManager.Instance.SpawnUnit(Vector3.zero, UnitTier.Company, UnitManager.Instance.GetLast().ToString(), unitEquipment, false, 0, GroundMovementType.Motorized, GroundTransportType.None, unitDomain));
			break;
			case 2:
			SetNavalUnit((NavalUnit)UnitManager.Instance.SpawnUnit(Vector3.zero, UnitTier.Company, UnitManager.Instance.GetLast().ToString(), unitEquipment, false, 0, GroundMovementType.Motorized, GroundTransportType.None, unitDomain));
			break;
			default:
			SetGroundUnit((GroundUnit)UnitManager.Instance.SpawnUnit(Vector3.zero, UnitTier.Company, UnitManager.Instance.GetLast().ToString(), unitEquipment, false, 0, GroundMovementType.Motorized, GroundTransportType.None, unitDomain));
			break;
		}

	}
}