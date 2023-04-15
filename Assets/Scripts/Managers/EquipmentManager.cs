using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour {
	public List<Equipment> equipmentNames = new List<Equipment>();
	internal List<Equipment> equipmentList = new List<Equipment>();
	private TMP_InputField amountInput;
	private TMP_Dropdown types;
	private UITextFloatAppender costLabel;
	private UITextFloatAppender sightLabel;
	private UITextFloatAppender rangeLabel;
	private GameObject buttonPanel;
	public GameObject buttonEquipment;
	public GameObject equipmentTemplate;
	private ApplicationController controller;
	private UnitConstructor constructor;
	private GameObject finish;
	private GameObject finishEdit;
	private GameObject menu;

	private int amount;
	Equipment eq;


	private void Awake() {
		amountInput = transform	.Find("Menu/Equipment/GameObject.1/EqAmount").GetComponent<TMP_InputField>();
		costLabel = transform	.Find("Menu/Equipment/GameObject.1/EqCost").GetComponent<UITextFloatAppender>();
		types = transform		.Find("Menu/Equipment/GameObject.2/EqType").GetComponent<TMP_Dropdown>();
		sightLabel = transform	.Find("Menu/Equipment/GameObject.2/EqSight").GetComponent<UITextFloatAppender>();
		rangeLabel = transform	.Find("Menu/Equipment/GameObject.3/EqRange").GetComponent<UITextFloatAppender>();
		buttonPanel = transform.Find("Menu/Equipment/ButtonPanel").gameObject;
		controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<ApplicationController>();
		constructor = transform.GetChild(0).GetComponent<UnitConstructor>();
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

	public void SetAmount(int amount) {
		this.amount = amount;
		amountInput.text = amount.ToString();
		costLabel.UpdateText(eq.cost * amount);
	}
	public void SetAmount(string amount) {
		SetAmount(Convert.ToInt16(amount));
	}

	public void SetType(int type) {
		eq = equipmentNames[type];
		SetAmount(1);
		sightLabel.UpdateText(eq.sightRange);
		rangeLabel.UpdateText(eq.movementRange);
	}

	public void AddEquipment(UnitConstructor menu) {
		constructor.unitEquipment = equipmentList.ToList();
		menu.transform.Find("Eq").GetComponent<TextMeshProUGUI>().text = string.Join("\n", equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.amount}"));
		CloseMenu();
	}
	public void AddEquipment() {
		constructor.constructedUnit.AddEquipment(equipmentList);
		menu.transform.Find("Eq").GetComponent<TextMeshProUGUI>().text = string.Join("\n", equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.amount}"));
		CloseMenu();
	}

	public void CloseMenu() {
		foreach (Transform item in buttonPanel.transform.Find("1")) {
			Destroy(item.gameObject);
		}
		foreach (Transform item in buttonPanel.transform.Find("2")) {
			Destroy(item.gameObject);
		}
		equipmentList.Clear();
		finish.SetActive(false);
		finishEdit.SetActive(false);
		menu = null;
	}

	public void CancelMenu() {
		foreach (Equipment equipment in equipmentList) {
			Destroy(equipment.gameObject);
		}
		CloseMenu();
	}

	public void RemoveEquipment(Equipment equipment) {
		equipmentList.Remove(equipment);
		Destroy(equipment.gameObject);
	}

	public void UpdateEquipmentList() {
		GameObject newEquipmentObject = Instantiate(equipmentTemplate, GameObject.FindWithTag("ServerSync").transform);
		Equipment newEquipment = newEquipmentObject.AddComponent<Equipment>();
		newEquipment.Initiate(eq.equipmentName, amount, eq.movementRange, eq.sightRange, eq.weaponRange, eq.cost, eq.side, eq.domain);

		UpdateEquipmentList(newEquipment);
	}

	public void UpdateEquipmentList(Equipment newEquipment) {
		foreach (Equipment equp in equipmentList) {
			if (equp.equipmentName == eq.equipmentName) {
				controller.generalPopup.GetComponent<UIPopup>().PopUp("Equipment already added");
				Destroy(newEquipment.gameObject);
				return;
			}
		}
		equipmentList.Add(newEquipment);
		CreateEquipmentButtons(newEquipment);
	}

	private void CreateEquipmentButtons(Equipment newEquipment) {
		// Loop through the button labels
		GameObject equipmentLabel = Instantiate(buttonEquipment, buttonPanel.transform.Find("1").transform);
		Button equipmentLabelButton = equipmentLabel.GetComponent<Button>();
		equipmentLabel.GetComponentInChildren<TextMeshProUGUI>().text = $"{newEquipment.equipmentName}:{newEquipment.amount}";

		GameObject deleteButton = Instantiate(buttonEquipment, buttonPanel.transform.Find("2").transform);
		Button deleteButtonButton = deleteButton.GetComponent<Button>();
		deleteButton.GetComponentInChildren<TextMeshProUGUI>().text = $"DELETE";

		deleteButton.GetComponent<EquipmentMenuButton>().mainButton = equipmentLabel;
		deleteButton.GetComponent<EquipmentMenuButton>().removeButton = deleteButton;
		deleteButtonButton.onClick.AddListener(() => {
			RemoveEquipment(newEquipment);
			EquipmentMenuButton b = deleteButton.GetComponent<EquipmentMenuButton>();
			Destroy(b.mainButton);
			Destroy(b.removeButton);
		});
	}

	public void UpdateEquipmentList(UnitConstructor menu) {
		foreach (Equipment item in constructor.unitEquipment) {
			eq = item;
			UpdateEquipmentList(item);
		}
		finish.SetActive(true);
		this.menu = menu.gameObject;
	}
}
