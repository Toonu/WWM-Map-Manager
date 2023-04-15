﻿using TMPro;
using UnityEngine;

public class Base : MonoBehaviour {
	public BaseType baseType { set; get; }
	public bool sideB;
	private MeshRenderer main;
	internal TextMeshProUGUI identification;
	protected ApplicationController aC;

	public void Initiate(string identification, Vector3 position, BaseType baseType, bool sideB) {
		transform.position = position;
		turnStartPosition = transform.position;
		this.baseType = baseType;
		this.sideB = sideB;

		//Texture management
		main = transform.Find("Main").GetComponent<MeshRenderer>();
		ChangeType(baseType);

		//Application controller for permissions and sides.
		aC = GameObject.FindWithTag("GameController").GetComponent<ApplicationController>();

		//Name identification management
		this.identification = transform.Find("Canvas/Name").GetComponent<TextMeshProUGUI>();
		ChangeIdentification(identification);

		Debug.Log($"[{identification}] Initiated");
	}

	#region Movement

	private Vector3 offset;
	internal Vector3 turnStartPosition;


	private void OnMouseDown() {
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = Camera.main.nearClipPlane;
		offset = transform.position - Camera.main.ScreenToWorldPoint(mousePosition);
	}

	private void OnMouseDrag() {
		//Movement allowed only for admins.
		if (aC.admin) {
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.z = Camera.main.nearClipPlane;
			transform.position = Camera.main.ScreenToWorldPoint(mousePosition) + offset;
		}
	}

	#endregion

	#region Attribute Get/Setters

	internal void ChangeAffiliation() {
		bool isEnemy = aC.sideB != sideB;
		main.material.color = isEnemy ? Color.red : Color.black;
	}

	internal void ChangeAffiliation(bool sideB) {
		this.sideB = sideB;
		ChangeAffiliation();
	}

	internal void ChangeType(BaseType type) {
		baseType = type;
		main.material.mainTexture = UnitManager.Instance.GetBaseTexture(baseType);
		main.material.color = sideB ? Color.red : Color.black;
		if (baseType == BaseType.Airfield) main.transform.localScale = new Vector3(1.5f, 1, 1);
		else main.transform.localScale = Vector3.one;
	}

	internal void ChangeIdentification(string identification) {
		this.identification.text = identification;
		name = identification;
	}

	#endregion
}