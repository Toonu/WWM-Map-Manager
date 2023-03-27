using System;
using TMPro;
using UnityEngine;

public class UnitEditor : MonoBehaviour {
    private int domain;
	private Unit unit;
	private GroundUnit groundUnit;
	private AerialUnit aerialUnit;
	private NavalUnit navalUnit;
	private TMP_Dropdown tier;
	private TMP_Dropdown specialization;
	private TMP_Dropdown movement;
	private TMP_Dropdown transport;
	private TMP_InputField unitName;

	public void Awake() { 
		tier = transform.Find("UnitTier").GetComponent<TMP_Dropdown>();
		specialization = transform.Find("Specialization").GetComponent<TMP_Dropdown>();
		movement = transform.Find("GroundMovementType").GetComponent<TMP_Dropdown>();
		transport = transform.Find("GroundTransportType").GetComponent<TMP_Dropdown>();
		unitName = transform.Find("UnitName").GetComponent<TMP_InputField>();
	}

	public void UpdateSpecialization(int i) {
		switch (domain) {
			case 1:
			aerialUnit.ChangeSpecialization((AerialSpecialization)i);
			break;
			case 2:
			navalUnit.ChangeSpecialization((NavalSpecialization)i);
			break;
			default:
			groundUnit.ChangeSpecialization((GroundSpecialization)i);
			break;
		}
	}
	public void UpdateMovementModifier(int i) {
		GroundMovementType movementModifier = (GroundMovementType)i;
		groundUnit.ChangeSpecialization(movementModifier);

	}
	public void UpdateTransportModifier(int i) {
		GroundTransportType transportModifier = (GroundTransportType)i;
		groundUnit.ChangeSpecialization(transportModifier);
	}
	public void UpdateTier(int i) {
		unit.ChangeTier(i);
		UpdateName(unit.name);
		unitName.text = unit.name;
	}
	public void UpdateName(string identification) {
		unit.ChangeName(identification);
	}
	public void UpdateUnit(Unit unit) {
		this.unit = unit;
		if (unit.GetType() == typeof(GroundUnit)) {
			groundUnit = (GroundUnit)unit;
			domain = 0;
		} else if (unit.GetType() == typeof(AerialUnit)) {
			aerialUnit = (AerialUnit)unit;
			domain = 1;
		} else {
			navalUnit = (NavalUnit)unit;
			domain = 2;
		}
		UnitManager.Instance.PopulateUI(gameObject, domain);
		UpdateLabelTier(Convert.ToInt16(unit.UnitTier));
		UpdateLabelSpecialization(Unit.GetSpecialization(domain, unit));
		UpdateLabelName(unit.name);
		if (domain == 0) {
			UpdateGroundItems((GroundUnit)unit);
		}
	}

	private void UpdateGroundItems(GroundUnit unit) {
		movement.value = (int)unit.movementModifier;
		transport.value = (int)unit.transportModifier;
	}

	private void UpdateLabelName(string name) {
		this.unitName.text = name;
	}

	private void UpdateLabelSpecialization(int specialization) {
		this.specialization.value = specialization;
	}

	private void UpdateLabelTier(int tier) {
		this.tier.value = tier;
	}
}