using System.Collections.Generic;
using UnityEngine;

public class NavalUnit : Unit {
	internal NavalSpecialization specialization = NavalSpecialization.None;

	public override void Initiate(int newID, Vector3 newPosition, UnitTier newTier, string newName, List<Equipment> newEquipment, bool newSideB, int newSpecialization) {
		//Disabling texture not required for naval units
		transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
		transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
		base.Initiate(newID, newPosition, newTier, newName, newEquipment, newSideB, newSpecialization: newSpecialization);
		//Naval aspect ratio rectangular.
		iconImage.transform.localScale = Vector3.one;
	}

	internal override void ChangeAffiliation() {
		iconImage.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, aC.sideB != SideB);
	}

	internal override void ChangeSpecialization(int specialization) {
		this.specialization = (NavalSpecialization)specialization;
		iconImage.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, SideB);
	}
}