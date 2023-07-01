using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {
	internal static Dictionary<int, List<Equipment>> equipmentHostile = new();
	internal static Dictionary<int, List<Equipment>> equipmentFriendly = new();
	public GameObject equipmentTemplate;
	public static EquipmentManager Instance {
		get { return _instance; }
	}
	private static EquipmentManager _instance;

	/// <summary>
	/// Method sets up equipment lists and coponents on startup.
	/// </summary>
	private void Awake() {
		_instance = GetComponent<EquipmentManager>();

		equipmentHostile.Add(0, new List<Equipment>());
		equipmentHostile.Add(1, new List<Equipment>());
		equipmentHostile.Add(2, new List<Equipment>());
		equipmentFriendly.Add(0, new List<Equipment>());
		equipmentFriendly.Add(1, new List<Equipment>());
		equipmentFriendly.Add(2, new List<Equipment>());
	}

	/// <summary>
	/// Method creates a new equipment object from template and returns it.
	/// </summary>
	/// <param name="template">Equipment template</param>
	/// <param name="amount">int amount of created equipment</param>
	/// <returns></returns>
	internal static Equipment CreateEquipment(Equipment template, int amount) {
		GameObject newEquipmentObject = Instantiate(_instance.equipmentTemplate, _instance.transform);
		Equipment newEquipment = newEquipmentObject.AddComponent<Equipment>();
		newEquipment.Initiate(template.equipmentName, amount, template.movementRange, template.sightRange, template.weaponRange, template.cost, template.sideB, template.domain, template.specialization, template.protection, template.transportation);
		return newEquipment;
	}

	/// <summary>
	/// Method creates equipment templates from sheet data list.
	/// </summary>
	/// <param name="equipmentData">2D List<List<Object>> of equipment data</param>
	internal static void CreateTemplates(IList<IList<object>> equipmentData) {
		GameObject templates = ApplicationController.Instance.transform.Find("Templates").gameObject;

		//Loop through each row of data and create a new equipment template from it.
		foreach (IList<object> col in equipmentData) {
			//Ignore weird or empty rows.
			if (col.Any(e => e.ToString() == "")) {
				//Reports any issues with equipment templates creation.
				if (col[6].ToString() == "" || Convert.ToInt16(col[6]) != 3) Debug.Log($"There was issue creating {col[0]} - F/{col[1]}|G/{col[2]}|H/{col[3]}|I/{col[4]}|J/{col[5]}|K/{col[6]}|L/{col[7]}|M/{col[8]}|N/{col[9]}!");
				continue;
			}
			GameObject newEquipmentObject = Instantiate(Instance.equipmentTemplate, templates.transform);
			Equipment newEquipment = newEquipmentObject.AddComponent<Equipment>();
			newEquipment.Initiate(
				col[0].ToString(),
				10,
				Convert.ToSingle(col[1], ApplicationController.culture),
				Convert.ToSingle(col[2], ApplicationController.culture),
				Convert.ToSingle(col[3], ApplicationController.culture),
				Convert.ToSingle(col[4], ApplicationController.culture),
				Convert.ToInt16(col[5]),
				Convert.ToInt16(col[6]),
				Convert.ToInt16(col[7]),
				Convert.ToInt16(col[8]),
				Convert.ToInt16(col[9]));
			newEquipmentObject.name = $"Template: {newEquipment.equipmentName}";
			if (newEquipment.sideB == 0) {
				equipmentHostile[newEquipment.domain].Add(newEquipment);
			} else {
				equipmentFriendly[newEquipment.domain].Add(newEquipment);
			}
		}
	}
}