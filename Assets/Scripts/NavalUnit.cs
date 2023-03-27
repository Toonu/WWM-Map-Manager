using System.Collections.Generic;
using UnityEngine;

public class NavalUnit : Unit {
	internal NavalSpecialization specialization = NavalSpecialization.None;

	public void Initiate(object ID, Vector3 position, UnitTier unitTier, string unitName, bool enemy, NavalSpecialization specialization, List<Equipment> unitEquipment) {
		this.specialization = specialization;
		
		Initiate(ID, position, unitTier, unitName, unitEquipment);
		main.transform.parent.GetChild(0).gameObject.SetActive(false);
		main.transform.parent.GetChild(1).gameObject.SetActive(false);
		main.transform.localScale = Vector3.one;
		ChangeSpecialization(specialization);
		ChangeAffiliation(enemy);
	}

	internal void ChangeAffiliation(bool enemy) {
		enemySide = enemy;
		main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemy);
	}

	internal void ChangeSpecialization(NavalSpecialization specialization) {
		this.specialization = specialization;
		main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemySide);
	}
}