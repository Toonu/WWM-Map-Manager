using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseConstructor : MonoBehaviour {
	private Base constructedBase;
	private TMP_Dropdown type;
	private TMP_InputField baseName;

	public void Awake() {
		type = transform.Find("BaseType").GetComponent<TMP_Dropdown>();
		baseName = transform.Find("BaseName").GetComponent<TMP_InputField>();
		//Populates the UI with base options.
		type.ClearOptions();
		List<string> options = new() { "Base", "Airfield", "Port" };
		if (ApplicationController.admin) {
			options.Add("Spawn");
		}

		type.AddOptions(options);
	}

	private void OnEnable() {
		type.value = (int)constructedBase.BaseType;
		baseName.text = constructedBase.name;
	}

	/// <summary>
	/// Updates the constructed base name attribute.
	/// </summary>
	/// <param name="identification"></param>
	public void UpdateName(string identification) {
		constructedBase.ChangeIdentification(identification);
	}
	/// <summary>
	/// Updates the constructed base type attribute.
	/// </summary>
	/// <param name="type"></param>
	public void UpdateType(int type) {
		constructedBase.BaseType = (BaseType)type;
	}
	/// <summary>
	/// Updates the constructed base position attribute.
	/// </summary>
	/// <param name="position"></param>
	public void UpdatePosition(Vector3 position) {
		constructedBase.transform.position = position;
		constructedBase.StartPosition = position;
	}
	/// <summary>
	/// Updates the constructed base affiliation attribute.
	/// </summary>
	/// <param name="sideB">New sideB</param>
	public void UpdateAffiliation(bool sideB) {
		constructedBase.ChangeAffiliation(sideB);
	}
	/// <summary>
	/// Updates the constructed base with the one that was clicked.
	/// </summary>
	/// <param name="b"></param>
	public void UpdateBase(Base b) {
		Debug.Log("Base editor opened.");
		constructedBase = b;
	}
	/// <summary>
	/// Creates a new empty base for construction.
	/// </summary>
	public void UpdateBase() {
		Debug.Log("Base editor opened.");
		constructedBase = UnitManager.Instance.SpawnBase("NewBase", Vector3.zero, BaseType.Base, false);
	}

	public void Cancel() {
		Debug.Log("Base editor canceled.");
		Destroy(constructedBase.gameObject);
		gameObject.SetActive(false);
	}

	public void Close() {
		Debug.Log("Base editor closed.");
		if (!ApplicationController.admin) {
			SheetSync.UpdatePoints(-100);
		}
		gameObject.SetActive(false);
	}
}