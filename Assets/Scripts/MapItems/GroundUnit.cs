using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class GroundUnit : Unit {
	[SerializeField]
	internal GroundSpecialization specialization = GroundSpecialization.None;
	internal MeshRenderer movementTexture;
	[SerializeField]
	internal GroundMovementType movementModifier = GroundMovementType.None;
	internal MeshRenderer transportTexture;
	[SerializeField]
	internal GroundTransportType transportModifier = GroundTransportType.None;

	public void Initiate(int newID, Vector3 newPosition, UnitTier newTier, string newName, List<Equipment> newEquipment, bool newSideB, int newSpecialization, GroundMovementType newMovement, GroundTransportType newTransport) {
		//Texture handling
		movementTexture = transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
		transportTexture = transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>();
		ChangeSpecialization(newMovement);
		ChangeSpecialization(newTransport);
		Initiate(newID, newPosition, newTier, newName, newEquipment, newSideB, newSpecialization: newSpecialization);
	}

	internal override void ChangeAffiliation() {
		bool isEnemy = aC.sideB != SideB;
		//True if the unit is on the same side as the user
		if (isEnemy) {
			iconImage.transform.localScale = Vector3.one;
			movementTexture.transform.localScale = Vector3.one;
			transportTexture.transform.localScale = Vector3.one;
		} else {
			iconImage.transform.localScale = new Vector3(1.5f, 1, 1);
			movementTexture.transform.localScale = new Vector3(1.5f, 1, 1);
			transportTexture.transform.localScale = new Vector3(1.5f, 1, 1);
		}
		iconImage.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, isEnemy);
		movementTexture.material.mainTexture = UnitManager.Instance.GetMovementTexture(this, isEnemy);
		transportTexture.material.mainTexture = UnitManager.Instance.GetTransportTexture(this, isEnemy);
	}

	internal override void ChangeSpecialization(int specialization) {
		this.specialization = (GroundSpecialization)specialization;
		iconImage.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, SideB);
	}
	internal void ChangeSpecialization(GroundMovementType movementModifier) {
		this.movementModifier = movementModifier;
		movementTexture.material.mainTexture = UnitManager.Instance.GetMovementTexture(this, SideB);
	}
	internal void ChangeSpecialization(GroundTransportType transportModifier) {
		this.transportModifier = transportModifier;
		transportTexture.material.mainTexture = UnitManager.Instance.GetTransportTexture(this, SideB);
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(GroundUnit))]
public class GroundUnitEditor : Editor {
	public override void OnInspectorGUI() {
		// Draw the default inspector
		DrawDefaultInspector();

		GroundUnit unit = (GroundUnit)target;
		FieldInfo[] fields = typeof(GroundUnit).GetFields(BindingFlags.NonPublic);
		foreach (FieldInfo field in fields) {
			EditorGUILayout.LabelField(field.Name, field.GetValue(unit).ToString());
		}
	}
}
#endif