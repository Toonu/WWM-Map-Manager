using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public abstract class Unit : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {
	#region Attribute Get/Setters

	internal int ID;
	internal bool SideB;

	/// <summary>
	/// Setter for the unit name which changes the name label and Object name.
	/// </summary>
	/// <param name="identification">New identifier</param>
	internal virtual void SetName(string identification) {
		if (GetUnitTier() > UnitTier.Division) {
			nameTextUI.text = EnumUtil.GetCorps(identification);
		} else {
			nameTextUI.text = EnumUtil.NumberWithSuffix(Convert.ToInt16(identification));
		}
		name = identification;
		Debug.Log($"[{ID}][{name}] Name changed to {identification}");
	}

	private UnitTier unitTier;
	public UnitTier GetUnitTier() { return unitTier; }
	/// <summary>
	/// Setter for the unit tier which changes the tier label and tier.
	/// </summary>
	/// <param name="echelon"></param>
	public void SetUnitTier(int echelon) {
		tierTextUI.text = EnumUtil.GetUnitTier(echelon);
		unitTier = (UnitTier)echelon;
		Debug.Log($"[{ID}][{name}] Tier changed to {unitTier}");
	}
	/// <summary>
	/// Setter for the unit tier which changes the tier label and tier.
	/// </summary>
	/// <param name="echelon"></param>
	public void SetUnitTier(UnitTier echelon) {
		tierTextUI.text = EnumUtil.GetUnitTier((int)echelon);
		unitTier = echelon;
		Debug.Log($"[{ID}][{name}] Tier changed to {unitTier}");
	}

	private Unit unitParent;
	public Unit UnitParent {
		get => unitParent;
		set {
			unitParent = value;
			parentTextUI.text = value.name;
		}
	}

	internal List<Equipment> unitEquipment;
	
	/// <summary>
	/// Changes unit affiliation textures based on user side and unit side.
	/// </summary>
	internal abstract void ChangeAffiliation();
	/// <summary>
	/// Changes unit affiliation directly.
	/// </summary>
	/// <param name="sideB"></param>
	internal virtual void ChangeAffiliation(bool sideB) {
		SideB = sideB;
		ChangeAffiliation();
	}

	internal abstract void ChangeSpecialization(int specialization);
	#endregion

	#region UnitVisuals
	protected MeshRenderer iconImage;
	private GameObject movementRangeCircle;
	private GameObject sightRangeCircle;
	internal GameObject WeaponRangeCircle { private set; get; }

	private TextMeshProUGUI nameTextUI;
	private TextMeshProUGUI tierTextUI;
	private TextMeshProUGUI parentTextUI;
	private TextMeshProUGUI equipmentTextUI;
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

		//For affecting units by their ID
		ID = newID;
		ChangeAffiliation(newSideB);
		ChangeAffiliation();
		ChangeSpecialization(newSpecialization);

		movementRange = 0.3f;
		transform.position = newPosition;
		startPosition = newPosition;

		//Adds equipment only if there is some to add, otherwise creates a new Eq list.
		if (newEquipment.Count > 0) {
			AddEquipment(newEquipment);
		} else {
			unitEquipment = new List<Equipment>();
		}

		SetUnitTier(newTier);
		SetName(newIdentifier);

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
	internal float movementRange;
	internal Vector3 startPosition;

	public void OnDrag(PointerEventData eventData) {
		if (!ApplicationController.admin && ApplicationController.sideB != SideB) {
			return;
		}
		Vector3 newPosition = eventData.pointerCurrentRaycast.worldPosition;
		newPosition = Vector3.ClampMagnitude(newPosition - startPosition, ApplicationController.admin ? 9999999f : movementRange) + startPosition;

		transform.position = newPosition;
		ResizeMovementCircle();
	}

	public void OnEndDrag(PointerEventData eventData) {
		Debug.Log($"[{ID}][{name}] Moved to {transform.position}");
		//transform.position = startPosition; //Returns unit to its original position.
	}

	/// <summary>
	/// Shows unit data.
	/// </summary>
	/// <param name="eventData"></param>
	public void OnPointerEnter(PointerEventData eventData) {
		if (!ApplicationController.admin && ApplicationController.sideB != SideB) {
			return;
		}
		sightRangeCircle.SetActive(true);
		movementRangeCircle.SetActive(true);
		equipmentTextUI.gameObject.SetActive(true);
	}

	/// <summary>
	/// Hides unit data.
	/// </summary>
	/// <param name="eventData"></param>
	public void OnPointerExit(PointerEventData eventData) {
		sightRangeCircle.SetActive(false);
		movementRangeCircle.SetActive(false);
		equipmentTextUI.gameObject.SetActive(false);
	}

	/// <summary>
	/// Resized range circle based on remaining range.
	/// </summary>
	internal void ResizeMovementCircle() {
		// Resize the range circle based on the distance between the starting position and the new position of the draggable object
		float maxRange = movementRange - Vector3.Distance(startPosition, transform.position);
		movementRangeCircle.transform.localScale = new Vector3(212 * maxRange, 212 * maxRange, 0);
	}
	#endregion
}