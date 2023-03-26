using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts {
	public class GroundUnit : Unit {
		private TextMeshProUGUI equipment;
		private TextMeshProUGUI higherEchelon;
		internal GroundMovementType movementModifier = GroundMovementType.None;
		internal MeshRenderer movementTexture;
		private Unit parentUnit;

		internal GroundSpecialization specialization = GroundSpecialization.None;
		internal GroundTransportType transportModifier = GroundTransportType.None;
		internal MeshRenderer transportTexture;
		private List<Equipment> unitEquipment = new List<Equipment>();

		public void Initiate(object ID, Vector3 position, UnitTier unitTier, string unitName, bool enemy,
			GroundSpecialization specialization, GroundMovementType movementModifier,
			GroundTransportType transportModifier, GroundUnit higherUnit, List<Equipment> unitEquipment) {
			equipment = transform.Find("Canvas/Eq").gameObject.GetComponent<TextMeshProUGUI>();
			equipment.text = "";
			higherEchelon = transform.Find("Canvas/HigherEchelon").gameObject.GetComponent<TextMeshProUGUI>();
			main = transform.GetChild(0).GetChild(2).GetComponent<MeshRenderer>();
			movementTexture = main.transform.parent.GetChild(0).GetComponent<MeshRenderer>();
			transportTexture = main.transform.parent.GetChild(1).GetComponent<MeshRenderer>();

			ChangeAffiliation(enemy);

			Initiate(ID, position, unitTier, unitName);

			ChangeSpecialization(specialization);
			ChangeSpecialization(movementModifier);
			ChangeSpecialization(transportModifier);


			if (higherUnit != null) {
				parentUnit = higherUnit;
				higherEchelon.text = parentUnit.unitName.text;
			}

			if (unitEquipment != null) this.unitEquipment = unitEquipment;
		}

		private new void OnMouseOver() {
			equipment.gameObject.SetActive(true);
			base.OnMouseOver();
		}

		private new void OnMouseExit() {
			equipment.gameObject.SetActive(false);
			base.OnMouseExit();
		}

		internal void ChangeSpecialization(GroundSpecialization specialization) {
			this.specialization = specialization;
			main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemySide);
		}

		internal void ChangeSpecialization(GroundMovementType movementModifier) {
			this.movementModifier = movementModifier;
			movementTexture.material.mainTexture = UnitManager.Instance.GetMovementTexture(this, enemySide);
		}

		internal void ChangeSpecialization(GroundTransportType transportModifier) {
			this.transportModifier = transportModifier;
			transportTexture.material.mainTexture = UnitManager.Instance.GetTransportTexture(this, enemySide);
		}

		internal void ChangeAffiliation(bool newEnemy) {
			enemySide = newEnemy;
			main.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, enemySide);
			movementTexture.material.mainTexture = UnitManager.Instance.GetMovementTexture(this, enemySide);
			transportTexture.material.mainTexture = UnitManager.Instance.GetTransportTexture(this, enemySide);
		}
	}
}