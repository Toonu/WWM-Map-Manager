using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class UnitEditor : MonoBehaviour {
    private int unitDomain;
	private string unitIdentificationNumber = "";
	private Vector3 unitPosition;

	internal Unit constructedUnit;
	public GroundUnit groundUnit { internal set { groundUnit = value; constructedUnit = value; } get { return groundUnit; } }
	public AerialUnit aerialUnit { internal set { aerialUnit = value; constructedUnit = value; } get { return aerialUnit; } }
	public NavalUnit navalUnit { internal set { navalUnit = value; constructedUnit = value; } get { return navalUnit; } }
	internal bool sideB;
	
	private int unitSpecialization = 0;
	private GroundMovementType unitMovementModifier = GroundMovementType.None;
	private GroundTransportType unitTransportModifier = GroundTransportType.None;
	private UnitTier unitTier = UnitTier.Team;
	
	public List<Equipment> unitEquipment = new List<Equipment>();
	public int higherUnitIdentifierNumber = 0;

	private TMP_Dropdown tierUI;
	private TMP_Dropdown specializationUI;
	private TMP_Dropdown movementUI;
	private TMP_Dropdown transportUI;
	private TMP_InputField nameUI;
	private TextMeshProUGUI equipmentTextUI;
	private UnitManager manager;
	
	public void Awake() { 
		tierUI = transform.Find("UnitTier").GetComponent<TMP_Dropdown>();
		specializationUI = transform.Find("Specialization").GetComponent<TMP_Dropdown>();
		movementUI = transform.Find("GroundMovementType").GetComponent<TMP_Dropdown>();
		transportUI = transform.Find("GroundTransportType").GetComponent<TMP_Dropdown>();
		nameUI = transform.Find("UnitName").GetComponent<TMP_InputField>();
		equipmentTextUI = transform.Find("Eq").GetComponent<TextMeshProUGUI>();
		manager = GameObject.FindWithTag("Units").GetComponent<UnitManager>();
	}

	private void OnEnable()	{
		UpdateUI();
		//Update from the current Unit on every opening

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
				break;
			case 2:
				enumNames = Enum.GetNames(typeof(NavalSpecialization));
				for (int i = 0; i < enumNames.Length; i++) {
					options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(manager.navalSpecialization[i],
					new Rect(0, 0, manager.navalSpecialization[i].width, manager.navalSpecialization[i].height), new Vector2(0.5f, 0.5f))));
				}
				break;
			default:
				enumNames = Enum.GetNames(typeof(GroundSpecialization));
				for (int i = 0; i < enumNames.Length; i++) {
					options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(manager.groundSpecialization[i],
						new Rect(0, 0, manager.groundSpecialization[i].width, manager.groundSpecialization[i].height), new Vector2(0.5f, 0.5f))));
				}
				break;
		}

		specializationUI.ClearOptions();
		specializationUI.AddOptions(options);

		//Removal of non-domain attributes
		switch (unitDomain) {
			case 0:
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
	}

	public void UpdateSpecialization(int i) {
		switch (unitDomain) {
			case 1:
			aerialUnit.ChangeSpecialization((AerialSpecialization)i);
			break;
			case 2:
			navalUnit.ChangeSpecialization((NavalSpecialization)i);
			break;
			default:
			groundUnit.ChangeSpecialization((GroundSpecialization)i);
			break;
		}
	}
	public void UpdateMovementModifier(int i) {
		GroundMovementType movementModifier = (GroundMovementType)i;
		groundUnit.ChangeSpecialization(movementModifier);

	}
	public void UpdateTransportModifier(int i) {
		GroundTransportType transportModifier = (GroundTransportType)i;
		groundUnit.ChangeSpecialization(transportModifier);
	}
	public void UpdateTier(int i) {
		constructedUnit.ChangeTier(i);
		UpdateName(constructedUnit.name);
		nameUI.text = constructedUnit.name;
	}
	public void UpdateName(string identification) {
		constructedUnit.ChangeName(identification);
	}
	public void UpdateUnit(Unit unit) {
		this.constructedUnit = unit;
		if (unit.GetType() == typeof(GroundUnit)) {
			groundUnit = (GroundUnit)unit;
			unitDomain = 0;
		} else if (unit.GetType() == typeof(AerialUnit)) {
			aerialUnit = (AerialUnit)unit;
			unitDomain = 1;
		} else {
			navalUnit = (NavalUnit)unit;
			unitDomain = 2;
		}
		UnitManager.Instance.PopulateUI(gameObject, unitDomain);
		UpdateLabels(unit);

		equipmentTextUI.text = string.Join("\n", unit.unitEquipment.Select(equipment => $"{equipment.equipmentName}:{equipment.amount}"));
	}

	public void UpdateUnit() {
		constructedUnit = GameObject.FindWithTag("Units").GetComponent<UnitManager>().SpawnUnit(Vector3.zero, 0, 2, "0", unitTier, sideB, unitMovementModifier, unitTransportModifier, unitEquipment);
	}


	private void UpdateLabels(GroundUnit unit) {
		movementUI.value = (int)unit.movementModifier;
		transportUI.value = (int)unit.transportModifier;
		UpdateLabels(unit);
	}

	private void UpdateLabels(Unit unit) {
		specializationUI.value = (int)unit.unitSpecialization;
		tierUI.value = (int)unit.unitTier;
		nameUI.text = name;
	}
}