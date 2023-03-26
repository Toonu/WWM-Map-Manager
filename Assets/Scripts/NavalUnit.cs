using UnityEngine;

public class NavalUnit : Unit {
	internal NavalSpecialization specialization = NavalSpecialization.None;

	public void Initiate(object ID, Vector3 position, UnitTier unitTier, string unitName, bool enemy, NavalSpecialization specialization) {
		this.specialization = specialization;
		
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

	internal void ChangeSpecialization(NavalSpecialization specialization) {
		this.specialization = specialization;
		main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemySide);
	}
}