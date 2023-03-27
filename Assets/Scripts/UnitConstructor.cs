using System.Collections.Generic;
using UnityEngine;

public class UnitConstructor : MonoBehaviour {
	private bool enemySide = false;
	private Vector3 basePosition;
	private int domain = 0;
	private int specialization = 0;
	private GroundMovementType movementModifier = GroundMovementType.None;
	private GroundTransportType transportModifier = GroundTransportType.None;
	private UnitTier tier = UnitTier.Team;
	private string identification = "";
	public List<Equipment> unitEquipment;

	public void UpdatePosition(Vector3 position) {
		basePosition = position;
	}
	public void UpdateDomain(int i) {
		domain = i;
	}
	public void UpdateSpecialization(int i) {
		specialization = i;
	}
	public void UpdateMovementModifier(int i) {
		movementModifier = (GroundMovementType)i;
	}
	public void UpdateTransportModifier(int i) {
		transportModifier = (GroundTransportType)i;
	}
	public void UpdateTier(int i) {
		tier = (UnitTier)i;
	}
	public void UpdateName(string identification) {
		this.identification = identification;
	}
	public void UpdateAffiliation(bool enemy) {
		enemySide = enemy;
	}
	public void Spawn() {
		UnitManager.Instance.SpawnUnit(new Vector3(basePosition.x, basePosition.y, basePosition.z), domain, specialization, identification, tier, enemySide, movementModifier, transportModifier, unitEquipment);
	}
}