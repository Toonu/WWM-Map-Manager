using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AerialUnit : Unit {
	internal AerialSpecialization specialization = AerialSpecialization.F;
	internal int altitude = 1000;

	public override void Initiate(int newID, Vector3 newPosition, UnitTier newTier, string newName, List<Equipment> newEquipment, bool newSideB, int newSpecialization) {
		//Disabling texture not required for aerial units
		transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
		transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
		base.Initiate(newID, newPosition, newTier, newName, newEquipment, newSideB, newSpecialization: newSpecialization);
	}

	internal override void ChangeAffiliation() {
		bool isEnemy = ApplicationController.isSideB != SideB;
		if (isEnemy) {
			iconImage.transform.localScale = new Vector3(0.8f, 0.8f, 1);
		} else {
			iconImage.transform.localScale = Vector3.one;
		}
		iconImage.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, isEnemy);
	}

	internal override void ChangeSpecialization(int newSpecialization) {
		specialization = (AerialSpecialization)newSpecialization;
		iconImage.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, SideB);
		if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Specialization changed | {specialization}");
	}

	internal override int GetSpecialization() {
		return (int)specialization;
	}

	internal override void RecalculateAttributes() {
		base.RecalculateAttributes();
		equipmentTextUI.text += $"\nAltitude: {altitude}m";

		//Returns the most numerous armour-traction type.
		SetUnitTier(EnumUtil.GetUnitTier(1, equipmentList.Sum(vehicle => vehicle.Amount)));

		//Returns the most numerous unit type in the Unit
		ChangeSpecialization((int)equipmentList.GroupBy(equipment => equipment.specialization)
							.Select(group => new { Specialization = group.Key, Amount = group.Sum(equipment => equipment.Amount) })
							.OrderByDescending(group => group.Amount)
							.ToList().FirstOrDefault()?.Specialization);

	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(AerialUnit))]
public class AerialUnitEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		AerialUnit unit = (AerialUnit)target;

		EditorGUILayout.LabelField("ID", unit.ID.ToString());
		EditorGUILayout.LabelField("Side", unit.SideB.ToString());
		EditorGUILayout.LabelField("Tier", unit.GetUnitTier().ToString());
		EditorGUILayout.LabelField("Sight", unit.sightRange.ToString());
		EditorGUILayout.LabelField("Movement", unit.movementRange.ToString());
		EditorGUILayout.LabelField("Specialization", unit.specialization.ToString());
		EditorGUILayout.LabelField("Ghost", unit.IsGhost.ToString());
		EditorGUILayout.LabelField("Equipment", string.Join("\n", unit.equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.Amount}")), EditorStyles.wordWrappedLabel);
	}
}
#endif
