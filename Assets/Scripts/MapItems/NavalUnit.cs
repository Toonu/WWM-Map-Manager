using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class NavalUnit : Unit {
	internal NavalSpecialization specialization = NavalSpecialization.TaskElement;

	public override void Initiate(int newID, Vector3 newPosition, UnitTier newTier, string newName, List<Equipment> newEquipment, bool newSideB, int newSpecialization) {
		//Disabling texture not required for naval units
		transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
		transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
		base.Initiate(newID, newPosition, newTier, newName, newEquipment, newSideB, newSpecialization: newSpecialization);
		//Naval aspect ratio rectangular.
		iconImage.transform.localScale = Vector3.one;
	}

	internal override void ChangeAffiliation() {
		iconImage.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, ApplicationController.isSideB != SideB);
	}

	internal override void ChangeSpecialization(int newSpecialization) {
		specialization = (NavalSpecialization)newSpecialization;
		iconImage.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, SideB);
		if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Specialization changed | {specialization}");
		SetUnitTier((int)specialization);
	}

	public override void SetUnitTier(int echelon) {
		base.SetUnitTier(echelon + 4);
	}

	internal override void RecalculateAttributes() {
		base.RecalculateAttributes();
		//Returns the most numerous armour-traction type.
		ChangeSpecialization(EnumUtil.GetUnitTier(2, equipmentList.Sum(vehicle => vehicle.Amount)));
	}

	internal override int GetSpecialization() {
		return (int)specialization;
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
		EditorGUILayout.LabelField("Tier", unit.GetUnitTier().ToString());
		EditorGUILayout.LabelField("Sight", unit.sightRange.ToString());
		EditorGUILayout.LabelField("Movement", unit.movementRange.ToString());
		EditorGUILayout.LabelField("Specialization", unit.specialization.ToString());
		EditorGUILayout.LabelField("Ghost", unit.isGhost.ToString());
		EditorGUILayout.LabelField("Equipment", string.Join("\n", unit.equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.Amount}")), EditorStyles.wordWrappedLabel);
	}
}
#endif