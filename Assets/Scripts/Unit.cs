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
	public GameObject movementRange;
	public GameObject sightRange;
	public GameObject weaponRange;

	internal TextMeshProUGUI unitName;
	internal TextMeshProUGUI tier;
	public bool sideB;

	protected ApplicationController aC;
	#endregion

	public void Initiate(object ID, Vector3 position, UnitTier unitTier, string unitName, List<Equipment> unitEquipment) {
		main = transform.GetChild(0).GetChild(2).GetComponent<MeshRenderer>();
		movementRange = transform.Find("Range").gameObject;
		sightRange = transform.Find("SightRange").gameObject;
		weaponRange = transform.Find("WeaponRange").gameObject;
		this.unitName = transform.Find("Canvas/unitName").gameObject.GetComponent<TextMeshProUGUI>();
		tier = transform.Find("Canvas/Tier").gameObject.GetComponent<TextMeshProUGUI>();
		equipment = transform.Find("Canvas/Eq").gameObject.GetComponent<TextMeshProUGUI>();

		//For deleting units by index/ID
		id = Convert.ToInt16(ID);
		//Application controller for checking permissions
		aC = GameObject.FindWithTag("GameController").GetComponent<ApplicationController>();

		offset = new Vector3(0, 0, 0);
		movementRangeValue = 0.3f;
		transform.position = position;
		turnStartPosition = position;

		Debug.Log($"[{name}][{id}] Initialization");

		//Adds equipment only if there is some to add, otherwise creates a new Eq list.
		if (unitEquipment.Count > 0) {
			AddEquipment(unitEquipment);
		} else {
			this.unitEquipment = new List<Equipment>();
		}

		ChangeTier(Convert.ToInt16(unitTier));
		ChangeName(unitName);
	}

	internal List<Equipment> unitEquipment;
	private TextMeshProUGUI equipment;

	/// <summary>
	/// Adds Equipment from the List to the Unit equipment and edits the equipment string label.
	/// </summary>
	/// <param name="equipmentList">List of Equipment to add.</param>
	internal void AddEquipment(List<Equipment> equipmentList) {
		if (equipmentList.Count > 0) {
			unitEquipment = equipmentList.ToList();
			equipment.text = string.Join("\n", equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.amount}"));

			unitEquipment.ForEach(eq => Debug.Log($"[{name}][{id}] Adding Equipment | {eq.amount} {eq.equipmentName}"));

			movementRangeValue = equipmentList.Min(e => e.movementRange);
			sightRangeValue = equipmentList.Max(e => e.sightRange);
			weaponRangeValue = equipmentList.Max(e => e.weaponRange);

			sightRange.transform.localScale = new Vector3(212 * sightRangeValue, 212 * sightRangeValue, 0);
			weaponRange.transform.localScale = new Vector3(212 * weaponRangeValue, 212 * weaponRangeValue, 0);
			ResizeMovementCircle();
		} else {
			unitEquipment = new List<Equipment>();
			equipment.text = "";
		}
	}

	internal float sightRangeValue = 0.25f;
	internal float weaponRangeValue = 0.2f;

	#region Movement

	private Vector3 offset;
	internal Vector3 turnStartPosition;
	public float movementRangeValue;

	/// <summary>
	/// Shows range circles.
	/// </summary>
	internal void OnMouseOver() {
		if (!aC.admin && aC.sideB != sideB) {
			return;
		}
		sightRange.SetActive(true);
		movementRange.SetActive(true);
		equipment.gameObject.SetActive(true);
	}

	/// <summary>
	/// Hides range circles.
	/// </summary>
	internal void OnMouseExit() {
		sightRange.SetActive(false);
		movementRange.SetActive(false);
		equipment.gameObject.SetActive(false);
	}

	/// <summary>
	/// Starts dragging function by checking the permissions and side and then saves offset.
	/// </summary>
	private void OnMouseDown() {
		if (!aC.admin && aC.sideB != sideB) {
			return;
		}
		// Calculate the offset between the object's position and the mouse position
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = Camera.main.nearClipPlane;
		offset = transform.position - Camera.main.ScreenToWorldPoint(mousePosition);
	}

	/// <summary>
	/// Drags the unit up to its maximal range based on range.
	/// </summary>
	private void OnMouseDrag() {
		if (!aC.admin && aC.sideB != sideB) {
			return;
		}
		// Calculate the new position of the object based on the mouse position and the range
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = Camera.main.nearClipPlane;
		Vector3 newPosition = Camera.main.ScreenToWorldPoint(mousePosition) + offset;
		newPosition = Vector3.ClampMagnitude(newPosition - turnStartPosition, GameObject.FindWithTag("GameController").GetComponent<ApplicationController>().admin ? 9999999f : movementRangeValue) + turnStartPosition;
		transform.position = newPosition;
		ResizeMovementCircle();
	}

	/// <summary>
	/// Resized range circle based on remaining range.
	/// </summary>
	internal void ResizeMovementCircle() {
		// Resize the range circle based on the distance between the starting position and the new position of the draggable object
		float maxRange = movementRangeValue - Vector3.Distance(turnStartPosition, transform.position);
		movementRange.transform.localScale = new Vector3(212 * maxRange, 212 * maxRange, 0);
	}

	#endregion

	#region Attribute Get/Setters

	/// <summary>
	/// Setter for the unit identifier name which changed the name label and Object name.
	/// </summary>
	/// <param name="identification">New identifier</param>
	internal void ChangeName(string identification) {
		if (UnitTier > UnitTier.Division) {
			unitName.text = EnumUtil.GetCorps(identification);
		} else {
			unitName.text = EnumUtil.NumberWithSuffix(Convert.ToInt16(identification));
		}
		name = identification;
		Debug.Log($"[{name}][{id}] Name changed to {identification}");
	}

	/// <summary>
	/// Changes the unit tier from int to Enum and assigns it to unit and its label.
	/// </summary>
	/// <param name="echelon">Echelon int</param>
	internal void ChangeTier(int echelon) {
		tier.text = EnumUtil.GetUnitTier(echelon);
		UnitTier = (UnitTier)echelon;
		Debug.Log($"[{name}][{id}] Tier changed to {UnitTier}");
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
	#endregion
}

