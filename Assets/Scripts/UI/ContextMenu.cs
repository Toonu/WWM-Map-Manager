using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class containing ContextMenu button data.
/// </summary>
[Serializable]
public class ContextMenuItem {
	public string text;             // TextLabelUI to display on button
	public Button button;           // Sample button prefab
	public Action<Image> action;    // Delegate to method that needs to be executed when button is clicked

	public ContextMenuItem(string text, Button button, Action<Image> action) {
		this.text = text;
		this.button = button;
		this.action = action;
	}
}

/// <summary>
/// Class for mouse click Context menus and main UI buttons.
/// </summary>
public class ContextMenu : MonoBehaviour {
	#region Attributes

	public Image contextMenuTemplate;              // Content panel prefab
	private Canvas canvas;                   // Link to icon canvas, where will be Context Menu
	private static ContextMenu instance;

	public static ContextMenu Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType(typeof(ContextMenu)) as ContextMenu;
				if (instance == null) {
#pragma warning disable UNT0010 // Component instance creation
					instance = new ContextMenu();
#pragma warning restore UNT0010 // Component instance creation
				}
			}
			return instance;
		}
	}

	private Button buttonUITurn;
	private Button buttonUISave;
	private Button buttonUIYield;
	private Button buttonUIExit;
	private Toggle toggleDrawArrow;
	private Toggle toggleDraw;
	private UILabelTextAppender pointsLabel; //Team points UI element
	private UILabelTextAppender turnLabelUI; //Turn label UI element

	#endregion

	/// <summary>
	/// Loading UI elements on awake.
	/// </summary>
	private void Awake() {
		//Assign main buttons actions
		SheetSync sheetSync = transform.parent.GetChild(0).gameObject.GetComponent<SheetSync>();
		canvas = transform.GetComponent<Canvas>();

		pointsLabel = transform.GetChild(1).GetChild(2).GetComponent<UILabelTextAppender>();
		turnLabelUI = transform.GetChild(1).GetChild(3).GetComponent<UILabelTextAppender>();

		ClickMapController cc = transform.parent.GetChild(1).GetChild(0).gameObject.GetComponent<ClickMapController>();
		

		toggleDrawArrow = transform.GetChild(0).GetChild(1).GetComponent<Toggle>();
		toggleDrawArrow.onValueChanged.RemoveAllListeners();
		toggleDrawArrow.onValueChanged.AddListener(delegate {
			Color colour;
			if (toggleDrawArrow.isOn) ColorUtility.TryParseHtmlString("#7783E3", out colour);
			else ColorUtility.TryParseHtmlString("#FFFFFF", out colour);
			toggleDrawArrow.image.color = colour;
		});
		toggleDraw = transform.GetChild(0).GetChild(2).GetComponent<Toggle>();
		toggleDraw.onValueChanged.RemoveAllListeners();
		toggleDraw.onValueChanged.AddListener(delegate {
			Color colour;
			if (toggleDraw.isOn) ColorUtility.TryParseHtmlString("#7783E3", out colour);
			else ColorUtility.TryParseHtmlString("#FFFFFF", out colour);
			toggleDraw.image.color = colour;
		});

		cc.AssignDrawingButtons(toggleDrawArrow, toggleDraw);

		buttonUITurn = transform.GetChild(2).GetComponent<Button>();
		buttonUITurn.onClick.RemoveAllListeners();
		buttonUITurn.onClick.AddListener(() => { ApplicationController.generalPopup.PopUpSticky(() => { sheetSync.FinishTurn(); }, "Are you sure to finish your turn?", true); });
		buttonUISave = transform.GetChild(4).GetComponent<Button>();
		buttonUISave.onClick.RemoveAllListeners();
		buttonUISave.onClick.AddListener(() => { ApplicationController.generalPopup.PopUpSticky(() => { sheetSync.SaveSheet(); }, "Really save to the server?"); });
		buttonUIYield = transform.GetChild(5).GetComponent<Button>();
		buttonUIYield.onClick.RemoveAllListeners();
		buttonUIYield.onClick.AddListener(() => { ApplicationController.generalPopup.PopUpSticky(() => { _ = sheetSync.CheckController(); }, "Yield/take controller to/from other players?"); });
		buttonUIExit = transform.GetChild(7).GetComponent<Button>();
		buttonUIExit.onClick.RemoveAllListeners();
		buttonUIExit.onClick.AddListener(() => { ApplicationController.generalPopup.PopUpSticky(() => { _ = ApplicationController.ExitApplication(); }, "Any unsaved changes would be discarded, exit?"); });
	}

	/// <summary>
	/// Method creates the context menu at specified position with a list of Buttonns or UI elements in it.
	/// </summary>
	/// <param name="items">List<ContextMenuItemL> of UI elements</param>
	/// <param name="position">Vector3 position of the context menu</param>
	public void CreateContextMenu(List<ContextMenuItem> items, Vector3 position) {
		Image panel = Instantiate(contextMenuTemplate, position, Quaternion.identity);
		panel.transform.SetParent(canvas.transform);
		panel.transform.SetAsLastSibling();

		//Looping through the list of items and creating buttons for each of them.
		foreach (var item in items) {
			ContextMenuItem tempReference = item;
			Button button = Instantiate(item.button);
			TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
			buttonText.text = item.text;
			button.GetComponent<LayoutElement>().minHeight = Screen.height / 16;
			button.GetComponent<LayoutElement>().minWidth = Screen.width / 8;
			button.onClick.AddListener(delegate { tempReference.action(panel); });
			button.transform.SetParent(panel.transform);
		}
		StartCoroutine(DelayTilEndOfFrame(panel));
	}

	/// <summary>
	/// Method checks position against the program resolution and adjusts it if needed.
	/// </summary>
	/// <param name="panel">Adjusted Image Panel for knowing its size</param>
	/// <returns></returns>
	private IEnumerator DelayTilEndOfFrame(Image panel) {
		yield return new WaitForEndOfFrame();
		panel.rectTransform.localScale = Vector3.one;
		panel.rectTransform.anchoredPosition = new Vector3(
			Mathf.Clamp(panel.rectTransform.anchoredPosition.x, panel.rectTransform.rect.xMax, panel.gameObject.transform.parent.GetComponent<RectTransform>().rect.width - panel.rectTransform.rect.xMax),
			Mathf.Clamp(panel.rectTransform.anchoredPosition.y, panel.rectTransform.rect.yMax, panel.gameObject.transform.parent.GetComponent<RectTransform>().rect.height - panel.rectTransform.rect.yMax), 0
		);
	}

	/// <summary>
	/// Updates main buttons depending on the permissions level.
	/// </summary>
	public void UpdateControllerButtons() {
		buttonUISave.interactable = ApplicationController.isController;
		buttonUITurn.interactable = ApplicationController.isController;
		buttonUIYield.GetComponentInChildren<TMP_Text>().text = ApplicationController.isController ? "  Yield control" : "  Take control";
		pointsLabel.UpdateText(ApplicationController.isSideB ? SheetSync.pointsB : SheetSync.pointsA);
		turnLabelUI.UpdateText(SheetSync.turn);
	}
}