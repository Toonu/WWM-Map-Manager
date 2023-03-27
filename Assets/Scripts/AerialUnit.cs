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
		this.unitEquipment = unitEquipment;
		
		Initiate(ID, position, unitTier, unitName);
		main.transform.parent.GetChild(0).gameObject.SetActive(false);
		main.transform.parent.GetChild(1).gameObject.SetActive(false);
		ChangeSpecialization(specialization);
		ChangeAffiliation(enemy);
	}

	internal void ChangeAffiliation(bool enemy) {
		enemySide = enemy;
		if (enemySide) {
			main.transform.localScale = new Vector3(0.6f, 1, 1);
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

