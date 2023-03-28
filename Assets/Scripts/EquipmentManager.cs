﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour {
	public List<Equipment> equipmentNames = new List<Equipment>();
	private List<Equipment> equipmentList = new List<Equipment>();
	private TMP_InputField amountInput;
	private TMP_Dropdown types;
	private TextFloatAppender costLabel;
	private TextFloatAppender sightLabel;
	private TextFloatAppender rangeLabel;
	public GameObject buttons;
	public GameObject button;
	public GameObject equipmentTemplate;
	public Popup generalPopup;
	public GameObject finish;
	public GameObject finishEdit;
	private GameObject menu;

	private int amount;
	Equipment eq;


	private void Awake() {
		amountInput = transform.Find("Menu/EqAmount").GetComponent<TMP_InputField>();
		types = transform.Find("Menu/EqType").GetComponent<TMP_Dropdown>();
		costLabel = transform.Find("Menu/EqCost").GetComponent<TextFloatAppender>();
		finish = transform.Find("Menu/Finish").gameObject;
		finishEdit = transform.Find("Menu/FinishEdit").gameObject;
		sightLabel = transform.Find("Menu/EqSight").GetComponent<TextFloatAppender>();
		rangeLabel = transform.Find("Menu/EqRange").GetComponent<TextFloatAppender>();
	}

	internal static List<Equipment> eqNaval = new List<Equipment>();
	internal static List<Equipment> eqAerial = new List<Equipment>();
	internal static List<Equipment> eqGround = new List<Equipment>();
	internal static List<Equipment> eqNavalB = new List<Equipment>();
	internal static List<Equipment> eqAerialB = new List<Equipment>();
	internal static List<Equipment> eqGroundB = new List<Equipment>();

	public void PopulateUI(UnitEditor menu) {
		List<string> eqNames = new List<string>();
		if (menu.GetComponent<UnitEditor>().unit.sideB) {
			if (menu.GetComponent<UnitEditor>().unit.GetType() == typeof(GroundUnit)) {
				eqNames = eqGroundB.Select(e => e.equipmentName).ToList();
				equipmentNames = eqGroundB;
			} else if (menu.GetComponent<UnitEditor>().unit.GetType() == typeof(AerialUnit)) {
				eqNames = eqAerialB.Select(e => e.equipmentName).ToList();
				equipmentNames = eqAerialB;
			} else if (menu.GetComponent<UnitEditor>().unit.GetType() == typeof(NavalUnit)) {
				eqNames = eqNavalB.Select(e => e.equipmentName).ToList();
				equipmentNames = eqNavalB;
			}
		} else {
			if (menu.GetComponent<UnitEditor>().unit.GetType() == typeof(GroundUnit)) {
				eqNames = eqGround.Select(e => e.equipmentName).ToList();
				equipmentNames = eqGround;
			} else if (menu.GetComponent<UnitEditor>().unit.GetType() == typeof(AerialUnit)) {
				eqNames = eqAerial.Select(e => e.equipmentName).ToList();
				equipmentNames = eqAerial;
			} else if (menu.GetComponent<UnitEditor>().unit.GetType() == typeof(NavalUnit)) {
				eqNames = eqNaval.Select(e => e.equipmentName).ToList();
				equipmentNames = eqNaval;
			}
		}
		PopulateUI(eqNames);
	}

	public void PopulateUI(List<string> eqNames) {
		types.ClearOptions();
		types.AddOptions(eqNames);
		SetType(0);
	}

	public void PopulateUI(UnitConstructor menu) {
		List<string> eqNames = new List<string>();
		if (menu.GetComponent<UnitConstructor>().sideB) {
			if (menu.GetComponent<UnitConstructor>().domain == 0) {
				eqNames = eqGroundB.Select(e => e.equipmentName).ToList();
				equipmentNames = eqGroundB;
			} else if (menu.GetComponent<UnitConstructor>().domain == 1) {
				eqNames = eqAerialB.Select(e => e.equipmentName).ToList();
				equipmentNames = eqAerialB;
			} else if (menu.GetComponent<UnitConstructor>().domain == 2) {
				eqNames = eqNavalB.Select(e => e.equipmentName).ToList();
				equipmentNames = eqNavalB;
			}
		} else {
			if (menu.GetComponent<UnitConstructor>().domain == 0) {
				eqNames = eqGround.Select(e => e.equipmentName).ToList();
				equipmentNames = eqGround;
			} else if (menu.GetComponent<UnitConstructor>().domain == 1) {
				eqNames = eqAerial.Select(e => e.equipmentName).ToList();
				equipmentNames = eqAerial;
			} else if (menu.GetComponent<UnitConstructor>().domain == 2) {
				eqNames = eqNaval.Select(e => e.equipmentName).ToList();
				equipmentNames = eqNaval;
			}
		}
		PopulateUI(eqNames);
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
		menu.GetComponent<UnitConstructor>().unitEquipment = equipmentList.ToList();
		menu.transform.Find("Eq").GetComponent<TextMeshProUGUI>().text = string.Join("\n", equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.amount}"));
		CloseMenu();
	}
	public void AddEquipment(UnitEditor menu) {
		menu.GetComponent<UnitEditor>().unit.AddEquipment(equipmentList);
		menu.transform.Find("Eq").GetComponent<TextMeshProUGUI>().text = string.Join("\n", equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.amount}"));
		CloseMenu();
	}

	public void CloseMenu() {
		foreach (Transform item in buttons.transform.Find("1")) {
			Destroy(item.gameObject);
		}
		foreach (Transform item in buttons.transform.Find("2")) {
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
				generalPopup.PopUp("Equipment already added");
				Destroy(newEquipment.gameObject);
				return;
			}
		}
		equipmentList.Add(newEquipment);
		CreateEquipmentButtons(newEquipment);
	}

	private void CreateEquipmentButtons(Equipment newEquipment) {
		// Loop through the button labels
		GameObject newButtonObject = Instantiate(button, buttons.transform.Find("1").transform);
		Button newButton1 = newButtonObject.GetComponent<Button>();
		newButtonObject.GetComponentInChildren<TextMeshProUGUI>().text = $"{newEquipment.equipmentName}:{newEquipment.amount}";

		GameObject newButtonObject1 = Instantiate(button, buttons.transform.Find("2").transform);
		Button newButton = newButtonObject1.GetComponent<Button>();
		newButtonObject1.GetComponentInChildren<TextMeshProUGUI>().text = $"DELETE";
		newButtonObject1.GetComponent<ButtonStorage>().button = newButtonObject;
		newButtonObject1.GetComponent<ButtonStorage>().buttons = newButtonObject1;
		newButton.onClick.AddListener(() => {
			RemoveEquipment(newEquipment);
			ButtonStorage b = newButtonObject1.GetComponent<ButtonStorage>();
			Destroy(b.button);
			Destroy(b.buttons);
		});
	}

	public void UpdateEquipmentList(UnitConstructor menu) {
		foreach (Equipment item in menu.GetComponent<UnitConstructor>().unitEquipment) {
			eq = item;
			UpdateEquipmentList(item);
		}
		finish.SetActive(true);
		this.menu = menu.gameObject;
	}

	public void UpdateEquipmentList(UnitEditor menu) {
		foreach (Equipment item in menu.GetComponent<UnitEditor>().unit.unitEquipment) {
			eq = item;
			UpdateEquipmentList(item);
		}
		finishEdit.SetActive(true);
		this.menu = menu.gameObject;
	}
}