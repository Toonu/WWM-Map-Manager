using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
		iconImage.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, ApplicationController.sideB != SideB);
	}

	internal override void ChangeSpecialization(int specialization) {
		this.specialization = (NavalSpecialization)specialization;
		iconImage.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, SideB);
	}
}


#if UNITY_EDITOR
[CustomEditor(typeof(NavalUnit))]
public class NavalUnitEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		NavalUnit unit = (NavalUnit)target;

		EditorGUILayout.LabelField("ID", unit.ID.ToString());
		EditorGUILayout.LabelField("Side", unit.SideB.ToString());
		EditorGUILayout.LabelField("Sight", unit.sightRange.ToString());
		EditorGUILayout.LabelField("Movement", unit.movementRange.ToString());
		EditorGUILayout.LabelField("Specialization", unit.specialization.ToString());
		EditorGUILayout.LabelField("Equipment", string.Join("\n", unit.unitEquipment.Select(equipment => $"{equipment.equipmentName}:{equipment.amount}")), EditorStyles.wordWrappedLabel);
	}
}
#endif