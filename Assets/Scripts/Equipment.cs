using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Equipment : MonoBehaviour {
	public static Dictionary<string, Equipment> equipmentNames;
	private TMP_InputField amountInput;
	private TMP_Dropdown types;
	private TMP_Text costInput;

	
	internal string equipmentName;
	internal float movementRange;
	internal float sightRange;
	internal float weaponRange;
	internal int cost;
	internal int amount;


	private void Awake() {
		amountInput = transform.Find("EqAmount").GetComponent<TMP_InputField>();
		types = transform.Find("EqType").GetComponent<TMP_Dropdown>();
		costInput = transform.Find("EqCost").GetComponent<TMP_Text>();
	}

	internal void PopulateUI() {
		List<string> eqNames;
		foreach (KeyValuePair<string, Equipment> equipment in equipmentNames) {
			//eqNames.Add(equipment.Key.ToString());
		}
	}

	internal Equipment getEquipment(string equipmentName) {
		Equipment template = equipmentNames[equipmentName];
		return new Equipment(template.equipmentName, template.amount, template.movementRange, template.sightRange, template.weaponRange, template.cost);
	}


	public Equipment(string equipmentName, int amount, float movementRange, float sightRange, float weaponRange, int cost) {
		this.equipmentName = equipmentName;
		this.amount = amount;
		this.movementRange = movementRange;
		this.movementRange = weaponRange;
		this.sightRange = sightRange;
		this.cost = cost;
	}
}
