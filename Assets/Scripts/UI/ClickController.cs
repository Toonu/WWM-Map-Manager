using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ClickController : MonoBehaviour, IPointerClickHandler {
	public Button sampleButton;
	private List<ContextMenuItem> contextMenuItems;
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
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.button == PointerEventData.InputButton.Right) {
			click = eventData;
			contextMenuItems.Clear();
			if (eventData.pointerClick.GetComponent<Unit>() != null && (eventData.pointerClick.GetComponent<Unit>().SideB == ApplicationController.isSideB || ApplicationController.isAdmin)) {
				sideB = eventData.pointerClick.GetComponent<Unit>().SideB;
				position = eventData.pointerCurrentRaycast.screenPosition;
				if (ApplicationController.isAdmin) {
					contextMenuItems.Add(new ContextMenuItem("Edit", sampleButton, edit));
					contextMenuItems.Add(new ContextMenuItem("Despawn", sampleButton, delete));
					contextMenuItems.Add(new ContextMenuItem("Soft Reset", sampleButton, softReset));
				}
				contextMenuItems.Add(new ContextMenuItem("Reset", sampleButton, reset));

			} else if (eventData.pointerClick.GetComponent<Base>() != null && (eventData.pointerClick.GetComponent<Base>().SideB == ApplicationController.isSideB || ApplicationController.isAdmin)) {
				sideB = eventData.pointerClick.GetComponent<Base>().SideB;
				position = eventData.pointerCurrentRaycast.screenPosition;
				contextMenuItems.Add(new ContextMenuItem("Spawn", sampleButton, spawn));
				if (ApplicationController.isAdmin) {
					contextMenuItems.Add(new ContextMenuItem("Edit", sampleButton, edit));
					contextMenuItems.Add(new ContextMenuItem("Despawn", sampleButton, delete));
					contextMenuItems.Add(new ContextMenuItem("Reset", sampleButton, reset));
				}
			} else {
				sideB = ApplicationController.isSideB;
				if (ApplicationController.isAdmin) {
					contextMenuItems.Add(new ContextMenuItem("Spawn", sampleButton, spawn));
				}
				contextMenuItems.Add(new ContextMenuItem("Spawn Base", sampleButton, spawnBase));
				position = eventData.pointerCurrentRaycast.screenPosition;
			}
			ContextMenu.Instance.CreateContextMenu(contextMenuItems, position);
			ApplicationController.isDeletingMenus = true;
		}
	}

	void SpawnAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		UnitConstructor constructor = UnitManager.Instance.unitMenu.GetComponent<UnitConstructor>();
		if (gameObject.GetComponent<Base>() != null) {
			constructor.UpdateUnit((int)gameObject.GetComponent<Base>().BaseType);
		} else {
			constructor.UpdateUnit(0);
		}
		constructor.UpdatePosition(new Vector3(click.pointerPressRaycast.worldPosition.x, click.pointerPressRaycast.worldPosition.y, -0.15f));
		constructor.UpdateAffiliation(sideB);
		UnitManager.Instance.unitMenu.SetActive(true);
	}

	void SpawnBaseAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		BaseConstructor constructor = UnitManager.Instance.baseMenu.GetComponent<BaseConstructor>();
		constructor.UpdateBase();
		constructor.UpdatePosition(new Vector3(click.pointerPressRaycast.worldPosition.x, click.pointerPressRaycast.worldPosition.y, -0.10f));
		constructor.UpdateAffiliation(sideB);
		UnitManager.Instance.baseMenu.SetActive(true);
	}

	void EditAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		if (GetComponent<Base>() == null) {
			UnitManager.Instance.unitMenu.GetComponent<UnitConstructor>().UpdateUnit(GetComponent<Unit>());
			UnitManager.Instance.unitMenu.SetActive(true);
		} else {
			UnitManager.Instance.baseMenu.GetComponent<BaseConstructor>().UpdateBase(GetComponent<Base>());
			UnitManager.Instance.baseMenu.SetActive(true);
		}
	}

	void DeleteAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		UnitManager.Instance.Despawn(gameObject);
	}

	void ResetAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		transform.position = GetComponent<IMovable>().StartPosition;
		Debug.Log($"[{name}] Position reset to [{transform.position}]");
	}

	void SoftResetAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		GetComponent<Unit>().StartPosition = transform.position;
	}
}