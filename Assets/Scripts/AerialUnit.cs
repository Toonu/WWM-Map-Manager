using System.Collections.Generic;
using UnityEngine;

public class AerialUnit : Unit {
	private bool isFixedAviation = true;
	internal AerialSpecialization specialization = AerialSpecialization.None;

	public void Initiate(object ID, Vector3 position, UnitTier unitTier, string unitName, bool sideB, AerialSpecialization specialization, List<Equipment> unitEquipment) {
		if (specialization > AerialSpecialization.UAV) {
			isFixedAviation = false;
		}
		
		Initiate(ID, position, unitTier, unitName, unitEquipment);
		main.transform.parent.GetChild(0).gameObject.SetActive(false);
		main.transform.parent.GetChild(1).gameObject.SetActive(false);
		ChangeSpecialization(specialization);
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
		Debug.Log($"[{name}][{id}] Affiliation changed");
	}

	internal void ChangeSpecialization(AerialSpecialization specialization) {
		this.specialization = specialization;
		main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, sideB);
		Debug.Log($"[{name}][{id}] Specialization changed | {specialization}");
	}
}

