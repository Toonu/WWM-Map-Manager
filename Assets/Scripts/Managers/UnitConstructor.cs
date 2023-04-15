using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
	private int higherUnitIdentifierNumber = 0;

	private TMP_Dropdown tierUI;
	private TMP_Dropdown domainUI;
	private TMP_Dropdown movementUI;
	private TMP_Dropdown transportUI;
	private TMP_Dropdown specializationUI;
	private TMP_InputField nameUI;
	private UnitManager manager;
	private ApplicationController controller;
	private EquipmentManager equipmentManager;


	public void Awake() {
		domainUI = transform.GetChild(1).Find("UnitDomain").GetComponent<TMP_Dropdown>();
		specializationUI = transform.GetChild(1).Find("UnitSpecialization").GetComponent<TMP_Dropdown>();
		nameUI = transform.GetChild(1).Find("UnitName").GetComponent<TMP_InputField>();
		tierUI = transform.GetChild(2).Find("UnitTier").GetComponent<TMP_Dropdown>();
		movementUI = transform.GetChild(3).Find("MovementType").GetComponent<TMP_Dropdown>();
		transportUI = transform.GetChild(3).Find("TransportType").GetComponent<TMP_Dropdown>();

		manager = GameObject.FindWithTag("Units").GetComponent<UnitManager>();
		controller = GameObject.FindWithTag("GameController").GetComponent<ApplicationController>();
		equipmentManager = GameObject.FindWithTag("Equipment").GetComponent<EquipmentManager>();
	}

	private void OnEnable()	{
		UpdateUI();
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
					options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(manager.aerialSpecialization[i],
						new Rect(0, 0, manager.aerialSpecialization[i].width, manager.aerialSpecialization[i].height), new Vector2(0.5f, 0.5f))));
				}
				specializationUI.value = (int)GetAerialUnit().specialization;
				break;
			case 2:
				enumNames = Enum.GetNames(typeof(NavalSpecialization));
				for (int i = 0; i < enumNames.Length; i++) {
					options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(manager.navalSpecialization[i],
					new Rect(0, 0, manager.navalSpecialization[i].width, manager.navalSpecialization[i].height), new Vector2(0.5f, 0.5f))));
				}
				specializationUI.value = (int)GetNavalUnit().specialization;
				break;
			default:
				enumNames = Enum.GetNames(typeof(GroundSpecialization));
				for (int i = 0; i < enumNames.Length; i++) {
					options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(manager.groundSpecialization[i],
						new Rect(0, 0, manager.groundSpecialization[i].width, manager.groundSpecialization[i].height), new Vector2(0.5f, 0.5f))));
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
				if (controller.admin) {
					movementUI.gameObject.SetActive(true);
					enumNames = Enum.GetNames(typeof(GroundMovementType));
					options = new List<TMP_Dropdown.OptionData>();
					for (int i = 0; i < enumNames.Length; i++) {
						options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(manager.movementType[i],
							new Rect(0, 0, manager.movementType[i].width, manager.movementType[i].height), new Vector2(0.5f, 0.5f))));
					}
					movementUI.ClearOptions();
					movementUI.AddOptions(options);
					transportUI.gameObject.SetActive(true);
					enumNames = Enum.GetNames(typeof(GroundTransportType));
					options = new List<TMP_Dropdown.OptionData>();
					for (int i = 0; i < enumNames.Length; i++) {
						options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(manager.transportType[i],
							new Rect(0, 0, manager.transportType[i].width, manager.transportType[i].height), new Vector2(0.5f, 0.5f))));
					}
					transportUI.ClearOptions();
					transportUI.AddOptions(options);
				}
				break;
			default:
				movementUI.gameObject.SetActive(false);
				transportUI.gameObject.SetActive(false);
				break;
		}

		enumNames = Enum.GetNames(typeof(UnitTier));
		tierUI.ClearOptions();
		tierUI.AddOptions(enumNames.ToList());
		nameUI.text = (manager.GetLast() + 1).ToString();
		tierUI.value = (int)constructedUnit.GetUnitTier();


		tierUI.gameObject.SetActive(controller.admin);
		nameUI.gameObject.SetActive(controller.admin);
		movementUI.gameObject.SetActive(controller.admin);
		transportUI.gameObject.SetActive(controller.admin);
		specializationUI.gameObject.SetActive (controller.admin);
	}

	public void UpdateSpecialization(int i) {
		constructedUnit.ChangeSpecialization(i);
	}
	public void UpdateMovementModifier(int i) {
		GroundMovementType movementModifier = (GroundMovementType)i;
		GetGroundUnit().ChangeSpecialization(movementModifier);
	}
	public void UpdateTransportModifier(int i) {
		GroundTransportType transportModifier = (GroundTransportType)i;
		GetGroundUnit().ChangeSpecialization(transportModifier);
	}
	public void UpdateTier(int i) {
		constructedUnit.SetUnitTier(i);
		UpdateName(constructedUnit.name);
		nameUI.text = constructedUnit.name;
	}
	public void UpdateName(string identification) {
		constructedUnit.ChangeName(identification);
	}
	
	public void UpdateDomain(int domain) {
		unitDomain = domain;
		manager.Despawn(constructedUnit.gameObject);
		UpdateUnit();
	}
	
	public void UpdatePosition(Vector3 position) {
		constructedUnit.transform.position = position;
	}
	public void UpdateAffiliation(bool sideB) {
		constructedUnit.ChangeAffiliation(sideB);
	}
	public void Close() {
		domainUI.value = 0;
		unitEquipment.Clear();
	}
	public void Cancel() {
		manager.Despawn(constructedUnit.gameObject);
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
		SetGroundUnit((GroundUnit)GameObject.FindWithTag("Units").GetComponent<UnitManager>().SpawnUnit(Vector3.zero, UnitTier.Company, "0", unitEquipment, false, 2, GroundMovementType.None, GroundTransportType.None, 0));
		unitDomain = 0;
	}
}