using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Base : MonoBehaviour, IDragHandler, IEndDragHandler, IMovable {
	[HideInInspector]
	public Vector3 StartPosition { get; set; }
	[HideInInspector]
	public bool SideB { get; set; }
	[HideInInspector]
	public BaseType BaseType {
		get { return baseType; }
		set {
			baseType = value;
			icon.material.mainTexture = UnitManager.Instance.GetBaseTexture(BaseType);
			icon.material.color = SideB == ApplicationController.isSideB ? Color.black : Color.red;
			icon.transform.localScale = BaseType == BaseType.Airfield ? new Vector3(1.5f, 1, 1) : Vector3.one;
		}
	}
	private BaseType baseType;
	private MeshRenderer icon;
	private TextMeshProUGUI nameUI;

	public void Initiate(string newName, Vector3 newPosition, BaseType newType, bool newSideB) {
		nameUI = transform.Find("Canvas/Name").GetComponent<TextMeshProUGUI>();
		icon = transform.Find("Main").GetComponent<MeshRenderer>();

		transform.position = newPosition;
		StartPosition = transform.position;
		BaseType = newType;
		SideB = newSideB;

		BaseType = newType;
		ChangeIdentification(newName);

		Debug.Log($"[{name}] Initiated");
	}

	#region Attribute Get/Setters

	internal void ChangeAffiliation() {
		bool isEnemy = ApplicationController.isSideB != SideB;
		icon.material.color = isEnemy ? Color.red : Color.black;
	}

	internal void ChangeAffiliation(bool sideB) {
		SideB = sideB;
		ChangeAffiliation();
	}

	internal void ChangeIdentification(string identification) {
		nameUI.text = identification;
		name = identification;
	}

	#endregion

	#region Movement

	/// <summary>
	/// Drags the unit up to its maximal range based on range.
	/// </summary>
	public void OnDrag(PointerEventData eventData) {
		if (!ApplicationController.isAdmin) {
			return;
		}
		transform.position = new Vector3(eventData.pointerCurrentRaycast.worldPosition.x, eventData.pointerCurrentRaycast.worldPosition.y, -0.1f);
	}

	/// <summary>
	/// Logs the final newPosition.
	/// </summary>
	/// <param name="eventData"></param>
	public void OnEndDrag(PointerEventData eventData) {
		Debug.Log($"[{name}] Moved to {transform.position}");
	}

	#endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(Base))]
public class BaseEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		Base b = (Base)target;

		EditorGUILayout.LabelField("ID", b.name);
		EditorGUILayout.LabelField("SideB", b.SideB.ToString());
		EditorGUILayout.LabelField("Type", b.BaseType.ToString());
	}
}
#endif