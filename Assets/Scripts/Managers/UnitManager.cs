using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour {
	public static UnitManager Instance {
		get { return _instance; }
	}
	private static UnitManager _instance;

	// Use this for initialization
	void Start() {
		_instance = GetComponent<UnitManager>();

		unitMenu = unitUIMenus.transform.Find("UnitMenu").gameObject;
		baseMenu = unitUIMenus.transform.Find("BaseMenu").gameObject;
		unitMenu.GetComponent<UnitConstructor>().Awake();
	}

	#region Textures
	public List<Sprite> movementType = new();
	public List<Sprite> transportType = new();
	public List<Sprite> movementTypeEnemy = new();
	public List<Sprite> transportTypeEnemy = new();

	public List<Sprite> groundSpecialization = new();
	public List<Sprite> aerialSpecialization = new();
	public List<Sprite> navalSpecialization = new();
	public List<Sprite> baseTypes = new();

	public List<Sprite> groundSpecializationEnemy = new();
	public List<Sprite> aerialSpecializationEnemy = new();
	public List<Sprite> navalSpecializationEnemy = new();
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
	public GameObject unitUIMenus;
	internal GameObject unitMenu;
	internal GameObject baseMenu;
	#endregion

	#region Units
	public List<GroundUnit> groundUnits = new();
	public List<AerialUnit> aerialUnits = new();
	public List<NavalUnit> navalUnits = new();
	public List<Base> bases = new();
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
			if (gameObject.GetComponent<GroundUnit>() != null) {
				groundUnits.RemoveAt(index);
				gameObject.GetComponent<GroundUnit>().equipmentList.ForEach(e => Destroy(e.gameObject));
				groundUnits.Insert(index, null);
			} else if (gameObject.GetComponent<AerialUnit>() != null) {
				aerialUnits.RemoveAt(index);
				gameObject.GetComponent<AerialUnit>().equipmentList.ForEach(e => Destroy(e.gameObject));
				aerialUnits.Insert(index, null);
			} else {
				navalUnits.RemoveAt(index);
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
		int count = Math.Max(groundUnits.Count, Math.Max(aerialUnits.Count, navalUnits.Count));

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

	public static string GenerateName(int domain) {
		System.Random random = new();
		bool notFound = true;
		while (notFound) {
			// Generate a random name
			string newIdentifier = random.Next(1, 100).ToString();
			// Check if the name is already taken
			foreach (Unit u in domain == 0 ? Instance.groundUnits.Cast<Unit>() : domain == 1 ? Instance.aerialUnits.Cast<Unit>() : Instance.navalUnits.Cast<Unit>()) {
				if (u != null && u.name != newIdentifier) {
					return newIdentifier;
				}
			}
		}
		return random.Next(1, 100).ToString();
	}

	#endregion

	#region Unit en-masse updates

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

	#region Initialization of sheet data
	internal void SpawnBases(IList<IList<object>> bases) {
		for (int i = 0; i < bases.Count; i++) {
			if (bases[i].Count == 5) {
				Instance.SpawnBase(bases[i][0].ToString(), new Vector3(Convert.ToSingle(bases[i][1], ApplicationController.culture), Convert.ToSingle(bases[i][2], ApplicationController.culture), -0.1f), (BaseType)Enum.Parse(typeof(BaseType), bases[i][3].ToString()), EnumUtil.ConvertIntToBool(Convert.ToInt16(bases[i][4])));
				SheetSync.basesLength++;
			}
		}
	}

	internal void SpawnUnits(IList<IList<object>> units) {
		for (int i = 0; i < units.Count; i++) {
			if (units[i].Count > 8) {
				int domain = Convert.ToInt16(units[i][2]);
				Unit newUnit = Instance.SpawnUnit(
					new Vector3(Convert.ToSingle(units[i][0], ApplicationController.culture), Convert.ToSingle(units[i][1], ApplicationController.culture), -0.1f),
					(UnitTier)Enum.Parse(typeof(UnitTier), units[i][5].ToString()), //Tier
					units[i][4].ToString(), //Spec.
					new List<Equipment>(), //Equipment blank list
					EnumUtil.ConvertIntToBool(Convert.ToInt16(units[i][6])), //Side
					Convert.ToInt16(units[i][3]), //Side
					(GroundMovementType)Enum.Parse(typeof(GroundMovementType), units[i][7].ToString()), //Ground movement type
					(GroundTransportType)Enum.Parse(typeof(GroundTransportType), units[i][8].ToString()), //Ground transport type
					domain); //Domain
				if (units[i].Count > 9) {
					//Equipment
					string[] lines = units[i][9].ToString().Split('\n');

					for (int j = 0; j < lines.Length; j++) {
						string[] word = lines[j].Split(':');

						if (!newUnit.SideB) {
							EquipmentManager.equipmentHostile[domain].ForEach(delegate (Equipment equipment) {
								if (equipment.equipmentName == word[0]) {
									newUnit.AddEquipment(EquipmentManager.CreateEquipment(equipment, Convert.ToInt16(word[1])));
									return;
								}
							});
						} else {
							EquipmentManager.equipmentFriendly[domain].ForEach(delegate (Equipment equipment) {
								if (equipment.equipmentName == word[0]) {
									newUnit.AddEquipment(EquipmentManager.CreateEquipment(equipment, Convert.ToInt16(word[1])));
									return;
								}
							});
						}
					}
				}
				SheetSync.unitsLength++;
			}
		}
	}
	#endregion
}

