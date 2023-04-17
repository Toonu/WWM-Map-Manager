using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour {
	public List<Equipment> equipmentNames = new List<Equipment>();
	private TMP_InputField amountInput;
	private TMP_Dropdown types;
	private UITextFloatAppender costLabel;
	private UITextFloatAppender sightLabel;
	private UITextFloatAppender rangeLabel;
	private GameObject buttonPanel;
	public GameObject buttonEquipment;
	public GameObject equipmentTemplate;
	private Transform equipmentFolder;
	private UnitConstructor constructor;

	private int amount;
	Equipment constructedEquipment;


	private void Awake() {
		amountInput = transform.GetChild(0).GetChild(3).Find("EqAmount").GetComponent<TMP_InputField>();
		costLabel = transform.GetChild(0).GetChild(3).Find("EqCost").GetComponent<UITextFloatAppender>();
		types = transform.GetChild(0).GetChild(3).Find("EqType").GetComponent<TMP_Dropdown>();
		sightLabel = transform.GetChild(0).GetChild(3).Find("EqSight").GetComponent<UITextFloatAppender>();
		rangeLabel = transform.GetChild(0).GetChild(3).Find("EqRange").GetComponent<UITextFloatAppender>();
		buttonPanel = transform.GetChild(0).GetChild(4).gameObject;
		constructor = transform.GetChild(0).GetComponent<UnitConstructor>();
		equipmentFolder = GameObject.FindGameObjectWithTag("EquipmentFolder").transform;
	}

	internal static List<Equipment> eqNaval = new List<Equipment>();
	internal static List<Equipment> eqAerial = new List<Equipment>();
	internal static List<Equipment> eqGround = new List<Equipment>();
	internal static List<Equipment> eqNavalB = new List<Equipment>();
	internal static List<Equipment> eqAerialB = new List<Equipment>();
	internal static List<Equipment> eqGroundB = new List<Equipment>();


	public void UpdateUI() {
		types.ClearOptions();
		List<string> eqNames = new List<string>();
		if (constructor.constructedUnit.SideB) {
			if (constructor.unitDomain == 0) {
				eqNames = eqGroundB.Select(e => e.equipmentName).ToList();
				equipmentNames = eqGroundB;
			} else if (constructor.unitDomain == 1) {
				eqNames = eqAerialB.Select(e => e.equipmentName).ToList();
				equipmentNames = eqAerialB;
			} else if (constructor.unitDomain == 2) {
				eqNames = eqNavalB.Select(e => e.equipmentName).ToList();
				equipmentNames = eqNavalB;
			}
		} else {
			if (constructor.unitDomain == 0) {
				eqNames = eqGround.Select(e => e.equipmentName).ToList();
				equipmentNames = eqGround;
			} else if (constructor.unitDomain == 1) {
				eqNames = eqAerial.Select(e => e.equipmentName).ToList();
				equipmentNames = eqAerial;
			} else if (constructor.unitDomain == 2) {
				eqNames = eqNaval.Select(e => e.equipmentName).ToList();
				equipmentNames = eqNaval;
			}
		}
		types.AddOptions(eqNames);
		SetType(0);
	}

	public void SetAmount(int newAmount) {
		amount = newAmount;
		amountInput.text = newAmount.ToString();
		costLabel.UpdateText(constructedEquipment.cost * newAmount);
	}
	public void SetAmount(string amount) {
		SetAmount(Convert.ToInt16(amount));
	}
	public void SetType(int type) {
		constructedEquipment = equipmentNames[type];
		SetAmount(1);
		sightLabel.UpdateText(constructedEquipment.sightRange);
		rangeLabel.UpdateText(constructedEquipment.movementRange);
	}

	public void Close() {
		foreach (Transform item in buttonPanel.transform.GetChild(0)) {
			Destroy(item.gameObject);
		}
		foreach (Transform item in buttonPanel.transform.GetChild(1)) {
			Destroy(item.gameObject);
		}
	}

	public void Cancel() {
		foreach (Equipment equipment in constructor.constructedUnit.equipmentList) {
			RemoveEquipment(equipment);
		}
		Close();
	}

	public void RemoveEquipment(Equipment equipment, bool refund = false) {
		if (refund) {
			SheetSync.UpdatePoints(equipment.cost * equipment.amount);
		}
		constructor.constructedUnit.equipmentList.Remove(equipment);
		Destroy(equipment.gameObject);
		
	}

	public void AddEquipment() {
		GameObject newEquipmentObject = Instantiate(equipmentTemplate, equipmentFolder);
		Equipment newEquipment = newEquipmentObject.AddComponent<Equipment>();
		newEquipment.Initiate(constructedEquipment.equipmentName, amount, constructedEquipment.movementRange, constructedEquipment.sightRange, constructedEquipment.weaponRange, constructedEquipment.cost, constructedEquipment.side, constructedEquipment.domain);

		foreach (Equipment equp in constructor.constructedUnit.equipmentList) {
			if (equp.equipmentName == constructedEquipment.equipmentName) {
				ApplicationController.generalPopup.GetComponent<UIPopup>().PopUp("Equipment already added");
				Destroy(newEquipment.gameObject);
				return;
			}
		}
		AddEquipment(newEquipment);
	}

	public void AddEquipment(Equipment newEquipment) {
		constructor.constructedUnit.equipmentList.Add(newEquipment);
		CreateEquipmentButtons(newEquipment);
		SheetSync.UpdatePoints(-newEquipment.cost * newEquipment.amount);
		//Add cost of all equipment to constructor of the unit
		//Logic for updating eq from existing unit
	}

	public void AddEquipmentList(List<Equipment> newEquipmentList) {
		foreach (Equipment equipment in newEquipmentList) {
			AddEquipment(equipment);
		}
	}

	private void CreateEquipmentButtons(Equipment newEquipment) {
		GameObject equipmentLabel = Instantiate(buttonEquipment, buttonPanel.transform);
		equipmentLabel.transform.GetChild(0).GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"[{newEquipment.amount,-3}] {newEquipment.equipmentName}";

		
		Button sellButton = equipmentLabel.transform.GetChild(1).GetComponent<Button>();
		Button deleteButton = equipmentLabel.transform.GetChild(2).GetComponent<Button>();

		if (!ApplicationController.admin) {
			Destroy(deleteButton.gameObject);
		}

		deleteButton.onClick.AddListener(() => {
			RemoveEquipment(newEquipment);
			Destroy(equipmentLabel);
		});

		sellButton.onClick.AddListener(() => {
			RemoveEquipment(newEquipment, true);
			Destroy(equipmentLabel);
		});
	}
}
