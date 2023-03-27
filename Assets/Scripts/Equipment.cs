using Google.Apis.Sheets.v4.Data;
using UnityEngine;

public class Equipment : MonoBehaviour {
	internal string equipmentName;
	internal float movementRange;
	internal float sightRange;
	internal float weaponRange;
	internal int amount;
	internal int cost;

	public void Initiate(string equipmentName, int amount, float movementRange, float sightRange, float weaponRange, int cost) {
		this.equipmentName = equipmentName;
		this.amount = amount;
		this.movementRange = movementRange;
		this.weaponRange = weaponRange;
		this.sightRange = sightRange;
		this.cost = cost;
	}

	public override string ToString() {
		return $"{equipmentName}:{amount}";
	}
}
