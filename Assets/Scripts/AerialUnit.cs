

using System.Collections.Generic;
using UnityEngine;

public class AerialUnit : Unit {
	private bool isFixedAviation = true;
	internal AerialSpecialization specialization = AerialSpecialization.None;
	private List<Equipment> unitEquipment = new List<Equipment>();

	public void Initiate(object ID, Vector3 position, UnitTier unitTier, string unitName, bool enemy, AerialSpecialization specialization, List<Equipment> unitEquipment) {
		if (specialization > AerialSpecialization.UAV) {
			isFixedAviation = false;
		}
		this.specialization = specialization;
		this.unitEquipment = unitEquipment;
		Initiate(ID, position, unitTier, unitName);
	}

	internal void SwapAffiliation(bool enemy) {
		main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemy);
	}

	internal void ChangeSpecialization(AerialSpecialization specialization) {
		this.specialization = specialization;
		main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemySide);
	}
}

