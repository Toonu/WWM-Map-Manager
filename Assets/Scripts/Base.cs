using TMPro;
using UnityEngine;

public class Base : MonoBehaviour {
	internal BaseType baseType;
	public bool sideB;
	private MeshRenderer main;
	internal TextMeshProUGUI identification;
	protected ApplicationController aC;

	public void Initiate(string identification, Vector3 position, BaseType baseType, bool sideB) {
		transform.position = position;
		this.baseType = baseType;
		this.sideB = sideB;

		main = transform.Find("Main").GetComponent<MeshRenderer>();
		main.material.mainTexture = UnitManager.Instance.GetBaseTexture(baseType);
		main.material.color = sideB ? Color.red : Color.black;
		if (baseType == BaseType.Airfield)	main.transform.localScale = new Vector3(1.5f, 1, 1);

		aC = GameObject.FindWithTag("GameController").GetComponent<ApplicationController>();
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

	internal void ChangeAffiliation() {
		bool sideB = aC.sideB == this.sideB;
		main.material.color = sideB ? Color.black : Color.red;
	}

	internal void ChangeType(BaseType type) {
		baseType = type;
		main.material.mainTexture = UnitManager.Instance.GetBaseTexture(baseType);
	}

	internal void ChangeIdentification(string identification) {
		this.identification.text = identification;
	}
}
