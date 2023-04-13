using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ClickController : MonoBehaviour, IPointerClickHandler {
	public Button sampleButton;
	private List<ContextMenuItem> contextMenuItems;
	ApplicationController aC;
	private Vector3 position;

	void Awake() {
		contextMenuItems = new List<ContextMenuItem>();
		Action<Image> spawn = new Action<Image>(SpawnAction);
		contextMenuItems.Add(new ContextMenuItem("Spawn", sampleButton, spawn));
		aC = GameObject.FindWithTag("GameController").GetComponent<ApplicationController>();
	}

	void OnMouseOver() {
		if (Input.GetMouseButtonDown(1)) {
			ContextMenu.Instance.CreateContextMenu(contextMenuItems, new Vector3(Screen.width / 2, Screen.height / 2));
			position = Input.mousePosition;
			aC.deletingMenus = true;
		}
	}

	void SpawnAction(Image contextPanel) {
		UnitManager.Instance.baseEditMenu.SetActive(true);
		UnitManager.Instance.baseEditMenu.GetComponent<BaseConstructor>().CreateBase();
		UnitManager.Instance.baseEditMenu.GetComponent<BaseConstructor>().UpdatePosition(Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, -0.1f)));
	}

	public void OnPointerClick(PointerEventData eventData) {
		/*if (eventData.button == PointerEventData.InputButton.Right) {
			position = eventData.position;
			ContextMenu.Instance.CreateContextMenu(contextMenuItems, position);
			aC.deletingMenus = true;
		}*/
	}
}