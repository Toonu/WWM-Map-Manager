using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseConstructor : MonoBehaviour {
	private Base b;
	private TMP_Dropdown type;
	private TMP_InputField baseName;

	public void Awake() { 
		type = transform.Find("BaseType").GetComponent<TMP_Dropdown>();
		baseName = transform.Find("BaseName").GetComponent<TMP_InputField>();
		//Populates the UI with base options.
		type.ClearOptions();
		type.AddOptions(new List<string>() { "Base", "Airfield", "Port", "Spawn" });
	}

	/// <summary>
	/// Updates the constructed base name attribute.
	/// </summary>
	/// <param name="identification"></param>
	public void UpdateName(string identification) {
		b.ChangeIdentification(identification);
	}
	/// <summary>
	/// Updates the constructed base type attribute.
	/// </summary>
	/// <param name="type"></param>
	public void UpdateType(int type) {
		b.ChangeType((BaseType)type);
	}
	/// <summary>
	/// Updates the constructed base with the one that was clicked.
	/// </summary>
	/// <param name="b"></param>
	public void UpdateBase(Base b) {
		this.b = b;
		type.value = (int)b.baseType;
		baseName.text = b.identification.text;
	}

	/// <summary>
	/// Creates a new empty base for construction.
	/// </summary>
	public void CreateBase() {
		b = GameObject.FindWithTag("Units").GetComponent<UnitManager>().SpawnBase("NewBase", Vector3.zero, BaseType.Base, false);
		//TODO add some form of movement to the base on creation so even user can place it
	}
}