using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseEditor : MonoBehaviour {
	private Base b;
	private TMP_Dropdown type;
	private TMP_InputField baseName;

	public void Awake() { 
		type = transform.Find("BaseType").GetComponent<TMP_Dropdown>();
		baseName = transform.Find("BaseName").GetComponent<TMP_InputField>();

		type.ClearOptions();
		type.AddOptions(new List<string>() { "Base", "Airfield", "Port", "Spawn" });
	}

	public void UpdateName(string identification) {
		b.ChangeIdentification(identification);
	}
	public void UpdateType(int type) {
		b.ChangeType((BaseType)type);
	}
	public void UpdateBase(Base b) {
		this.b = b;
		type.value = (int)b.baseType;
		baseName.text = b.identification.text;
	}
}