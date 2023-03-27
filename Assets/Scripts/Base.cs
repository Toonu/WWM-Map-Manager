using TMPro;
using UnityEngine;

public class Base : MonoBehaviour {
	internal BaseType baseType;
	internal bool enemySide = false;
	private MeshRenderer main;
	private TextMeshProUGUI identification;

	public void Initiate(string identification, Vector3 position, BaseType baseType) {
		transform.position = position;
		this.baseType = baseType;

		main = transform.Find("Main").GetComponent<MeshRenderer>();
		main.material.mainTexture = UnitManager.Instance.GetBaseTexture(baseType);
		if (baseType == BaseType.Airfield)	main.transform.localScale = new Vector3(1.5f, 1, 1);
		
		this.identification = transform.Find("Canvas/Name").GetComponent<TextMeshProUGUI>();
		this.identification.text = identification;
		turnStartPosition = transform.position;
	}

	private Vector3 offset;
	internal Vector3 turnStartPosition;
	private void OnMouseDown() {
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = Camera.main.nearClipPlane;
		offset = transform.position - Camera.main.ScreenToWorldPoint(mousePosition);
	}

	private void OnMouseDrag() {
		//Movement allowed only for admins.
		if (GameObject.FindWithTag("GameController").GetComponent<ApplicationController>().admin) {
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.z = Camera.main.nearClipPlane;
			transform.position = Camera.main.ScreenToWorldPoint(mousePosition) + offset;
		}
	}

	internal void ChangeAffiliation(bool enemySide) {
		this.enemySide = enemySide;
	}

	internal void ChangeType(BaseType type) {
		baseType = type;
		main.material.mainTexture = UnitManager.Instance.GetBaseTexture(baseType);
	}

	internal void ChangeIdentification(string identification) {
		this.identification.text = identification;
	}
}
