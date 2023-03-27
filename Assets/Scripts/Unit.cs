using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Unit : MonoBehaviour {
	public int id;
	public UnitTier UnitTier { get; private set; }

	#region UnitVisuals
	protected MeshRenderer main;
	public GameObject range;
	public GameObject sightRange;
	public GameObject weaponRange;

	internal TextMeshProUGUI unitName;
	internal TextMeshProUGUI tier;
	internal bool enemySide;
	#endregion

	public void Initiate(object ID, Vector3 position, UnitTier unitTier, string unitName, List<Equipment> unitEquipment) {
		main = transform.GetChild(0).GetChild(2).GetComponent<MeshRenderer>();
		range = transform.Find("Range").gameObject;
		sightRange = transform.Find("SightRange").gameObject;
		weaponRange = transform.Find("WeaponRange").gameObject;
		this.unitName = transform.Find("Canvas/unitName").gameObject.GetComponent<TextMeshProUGUI>();
		tier = transform.Find("Canvas/Tier").gameObject.GetComponent<TextMeshProUGUI>();
		equipment = transform.Find("Canvas/Eq").gameObject.GetComponent<TextMeshProUGUI>();
		equipment.text = "";
		id = Convert.ToInt16(ID);

		//distance = 0f;
		offset = new Vector3(0, 0, 0);
		distanceRange = 0.5f;
		transform.position = position;
		turnStartPosition = position;

		if (unitEquipment != null) {
			AddEquipment(unitEquipment);
		}

		ChangeTier(Convert.ToInt16(unitTier));
		ChangeName(unitTier >= UnitTier.Corps ? EnumUtil.GetCorps(unitName) : unitName);

	}

	internal void OnMouseOver() {
		sightRange.SetActive(true);
		range.SetActive(true);
		equipment.gameObject.SetActive(true);
	}

	internal void OnMouseExit() {
		sightRange.SetActive(false);
		range.SetActive(false);
		equipment.gameObject.SetActive(false);
	}

	private Vector3 offset;
	internal Vector3 turnStartPosition;
	public float distanceRange;
	private List<Equipment> unitEquipment = new List<Equipment>();
	private TextMeshProUGUI equipment;

	private void OnMouseDown() {
		// Calculate the offset between the object's position and the mouse position
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = Camera.main.nearClipPlane;
		offset = transform.position - Camera.main.ScreenToWorldPoint(mousePosition);
	}

	private void OnMouseDrag() {
		// Calculate the new position of the object based on the mouse position and the range
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = Camera.main.nearClipPlane;
		Vector3 newPosition = Camera.main.ScreenToWorldPoint(mousePosition) + offset;
		newPosition = Vector3.ClampMagnitude(newPosition - turnStartPosition, GameObject.FindWithTag("GameController").GetComponent<ApplicationController>().admin ? 9999999f : distanceRange) + turnStartPosition;
		transform.position = newPosition;
		float distance = Vector3.Distance(turnStartPosition, transform.position);
		// Resize the range circle based on the distance between the starting position and the new position of the draggable object
		float maxRange = distanceRange - distance;
		range.transform.localScale = new Vector3(212 * maxRange, 212 * maxRange, 0);
	}

	internal void AddEquipment(List<Equipment> equipmentList) {
		unitEquipment = equipmentList;
		equipment.text = "";
		string.Join("\n", equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.amount}"));
	}

	internal void ChangeName(string identification) {
		
		if (UnitTier > UnitTier.Division) {
			unitName.text = EnumUtil.GetCorps(identification);
		} else {
			unitName.text = EnumUtil.NumberWithSuffix(Convert.ToInt16(identification));
		}
		name = identification;
	}

	internal void ChangeTier(int echelon) {
		tier.text = EnumUtil.GetUnitTier(echelon);
		UnitTier = (UnitTier)echelon;
	}

	internal static int GetSpecialization(GroundUnit unit) {
		return (int)unit.specialization;
	}
	internal static int GetSpecialization(AerialUnit unit) {
		return (int)unit.specialization;
	}
	internal static int GetSpecialization(NavalUnit unit) {
		return (int)unit.specialization;
	}
	internal static int GetSpecialization(int domain, Unit unit) {
		if (domain == 0) {
			return GetSpecialization((GroundUnit)unit);
		} else if (domain == 1) {
			return GetSpecialization((AerialUnit)unit);
		} else {
			return GetSpecialization((NavalUnit)unit);
		}
	}
}

