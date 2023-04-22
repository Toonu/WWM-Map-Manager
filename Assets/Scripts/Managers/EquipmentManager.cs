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

	private void Awake() {
		_instance = GetComponent<EquipmentManager>();

		equipmentHostile.Add(0, new List<Equipment>());
		equipmentHostile.Add(1, new List<Equipment>());
		equipmentHostile.Add(2, new List<Equipment>());
		equipmentFriendly.Add(0, new List<Equipment>());
		equipmentFriendly.Add(1, new List<Equipment>());
		equipmentFriendly.Add(2, new List<Equipment>());
	}

	internal static Equipment CreateEquipment(Equipment template, int amount) {
		GameObject newEquipmentObject = Instantiate(_instance.equipmentTemplate, _instance.transform);
		Equipment newEquipment = newEquipmentObject.AddComponent<Equipment>();
		newEquipment.Initiate(template.equipmentName, amount, template.movementRange, template.sightRange, template.weaponRange, template.cost, template.sideB, template.domain, template.specialization, template.protection, template.transportation);
		return newEquipment;
	}

	internal static void CreateTemplates(IList<IList<object>> equipmentData) {
		GameObject templates = ApplicationController.Instance.transform.Find("Templates").gameObject;

		foreach (IList<object> col in equipmentData) {
			if (col.Any(e => e.ToString() == "")) {
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