using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class UnitManager : MonoBehaviour {
	public static UnitManager Instance {
		get { return _instance; }
	}
	private static UnitManager _instance;

	/// <summary>
	/// Method gathers the components on startup
	/// </summary>
	void Start() {
		_instance = GetComponent<UnitManager>();

		unitMenu = unitUIMenus.transform.Find("UnitMenu").gameObject;
		baseMenu = unitUIMenus.transform.Find("BaseMenu").gameObject;
		unitMenu.GetComponent<UnitConstructor>().Awake();
	}

	#region Textures
	public List<Sprite> protectionType = new();
	public List<Sprite> transportType = new();
	public List<Sprite> protectionTypeEnemy = new();
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
			return groundSpecializationEnemy[(int)unit.specialization].texture;
		}
		return groundSpecialization[(int)unit.specialization].texture;
	}
	internal Texture2D GetSpecialisationTexture(AerialUnit unit, bool isEnemy) {
		if (isEnemy) {
			return aerialSpecializationEnemy[(int)unit.specialization].texture;
		}
		return aerialSpecialization[(int)unit.specialization].texture;
	}
	internal Texture2D GetSpecialisationTexture(NavalUnit unit, bool isEnemy) {
		if (isEnemy) {
			return navalSpecializationEnemy[(int)unit.specialization].texture;
		}
		return navalSpecialization[(int)unit.specialization].texture;
	}
	internal Texture2D GetProtectionTexture(GroundUnit unit, bool isEnemy) {
		if (isEnemy) {
			return protectionTypeEnemy[(int)unit.protectionType].texture;
		}
		return protectionType[(int)unit.protectionType].texture;
	}
	internal Texture2D GetTransportTexture(GroundUnit unit, bool isEnemy) {
		if (isEnemy) {
			return transportTypeEnemy[(int)unit.transportType].texture;
		}
		return transportType[(int)unit.transportType].texture;
	}
	internal Texture2D GetBaseTexture(BaseType type) {
		return baseTypes[(int)type].texture;
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
	internal List<HigherUnit> higherEchelons = new();
	#endregion

	#region Spawning

	/// <summary>
	/// Method spawns a base and returns it.
	/// </summary>
	/// <param name="identification"></param>
	/// <param name="position"></param>
	/// <param name="baseType"></param>
	/// <param name="sideB"></param>
	/// <returns></returns>
	internal Base SpawnBase(string identification, Vector3 position, BaseType baseType, bool sideB, bool isGhost) {
		GameObject newBase = Instantiate(baseTemplate, transform.GetChild(0));
		Base b = newBase.AddComponent<Base>();
		b.Initiate(identification, position, baseType, sideB);
		b.isGhost = isGhost;
		bases.Add(b);
		return b;
	}

	/// <summary>
	/// Method spawns a higher echelon unit and returns it.
	/// </summary>
	/// <param name="position"></param>
	/// <param name="lowerUnits"></param>
	/// <param name="identification"></param>
	/// <returns></returns>
	internal HigherUnit SpawnHigherEchelonUnit(Vector3 position, List<Unit> lowerUnits, int identification = 0) {
		GameObject newUnit = Instantiate(groundTemplate, transform.GetChild(1));
		HigherUnit unit = newUnit.AddComponent<HigherUnit>();
		higherEchelons.Add(unit);
		int i = identification == 0 ? higherEchelons.IndexOf(unit) : identification;
		unit.Initiate(i, position, lowerUnits);
		unit.UnitParent = higherEchelons[0];
		return unit;
	}

	/// <summary>
	/// Method spawns a unit and returns it.
	/// </summary>
	/// <returns>Unit spawned unit</returns>
	internal Unit SpawnUnit(Vector3 position, UnitTier tier, string identification,
		List<Equipment> unitEquipment, bool sideB, int specialization, GroundProtectionType protectionType,
		GroundTransportType transportType, int domain) {
		GameObject newUnit = Instantiate(groundTemplate, transform);
		int i = GetLast();
		if (domain == 0) {
			GroundUnit unit = newUnit.AddComponent<GroundUnit>();
			unit.Initiate(i, position, tier, identification, unitEquipment, sideB, specialization, protectionType, transportType);
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

	/// <summary>
	/// Method despawns an Object.
	/// </summary>
	/// <param name="gameObject">Unit or Base Object</param>
	internal void Despawn(GameObject gameObject) {
		if (gameObject != null) {
			if (gameObject.GetComponent<Base>() == null) {
				int index = gameObject.GetComponent<Unit>().ID;
				if (gameObject.GetComponent<GroundUnit>() != null) {
					groundUnits.RemoveAt(index);
					gameObject.GetComponent<GroundUnit>().equipmentList.ForEach(e => Destroy(e.gameObject));
					groundUnits.Insert(index, null);
				}
				else if (gameObject.GetComponent<AerialUnit>() != null) {
					aerialUnits.RemoveAt(index);
					gameObject.GetComponent<AerialUnit>().equipmentList.ForEach(e => Destroy(e.gameObject));
					aerialUnits.Insert(index, null);
				}
				else {
					navalUnits.RemoveAt(index);
					gameObject.GetComponent<NavalUnit>().equipmentList.ForEach(e => Destroy(e.gameObject));
					navalUnits.Insert(index, null);
				}
			}
			else {
				bases.Remove(gameObject.GetComponent<Base>());
			}
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Method gets the last index of a list.
	/// </summary>
	/// <returns></returns>
	public int GetLast() {
		int maxLength = Math.Max(groundUnits.Count, Math.Max(aerialUnits.Count, navalUnits.Count));
		for (int i = 0; i < maxLength; i++) {
			if (groundUnits[i] == null && aerialUnits[i] == null && navalUnits[i] == null) {
				return i;
			}
		}
		return maxLength;
	}

	/// <summary>
	/// Method appends a unit to a list based on its domain.
	/// </summary>
	/// <typeparam name="J"></typeparam>
	/// <typeparam name="K"></typeparam>
	/// <typeparam name="L"></typeparam>
	/// <param name="obj"></param>
	/// <param name="index"></param>
	/// <param name="list"></param>
	/// <param name="otherList"></param>
	/// <param name="theOtherList"></param>
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

	/// <summary>
	/// Method generates a random number for a unit ID.
	/// </summary>
	/// <param name="domain"></param>
	/// <returns></returns>
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

	private class DistancePair {
		public Unit unit;
		public Unit distantUnit;
		public float distance;
		public DistancePair(Unit unit, Unit distantUnit, float distance) {
			this.unit = unit;
			this.distantUnit = distantUnit;
			this.distance = distance;
		}
		public override string ToString() => unit.name + " -> " + distantUnit.name + " : " + distance;
	}

	/// <summary>
	/// Method merges units of all three types into a single Unit-type list.
	/// </summary>
	/// <returns>List<Unit> containing all three unit types.</Unit></returns>
	public static List<Unit> MergeUnitLists() {
		List<Unit> units = new();
		units.AddRange(Instance.aerialUnits.Where(unit => unit != null).Cast<Unit>());
		units.AddRange(Instance.groundUnits.Where(unit => unit != null).Cast<Unit>());
		units.AddRange(Instance.navalUnits.Where(unit => unit != null).Cast<Unit>());
		return units;
	}

	/// <summary>
	/// Method updates all units weapon range visibility in the game.
	/// </summary>
	/// <param name="show"></param>
	public void ShowMissileRanges(bool show) {
		groundUnits.ForEach(unit => { if (unit != null && unit.specialization == GroundSpecialization.SAM) { unit.WeaponRangeCircle.SetActive(show); } });
	}

	/// <summary>
	/// Method updated all units affiliation on switching the user side.
	/// </summary>
	public void SwitchSide() {
		MergeUnitLists().ForEach(unit => unit.ChangeAffiliation());
		bases.ForEach(b => { if (b != null) { b.ChangeAffiliation(); } });
		higherEchelons.ForEach(e => { if (e != null) { e.ChangeAffiliation(); } });
	}

	/// <summary>
	/// Method groups units into groups of units that are close to each other.
	/// </summary>
	/// <returns>Units grouped by distance</returns>
	public static Dictionary<Unit, List<Unit>> GroupUnits() {
		//Merge unit lists into one
		List<Unit> units = MergeUnitLists();

		//Dictionary of distances to singular unit.
		Dictionary<Unit, List<Unit>> objectGroups = new();

		// Calculate distances between all units
		Dictionary<Unit, List<DistancePair>> objectDistances = new();
		foreach (Unit obj in units) {
			if (obj == null) continue;
			List<DistancePair> closeObjects = new();
			foreach (Unit otherObj in units) {
				if (otherObj == null || obj == otherObj || obj.SideB != otherObj.SideB) continue;
				closeObjects.Add(new DistancePair(obj, otherObj, Vector3.Distance(obj.transform.position, otherObj.transform.position)));
			}
			closeObjects.OrderBy(obj => obj.distance).ToList();
			objectDistances[obj] = closeObjects;
		}

		HashSet<Unit> usedUnits = new();

		// Group objects based on their distance from each other
		foreach (KeyValuePair<Unit, List<DistancePair>> keyPair in objectDistances) {
			if (keyPair.Value.Count == 0) continue;
			//Getting average distance
			float distance = 0.75f * keyPair.Value.Average(unit => unit.distance);
			//Sort through the distances and add the closest groups
			List<Unit> closeGroups = new();
			foreach (DistancePair distancePair in keyPair.Value) {
				//Add only units that aren't already grouped within the distance.
				if (distancePair.distance < distance && !usedUnits.Contains(distancePair.distantUnit)) {
					closeGroups.Add(distancePair.distantUnit);
					usedUnits.Add(distancePair.unit);
					usedUnits.Add(distancePair.distantUnit);
				}
			}
			if (closeGroups.Count > 0) {
				objectGroups[keyPair.Key] = closeGroups;
			}
		}

		//Add units that had no groups to fit inside
		foreach (Unit unit in units) {
			if (unit != null && !usedUnits.Contains(unit)) {
				objectGroups[unit] = new List<Unit>();
			}
		}

		//Creating higher echelons
		foreach (KeyValuePair<Unit, List<Unit>> keyPair in objectGroups) {
			keyPair.Value.Add(keyPair.Key);
			Unit higherUnit = Instance.SpawnHigherEchelonUnit(keyPair.Key.StartPosition, keyPair.Value);
		}

		//Hiding small units
		foreach (Unit unit in units) {
			if (unit != null) {
				unit.gameObject.SetActive(false);
			}
		}
		return objectGroups;
	}

	/// <summary>
	/// Method ungroups units and returns them to their original positions.
	/// </summary>
	internal void UnGroupUnits() {
		MergeUnitLists().ForEach(unit => unit.gameObject.SetActive(true));
		higherEchelons.Clear();
		for (int i = 0; i < transform.GetChild(1).childCount; i++) {
			Destroy(transform.GetChild(1).GetChild(i).gameObject);
		}
	}


	public void CalculateSpotting() {
		/*
		foreach (Unit unit in groundUnits) {
			if (unit != null) {
				foreach (Unit other in groundUnits) {
					if (other.SideB != ApplicationController.isSideB) {

					}
				}
			}
		}*/
		//TODO Fog of War - when moving and unit is close, spot it and disable reseting the unit, otherwise spotting at the end of the turn
	}


	public void ResetStartPositions() {
		//Check if the Unit or Base is on the right side and if so, reset its position.
		MergeUnitLists().ForEach(unit => {
			if (unit.SideB == ApplicationController.isSideB) {
				unit.StartPosition = unit.transform.position;
			}
		});
		bases.ForEach(b => { if (b != null && b.SideB == ApplicationController.isSideB) { b.StartPosition = b.transform.position; } });
		//TODO - Need to duplicate units using the isGhost attribute in Unit and its StartPosition from last turn. also add a method soft reseting everything at the end of the turn so their movement is fixed
	}


	/// <summary>
	/// Method removes units flagged as ghost, eg. spotted units.
	/// </summary>
	public void DeleteGhostUnits() {
		MergeUnitLists().RemoveAll(unit => unit.isGhost);
		bases.RemoveAll(b => b.isGhost == true);
	}

	#endregion

	#region Initialization of sheet data
	internal void SpawnBases(IList<IList<object>> bases) {
		for (int i = 0; i < bases.Count; i++) {
			if (bases[i].Count > 5) {
				Instance.SpawnBase(bases[i][0].ToString(), new Vector3(Convert.ToSingle(bases[i][1], ApplicationController.culture), Convert.ToSingle(bases[i][2], ApplicationController.culture), -0.1f), (BaseType)Enum.Parse(typeof(BaseType), bases[i][3].ToString()), EnumUtil.ConvertIntToBool(Convert.ToInt16(bases[i][4])), EnumUtil.ConvertIntToBool(Convert.ToInt16(bases[i][5])));
				SheetSync.basesLength++;
			}
		}
	}

	internal void SpawnUnits(IList<IList<object>> units) {
		for (int i = 0; i < units.Count; i++) {
			if (units[i].Count > 9) {
				int domain = Convert.ToInt16(units[i][0]);
				Vector3 startingFrom = new(Convert.ToSingle(units[i][9], ApplicationController.culture), Convert.ToSingle(units[i][10], ApplicationController.culture), -0.1f);
				Unit newUnit = Instance.SpawnUnit(
					new Vector3(Convert.ToSingle(units[i][1], ApplicationController.culture), Convert.ToSingle(units[i][2], ApplicationController.culture), -0.1f),
					(UnitTier)Convert.ToInt16(units[i][3].ToString()), //Tier
					units[i][4].ToString(), //Identification
					new List<Equipment>(), //Equipment blank list
					EnumUtil.ConvertIntToBool(Convert.ToInt16(units[i][5])), //Side
					Convert.ToInt16(units[i][6]), //Spec
					(GroundProtectionType)Convert.ToInt16(units[i][7]), //Ground protection
					(GroundTransportType)Convert.ToInt16(units[i][8]), //Ground transport
					domain); //Domain

				newUnit.StartPosition = startingFrom;
				newUnit.parentTextUI.text = EnumUtil.GetCorps(Convert.ToInt16(units[i][12]));
				newUnit.isGhost = EnumUtil.ConvertIntToBool(Convert.ToInt16(units[i][13]));
				
				string[] lines = units[i][11].ToString().Split('\n'); //Equipment

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
				SheetSync.unitsLength++;
			}
		}
	}
	#endregion
}

