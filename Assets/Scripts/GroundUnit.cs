using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GroundUnit : Unit {
	private Unit parentUnit;
	private TextMeshProUGUI higherEchelon;

	internal GroundSpecialization specialization = GroundSpecialization.None;
	internal MeshRenderer movementTexture;
	internal GroundMovementType movementModifier = GroundMovementType.None;
	internal MeshRenderer transportTexture;
	internal GroundTransportType transportModifier = GroundTransportType.None;

	public void Initiate(object ID, Vector3 position, UnitTier unitTier, string unitName, bool enemy, GroundSpecialization specialization, GroundMovementType movementModifier, GroundTransportType transportModifier, List<Equipment> unitEquipment, GroundUnit higherUnit) {
		higherEchelon = transform.Find("Canvas/HigherEchelon").gameObject.GetComponent<TextMeshProUGUI>();

		Initiate(ID, position, unitTier, unitName, unitEquipment);
		movementTexture = main.transform.parent.GetChild(0).GetComponent<MeshRenderer>();
		transportTexture = main.transform.parent.GetChild(1).GetComponent<MeshRenderer>();

		ChangeSpecialization(specialization);
		ChangeSpecialization(movementModifier);
		ChangeSpecialization(transportModifier);
		ChangeAffiliation(enemy);

		if (higherUnit != null) {
			parentUnit = higherUnit;
			higherEchelon.text = parentUnit.unitName.text;
		}
	}

	internal void ChangeSpecialization(GroundSpecialization specialization) {
		this.specialization = specialization;
		main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemySide);
	}
	internal void ChangeSpecialization(GroundMovementType movementModifier) {
		this.movementModifier = movementModifier;
		movementTexture.material.mainTexture = UnitManager.Instance.GetMovementTexture(this, enemySide);
	}
	internal void ChangeSpecialization(GroundTransportType transportModifier) {
		this.transportModifier = transportModifier;
		transportTexture.material.mainTexture = UnitManager.Instance.GetTransportTexture(this, enemySide);
	}

	internal void ChangeAffiliation(bool newEnemy) {
		enemySide = newEnemy;
		if (enemySide) {
			main.transform.localScale = Vector3.one;
			movementTexture.transform.localScale = Vector3.one;
			transportTexture.transform.localScale = Vector3.one;
		} else {
			main.transform.localScale = new Vector3(1.5f, 1, 1);
			movementTexture.transform.localScale = new Vector3(1.5f, 1, 1);
			transportTexture.transform.localScale = new Vector3(1.5f, 1, 1);
		}
		main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemySide);
		movementTexture.material.mainTexture = UnitManager.Instance.GetMovementTexture(this, enemySide);
		transportTexture.material.mainTexture = UnitManager.Instance.GetTransportTexture(this, enemySide);
	}
}