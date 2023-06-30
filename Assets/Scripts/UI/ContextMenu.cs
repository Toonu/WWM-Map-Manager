using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for mouse click Context menus.
/// </summary>
[Serializable]
public class ContextMenuItem {
	public string text;             // text to display on button
	public Button button;           // sample button prefab
	public Action<Image> action;    // delegate to method that needs to be executed when button is clicked

	public ContextMenuItem(string text, Button button, Action<Image> action) {
		this.text = text;
		this.button = button;
		this.action = action;
	}
}

public class ContextMenu : MonoBehaviour {
	public Image contentPanel;              // content panel prefab
	public Canvas canvas;                   // link to icon canvas, where will be Context Menu

	private static ContextMenu instance;    // some kind of singleton here

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

	/// <summary>
	/// Method creates the context menu at specified position with a list of Buttonns or UI elements in it.
	/// </summary>
	/// <param name="items">List<ContextMenuItemL> of UI elements</param>
	/// <param name="position">Vector3 position of the context menu</param>
	public void CreateContextMenu(List<ContextMenuItem> items, Vector3 position) {
		Image panel = Instantiate(contentPanel, position, Quaternion.identity);
		panel.transform.SetParent(canvas.transform);
		panel.transform.SetAsLastSibling();

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

	IEnumerator DelayTilEndOfFrame(Image panel) {
		yield return new WaitForEndOfFrame();
		panel.rectTransform.localScale = Vector3.one;
		panel.rectTransform.anchoredPosition = new Vector3(
			Mathf.Clamp(panel.rectTransform.anchoredPosition.x, panel.rectTransform.rect.xMax, panel.gameObject.transform.parent.GetComponent<RectTransform>().rect.width - panel.rectTransform.rect.xMax),
			Mathf.Clamp(panel.rectTransform.anchoredPosition.y, panel.rectTransform.rect.yMax, panel.gameObject.transform.parent.GetComponent<RectTransform>().rect.height - panel.rectTransform.rect.yMax), 0
		);
	}
}