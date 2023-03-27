﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemController : MonoBehaviour {
	public Button sampleButton;
	private List<ContextMenuItem> contextMenuItems;
	ApplicationController applicationController;

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

		applicationController = GameObject.FindWithTag("GameController").GetComponent<ApplicationController>();
	}

	void OnMouseOver() {
		if (Input.GetMouseButtonDown(1) || Input.mouseScrollDelta.y != 0) {
			ContextMenu.Instance.CreateContextMenu(contextMenuItems, Camera.main.WorldToScreenPoint(transform.position));
			applicationController.deletingMenus = true;
		}
	}

	void EditAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		if (GetComponent<Base>() == null && applicationController.admin) {
			UnitManager.Instance.unitEditMenu.SetActive(true);
			UnitManager.Instance.unitEditMenu.GetComponent<UnitEditor>().UpdateUnit(GetComponent<Unit>());
		} else if (applicationController.admin) {
			UnitManager.Instance.baseEditMenu.SetActive(true);
			UnitManager.Instance.baseEditMenu.GetComponent<BaseEditor>().UpdateBase(GetComponent<Base>());
		}
	}

	void SpawnAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		if (GetComponent<Base>() != null || applicationController.admin) {
			UnitManager.Instance.PopulateUI(0);
			UnitManager.Instance.unitSpawnMenu.SetActive(true);
			UnitConstructor constructor = UnitManager.Instance.unitSpawnMenu.GetComponent<UnitConstructor>();
			constructor.UpdatePosition(transform.position);
			constructor.UpdateAffiliation(GetComponent<Base>() == null ? GetComponent<Unit>().enemySide : GetComponent<Base>().enemySide);
		}
	}

	void DeleteAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		if (applicationController.admin) {
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
		if (GetComponent<Base>() == null && applicationController.admin) {
			GetComponent<Unit>().turnStartPosition = transform.position;
		}
	}
}