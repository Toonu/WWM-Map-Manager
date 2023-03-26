using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts {
	public class UnitManager : MonoBehaviour {
		public static UnitManager Instance { get; private set; }


		// Use this for initialization
		private void Start() {
			Instance = GetComponent<UnitManager>();

			//SpawnUnit(Vector3.zero, 0, Convert.ToInt16(GroundSpecialization.SPG), GroundMovementType.Wheeled, GroundTransportType.None, UnitTier.Battalion, "5");
			SpawnBase("Lund", new Vector3(0.55f, -0.74f, -1), BaseType.Airfield);


			unitSpawnMenu = unitUIMenus.transform.Find("UnitSpawningMenu").gameObject;
			unitEditMenu = unitUIMenus.transform.Find("UnitEditMenu").gameObject;
			equipmentMenu = unitUIMenus.transform.Find("EquipmentMenu").gameObject;

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


		#region Spawning

		internal void SpawnBase(string identification, Vector3 position, BaseType baseType) {
			var newBase = Instantiate(baseTemplate, transform);
			var b = newBase.AddComponent<Base>();
			b.Initiate(identification, position, baseType);
			bases.Add(b);
		}

		internal void SpawnUnit(Vector3 position, int domain, int specialization,
			string identification, UnitTier tier, bool isEnemy, GroundMovementType movementModifier,
			GroundTransportType transportModifier) {
			var newUnit = Instantiate(groundTemplate, transform);
			var i = GetLast();
			if (domain == 0) {
				var unit = newUnit.AddComponent<GroundUnit>();
				unit.Initiate(i, position, tier, identification, isEnemy, (GroundSpecialization)specialization,
					movementModifier, transportModifier, null, null);
				groundUnits.Insert(i, unit);
			}
			else if (domain == 1) {
				var unit = newUnit.AddComponent<AerialUnit>();
				unit.Initiate(i, position, tier, identification, isEnemy, (AerialSpecialization)specialization, null);
				aerialUnits.Insert(i, unit);
			}
			else {
				var unit = newUnit.AddComponent<NavalUnit>();
				unit.Initiate(i, position, tier, identification, isEnemy, (NavalSpecialization)specialization);
				navalUnits.Insert(i, unit);
			}
		}

		internal void Despawn(GameObject gameObject) {
			if (gameObject.GetComponent<GroundUnit>() != null)
				groundUnits.RemoveAt(gameObject.GetComponent<Unit>().id);
			else if (gameObject.GetComponent<AerialUnit>() != null)
				aerialUnits.RemoveAt(gameObject.GetComponent<Unit>().id);
			else
				navalUnits.RemoveAt(gameObject.GetComponent<Unit>().id);
		}

		public int GetLast() {
			return groundUnits.Count;
			for (var i = 0;
			     i <= groundUnits.Count + 1 || i <= aerialUnits.Count + 1 || i <= navalUnits.Count + 1;
			     i++) {
				Debug.Log(i);
				if (groundUnits[i] || aerialUnits[i] == null || navalUnits[i] == null) {
					Debug.Log(i);
					return i;
				}
			}

			return 0;
		}

		#endregion

		#region TextureHandling

		internal Texture2D GetSpecialisationTexture(GroundUnit unit, bool enemy) {
			if (enemy) return groundSpecializationEnemy[Convert.ToInt16(unit.specialization)];

			return groundSpecialization[Convert.ToInt16(unit.specialization)];
		}

		internal Texture2D GetSpecialisationTexture(AerialUnit unit, bool enemy) {
			if (enemy) return navalSpecializationEnemy[Convert.ToInt16(unit.specialization)];

			return navalSpecialization[Convert.ToInt16(unit.specialization)];
		}

		internal Texture2D GetSpecialisationTexture(NavalUnit unit, bool enemy) {
			if (enemy) return aerialSpecializationEnemy[Convert.ToInt16(unit.specialization)];

			return aerialSpecialization[Convert.ToInt16(unit.specialization)];
		}

		internal Texture2D GetMovementTexture(GroundUnit unit, bool enemy) {
			if (enemy) return movementType[Convert.ToInt16(unit.movementModifier)];

			return movementType[Convert.ToInt16(unit.movementModifier)];
		}

		internal Texture2D GetTransportTexture(GroundUnit unit, bool enemy) {
			if (enemy) return transportType[Convert.ToInt16(unit.transportModifier)];

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
				case int i when i >= 7 && i <= 11:
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
			       hundreds[unit % 1000 / 100] +
			       tens[unit % 100 / 10] +
			       ones[unit % 10];
		}

		internal string GetCorps(string unit) {
			if (unit == "") return "0";

			return GetCorps(Convert.ToInt16(unit));
		}

		public void ShowMissileRanges(bool show) {
			foreach (var unit in groundUnits)
				if (unit.specialization == GroundSpecialization.SAM)
					unit.weaponRange.SetActive(show);
		}

		public void SwitchSide(bool side) {
			foreach (var unit in groundUnits) unit.ChangeAffiliation(side);

			foreach (var unit in aerialUnits) unit.SwapAffiliation(side);

			foreach (var unit in navalUnits) unit.SwapAffiliation(side);
		}

		#endregion

		#region UI

		public void PopulateUI(int domain) {
			PopulateUI(unitSpawnMenu, domain);
		}

		public void PopulateUI(GameObject menu, int domain = 0) {
			var currentSpawningDropdownMenu =
				menu.transform.Find("Specialization").GetComponent<TMP_Dropdown>();
			string[] enumNames;
			var options = new List<TMP_Dropdown.OptionData>();

			switch (domain) {
				case 1:
					enumNames = Enum.GetNames(typeof(AerialSpecialization));
					for (var i = 0; i < enumNames.Length; i++)
						options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(aerialSpecialization[i],
							new Rect(0, 0, aerialSpecialization[i].width, aerialSpecialization[i].height),
							new Vector2(0.5f, 0.5f))));

					break;
				case 2:
					enumNames = Enum.GetNames(typeof(NavalSpecialization));
					for (var i = 0; i < enumNames.Length; i++)
						options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(navalSpecialization[i],
							new Rect(0, 0, navalSpecialization[i].width, navalSpecialization[i].height),
							new Vector2(0.5f, 0.5f))));

					break;
				default:
					enumNames = Enum.GetNames(typeof(GroundSpecialization));
					for (var i = 0; i < enumNames.Length; i++)
						options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(groundSpecialization[i],
							new Rect(0, 0, groundSpecialization[i].width, groundSpecialization[i].height),
							new Vector2(0.5f, 0.5f))));

					break;
			}


			currentSpawningDropdownMenu.ClearOptions();
			currentSpawningDropdownMenu.AddOptions(options);


			//UnitAffiliation set by spawning base affiliation

			switch (domain) {
				case 0:
					currentSpawningDropdownMenu =
						menu.transform.Find("GroundMovementType").GetComponent<TMP_Dropdown>();
					currentSpawningDropdownMenu.gameObject.SetActive(true);
					enumNames = Enum.GetNames(typeof(GroundMovementType));
					options = new List<TMP_Dropdown.OptionData>();
					for (var i = 0; i < enumNames.Length; i++)
						options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(movementType[i],
							new Rect(0, 0, movementType[i].width, movementType[i].height), new Vector2(0.5f, 0.5f))));

					currentSpawningDropdownMenu.ClearOptions();
					currentSpawningDropdownMenu.AddOptions(options);
					currentSpawningDropdownMenu =
						menu.transform.Find("GroundTransportType").GetComponent<TMP_Dropdown>();
					currentSpawningDropdownMenu.gameObject.SetActive(true);
					enumNames = Enum.GetNames(typeof(GroundTransportType));
					options = new List<TMP_Dropdown.OptionData>();
					for (var i = 0; i < enumNames.Length; i++)
						options.Add(new TMP_Dropdown.OptionData(enumNames[i], Sprite.Create(transportType[i],
							new Rect(0, 0, transportType[i].width, transportType[i].height), new Vector2(0.5f, 0.5f))));

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
}