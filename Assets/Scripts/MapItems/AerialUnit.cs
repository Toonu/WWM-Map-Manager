using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AerialUnit : Unit {
	internal AerialSpecialization specialization = AerialSpecialization.None;

	public override void Initiate(int newID, Vector3 newPosition, UnitTier newTier, string newName, List<Equipment> newEquipment, bool newSideB, int newSpecialization) {
		//Disabling texture not required for aerial units
		transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
		transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
		base.Initiate(newID, newPosition, newTier, newName, newEquipment, newSideB, newSpecialization: newSpecialization);
	}

	internal override void ChangeAffiliation() {
		bool isEnemy = aC.sideB != SideB;
		if (isEnemy) {
			iconImage.transform.localScale = new Vector3(0.8f, 0.8f, 1);
		} else {
			iconImage.transform.localScale = Vector3.one;
		}
		iconImage.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, isEnemy);
		Debug.Log($"[{ID}][{name}] Affiliation changed");
	}

	internal override void ChangeSpecialization(int specialization) {
		this.specialization = (AerialSpecialization)specialization;
		iconImage.material.mainTexture = UnitManager.Instance.GetSpecialisationTexture(this, SideB);
		Debug.Log($"[{ID}][{name}] Specialization changed | {specialization}");
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(AerialUnit))]
public class AerialUnitEditor : Editor {
	public override void OnInspectorGUI() {
		// Draw the default inspector
		DrawDefaultInspector();

		AerialUnit unit = (AerialUnit)target;
		FieldInfo[] fields = typeof(AerialUnit).GetFields(BindingFlags.NonPublic);
		foreach (FieldInfo field in fields) {
			EditorGUILayout.LabelField(field.Name, field.GetValue(unit).ToString());
		}
	}
}
#endif
