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
	private bool sideB;
	private PointerEventData click;

	Action<Image> edit;
	Action<Image> delete;
	Action<Image> spawn;
	Action<Image> reset;
	Action<Image> softReset;
	Action<Image> spawnBase;

	void Awake() {
		contextMenuItems = new List<ContextMenuItem>();
		edit = new Action<Image>(EditAction);
		delete = new Action<Image>(DeleteAction);
		spawn = new Action<Image>(SpawnAction);
		spawnBase = new Action<Image>(SpawnBaseAction);
		reset = new Action<Image>(ResetAction);
		softReset = new Action<Image>(SoftResetAction);
		aC = GameObject.FindWithTag("GameController").GetComponent<ApplicationController>();
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.button == PointerEventData.InputButton.Right) {
			click = eventData;
			contextMenuItems.Clear();
			if (eventData.pointerClick.GetComponent<Base>() != null) {
				sideB = GetComponent<Unit>().SideB;
				position = eventData.selectedObject.transform.position;
				contextMenuItems.Add(new ContextMenuItem("Reset", sampleButton, reset));
				contextMenuItems.Add(new ContextMenuItem("Spawn", sampleButton, spawn));
				contextMenuItems.Add(new ContextMenuItem("Edit", sampleButton, edit));
				contextMenuItems.Add(new ContextMenuItem("Despawn", sampleButton, delete));
			} else if (GetComponent<Base>() != null) {
				sideB = GetComponent<Base>().sideB;
				position = eventData.selectedObject.transform.position;
				contextMenuItems.Add(new ContextMenuItem("Reset", sampleButton, reset));
				contextMenuItems.Add(new ContextMenuItem("Edit", sampleButton, edit));
				contextMenuItems.Add(new ContextMenuItem("Despawn", sampleButton, delete));
				contextMenuItems.Add(new ContextMenuItem("Soft Reset", sampleButton, softReset));
			} else {
				sideB = aC.sideB;
				contextMenuItems.Add(new ContextMenuItem("Spawn", sampleButton, spawn));
				contextMenuItems.Add(new ContextMenuItem("Spawn Base", sampleButton, spawnBase));
				position = eventData.pointerCurrentRaycast.screenPosition;
			}
			ContextMenu.Instance.CreateContextMenu(contextMenuItems, position);
			aC.deletingMenus = true;
		}
	}

	void SpawnAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		UnitConstructor constructor = UnitManager.Instance.unitMenu.GetComponent<UnitConstructor>();
		constructor.UpdateUnit();
		constructor.UpdatePosition(position);
		constructor.UpdateAffiliation(sideB);
		UnitManager.Instance.unitMenu.SetActive(true);
	}

	void SpawnBaseAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		BaseConstructor constructor = UnitManager.Instance.baseMenu.GetComponent<BaseConstructor>();
		constructor.UpdateBase();
		constructor.UpdatePosition(click.pointerPressRaycast.worldPosition);
		constructor.UpdateAffiliation(sideB);
		UnitManager.Instance.baseMenu.SetActive(true);
	}

	void EditAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		if (GetComponent<Base>() == null && aC.admin) {
			UnitManager.Instance.unitMenu.GetComponent<UnitConstructor>().UpdateUnit(GetComponent<Unit>());
			UnitManager.Instance.unitMenu.SetActive(true);
		} else if (aC.admin) {
			UnitManager.Instance.baseMenu.GetComponent<BaseConstructor>().UpdateBase(GetComponent<Base>());
			UnitManager.Instance.baseMenu.SetActive(true);
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
		if (aC.admin) {
			GetComponent<Unit>().turnStartPosition = transform.position;
			GetComponent<Unit>().ResizeMovementCircle();
		}
	}
}