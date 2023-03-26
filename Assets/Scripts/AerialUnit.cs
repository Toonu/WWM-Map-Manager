using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
	public class AerialUnit : Unit {
		private bool isFixedAviation = true;
		internal AerialSpecialization specialization = AerialSpecialization.None;
		private List<Equipment> unitEquipment = new List<Equipment>();

		public void Initiate(object unitID, Vector3 position, UnitTier unitTier, string unitName, bool enemy,
			AerialSpecialization specialization, List<Equipment> unitEquipment) {
			if (specialization > AerialSpecialization.UAV) isFixedAviation = false;
			this.specialization = specialization;
			this.unitEquipment = unitEquipment;
			Initiate(unitID, position, unitTier, unitName);
		}

		internal void SwapAffiliation(bool enemy) {
			main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemy);
		}

		internal void ChangeSpecialization(AerialSpecialization specialization) {
			this.specialization = specialization;
			main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemySide);
		}
	}
}