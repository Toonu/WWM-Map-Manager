using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class UnitManager : MonoBehaviour {
	public static UnitManager Instance {
		get { return _instance; }
	}
	private static UnitManager _instance;

	// Use this for initialization
	void Start() {
		_instance = GetComponent<UnitManager>();

		unitSpawnMenu = unitUIMenus.transform.Find("UnitSpawningMenu").gameObject;
		unitEditMenu = unitUIMenus.transform.Find("UnitEditMenu").gameObject;
		equipmentMenu = unitUIMenus.transform.Find("EquipmentMenu").gameObject;
		baseEditMenu = unitUIMenus.transform.Find("BaseEditMenu").gameObject;

		PopulateUI(unitSpawnMenu);
	}

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
	#region TextureHandling

	internal Texture2D GetSpecialisationTexture(GroundUnit unit, bool sideB) {
		if (sideB) {
			return groundSpecializationEnemy[Convert.ToInt16(unit.specialization)];
		}
		return groundSpecialization[Convert.ToInt16(unit.specialization)];
	}
	internal Texture2D GetSpecialisationTexture(AerialUnit unit, bool sideB) {
		if (sideB) {
			return aerialSpecializationEnemy[Convert.ToInt16(unit.specialization)];
		}
		return aerialSpecialization[Convert.ToInt16(unit.specialization)];
	}
	internal Texture2D GetSpecialisationTexture(NavalUnit unit, bool sideB) {
		if (sideB) {
			return navalSpecializationEnemy[Convert.ToInt16(unit.specialization)];
		}
		return navalSpecialization[Convert.ToInt16(unit.specialization)];
	}
	internal Texture2D GetMovementTexture(GroundUnit unit, bool sideB) {
		if (sideB) {
			return movementTypeEnemy[Convert.ToInt16(unit.movementModifier)];
		}
		return movementType[Convert.ToInt16(unit.movementModifier)];
	}
	internal Texture2D GetTransportTexture(GroundUnit unit, bool sideB) {
		if (sideB) {
			return transportTypeEnemy[Convert.ToInt16(unit.transportModifier)];
		}
		return transportType[Convert.ToInt16(unit.transportModifier)];
	}
	internal Texture2D GetBaseTexture(BaseType type) {
		return baseTypes[Convert.ToInt16(type)];
	}

	#endregion

	#region UI
	public GameObject groundTemplate;
	public GameObject baseTemplate;
	public GameObject equipmentTemplate;
	public GameObject unitUIMenus;
	internal GameObject unitSpawnMenu;
	internal GameObject unitEditMenu;
	internal GameObject equipmentMenu;
	internal GameObject baseEditMenu;
	#endregion
	#region UI Methods
	public void PopulateUI(int domain) {
		PopulateUI(unitSpawnMenu, domain);
	}

	public void PopulateUI(GameObject menu, int domain = 0) {

		TMP_Dropdown currentSpawningDropdownMenu = menu.transform.Find("GameObject/Specialization").GetComponent<TMP_Dropdown>();
		string[] enumNames;
		List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

		//Specialization domain switch
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
		if (menu.GetComponent<UnitConstructor>() != null) {
			currentSpawningDropdownMenu.value = 2;
		}

		//Removal of non-domain attributes
		switch (domain) {
			case 0:
			menu.transform.Find("GameObject.1/UnitImages/Movement").gameObject.SetActive(true);
			menu.transform.Find("GameObject.1/UnitImages/Transport").gameObject.SetActive(true);
			currentSpawningDropdownMenu = menu.transform.Find("GameObject.2/GroundMovementType").GetComponent<TMP_Dropdown>();
			currentSpawningDropdownMenu.gameObject.SetActive(true);
			enumNames = Enum.GetNames(typeof(GroundMovementType));
			options = new List<TMP_Dropdown.OptionData>();
			for (int i = 0; i < enumNames.Length; i++) {
				options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(movementType[i],
					new Rect(0, 0, movementType[i].width, movementType[i].height), new Vector2(0.5f, 0.5f))));
			}
			currentSpawningDropdownMenu.ClearOptions();
			currentSpawningDropdownMenu.AddOptions(options);
			currentSpawningDropdownMenu = menu.transform.Find("GameObject.2/GroundTransportType").GetComponent<TMP_Dropdown>();
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
			menu.transform.Find("GameObject.2/GroundMovementType").gameObject.SetActive(false);
			menu.transform.Find("GameObject.2/GroundTransportType").gameObject.SetActive(false);
			menu.transform.Find("GameObject.1/UnitImages/Movement").gameObject.SetActive(false);
			menu.transform.Find("GameObject.1/UnitImages/Transport").gameObject.SetActive(false);
			break;
		}

		currentSpawningDropdownMenu = menu.transform.Find("GameObject.1/UnitTier").GetComponent<TMP_Dropdown>();
		enumNames = Enum.GetNames(typeof(UnitTier));
		currentSpawningDropdownMenu.ClearOptions();
		currentSpawningDropdownMenu.AddOptions(enumNames.ToList());

		if (menu.GetComponent<UnitConstructor>() != null) {
			menu.transform.Find("GameObject/UnitName").GetComponent<TMP_InputField>().text = (GetLast() + 1).ToString();
		}
	}

	#endregion

	#region Units
	public List<GroundUnit> groundUnits = new List<GroundUnit>();
	public List<AerialUnit> aerialUnits = new List<AerialUnit>();
	public List<NavalUnit> navalUnits = new List<NavalUnit>();
	public List<Base> bases = new List<Base>();
	#endregion

	#region Spawning

	internal Base SpawnBase(string identification, Vector3 position, BaseType baseType, bool sideB) {
		GameObject newBase = Instantiate(baseTemplate, transform);
		Base b = newBase.AddComponent<Base>();
		b.Initiate(identification, position, baseType, sideB);
		bases.Add(b);
		return b;
	}

	internal Unit SpawnUnit(Vector3 position, int domain, int specialization,
		string identification, UnitTier tier, bool sideB, GroundMovementType movementModifier,
		GroundTransportType transportModifier, List<Equipment> unitEquipment)
	{
		GameObject newUnit = Instantiate(groundTemplate, transform);
		int i = GetLast();
		if (domain == 0) {
			GroundUnit unit = newUnit.AddComponent<GroundUnit>();
			unit.Initiate(i, position, tier, identification, sideB, (GroundSpecialization)specialization, movementModifier, transportModifier, unitEquipment, null);
			AppendList(unit, i, groundUnits, aerialUnits, navalUnits);
			return unit;
		} else if (domain == 1) {
			AerialUnit unit = newUnit.AddComponent<AerialUnit>();
			unit.Initiate(i, position, tier, identification, sideB, (AerialSpecialization)specialization, unitEquipment);
			AppendList(unit, i, aerialUnits, groundUnits, navalUnits);
			return unit;
		} else {
			NavalUnit unit = newUnit.AddComponent<NavalUnit>();
			unit.Initiate(i, position, tier, identification, sideB, (NavalSpecialization)specialization, unitEquipment);
			AppendList(unit, i, navalUnits, groundUnits, aerialUnits);
			return unit;
		}
	}

	internal void Despawn(GameObject gameObject) {
		if (gameObject.GetComponent<Base>() == null) {
			int index = gameObject.GetComponent<Unit>().id;
			groundUnits.RemoveAt(index);
			if (gameObject.GetComponent<GroundUnit>() != null) {
				gameObject.GetComponent<GroundUnit>().unitEquipment.ForEach(e => Destroy(e.gameObject));
				groundUnits.Insert(index, null);
			} else if (gameObject.GetComponent<AerialUnit>() != null) {
				gameObject.GetComponent<AerialUnit>().unitEquipment.ForEach(e => Destroy(e.gameObject));
				aerialUnits.Insert(index, null);
			} else {
				gameObject.GetComponent<NavalUnit>().unitEquipment.ForEach(e => Destroy(e.gameObject));
				navalUnits.Insert(index, null);
			}
		} else {
			bases.Remove(gameObject.GetComponent<Base>());
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

	#region Unit overall updates

	public void ShowMissileRanges(bool show) {
		groundUnits.ForEach(unit => { if (unit != null && unit.specialization == GroundSpecialization.SAM) { unit.weaponRange.SetActive(show); } });
	}

	public void SwitchSide(bool sideB) {
		groundUnits.ForEach(unit => { if (unit != null) { unit.ChangeAffiliation();}});
		aerialUnits.ForEach(unit => { if (unit != null) { unit.ChangeAffiliation();}});
		navalUnits.ForEach(unit => { if (unit != null) { unit.ChangeAffiliation(); }});
		bases.ForEach(b => { if (b != null) { b.ChangeAffiliation(); } });
	}

	#endregion
}

