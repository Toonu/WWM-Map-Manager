using UnityEngine;

public class NavalUnit : Unit {
	internal NavalSpecialization specialization = NavalSpecialization.None;

	public void Initiate(object ID, Vector3 position, UnitTier unitTier, string unitName, bool enemy, NavalSpecialization specialization) {
		this.specialization = specialization;
		Initiate(ID, position, unitTier, unitName);
	}

	internal void SwapAffiliation(bool enemy) {
		main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemy);
	}

	internal void ChangeSpecialization(NavalSpecialization specialization) {
		this.specialization = specialization;
		main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemySide);
	}
}