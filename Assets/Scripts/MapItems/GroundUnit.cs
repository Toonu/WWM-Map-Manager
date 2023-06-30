using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GroundUnit : Unit {
	internal GroundSpecialization specialization = GroundSpecialization.Infantry;
	internal MeshRenderer protectionTexture;
	internal GroundProtectionType protectionType = GroundProtectionType.Motorized;
	internal MeshRenderer transportTexture;
	internal GroundTransportType transportType = GroundTransportType.None;

	public void Initiate(int newID, Vector3 newPosition, UnitTier newTier, string newName, List<Equipment> newEquipment, bool newSideB, int newSpecialization, GroundProtectionType newProtection, GroundTransportType newTransport) {
		//Texture Component handling
		protectionTexture = transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
		transportTexture = transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>();
		ChangeSpecialization(newProtection);
		ChangeSpecialization(newTransport);
		Initiate(newID, newPosition, newTier, newName, newEquipment, newSideB, newSpecialization: newSpecialization);
	}

	internal override void ChangeAffiliation() {
		bool isEnemy = ApplicationController.isSideB != SideB;
		//True if the unit is on the same isSideB as the user
		if (isEnemy) {
			iconImage.transform.localScale = Vector3.one;
			protectionTexture.transform.localScale = Vector3.one;
			transportTexture.transform.localScale = Vector3.one;
		} else {
			iconImage.transform.localScale = new Vector3(1.5f, 1, 1);
			protectionTexture.transform.localScale = new Vector3(1.5f, 1, 1);
			transportTexture.transform.localScale = new Vector3(1.5f, 1, 1);
		}
		iconImage.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, isEnemy);
		protectionTexture.material.mainTexture = UnitManager.Instance.GetProtectionTexture(this, isEnemy);
		transportTexture.material.mainTexture = UnitManager.Instance.GetTransportTexture(this, isEnemy);
	}

	internal override void ChangeSpecialization(int newSpecialization) {
		specialization = (GroundSpecialization)newSpecialization;
		iconImage.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, SideB);
		if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Specialization changed | {specialization}");
	}
	internal void ChangeSpecialization(GroundProtectionType protectionType) {
		this.protectionType = protectionType;
		protectionTexture.material.mainTexture = UnitManager.Instance.GetProtectionTexture(this, SideB);
		if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Protection changed | {protectionType}");
	}
	internal void ChangeSpecialization(GroundTransportType transportType) {
		this.transportType = transportType;
		transportTexture.material.mainTexture = UnitManager.Instance.GetTransportTexture(this, SideB);
		if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Transport changed | {transportType}");
	}

	internal override int GetSpecialization() {
		return (int)specialization;
	}

	internal override void RecalculateAttributes() {
		base.RecalculateAttributes();
		//Returns the most numerous armour/traction type.
		ChangeSpecialization((GroundProtectionType)equipmentList.GroupBy(equipment => equipment.protection).OrderByDescending(group => group.Count()).FirstOrDefault()?.Key);
		//Transportation
		GroundTransportType type = GroundTransportType.None;
		if (equipmentList.Select(e => e.transportation).Distinct().Count() == 1) {
			type = (GroundTransportType)equipmentList.First().transportation;
		}
		ChangeSpecialization(type);
		//Tier
		SetUnitTier(EnumUtil.GetUnitTier(0, equipmentList.Sum(vehicle => vehicle.Amount)));
		//Returns the most numerous unit specialization in the Unit
		ChangeSpecialization((int)equipmentList.GroupBy(equipment => equipment.specialization)
							.Select(group => new { Specialization = group.Key, Amount = group.Sum(equipment => equipment.Amount) })
							.OrderByDescending(group => group.Amount)
							.ToList().FirstOrDefault()?.Specialization);
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
		EditorGUILayout.LabelField("Protection", unit.protectionType.ToString());
		EditorGUILayout.LabelField("Transport", unit.transportType.ToString());
		EditorGUILayout.LabelField("Ghost", unit.isGhost.ToString());
		EditorGUILayout.LabelField("Equipment", string.Join("\n", unit.equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.Amount}")), EditorStyles.wordWrappedLabel);
	}
}
#endif