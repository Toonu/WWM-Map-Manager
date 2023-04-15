using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemController : MonoBehaviour {
	public Button sampleButton;
	private List<ContextMenuItem> contextMenuItems;
	ApplicationController aC;
	private bool sideB;

	void Awake() {
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

	public void Start() {
		aC = GameObject.FindWithTag("GameController").GetComponent<ApplicationController>();
		if (GetComponent<Base>() == null) {
			
			sideB = GetComponentInParent<Unit>().SideB;
		} else {
			sideB = GetComponentInParent<Base>().sideB;
		}
	}

	void OnMouseOver() {
		/*if (Input.GetMouseButtonDown(1) && (aC.admin || aC.sideB == sideB)) {
			ContextMenu.Instance.CreateContextMenu(contextMenuItems, Camera.main.WorldToScreenPoint(transform.position));
			aC.deletingMenus = true;
		}*/
	}

	void EditAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		if (GetComponent<Base>() == null && aC.admin) {
			UnitManager.Instance.unitMenu.SetActive(true);
			UnitManager.Instance.unitMenu.GetComponent<UnitConstructor>().UpdateUnit(GetComponent<Unit>());
		} else if (aC.admin) {
			UnitManager.Instance.baseMenu.SetActive(true);
			UnitManager.Instance.baseMenu.GetComponent<BaseConstructor>().UpdateBase(GetComponent<Base>());
		}
	}

	void SpawnAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		if (GetComponent<Base>() != null || aC.admin) {
			UnitManager.Instance.unitMenu.SetActive(true);
			UnitConstructor constructor = UnitManager.Instance.unitMenu.GetComponent<UnitConstructor>();
			constructor.UpdatePosition(transform.position);
			if (GetComponent<Base>() == null) {
				constructor.UpdateAffiliation(GetComponent<Unit>().SideB);
			} else {
				constructor.UpdateAffiliation(GetComponent<Base>().sideB);
			}	
		}
	}

	void DeleteAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		if (aC.admin) {
			UnitManager.Instance.Despawn(gameObject);
		}
	}

	void ResetAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		if (GetComponent<Base>() == null) {
			transform.position = GetComponent<Unit>().turnStartPosition;
		} else {
			transform.position = GetComponent<Base>().turnStartPosition;
		}
	}

	void SoftResetAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		if (GetComponent<Base>() == null && aC.admin) {
			GetComponent<Unit>().turnStartPosition = transform.position;
			GetComponent<Unit>().ResizeMovementCircle();
		}
	}
}