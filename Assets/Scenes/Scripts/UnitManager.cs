using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;
using System.Linq;

public class UnitManager : MonoBehaviour {
    public static UnitManager Instance {
        get { return _instance; }
    }
    private static UnitManager _instance;

    public Dictionary<UnitType, Texture2D> unitTextures = new Dictionary<UnitType, Texture2D>();
    public GameObject spawningUI;
	public GameObject editorUI;
	public List<Unit> units = new List<Unit>();

    // Use this for initialization
    void Start() => _instance = GetComponent<UnitManager>();

    // Update is called once per frame
    void Update() {
			
	}


	internal void PopulateUI() {
        TMP_Dropdown currentSpawningBottom = spawningUI.transform.Find("SpawningMenu/UnitType").GetComponent<TMP_Dropdown>();
		string[] enumNames = Enum.GetNames(typeof(UnitType));
		currentSpawningBottom.AddOptions(enumNames.ToList());

		//UnitAffiliation set by spawning base affiliation
		//Equipment is set by sheet per unit setup
		currentSpawningBottom = spawningUI.transform.Find("SpawningMenu/UnitMobility").GetComponent<TMP_Dropdown>();
		enumNames = Enum.GetNames(typeof(UnitMobility));
		currentSpawningBottom.AddOptions(enumNames.ToList());
		currentSpawningBottom = spawningUI.transform.Find("SpawningMenu/UnitMobilityModifier").GetComponent<TMP_Dropdown>();
		enumNames = Enum.GetNames(typeof(UnitMobilityModifier));
		currentSpawningBottom.AddOptions(enumNames.ToList());
		currentSpawningBottom = spawningUI.transform.Find("SpawningMenu/UnitTopModifier").GetComponent<TMP_Dropdown>();
		enumNames = Enum.GetNames(typeof(UnitTopModifier));
		currentSpawningBottom.AddOptions(enumNames.ToList());
		currentSpawningBottom = spawningUI.transform.Find("SpawningMenu/UnitTier").GetComponent<TMP_Dropdown>();
		enumNames = Enum.GetNames(typeof(UnitTier));
		currentSpawningBottom.AddOptions(enumNames.ToList());
		
	}


	internal Texture2D GetUnitTexture(UnitType type) {
		Texture2D output;
		unitTextures.TryGetValue(type, out output);
		return output;
    }

	public void ShowMissileRanges(bool show) {
		foreach (Unit unit in units) {
			if (unit.unitType == UnitType.SAM) {
				unit.weaponRange.SetActive(show);
			}
		}
	}
}

