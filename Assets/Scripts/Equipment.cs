using Newtonsoft.Json;
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

	[JsonIgnore]
	public GameObject gameObject;

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
