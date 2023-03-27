using System;
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
	public GameObject buttons;
	public GameObject button;
	public GameObject equipmentTemplate;
	public Popup generalPopup;

	private int amount;
	Equipment eq;


	private void Awake() {
		amountInput = transform.Find("Menu/EqAmount").GetComponent<TMP_InputField>();
		types = transform.Find("Menu/EqType").GetComponent<TMP_Dropdown>();
		costLabel = transform.Find("Menu/EqCost").GetComponent<TextFloatAppender>();
	}

	public void PopulateUI() {
		List<string> eqNames = new List<string>();
		foreach (Equipment equipment in equipmentNames) {
			eqNames.Add(equipment.equipmentName);
		}
		types.ClearOptions();
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
	}


	public void AddEquipment(GameObject menu) {
		menu.GetComponent<UnitConstructor>().unitEquipment = equipmentList.ToList();
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
	}

	public void RemoveEquipment(Equipment equipment) {
		equipmentList.Remove(equipment);
		Destroy(equipment.gameObject);
	}

	public void UpdateEquipmentList() {
		foreach (Equipment	equp in equipmentList) {
			if (equp.equipmentName == eq.equipmentName) {
				generalPopup.PopUp("Equipment already added");
				return;
			}
		}

		GameObject newEquipmentObject = Instantiate(equipmentTemplate, GameObject.FindWithTag("ServerSync").transform);
		Equipment newEquipment = newEquipmentObject.AddComponent<Equipment>();
		newEquipment.Initiate(eq.equipmentName, amount, eq.movementRange, eq.sightRange, eq.weaponRange, eq.cost);
		equipmentList.Add(newEquipment);

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

	public void UpdateEquipmentList(Equipment newEquipment) {
		foreach (Equipment equp in equipmentList) {
			if (newEquipment.equipmentName == eq.equipmentName) {
				generalPopup.PopUp("Equipment already added");
				return;
			}
		}

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

	public void UpdateEquipmentList(GameObject menu) {
		foreach (Equipment item in menu.GetComponent<UnitConstructor>().unitEquipment) {
			eq = item;
			UpdateEquipmentList(item);
		}
	}
}
