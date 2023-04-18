using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GroundUnit : Unit {
	internal GroundSpecialization specialization = GroundSpecialization.Infantry;
	internal MeshRenderer movementTexture;
	internal GroundMovementType movementModifier = GroundMovementType.Motorized;
	internal MeshRenderer transportTexture;
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
		bool isEnemy = ApplicationController.sideB != SideB;
		//True if the unit is on the same sideB as the user
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

	internal override void ChangeSpecialization(int newSpecialization) {
		specialization = (GroundSpecialization)newSpecialization;
		iconImage.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, SideB);
		Debug.Log($"[{ID}][{name}] Specialization changed | {specialization}");
	}
	internal void ChangeSpecialization(GroundMovementType movementModifier) {
		this.movementModifier = movementModifier;
		movementTexture.material.mainTexture = UnitManager.Instance.GetMovementTexture(this, SideB);
		Debug.Log($"[{ID}][{name}] Movement changed | {movementModifier}");
	}
	internal void ChangeSpecialization(GroundTransportType transportModifier) {
		this.transportModifier = transportModifier;
		transportTexture.material.mainTexture = UnitManager.Instance.GetTransportTexture(this, SideB);
		Debug.Log($"[{ID}][{name}] Transport changed | {transportModifier}");
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(GroundUnit))]
public class GroundUnitEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		GroundUnit unit = (GroundUnit)target;

		EditorGUILayout.LabelField("ID", unit.ID.ToString());
		EditorGUILayout.LabelField("Side", unit.SideB.ToString());
		EditorGUILayout.LabelField("Tier", unit.GetUnitTier().ToString());
		EditorGUILayout.LabelField("Sight", unit.sightRange.ToString());
		EditorGUILayout.LabelField("Movement", unit.movementRange.ToString());
		EditorGUILayout.LabelField("Specialization", unit.specialization.ToString());
		EditorGUILayout.LabelField("Movement", unit.movementModifier.ToString());
		EditorGUILayout.LabelField("Transport", unit.transportModifier.ToString());
		EditorGUILayout.LabelField("Equipment", string.Join("\n", unit.equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.Amount}")), EditorStyles.wordWrappedLabel);
	}
}
#endif