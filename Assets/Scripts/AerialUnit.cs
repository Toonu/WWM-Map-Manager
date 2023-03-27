using System.Collections.Generic;
using UnityEngine;

public class AerialUnit : Unit {
	private bool isFixedAviation = true;
	internal AerialSpecialization specialization = AerialSpecialization.None;

	public void Initiate(object ID, Vector3 position, UnitTier unitTier, string unitName, bool enemy, AerialSpecialization specialization, List<Equipment> unitEquipment) {
		if (specialization > AerialSpecialization.UAV) {
			isFixedAviation = false;
		}
		
		Initiate(ID, position, unitTier, unitName, unitEquipment);
		main.transform.parent.GetChild(0).gameObject.SetActive(false);
		main.transform.parent.GetChild(1).gameObject.SetActive(false);
		ChangeSpecialization(specialization);
		ChangeAffiliation(enemy);
	}

	internal void ChangeAffiliation(bool enemy) {
		enemySide = enemy;
		if (enemy) {
			main.transform.localScale = new Vector3(0.8f, 0.8f, 1);
		} else {
			main.transform.localScale = Vector3.one;
		}
		main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemy);
	}

	internal void ChangeSpecialization(AerialSpecialization specialization) {
		this.specialization = specialization;
		main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemySide);
	}
}

