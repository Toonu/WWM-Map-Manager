using UnityEngine;

public class UnitEditor : MonoBehaviour {
    public int domain;
	private Unit unit;
	private GroundUnit groundUnit;
	private AerialUnit aerialUnit;
	private NavalUnit navalUnit;

	public void UpdateSpecialization(int i) {
		Debug.Log(domain);
		switch (domain) {
			case 1:
			aerialUnit.ChangeSpecialization((AerialSpecialization)i);
			break;
			case 2:
			navalUnit.ChangeSpecialization((NavalSpecialization)i);
			break;
			default:
			groundUnit.ChangeSpecialization((GroundSpecialization)i);
			break;
		}

	}
	public void UpdateMovementModifier(int i) {
		GroundMovementType movementModifier = (GroundMovementType)i;
		groundUnit.ChangeSpecialization(movementModifier);

	}
	public void UpdateTransportModifier(int i) {
		GroundTransportType transportModifier = (GroundTransportType)i;
		groundUnit.ChangeSpecialization(transportModifier);
	}
	public void UpdateTier(int i) {
		unit.ChangeTier(i);
	}
	public void UpdateName(string identification) {
		unit.ChangeName(identification);
	}
	public void UpdateUnit(Unit unit) {
		this.unit = unit;
		if (unit.GetType() == typeof(GroundUnit)) {
			groundUnit = (GroundUnit)unit;
			domain = 0;
		} else if (unit.GetType() == typeof(AerialUnit)) {
			aerialUnit = (AerialUnit)unit;
			domain = 1;
		} else {
			navalUnit = (NavalUnit)unit;
			domain = 2;
		}
		UnitManager.Instance.PopulateUI(gameObject, domain);
	}
}