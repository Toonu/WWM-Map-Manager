using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour {
	public Button sampleButton;                         // sample button prefab
	private List<ContextMenuItem> contextMenuItems;     // list of items in menu

	void Awake() {
		// Here we are creating and populating our future Context Menu.
		// I do it in Awake once, but as you can see, 
		// it can be edited at runtime anywhere and anytime.

		contextMenuItems = new List<ContextMenuItem>();
		Action<Image> edit = new Action<Image>(EditAction);
		Action<Image> delete = new Action<Image>(DeleteAction);
		Action<Image> spawn = new Action<Image>(SpawnAction);
		Action<Image> reset = new Action<Image>(ResetAction);
		Action<Image> softReset = new Action<Image>(SoftResetAction);



		contextMenuItems.Add(new ContextMenuItem("Reset", sampleButton, reset));
		contextMenuItems.Add(new ContextMenuItem("Spawn", sampleButton, spawn));
		contextMenuItems.Add(new ContextMenuItem("Edit", sampleButton, edit));
		contextMenuItems.Add(new ContextMenuItem("Despawn", sampleButton, delete));
		contextMenuItems.Add(new ContextMenuItem("Soft Reset", sampleButton, softReset));
	}

	void OnMouseOver() {
		if (Input.GetMouseButtonDown(1)) {
			ContextMenu.Instance.CreateContextMenu(contextMenuItems, Camera.main.WorldToScreenPoint(transform.position));
			GameObject.FindWithTag("GameController").GetComponent<ApplicationController>().deletingMenus = true;
		}
	}

	void EditAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		if (GetComponent<Base>() == null && GameObject.FindWithTag("GameController").GetComponent<ApplicationController>().admin) {
			UnitManager.Instance.unitEditMenu.SetActive(true);
			UnitManager.Instance.unitEditMenu.GetComponent<UnitEditor>().UpdateUnit(GetComponent<Unit>());
		}
	}

	void SpawnAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		if (GetComponent<Base>() != null || GameObject.FindWithTag("GameController").GetComponent<ApplicationController>().admin) {
			UnitManager.Instance.unitSpawnMenu.SetActive(true);
			UnitManager.Instance.unitSpawnMenu.GetComponent<UnitConstructor>().UpdatePosition(transform.position);
		}
	}

	void DeleteAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		if (GameObject.FindWithTag("GameController").GetComponent<ApplicationController>().admin) {
			UnitManager.Instance.Despawn(gameObject);
			Destroy(gameObject);
		}
	}

	void ResetAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		if (GetComponent<Base>() == null) {
			transform.position = GetComponent<Unit>().turnStartPosition;
		}
	}

	void SoftResetAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		if (GetComponent<Base>() == null && GameObject.FindWithTag("GameController").GetComponent<ApplicationController>().admin) {
			GetComponent<Unit>().turnStartPosition = transform.position;
		}
	}
}