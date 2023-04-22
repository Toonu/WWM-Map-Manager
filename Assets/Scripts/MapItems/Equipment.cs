using UnityEditor;
using UnityEngine;

public class Equipment : MonoBehaviour {
	internal string equipmentName;
	internal float movementRange;
	internal float sightRange;
	internal float weaponRange;
	private int amount;
	internal int Amount { get { return amount; } set { amount = value; name = $"{equipmentName}:{amount}"; } }
	internal float cost;
	internal int sideB;
	internal int domain;
	internal int specialization;
	internal int protection;
	internal int transportation;

	public void Initiate(string equipmentName, int amount, float movementRange, float sightRange, float weaponRange, float cost, int sideB, int domain, int specialization, int protection, int transportation) {
		this.equipmentName = equipmentName;
		Amount = amount;
		this.movementRange = movementRange;
		this.weaponRange = weaponRange;
		this.sightRange = sightRange;
		this.cost = cost;
		this.sideB = sideB;
		this.domain = domain;
		this.specialization = specialization;
		this.protection = protection;
		this.transportation = transportation;
		Debug.Log(this + " initiated.");
	}

	public override string ToString() {
		return $"{equipmentName}:{Amount}";
	}
}


#if UNITY_EDITOR
[CustomEditor(typeof(Equipment))]
public class EquipmentEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		Equipment eq = (Equipment)target;

		EditorGUILayout.LabelField("Name  ", eq.equipmentName.ToString());
		EditorGUILayout.LabelField("amount", eq.Amount.ToString());
	}
}
#endif