using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ClickController : MonoBehaviour, IPointerClickHandler, IPointerMoveHandler {
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

	/// <summary>
	/// Function handles screen click
	/// </summary>
	/// <param name="eventData">Click PointeEventData</param>
	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.button == PointerEventData.InputButton.Right) {
			click = eventData;
			contextMenuItems.Clear();
			//Checks if click is above Unit, Base or the Map and shows contextual menu depending on those and User permission level.
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
			//Deletes the context menu after clicking any option or sets the isDeletingMenus flag in program so its closed when clicked anywhere else.
			ContextMenu.Instance.CreateContextMenu(contextMenuItems, position);
			ApplicationController.isDeletingMenus = true;
		}
	}

	/// <summary>
	/// Method opens Unit spawning menu.
	/// </summary>
	/// <param name="contextPanel"></param>
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

	/// <summary>
	/// Method opens Base spawning menu.
	/// </summary>
	/// <param name="contextPanel"></param>
	void SpawnBaseAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		BaseConstructor constructor = UnitManager.Instance.baseMenu.GetComponent<BaseConstructor>();
		constructor.UpdateBase();
		constructor.UpdatePosition(new Vector3(click.pointerPressRaycast.worldPosition.x, click.pointerPressRaycast.worldPosition.y, -0.10f));
		constructor.UpdateAffiliation(sideB);
		UnitManager.Instance.baseMenu.SetActive(true);
	}

	/// <summary>
	/// Method opens Unit or Base editing menu.
	/// </summary>
	/// <param name="contextPanel"></param>
	void EditAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		//Checks if Base or Unit.
		if (GetComponent<Base>() == null) {
			UnitManager.Instance.unitMenu.GetComponent<UnitConstructor>().UpdateUnit(GetComponent<Unit>());
			UnitManager.Instance.unitMenu.SetActive(true);
		} else {
			UnitManager.Instance.baseMenu.GetComponent<BaseConstructor>().UpdateBase(GetComponent<Base>());
			UnitManager.Instance.baseMenu.SetActive(true);
		}
	}

	/// <summary>
	/// Deletes Unit or Base.
	/// </summary>
	/// <param name="contextPanel"></param>
	void DeleteAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		UnitManager.Instance.Despawn(gameObject);
	}

	/// <summary>
	/// Method resets the Unit or Base to its Startposition.
	/// </summary>
	/// <param name="contextPanel"></param>
	void ResetAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		transform.position = GetComponent<IMovable>().StartPosition;
	}

	/// <summary>
	/// Method sets the Unit or Base Start position to its current position.
	/// </summary>
	/// <param name="contextPanel"></param>
	void SoftResetAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		GetComponent<IMovable>().StartPosition = transform.position;
	}

	/// <summary>
	/// Method returns Color under the cursor.
	/// </summary>
	/// <param name="pointerCurrentRaycast"></param>
	/// <param name="target"></param>
	/// <returns></returns>
	public static Color GetColour(RaycastResult pointerCurrentRaycast, Transform target) {
		RaycastHit2D hit = Physics2D.Raycast(pointerCurrentRaycast.worldPosition, pointerCurrentRaycast.worldNormal);
		SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
		Texture2D texture = spriteRenderer.sprite.texture;
		Vector2 localPoint = target.InverseTransformPoint(hit.point);

		// Calculates the pixel position in the texture
		float x = (localPoint.x / spriteRenderer.bounds.size.x + 0.5f) * texture.width;
		float y = (localPoint.y / spriteRenderer.bounds.size.y + 0.5f) * texture.height;
		return texture.GetPixel(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
		//string hex = ColorUtility.ToHtmlStringRGB(color);
		//Debug.Log(hex);
	}

	/// <summary>
	/// Method updates Color under the cursor every time a cursor moves.
	/// </summary>
	/// <param name="eventData"></param>
	public void OnPointerMove(PointerEventData eventData) {
		//Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGB(GetColour(eventData.pointerCurrentRaycast, transform.parent)) + ">PIXEL</color>");
	}
}