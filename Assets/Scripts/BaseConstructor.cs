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
		type.AddOptions(new List<string>() { "Base", "Airfield", "Port", "Spawn" });
	}

	private void OnEnable() {
		type.value = (int)constructedBase.baseType;
		baseName.text = constructedBase.identification.text;
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
		constructedBase.ChangeType((BaseType)type);
	}
	/// <summary>
	/// Updates the constructed base position attribute.
	/// </summary>
	/// <param name="position"></param>
	public void UpdatePosition(Vector3 position) {
		constructedBase.transform.position = position;
	}
	/// <summary>
	/// Updates the constructed base affiliation attribute.
	/// </summary>
	/// <param name="sideB">New side</param>
	public void UpdateAffiliation(bool sideB) {
		constructedBase.ChangeAffiliation(sideB);
	}
	/// <summary>
	/// Updates the constructed base with the one that was clicked.
	/// </summary>
	/// <param name="b"></param>
	public void UpdateBase(Base b) {
		constructedBase = b;
	}
	/// <summary>
	/// Creates a new empty base for construction.
	/// </summary>
	public void UpdateBase() {
		constructedBase = GameObject.FindWithTag("Units").GetComponent<UnitManager>().SpawnBase("NewBase", Vector3.zero, BaseType.Base, false);
	}
}