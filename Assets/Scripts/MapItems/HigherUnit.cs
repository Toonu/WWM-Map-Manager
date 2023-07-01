using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class HigherUnit : Unit {
	internal List<Unit> lowerUnits = new();
	private readonly Dictionary<string, int> equipment = new();

	public void Initiate(int ID, Vector3 position, List<Unit> lowerUnits) {
		this.lowerUnits = lowerUnits;
		//Unnecessary UI elements
		transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
		transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
		name = EnumUtil.GetCorpsInt(lowerUnits.GroupBy(unit => unit.parentTextUI.text).OrderByDescending(group => group.Count()).Select(group => group.Key).First()).ToString();

		//name = EnumUtil.GetCorps(lowerUnits.GroupBy(unit => unit.parentTextUI.text).Max(group => group.Count()));
		base.Initiate(ID, position, UnitTier.Corps, name, new List<Equipment>(), lowerUnits[0].SideB, 9);
		parentTextUI.text = "I"; //1st Army parent to all of them.
		lowerUnits.ForEach(unit => { unit.UnitParent = this; }); //Assign itself as parent to all lower units
		transform.localScale *= 5;

		//Add equipment to the higher unit from the lower units for labeling purposes.
		lowerUnits.ForEach(unit => {
			unit.equipmentList.ForEach(e => {
				if (equipment.ContainsKey(e.equipmentName)) {
					equipment[e.equipmentName] += e.Amount;
				} else {
					equipment[e.equipmentName] = e.Amount;
				}
			});
		});

		equipmentTextUI.text = string.Join("\n", equipment.Select(equipment => $"{equipment.Key}:{equipment.Value}"));
	}
	
	internal override void ChangeAffiliation() {
		bool isEnemy = ApplicationController.isSideB != SideB;
		if (isEnemy) {
			iconImage.transform.localScale = Vector3.one;
			iconImage.material.mainTexture = UnitManager.Instance.groundSpecializationEnemy[9].texture;
		} else { 
			iconImage.transform.localScale = new Vector3(1.5f, 1, 1); 
			iconImage.material.mainTexture = UnitManager.Instance.groundSpecialization[9].texture; 
		}
	}

	internal override void ChangeSpecialization(int newSpecialization) {
		if (SideB) {
			iconImage.material.mainTexture = UnitManager.Instance.groundSpecializationEnemy[9].texture;
		} else { iconImage.material.mainTexture = UnitManager.Instance.groundSpecialization[9].texture; }
		ChangeAffiliation();
	}

	internal override int GetSpecialization() {
		return 9;
	}

	public override void OnDrag(PointerEventData eventData) {
		return;
	}
	public override void OnEndDrag(PointerEventData eventData) {
		return;
	}
	public override void OnPointerEnter(PointerEventData eventData) {
		if (!ApplicationController.isAdmin && ApplicationController.isSideB != SideB) {
			return;
		}
		equipmentTextUI.gameObject.SetActive(true);
	}
}


#if UNITY_EDITOR
[CustomEditor(typeof(HigherUnit))]
public class HigherUnitEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		HigherUnit unit = (HigherUnit)target;

		EditorGUILayout.LabelField("ID", unit.ID.ToString());
		EditorGUILayout.LabelField("Side", unit.SideB.ToString());
		EditorGUILayout.LabelField("Units", string.Join("\n", unit.lowerUnits.Select(unit => $"{unit.name}")), EditorStyles.wordWrappedLabel);
	}
}
#endif
