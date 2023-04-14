using System.Collections.Generic;
using UnityEngine;

public class NavalUnit : Unit {
	internal NavalSpecialization specialization = NavalSpecialization.None;

	public void Initiate(int ID, Vector3 position, UnitTier unitTier, string unitName, bool sideB, int specialization, List<Equipment> unitEquipment) {
		Initiate(ID, position, unitTier, unitName, specialization, unitEquipment);
		main.transform.parent.GetChild(0).gameObject.SetActive(false);
		main.transform.parent.GetChild(1).gameObject.SetActive(false);
		main.transform.localScale = Vector3.one;
		this.sideB = sideB;
		ChangeAffiliation();
	}

	internal void ChangeAffiliation() {
		main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, !(aC.sideB == sideB));
	}
}