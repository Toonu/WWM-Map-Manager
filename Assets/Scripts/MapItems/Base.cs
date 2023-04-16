using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Base : MonoBehaviour, IDragHandler, IEndDragHandler {
	internal BaseType BaseType;
	internal bool sideB;
	private MeshRenderer main;
	private TextMeshProUGUI nameUI;

	public void Initiate(string identification, Vector3 position, BaseType baseType, bool sideB) {
		nameUI = transform.Find("Canvas/Name").GetComponent<TextMeshProUGUI>();
		main = transform.Find("Main").GetComponent<MeshRenderer>();

		transform.position = position;
		startPosition = transform.position;
		BaseType = baseType;
		this.sideB = sideB;

		ChangeType(baseType);
		ChangeIdentification(identification);

		Debug.Log($"[{name}] Initiated");
	}

	#region Movement
	internal Vector3 startPosition;

	/// <summary>
	/// Drags the unit up to its maximal range based on range.
	/// </summary>
	public void OnDrag(PointerEventData eventData) {
		if (!ApplicationController.admin) {
			return;
		}
		transform.position = eventData.pointerCurrentRaycast.worldPosition;
	}

	public void OnEndDrag(PointerEventData eventData) {
		Debug.Log($"[{name}] Moved to {transform.position}");
	}

	#endregion

	#region Attribute Get/Setters

	internal void ChangeAffiliation() {
		bool isEnemy = ApplicationController.sideB != sideB;
		main.material.color = isEnemy ? Color.red : Color.black;
	}

	internal void ChangeAffiliation(bool sideB) {
		this.sideB = sideB;
		ChangeAffiliation();
	}

	internal void ChangeType(BaseType type) {
		BaseType = type;
		main.material.mainTexture = UnitManager.Instance.GetBaseTexture(BaseType);
		main.material.color = sideB ? Color.red : Color.black;
		if (BaseType == BaseType.Airfield) main.transform.localScale = new Vector3(1.5f, 1, 1);
		else main.transform.localScale = Vector3.one;
	}

	internal void ChangeIdentification(string identification) {
		nameUI.text = identification;
		name = identification;
	}

	#endregion
}
