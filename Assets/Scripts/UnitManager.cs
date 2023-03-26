using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UnitManager : MonoBehaviour {
	public static UnitManager Instance {
		get { return _instance; }
	}
	private static UnitManager _instance;

	#region Textures
	public List<Texture2D> movementType = new List<Texture2D>();
	public List<Texture2D> transportType = new List<Texture2D>();
	public List<Texture2D> movementTypeEnemy = new List<Texture2D>();
	public List<Texture2D> transportTypeEnemy = new List<Texture2D>();

	public List<Texture2D> groundSpecialization = new List<Texture2D>();
	public List<Texture2D> aerialSpecialization = new List<Texture2D>();
	public List<Texture2D> navalSpecialization = new List<Texture2D>();
	public List<Texture2D> baseTypes = new List<Texture2D>();

	public List<Texture2D> groundSpecializationEnemy = new List<Texture2D>();
	public List<Texture2D> aerialSpecializationEnemy = new List<Texture2D>();
	public List<Texture2D> navalSpecializationEnemy = new List<Texture2D>();
	#endregion

	#region UI
	public GameObject groundTemplate;
	public GameObject baseTemplate;
	public GameObject unitUIMenus;
	internal GameObject unitSpawnMenu;
	internal GameObject unitEditMenu;
	internal GameObject equipmentMenu;
	#endregion

	#region Units
	//TODO Change back to internal accessors after finding out if the ID assignation works correctly.
	public List<GroundUnit> groundUnits = new List<GroundUnit>();
	public List<AerialUnit> aerialUnits = new List<AerialUnit>();
	public List<NavalUnit> navalUnits = new List<NavalUnit>();
	public List<Base> bases = new List<Base>();
	#endregion


	// Use this for initialization
	void Start() {
		_instance = GetComponent<UnitManager>();

		//SpawnUnit(Vector3.zero, 0, Convert.ToInt16(GroundSpecialization.SPG), GroundMovementType.Wheeled, GroundTransportType.None, UnitTier.Battalion, "5");
		SpawnBase("Lund", new Vector3(0.55f, -0.74f, -1), BaseType.Airfield);


		unitSpawnMenu = unitUIMenus.transform.Find("UnitSpawningMenu").gameObject;
		unitEditMenu = unitUIMenus.transform.Find("UnitEditMenu").gameObject;
		equipmentMenu = unitUIMenus.transform.Find("EquipmentMenu").gameObject;

		PopulateUI(unitSpawnMenu);
	}



	#region Spawning

	internal void SpawnBase(string identification, Vector3 position, BaseType baseType) {
		GameObject newBase = Instantiate(baseTemplate, transform);
		Base b = newBase.AddComponent<Base>();
		b.Initiate(identification, position, baseType);
		bases.Add(b);
	}

	internal void SpawnUnit(Vector3 position, int domain, int specialization,
		string identification, UnitTier tier, bool isEnemy, GroundMovementType movementModifier,
		GroundTransportType transportModifier)
	{
		GameObject newUnit = Instantiate(groundTemplate, transform);
		int i = GetLast();
		if (domain == 0) {
			GroundUnit unit = newUnit.AddComponent<GroundUnit>();
			unit.Initiate(i, position, tier, identification, isEnemy, (GroundSpecialization)specialization, movementModifier, transportModifier, null, null);
			AppendList(unit, i, groundUnits, aerialUnits, navalUnits);
		} else if (domain == 1) {
			AerialUnit unit = newUnit.AddComponent<AerialUnit>();
			unit.Initiate(i, position, tier, identification, isEnemy, (AerialSpecialization)specialization, null);
			AppendList(unit, i, aerialUnits, groundUnits, navalUnits);
		} else {
			NavalUnit unit = newUnit.AddComponent<NavalUnit>();
			unit.Initiate(i, position, tier, identification, isEnemy, (NavalSpecialization)specialization);
			AppendList(unit, i, navalUnits, groundUnits, aerialUnits);
		}
	}

	internal void Despawn(GameObject gameObject) {
		int index = gameObject.GetComponent<Unit>().id;
		groundUnits.RemoveAt(index);
		if (gameObject.GetComponent<GroundUnit>() != null) {
			groundUnits.Insert(index, null);
		} else if (gameObject.GetComponent<AerialUnit>() != null) {
			aerialUnits.Insert(index, null);
		} else {
			navalUnits.Insert(index, null);
		}
		Destroy(gameObject);
	}

	public int GetLast() {
		int maxLength = Math.Max(groundUnits.Count, Math.Max(aerialUnits.Count, navalUnits.Count));
		for (int i = 0; i < maxLength; i++) {
			if (groundUnits[i] == null && aerialUnits[i] == null && navalUnits[i] == null) {
				return i;
			}
		}
		return maxLength;
	}

	private void AppendList<J, K, L>(J obj, int index, List<J> list, List<K> otherList, List<L> theOtherList) {
		int count = list.Count;
		
		// If the index is greater than the current count, add null elements until the index is reached
		while (index > count) {
			list.Add(default(J));
			count++;
		}
		count = otherList.Count;
		while (index >= count) {
			otherList.Add(default(K));
			count++;
		}
		count = theOtherList.Count;
		while (index >= count) {
			theOtherList.Add(default(L));
			count++;
		}

		if (list.Count > index) {
			list.Insert(index, obj);
			list.RemoveAt(index + 1);
		} else {
			list.Add(obj);
		}
	}

	#endregion

	#region TextureHandling

	internal Texture2D GetSpecialisationTexture(GroundUnit unit, bool enemy) {
		if (enemy) {
			return groundSpecializationEnemy[Convert.ToInt16(unit.specialization)];
		}
		return groundSpecialization[Convert.ToInt16(unit.specialization)];
	}
	internal Texture2D GetSpecialisationTexture(AerialUnit unit, bool enemy) {
		if (enemy) {
			return aerialSpecializationEnemy[Convert.ToInt16(unit.specialization)];
		}
		return aerialSpecialization[Convert.ToInt16(unit.specialization)];
	}
	internal Texture2D GetSpecialisationTexture(NavalUnit unit, bool enemy) {
		if (enemy) {
			return navalSpecializationEnemy[Convert.ToInt16(unit.specialization)];
		}
		return navalSpecialization[Convert.ToInt16(unit.specialization)];
	}
	internal Texture2D GetMovementTexture(GroundUnit unit, bool enemy) {
		if (enemy) {
			return movementType[Convert.ToInt16(unit.movementModifier)];
		}
		return movementType[Convert.ToInt16(unit.movementModifier)];
	}
	internal Texture2D GetTransportTexture(GroundUnit unit, bool enemy) {
		if (enemy) {
			return transportType[Convert.ToInt16(unit.transportModifier)];
		}
		return transportType[Convert.ToInt16(unit.transportModifier)];
	}
	internal Texture2D GetBaseTexture(BaseType type) {
		return baseTypes[Convert.ToInt16(type)];
	}

	#endregion

	#region GettingAttributes

	internal string GetUnitTier(int tier) {
		switch (tier) {
			case 0:
			return "Ø";
			case int i when i >= 1 && i <= 3:
			return new string('●', i);
			case int i when i >= 4 && i <= 6:
			return new string('I', i - 3);
			case int i when i >= 7:
			return new string('X', i - 6);
			default:
			return "";
		}
	}

	internal string GetCorps(int unit) {
		string[] thousands = { "", "M", "MM", "MMM" };
		string[] hundreds = { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
		string[] tens = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
		string[] ones = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

		return thousands[unit / 1000] +
			   hundreds[(unit % 1000) / 100] +
			   tens[(unit % 100) / 10] +
			   ones[unit % 10];
	}

	internal string GetCorps(string unit) {
		if (unit == "") {
			return "0";
		}
		return GetCorps(Convert.ToInt16(unit));
	}

	public void ShowMissileRanges(bool show) {
		foreach (GroundUnit unit in groundUnits) {
			if (unit != null && unit.specialization == GroundSpecialization.SAM) {
				unit.weaponRange.SetActive(show);
			}
		}
	}

	public void SwitchSide(bool side) {
		foreach (GroundUnit unit in groundUnits) {
			if (unit != null) {
				unit.ChangeAffiliation(side);
			}
		}
		foreach (AerialUnit unit in aerialUnits) {
			if (unit != null) {
				unit.ChangeAffiliation(side);
			}
		}
		foreach (NavalUnit unit in navalUnits) {
			if (unit != null) {
				unit.ChangeAffiliation(side);
			}
		}
	}

	#endregion

	#region UI
	public void PopulateUI(int domain) {
		PopulateUI(unitSpawnMenu, domain);
	}

	public void PopulateUI(GameObject menu, int domain = 0) {

		TMP_Dropdown currentSpawningDropdownMenu = menu.transform.Find("Specialization").GetComponent<TMP_Dropdown>();
		string[] enumNames;
		List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

		switch (domain) {
			case 1:
			enumNames = Enum.GetNames(typeof(AerialSpecialization));
			for (int i = 0; i < enumNames.Length; i++) {
				options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(aerialSpecialization[i],
					new Rect(0, 0, aerialSpecialization[i].width, aerialSpecialization[i].height), new Vector2(0.5f, 0.5f))));
			}
			break;
			case 2:
			enumNames = Enum.GetNames(typeof(NavalSpecialization));
			for (int i = 0; i < enumNames.Length; i++) {
				options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(navalSpecialization[i],
				new Rect(0, 0, navalSpecialization[i].width, navalSpecialization[i].height), new Vector2(0.5f, 0.5f))));
			}
			break;
			default:
			enumNames = Enum.GetNames(typeof(GroundSpecialization));
			for (int i = 0; i < enumNames.Length; i++) {
				options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(groundSpecialization[i],
					new Rect(0, 0, groundSpecialization[i].width, groundSpecialization[i].height), new Vector2(0.5f, 0.5f))));
			}
			break;
		}


		currentSpawningDropdownMenu.ClearOptions();
		currentSpawningDropdownMenu.AddOptions(options);


		//UnitAffiliation set by spawning base affiliation

		switch (domain) {
			case 0:
			currentSpawningDropdownMenu = menu.transform.Find("GroundMovementType").GetComponent<TMP_Dropdown>();
			currentSpawningDropdownMenu.gameObject.SetActive(true);
			enumNames = Enum.GetNames(typeof(GroundMovementType));
			options = new List<TMP_Dropdown.OptionData>();
			for (int i = 0; i < enumNames.Length; i++) {
				options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(movementType[i],
					new Rect(0, 0, movementType[i].width, movementType[i].height), new Vector2(0.5f, 0.5f))));
			}
			currentSpawningDropdownMenu.ClearOptions();
			currentSpawningDropdownMenu.AddOptions(options);
			currentSpawningDropdownMenu = menu.transform.Find("GroundTransportType").GetComponent<TMP_Dropdown>();
			currentSpawningDropdownMenu.gameObject.SetActive(true);
			enumNames = Enum.GetNames(typeof(GroundTransportType));
			options = new List<TMP_Dropdown.OptionData>();
			for (int i = 0; i < enumNames.Length; i++) {
				options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(transportType[i],
					new Rect(0, 0, transportType[i].width, transportType[i].height), new Vector2(0.5f, 0.5f))));
			}
			currentSpawningDropdownMenu.ClearOptions();
			currentSpawningDropdownMenu.AddOptions(options);
			break;
			default:
			menu.transform.Find("GroundMovementType").gameObject.SetActive(false);
			menu.transform.Find("GroundTransportType").gameObject.SetActive(false);
			break;
		}

		currentSpawningDropdownMenu = menu.transform.Find("UnitTier").GetComponent<TMP_Dropdown>();
		enumNames = Enum.GetNames(typeof(UnitTier));
		currentSpawningDropdownMenu.ClearOptions();
		currentSpawningDropdownMenu.AddOptions(enumNames.ToList());
	}

	#endregion

}

