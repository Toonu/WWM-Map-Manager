using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseConstructor : MonoBehaviour {
	private Base constructedBase;
	private TMP_Dropdown type;
	private TMP_InputField baseName;
	public static bool Editing = false;

	/// <summary>
	/// Method sets up Component references on startup and populates the UI with base options.
	/// </summary>
	public void Awake() {
		type = transform.Find("BaseType").GetComponent<TMP_Dropdown>();
		baseName = transform.Find("BaseName").GetComponent<TMP_InputField>();
		//Populates the UI with base options.
		type.ClearOptions();
		List<string> options = new() { "Base", "Airfield", "Port" };
		if (ApplicationController.isAdmin) {
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
	/// <param name="sideB">New isSideB</param>
	public void UpdateAffiliation(bool sideB) {
		constructedBase.ChangeAffiliation(sideB);
	}
	/// <summary>
	/// Updates the constructed base with the one that was clicked.
	/// </summary>
	/// <param name="b"></param>
	public void UpdateBase(Base b) {
		Editing = true;
		if (ApplicationController.isDebug) Debug.Log("Base editor opened.");
		constructedBase = b;
	}
	/// <summary>
	/// Creates a new empty base for construction.
	/// </summary>
	public void UpdateBase() {
		Editing = false;
		if (ApplicationController.isDebug) Debug.Log("Base editor opened.");
		constructedBase = UnitManager.Instance.SpawnBase("NewBase", Vector3.zero, BaseType.Base, false, false);
	}

	public void Cancel() {
		if (ApplicationController.isDebug) Debug.Log("Base editor canceled.");
		if (!Editing) {
			Destroy(constructedBase.gameObject);
		}
		gameObject.SetActive(false);
	}

	public void Close() {
		if (ApplicationController.isDebug) Debug.Log("Base editor closed.");
		if (!ApplicationController.isAdmin) {
			SheetSync.UpdatePoints(-100);
		}
		gameObject.SetActive(false);
	}
}