using System.Linq;
using UnityEditor;
using UnityEngine;

public class Equipment : MonoBehaviour {
	internal string equipmentName;
	internal float movementRange;
	internal float sightRange;
	internal float weaponRange;
	internal int amount;
	internal int cost;
	internal int side;
	internal int domain;

	public void Initiate(string equipmentName, int amount, float movementRange, float sightRange, float weaponRange, int cost, int side, int domain) {
		this.equipmentName = equipmentName;
		this.amount = amount;
		this.movementRange = movementRange;
		this.weaponRange = weaponRange;
		this.sightRange = sightRange;
		this.cost = cost;
		this.side = side;
		this.domain = domain;
	}

	public override string ToString() {
		return $"{equipmentName}:{amount}";
	}
}


#if UNITY_EDITOR
[CustomEditor(typeof(Equipment))]
public class EquipmentEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		Equipment eq = (Equipment)target;

		EditorGUILayout.LabelField("Name  ", eq.equipmentName.ToString());
		EditorGUILayout.LabelField("Amount", eq.amount.ToString());
	}
}
#endif