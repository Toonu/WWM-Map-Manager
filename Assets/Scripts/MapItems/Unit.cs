using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Unit : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IMovable, IEquatable<Unit> {
	#region Attribute Get/Setters

	public int ID { get; set; }												//Unit identifier.
	public bool SideB { get; set; }                                         //Unit affiliation.
	public bool isGhost { get; set; }										//Unit spotting.

	/// <summary>
	/// Setter for the unit name which changes the name label and Object name.
	/// </summary>
	/// <param name="identification">New identifier</param>
	internal virtual void SetName(string identification) {
		//Change name to roman numberals if corps+.
		if (GetUnitTier() > UnitTier.Division) {
			nameTextUI.text = EnumUtil.GetCorps(identification);
		} else {
			nameTextUI.text = EnumUtil.NumberWithSuffix(Convert.ToInt16(identification));
		}
		name = identification;
		if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Name changed to {identification}");
	}

	private UnitTier unitTier;												//Unit tier.
	public UnitTier GetUnitTier() { return unitTier; }
	public string GetUnitTierText() { return tierTextUI.text; }
	/// <summary>
	/// Setter for the unit tier which changes the tier label and tier.
	/// </summary>
	/// <param name="echelon"></param>
	public virtual void SetUnitTier(int echelon) {
		tierTextUI.text = EnumUtil.GetUnitTier(echelon);
		unitTier = (UnitTier)echelon;
		if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Tier changed to {unitTier}");
	}
	/// <summary>
	/// Setter for the unit tier which changes the tier label and tier.
	/// </summary>
	/// <param name="echelon"></param>
	public void SetUnitTier(UnitTier echelon) {
		tierTextUI.text = EnumUtil.GetUnitTier((int)echelon);
		unitTier = echelon;
		if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Tier changed to {unitTier}");
	}

	private Unit unitParent;												//Higher unit
	public Unit UnitParent {
		get => unitParent;
		set {
			unitParent = value;
			parentTextUI.text = EnumUtil.GetCorps(value.name);
			if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Parent unit changed to {value.name}");
		}
	}

	internal List<Equipment> equipmentList;									//Equipment

	/// <summary>
	/// Method changes unit affiliation textures based on user isSideB and unit isSideB.
	/// </summary>
	internal abstract void ChangeAffiliation();
	/// <summary>
	/// Method changes unit affiliation directly.
	/// </summary>
	/// <param name="sideB"></param>
	internal virtual void ChangeAffiliation(bool sideB) {
		SideB = sideB;
		ChangeAffiliation();
		if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Affiliation changed to side B: {sideB}");
	}
	/// <summary>
	/// Method changes the unit specialization.
	/// </summary>
	/// <param name="specialization"></param>
	internal abstract void ChangeSpecialization(int specialization);

	#endregion

	#region UnitVisuals
	protected MeshRenderer iconImage;
	private GameObject movementRangeCircle;
	private GameObject sightRangeCircle;
	internal GameObject WeaponRangeCircle { private set; get; }

	private TextMeshProUGUI nameTextUI;
	private TextMeshProUGUI tierTextUI;
	internal TextMeshProUGUI parentTextUI;
	protected TextMeshProUGUI equipmentTextUI;
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
		//Set unit affiliation side and also its textures.
		ChangeAffiliation(newSideB);
		ChangeSpecialization(newSpecialization);
		//For spotted units.
		isGhost = false;

		movementRange = 0.3f;
		transform.position = new Vector3(newPosition.x, newPosition.y, -0.15f);
		StartPosition = new Vector3(newPosition.x, newPosition.y, -0.15f);

		//Adds equipment only if there is some to add, otherwise creates a new Eq list.
		if (newEquipment.Count > 0) {
			AddEquipment(newEquipment);
		} else {
			equipmentList = new List<Equipment>();
		}

		SetUnitTier(newTier);
		SetName(newIdentifier);

		if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Initiated");
	}

	/// <summary>
	/// Method adds Equipment from the List to the Unit equipment and edits the equipment string label.
	/// </summary>
	/// <param name="newEquipment">List of Equipment to add.</param>
	internal void AddEquipment(List<Equipment> newEquipment) {
		if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Adding new equipment list.");
		equipmentList = newEquipment.ToList();
		RecalculateAttributes();
	}
	/// <summary>
	/// Method adds the equipment to the list.
	/// </summary>
	/// <param name="newEquipment"></param>
	internal void AddEquipment(Equipment newEquipment) {
		if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Adding {newEquipment.Amount} of equipment {newEquipment.name}.");
		equipmentList.Add(newEquipment);
		
		RecalculateAttributes();
	}
	/// <summary>
	/// Method removes the equipment from the list and destroys its Object.
	/// </summary>
	/// <param name="template"></param>
	internal void RemoveEquipment(Equipment template) {
		Equipment equipment = equipmentList.Find(e => template.equipmentName == e.equipmentName);
		if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Equipment {equipment} removed.");
		equipmentList.Remove(equipment);
		Destroy(equipment.gameObject);
		RecalculateAttributes();
	}
	
	/// <summary>
	/// Method updates UI labels of the unit.
	/// </summary>
	internal virtual void RecalculateAttributes() {
		if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Recalculating unit attributes.");
		//Equipment UI label.
		equipmentTextUI.text = string.Join("\n", equipmentList.Select(equipment => $"{equipment.equipmentName}:{equipment.Amount}"));
		equipmentList.ForEach(eq => { if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Adding Equipment | {eq.Amount} {eq.equipmentName}"); });

		//Unit ranges.
		movementRange = equipmentList.Min(e => e.movementRange);
		sightRange = equipmentList.Max(e => e.sightRange);
		weaponRange = equipmentList.Max(e => e.weaponRange);

		sightRangeCircle.transform.localScale = new Vector3(212 * sightRange, 212 * sightRange, 0);
		WeaponRangeCircle.transform.localScale = new Vector3(212 * weaponRange, 212 * weaponRange, 0);
		ResizeMovementCircle();
	}

	internal float sightRange = 0.25f;
	internal float weaponRange = 0.2f;

	#region Movement
	internal float movementRange;
	public Vector3 StartPosition {
		get {
			return startPosition;
		}
		set {
			startPosition = value;
			ResizeMovementCircle();
			if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Soft reset at [{transform.position}]");
		}
	}

	private Vector3 startPosition;
	public virtual void OnDrag(PointerEventData eventData) {
		if (!ApplicationController.isAdmin && ApplicationController.isSideB != SideB) {
			return;
		}
		Vector3 newPosition = eventData.pointerCurrentRaycast.worldPosition;
		newPosition = Vector3.ClampMagnitude(newPosition - StartPosition, ApplicationController.isAdmin ? 9999999f : movementRange) + StartPosition;

		transform.position = new Vector3(newPosition.x, newPosition.y, -0.15f);
		ResizeMovementCircle();
	}

	public virtual void OnEndDrag(PointerEventData eventData) {
		if (ApplicationController.isDebug) Debug.Log($"[{ID}][{name}] Moved to {transform.position}");
		ApplicationController.Instance.server.SaveUnits();
	}

	/// <summary>
	/// Shows unit data.
	/// </summary>
	/// <param name="eventData"></param>
	public virtual void OnPointerEnter(PointerEventData eventData) {
		if (!ApplicationController.isAdmin && ApplicationController.isSideB != SideB) {
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
	public virtual void OnPointerExit(PointerEventData eventData) {
		sightRangeCircle.SetActive(false);
		movementRangeCircle.SetActive(false);
		equipmentTextUI.gameObject.SetActive(false);
	}

	/// <summary>
	/// Resized range circle based on remaining range.
	/// </summary>
	internal void ResizeMovementCircle() {
		// Resize the range circle based on the distance between the starting position and the new position of the draggable object
		float maxRange = movementRange - Vector3.Distance(StartPosition, transform.position);
		movementRangeCircle.transform.localScale = new Vector3(212 * maxRange, 212 * maxRange, 0);
	}
	#endregion

	public bool Equals(Unit other) {
		return !(other == null) &&
			   base.Equals(other) &&
			   ID == other.ID;
	}

	public override int GetHashCode() {
		return ID;
	}

	internal abstract int GetSpecialization();
}