using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemController1 : MonoBehaviour {
	public Button sampleButton;
	private List<ContextMenuItem> contextMenuItems;
	ApplicationController aC;
	private Vector3 position;

	void Awake() {
		contextMenuItems = new List<ContextMenuItem>();
		Action<Image> spawn = new Action<Image>(SpawnAction);
		contextMenuItems.Add(new ContextMenuItem("Spawn", sampleButton, spawn));
	}

	public void Start() {
		aC = GameObject.FindWithTag("GameController").GetComponent<ApplicationController>();
	}

	void OnMouseOver() {
		if (Input.GetMouseButtonDown(1)) {
			ContextMenu.Instance.CreateContextMenu(contextMenuItems, new Vector3(Screen.width/2, Screen.height/2));
			position = Input.mousePosition;
			aC.deletingMenus = true;
		}
	}

	void SpawnAction(Image contextPanel) {
		UnitManager.Instance.baseEditMenu.SetActive(true);
		UnitManager.Instance.baseEditMenu.GetComponent<BaseConstructor>().CreateBase();
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = -Camera.main.transform.position.z;
		Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

		UnitManager.Instance.baseEditMenu.GetComponent<BaseConstructor>().UpdatePosition(worldPosition);
	}
}