using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ClickMapController : MonoBehaviour, IPointerClickHandler, IPointerMoveHandler {
	public Button sampleButton;                     //Button prefab
	private List<ContextMenuItem> contextMenuItems; //List of context menu items
	private Vector3 position;                       //Vector3 position of the click
	private bool sideB;                             //User allegiance
	private PointerEventData click;                 //Click event data
	internal GameObject mapObject;
	internal Toggle toggleDrawArrowUI;              //Is user creating a drawing
	internal Toggle toggleDrawUI;           //Is user creating a drawing
	internal IDrawing drawing;                     //Current drawing

	private static ClickMapController instance;
	public static ClickMapController Instance {
		get { return instance; }
	}

	private Action<Image> edit;
	private Action<Image> delete;
	private Action<Image> spawn;
	private Action<Image> reset;
	private Action<Image> softReset;
	private Action<Image> spawnBase;
	private Action<Image> manageBase;
	private Action<Image> storeUnit;

	/// <summary>
	/// Method sets up actions
	/// </summary>
	void Awake() {
		instance = this;
		contextMenuItems = new List<ContextMenuItem>();
		edit = new Action<Image>(EditAction);
		delete = new Action<Image>(DeleteAction);
		spawn = new Action<Image>(SpawnAction);
		spawnBase = new Action<Image>(SpawnBaseAction);
		reset = new Action<Image>(ResetAction);
		softReset = new Action<Image>(SoftResetAction);
		manageBase = new Action<Image>(ManageBaseAction);
		storeUnit = new Action<Image>(StoreUnitAction);
		mapObject = transform.parent.gameObject;
	}

	internal void AssignDrawingButtons(Toggle arrow, Toggle drawing) {
		toggleDrawUI = drawing;
		toggleDrawArrowUI = arrow;
	}

	/// <summary>
	/// Function handles screen click
	/// </summary>
	/// <param name="eventData">Click PointeEventData</param>
	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.button == PointerEventData.InputButton.Right) {
			if (drawing != null) { drawing.IsExtending = false; toggleDrawUI.isOn = false; drawing = null; }

			click = eventData;
			contextMenuItems.Clear();

			//Checks if click is above Unit, Base or the Map, unit allegianance, user allegiance and user permissions and shows contextual menu depending on those.
			if (eventData.pointerClick.GetComponent<Unit>() != null && (eventData.pointerClick.GetComponent<Unit>().SideB == ApplicationController.isSideB || ApplicationController.isAdmin)) {
				//Unit click or admin
				sideB = eventData.pointerClick.GetComponent<Unit>().SideB;
				position = eventData.pointerCurrentRaycast.screenPosition;
				contextMenuItems.Add(new ContextMenuItem("Store", sampleButton, storeUnit));
				if (ApplicationController.isAdmin) {
					contextMenuItems.Add(new ContextMenuItem("Edit", sampleButton, edit));
					contextMenuItems.Add(new ContextMenuItem("Despawn", sampleButton, delete));
					contextMenuItems.Add(new ContextMenuItem("Soft Reset", sampleButton, softReset));
				}
				contextMenuItems.Add(new ContextMenuItem("Reset", sampleButton, reset));
			} else if (eventData.pointerClick.GetComponent<Base>() != null && (eventData.pointerClick.GetComponent<Base>().SideB == ApplicationController.isSideB || ApplicationController.isAdmin)) {
				//Base click or admin
				sideB = eventData.pointerClick.GetComponent<Base>().SideB;
				position = eventData.pointerCurrentRaycast.screenPosition;
				contextMenuItems.Add(new ContextMenuItem("Spawn", sampleButton, spawn));
				contextMenuItems.Add(new ContextMenuItem("Manage", sampleButton, manageBase));
				if (ApplicationController.isAdmin) {
					contextMenuItems.Add(new ContextMenuItem("Edit", sampleButton, edit));
					contextMenuItems.Add(new ContextMenuItem("Despawn", sampleButton, delete));
					contextMenuItems.Add(new ContextMenuItem("Reset", sampleButton, reset));
				}
			} else if (eventData.pointerClick.GetComponent<IDrawing>() != null) {
				contextMenuItems.Add(new ContextMenuItem("Delete", sampleButton, delete));
			} else {
				//Admin
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
		} else if (eventData.button == PointerEventData.InputButton.Left) {
			if (drawing == null) {
				//Creates new drawing
				if (toggleDrawArrowUI.isOn) drawing = DrawingManager.Instance.SpawnArrow(eventData.pointerCurrentRaycast.worldPosition + new Vector3(0, 0, -0.1f), false);
				else if (toggleDrawUI.isOn) drawing = DrawingManager.Instance.Spawn(eventData.pointerCurrentRaycast.worldPosition + new Vector3(0, 0, -0.1f), false);
			} else if (toggleDrawArrowUI.isOn) {
				//Ends drawing
				drawing.IsFinished = true;
				toggleDrawArrowUI.isOn = false;
				toggleDrawUI.isOn = false;
				drawing = null;
			} else if (toggleDrawUI.isOn) {
				//Starts extending line on finishing the first segment.
				drawing.IsFinished = true;
				drawing.IsExtending = true;
				drawing.I = 0;
			}
		}
	}

	private Base ReturnBase(PointerEventData eventData) {
		PointerEventData pointerEventData = new(EventSystem.current) { position = eventData.pressPosition };
		List<RaycastResult> raycastResult = new();
		EventSystem.current.RaycastAll(pointerEventData, raycastResult);
		for (int i = 0; i < raycastResult.Count; i++) {
			//Returns first base under the mouse pointer
			if (raycastResult[i].gameObject.transform.parent.GetComponent<Base>() != null) {
				return raycastResult[i].gameObject.transform.parent.GetComponent<Base>();
			}
		}
		return null;
	}

	/// <summary>
	/// Method opens Unit spawning menu.
	/// </summary>
	/// <param name="contextPanel"></param>
	private void SpawnAction(Image contextPanel) {
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
	private void SpawnBaseAction(Image contextPanel) {
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
	private void EditAction(Image contextPanel) {
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
	private void DeleteAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		if (GetComponent<IDrawing>() == null) UnitManager.Instance.Despawn(gameObject, false);
		else Destroy(gameObject);
	}

	/// <summary>
	/// Method resets the Unit or Base to its Startposition.
	/// </summary>
	/// <param name="contextPanel"></param>
	private void ResetAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		transform.position = GetComponent<IMovable>().StartPosition;
		//Saves base reseting its position.
		if (ApplicationController.isController && GetComponent<Base>() != null) {
			ApplicationController.Instance.server.SaveBases();
		}
	}

	/// <summary>
	/// Method sets the Unit or Base Start position to its current position.
	/// </summary>
	/// <param name="contextPanel"></param>
	private void SoftResetAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		GetComponent<IMovable>().StartPosition = transform.position;
		//Saves unit soft reseting its base position.
		if (ApplicationController.isController) {
			ApplicationController.Instance.server.SaveUnits();
		}
	}

	/// <summary>
	/// Method opens Base menu for stored units management.
	/// </summary>
	/// <param name="contextPanel"></param>
	private void ManageBaseAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		BaseUnitMenu baseUnitMenu = UnitManager.Instance.baseUnitMenu.GetComponent<BaseUnitMenu>();
		if (gameObject.TryGetComponent<Base>(out var unitBase)) {
			baseUnitMenu.ManagedBase = unitBase;
			UnitManager.Instance.baseUnitMenu.SetActive(true);
		}
	}

	/// <summary>
	/// Method stores unit to nearest base.
	/// </summary>
	/// <param name="contextPanel"></param>
	private void StoreUnitAction(Image contextPanel) {
		Destroy(contextPanel.gameObject);
		Unit unit = gameObject.GetComponent<Unit>();
		Base unitBase = ReturnBase(click);
		if (unitBase != null && unit != null) {
			unitBase.unitList.Add(unit);
			unit.SetVisibility(false);
		}
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