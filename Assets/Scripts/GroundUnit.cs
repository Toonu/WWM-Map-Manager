
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GroundUnit : Unit {
	private List<Equipment> unitEquipment = new List<Equipment>();
	private Unit parentUnit;
	private TextMeshProUGUI equipment;
	private TextMeshProUGUI higherEchelon;

	internal GroundSpecialization specialization = GroundSpecialization.None;
	internal MeshRenderer movementTexture;
	internal GroundMovementType movementModifier = GroundMovementType.None;
	internal MeshRenderer transportTexture;
	internal GroundTransportType transportModifier = GroundTransportType.None;

	public void Initiate(object ID, Vector3 position, UnitTier unitTier, string unitName, bool enemy, GroundSpecialization specialization, GroundMovementType movementModifier, GroundTransportType transportModifier, GroundUnit higherUnit, List<Equipment> unitEquipment) {
		equipment = transform.Find("Canvas/Eq").gameObject.GetComponent<TextMeshProUGUI>();
		equipment.text = "";
		higherEchelon = transform.Find("Canvas/HigherEchelon").gameObject.GetComponent<TextMeshProUGUI>();
		main = transform.GetChild(0).GetChild(2).GetComponent<MeshRenderer>();
		movementTexture = main.transform.parent.GetChild(0).GetComponent<MeshRenderer>();
		transportTexture = main.transform.parent.GetChild(1).GetComponent<MeshRenderer>();

		SwapAffiliation(enemy);

		Initiate(ID, position, unitTier, unitName);

		ChangeSpecialization(specialization);
		ChangeSpecialization(movementModifier);
		ChangeSpecialization(transportModifier);
		

		if (higherUnit != null) {
			parentUnit = higherUnit;
			higherEchelon.text = parentUnit.unitName.text;
		}

		if (unitEquipment != null) {
			this.unitEquipment = unitEquipment;
		}
		
	}

	private void OnMouseOver() {
		equipment.gameObject.SetActive(true);
		base.OnMouseOver();
	}

	private void OnMouseExit() {
		equipment.gameObject.SetActive(false);
		base.OnMouseExit();
	}

	internal void ChangeSpecialization(GroundSpecialization specialization) {
		this.specialization = specialization;
		main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemy);
	}
	internal void ChangeSpecialization(GroundMovementType movementModifier) {
		this.movementModifier = movementModifier;
		movementTexture.material.mainTexture = UnitManager.Instance.GetMovementTexture(this, enemy);
	}
	internal void ChangeSpecialization(GroundTransportType transportModifier) {
		this.transportModifier = transportModifier;
		transportTexture.material.mainTexture = UnitManager.Instance.GetTransportTexture(this, enemy);
	}

	internal void SwapAffiliation(bool newEnemy) {
		enemy = newEnemy;
		main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemy);
		movementTexture.material.mainTexture = UnitManager.Instance.GetMovementTexture(this, enemy);
		transportTexture.material.mainTexture = UnitManager.Instance.GetTransportTexture(this, enemy);
	}
}