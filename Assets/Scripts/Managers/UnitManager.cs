﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {
	public static UnitManager Instance {
		get { return _instance; }
	}
	private static UnitManager _instance;

	// Use this for initialization
	void Start() {
		_instance = GetComponent<UnitManager>();

		unitMenu = unitUIMenus.transform.Find("UnitMenu/Menu").gameObject;
		baseMenu = unitUIMenus.transform.Find("BaseMenu").gameObject;
	}

	#region Textures
	public List<Sprite> movementType = new List<Sprite>();
	public List<Sprite> transportType = new List<Sprite>();
	public List<Sprite> movementTypeEnemy = new List<Sprite>();
	public List<Sprite> transportTypeEnemy = new List<Sprite>();

	public List<Sprite> groundSpecialization = new List<Sprite>();
	public List<Sprite> aerialSpecialization = new List<Sprite>();
	public List<Sprite> navalSpecialization = new List<Sprite>();
	public List<Sprite> baseTypes = new List<Sprite>();

	public List<Sprite> groundSpecializationEnemy = new List<Sprite>();
	public List<Sprite> aerialSpecializationEnemy = new List<Sprite>();
	public List<Sprite> navalSpecializationEnemy = new List<Sprite>();
	#endregion
	#region TextureHandling

	internal Texture2D GetSpecialisationTexture(GroundUnit unit, bool isEnemy) {
		if (isEnemy) {
			return groundSpecializationEnemy[Convert.ToInt16(unit.specialization)].texture;
		}
		return groundSpecialization[Convert.ToInt16(unit.specialization)].texture;
	}
	internal Texture2D GetSpecialisationTexture(AerialUnit unit, bool isEnemy) {
		if (isEnemy) {
			return aerialSpecializationEnemy[Convert.ToInt16(unit.specialization)].texture;
		}
		return aerialSpecialization[Convert.ToInt16(unit.specialization)].texture;
	}
	internal Texture2D GetSpecialisationTexture(NavalUnit unit, bool isEnemy) {
		if (isEnemy) {
			return navalSpecializationEnemy[Convert.ToInt16(unit.specialization)].texture;
		}
		return navalSpecialization[Convert.ToInt16(unit.specialization)].texture;
	}
	internal Texture2D GetMovementTexture(GroundUnit unit, bool isEnemy) {
		if (isEnemy) {
			return movementTypeEnemy[Convert.ToInt16(unit.movementModifier)].texture;
		}
		return movementType[Convert.ToInt16(unit.movementModifier)].texture;
	}
	internal Texture2D GetTransportTexture(GroundUnit unit, bool isEnemy) {
		if (isEnemy) {
			return transportTypeEnemy[Convert.ToInt16(unit.transportModifier)].texture;
		}
		return transportType[Convert.ToInt16(unit.transportModifier)].texture;
	}
	internal Texture2D GetBaseTexture(BaseType type) {
		return baseTypes[Convert.ToInt16(type)].texture;
	}

	#endregion

	#region UI
	public GameObject groundTemplate;
	public GameObject baseTemplate;
	public GameObject equipmentTemplate;
	public GameObject unitUIMenus;
	internal GameObject unitMenu;
	internal GameObject baseMenu;
	#endregion

	#region Units
	public List<GroundUnit> groundUnits = new List<GroundUnit>();
	public List<AerialUnit> aerialUnits = new List<AerialUnit>();
	public List<NavalUnit> navalUnits = new List<NavalUnit>();
	public List<Base> bases = new List<Base>();
	#endregion

	#region Spawning

	internal Base SpawnBase(string identification, Vector3 position, BaseType baseType, bool sideB) {
		GameObject newBase = Instantiate(baseTemplate, transform.GetChild(0));
		Base b = newBase.AddComponent<Base>();
		b.Initiate(identification, position, baseType, sideB);
		bases.Add(b);
		return b;
	}

	internal Unit SpawnUnit(Vector3 position, UnitTier tier, string identification,
		List<Equipment> unitEquipment, bool sideB, int specialization, GroundMovementType movementModifier,
		GroundTransportType transportModifier, int domain) {
		GameObject newUnit = Instantiate(groundTemplate, transform);
		int i = GetLast();
		if (domain == 0) {
			GroundUnit unit = newUnit.AddComponent<GroundUnit>();
			unit.Initiate(i, position, tier, identification, unitEquipment, sideB, specialization, movementModifier, transportModifier);
			AppendList(unit, i, groundUnits, aerialUnits, navalUnits);
			return unit;
		} else if (domain == 1) {
			AerialUnit unit = newUnit.AddComponent<AerialUnit>();
			unit.Initiate(i, position, tier, identification, unitEquipment, sideB, specialization);
			AppendList(unit, i, aerialUnits, groundUnits, navalUnits);
			return unit;
		} else {
			NavalUnit unit = newUnit.AddComponent<NavalUnit>();
			unit.Initiate(i, position, tier, identification, unitEquipment, sideB, specialization);
			AppendList(unit, i, navalUnits, groundUnits, aerialUnits);
			return unit;
		}
	}

	internal void Despawn(GameObject gameObject) {
		if (gameObject.GetComponent<Base>() == null) {
			int index = gameObject.GetComponent<Unit>().ID;
			groundUnits.RemoveAt(index);
			if (gameObject.GetComponent<GroundUnit>() != null) {
				gameObject.GetComponent<GroundUnit>().equipmentList.ForEach(e => Destroy(e.gameObject));
				groundUnits.Insert(index, null);
			} else if (gameObject.GetComponent<AerialUnit>() != null) {
				gameObject.GetComponent<AerialUnit>().equipmentList.ForEach(e => Destroy(e.gameObject));
				aerialUnits.Insert(index, null);
			} else {
				gameObject.GetComponent<NavalUnit>().equipmentList.ForEach(e => Destroy(e.gameObject));
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
			list.Add(default);
			count++;
		}
		count = otherList.Count;
		while (index >= count) {
			otherList.Add(default);
			count++;
		}
		count = theOtherList.Count;
		while (index >= count) {
			theOtherList.Add(default);
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
		groundUnits.ForEach(unit => { if (unit != null && unit.specialization == GroundSpecialization.SAM) { unit.WeaponRangeCircle.SetActive(show); } });
	}

	public void SwitchSide() {
		groundUnits.ForEach(unit => { if (unit != null) { unit.ChangeAffiliation(); } });
		aerialUnits.ForEach(unit => { if (unit != null) { unit.ChangeAffiliation(); } });
		navalUnits.ForEach(unit => { if (unit != null) { unit.ChangeAffiliation(); } });
		bases.ForEach(b => { if (b != null) { b.ChangeAffiliation(); } });
	}

	#endregion
}

