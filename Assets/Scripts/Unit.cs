using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public abstract class Unit : MonoBehaviour {
	public int ID;
	public UnitTier UnitTier;
	internal List<Equipment> unitEquipment;
	private Unit UnitParent {
		get => UnitParent;
		set {
			UnitParent = value;
			parentTextUI.text = value.name;
		}
	}

	#region UnitVisuals
	protected MeshRenderer iconImage;
	private GameObject movementRangeCircle;
	private GameObject sightRangeCircle;
	internal GameObject WeaponRangeCircle { private set; get; }

	private TextMeshProUGUI nameTextUI;
	private TextMeshProUGUI tierTextUI;
	private TextMeshProUGUI parentTextUI;
	private TextMeshProUGUI equipmentTextUI;
	public bool SideB;

	protected ApplicationController aC;
	#endregion

	public virtual void Initiate(int newID, Vector3 newPosition, UnitTier newTier, string newIdentifier, List<Equipment> newEquipment, bool newSideB, int newSpecialization) {
		iconImage = transform.GetChild(0).GetChild(2).GetComponent<MeshRenderer>();
		movementRangeCircle = transform.Find("Range").gameObject;
		sightRangeCircle = transform.Find("SightRange").gameObject;
		WeaponRangeCircle = transform.Find("WeaponRange").gameObject;
		nameTextUI = transform.Find("Canvas/unitName").gameObject.GetComponent<TextMeshProUGUI>();
		tierTextUI = transform.Find("Canvas/Tier").gameObject.GetComponent<TextMeshProUGUI>();
		equipmentTextUI = transform.Find("Canvas/Eq").gameObject.GetComponent<TextMeshProUGUI>();
		parentTextUI = transform.Find("Canvas/HigherEchelon").gameObject.GetComponent<TextMeshProUGUI>();

		

		//For deleting units by index/ID
		ID = Convert.ToInt16(newID);
		//Application controller for checking permissions
		aC = GameObject.FindWithTag("GameController").GetComponent<ApplicationController>();
		SideB = newSideB;
		ChangeAffiliation();
		ChangeSpecialization(newSpecialization);

		offset = new Vector3(0, 0, 0);
		movementRange = 0.3f;
		transform.position = newPosition;
		turnStartPosition = newPosition;

		//Adds equipment only if there is some to add, otherwise creates a new Eq list.
		if (newEquipment.Count > 0) {
			AddEquipment(newEquipment);
		} else {
			unitEquipment = new List<Equipment>();
		}

		ChangeTier(Convert.ToInt16(newTier));
		ChangeName(newIdentifier);

		Debug.Log($"[{ID}][{name}] Initiated");
	}

	/// <summary>
	/// Adds Equipment from the List to the Unit equipment and edits the equipment string label.
	/// </summary>
	/// <param name="equipmentList">List of Equipment to add.</param>
	internal void AddEquipment(List<Equipment> equipmentList) {
		if (equipmentList.Count > 0) {
			unitEquipment = equipmentList.ToList();
			equipmentTextUI.text = string.Join("\n", equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.amount}"));

			unitEquipment.ForEach(eq => Debug.Log($"[{ID}][{name}] Adding Equipment | {eq.amount} {eq.equipmentName}"));

			movementRange = equipmentList.Min(e => e.movementRange);
			sightRange = equipmentList.Max(e => e.sightRange);
			weaponRange = equipmentList.Max(e => e.weaponRange);

			sightRangeCircle.transform.localScale = new Vector3(212 * sightRange, 212 * sightRange, 0);
			WeaponRangeCircle.transform.localScale = new Vector3(212 * weaponRange, 212 * weaponRange, 0);
			ResizeMovementCircle();
		} else {
			unitEquipment = new List<Equipment>();
			equipmentTextUI.text = "";
		}
	}

	internal float sightRange = 0.25f;
	internal float weaponRange = 0.2f;

	#region Movement

	private Vector3 offset;
	internal Vector3 turnStartPosition;
	public float movementRange;

	/// <summary>
	/// Shows range circles.
	/// </summary>
	internal void OnMouseOver() {
		if (!aC.admin && aC.sideB != SideB) {
			return;
		}
		sightRangeCircle.SetActive(true);
		movementRangeCircle.SetActive(true);
		equipmentTextUI.gameObject.SetActive(true);
	}

	/// <summary>
	/// Hides range circles.
	/// </summary>
	internal void OnMouseExit() {
		sightRangeCircle.SetActive(false);
		movementRangeCircle.SetActive(false);
		equipmentTextUI.gameObject.SetActive(false);
	}

	/// <summary>
	/// Starts dragging function by checking the permissions and side and then saves offset.
	/// </summary>
	private void OnMouseDown() {
		if (!aC.admin && aC.sideB != SideB) {
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
		if (!aC.admin && aC.sideB != SideB) {
			return;
		}
		// Calculate the new position of the object based on the mouse position and the range
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = Camera.main.nearClipPlane;
		Vector3 newPosition = Camera.main.ScreenToWorldPoint(mousePosition) + offset;
		newPosition = Vector3.ClampMagnitude(newPosition - turnStartPosition, GameObject.FindWithTag("GameController").GetComponent<ApplicationController>().admin ? 9999999f : movementRange) + turnStartPosition;
		transform.position = newPosition;
		ResizeMovementCircle();
	}

	/// <summary>
	/// Resized range circle based on remaining range.
	/// </summary>
	internal void ResizeMovementCircle() {
		// Resize the range circle based on the distance between the starting position and the new position of the draggable object
		float maxRange = movementRange - Vector3.Distance(turnStartPosition, transform.position);
		movementRangeCircle.transform.localScale = new Vector3(212 * maxRange, 212 * maxRange, 0);
	}

	#endregion

	#region Attribute Get/Setters

	/// <summary>
	/// Setter for the unit identifier name which changed the name label and Object name.
	/// </summary>
	/// <param name="identification">New identifier</param>
	internal virtual void ChangeName(string identification) {
		if (UnitTier > UnitTier.Division) {
			nameTextUI.text = EnumUtil.GetCorps(identification);
		} else {
			nameTextUI.text = EnumUtil.NumberWithSuffix(Convert.ToInt16(identification));
		}
		name = identification;
		Debug.Log($"[{ID}][{name}] Name changed to {identification}");
	}


	internal abstract void ChangeAffiliation();
	internal virtual void ChangeAffiliation(bool sideB) {
		SideB = sideB;
		ChangeAffiliation();
	}

	internal abstract void ChangeSpecialization(int specialization);

	/// <summary>
	/// Changes the unit tier from int to Enum and assigns it to unit and its label.
	/// </summary>
	/// <param name="echelon">Echelon int</param>
	internal virtual void ChangeTier(int echelon) {
		tierTextUI.text = EnumUtil.GetUnitTier(echelon);
		UnitTier = (UnitTier)echelon;
		Debug.Log($"[{ID}][{name}] Tier changed to {UnitTier}");
	}
	#endregion
}

