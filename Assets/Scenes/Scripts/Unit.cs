using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Unit : MonoBehaviour {
	internal UnitType unitType;
	private UnitAffiliation unitAffiliation;
	private UnitMobility unitMobility;
	private UnitMobilityModifier unitMobilityModifiers;
	private UnitTopModifier unitTopModifier;
	private UnitTier unitTier;
	private string unitName = "";
	private List<string> unitEquipment = new List<string>();
	private Texture2D unitIcon;
	private GameObject range;
	private GameObject sightRange;

	// Use this for initialization
	void Start() {
		unitIcon = UnitTypesManager.Instance.GetUnitTexture(unitType);
		range = transform.Find("Range").gameObject;
		sightRange = transform.Find("Sight").gameObject;
	}

	// Update is called once per frame
	void Update() {
			
	}

	private void OnMouseOver() {
		sightRange.gameObject.SetActive(true);
		range.gameObject.SetActive(true);
	}

	private void OnMouseExit() {
		sightRange.gameObject.SetActive(false);
		range.gameObject.SetActive(false);
	}

	public Unit(UnitType unitType, UnitAffiliation unitAffiliation, UnitMobility unitMobility, UnitMobilityModifier unitMobilityModifiers, UnitTopModifier unitTopModifier, UnitTier unitTier, string unitName, List<string> unitEquipment) {
		this.unitType = unitType;
		this.unitAffiliation = unitAffiliation;
		this.unitMobility = unitMobility;
		this.unitMobilityModifiers = unitMobilityModifiers;
		this.unitTopModifier = unitTopModifier;
		this.unitTier = unitTier;
		this.unitName = unitName ?? throw new ArgumentNullException(nameof(unitName));
		this.unitEquipment = unitEquipment ?? throw new ArgumentNullException(nameof(unitEquipment));
	}

	internal void ChangeType(UnitType newType, UnitMobility newMobility, UnitMobilityModifier newBottomModifier, UnitTopModifier newTopModifier) {
		unitType = newType;
		UnitTypesManager.Instance.GetUnitTexture(newType);
	}
}

