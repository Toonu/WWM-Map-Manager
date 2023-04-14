using System.Collections.Generic;
using UnityEngine;

public class AerialUnit : Unit {
	internal AerialSpecialization specialization = AerialSpecialization.None;

	public void Initiate(int ID, Vector3 position, UnitTier unitTier, string unitName, bool sideB, int specialization, List<Equipment> unitEquipment) {
		Initiate(ID, position, unitTier, unitName, specialization, unitEquipment);
		main.transform.parent.GetChild(0).gameObject.SetActive(false);
		main.transform.parent.GetChild(1).gameObject.SetActive(false);
		this.sideB = sideB;
		ChangeAffiliation();
	}

	internal void ChangeAffiliation() {
		bool sideB = aC.sideB == this.sideB;
		if (sideB) {
			main.transform.localScale = Vector3.one;
		} else {
			main.transform.localScale = new Vector3(0.8f, 0.8f, 1); 
		}
		main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, !sideB);
		Debug.Log($"[{id}][{name}] Affiliation changed");
	}
}

