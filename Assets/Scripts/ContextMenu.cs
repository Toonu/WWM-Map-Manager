using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {
	[Serializable]
	public class ContextMenuItem {
		public Action<Image> action; // delegate to method that needs to be executed when button is clicked
		public Button button; // sample button prefab
		public string text; // text to display on button

		public ContextMenuItem(string text, Button button, Action<Image> action) {
			this.text = text;
			this.button = button;
			this.action = action;
		}
	}

	public class ContextMenu : MonoBehaviour {
		private static ContextMenu instance; // some kind of singleton here
		public Canvas canvas; // link to main canvas, where will be Context Menu
		public Image contentPanel; // content panel prefab

		public static ContextMenu Instance {
			get {
				if (instance == null) {
					instance = FindObjectOfType(typeof(ContextMenu)) as ContextMenu;
					if (instance == null) instance = new ContextMenu();
				}

				return instance;
			}
		}

		public void CreateContextMenu(List<ContextMenuItem> items, Vector3 position) {
			// here we are creating and displaying Context Menu
			position -= new Vector3(0.15f * position.x, 0.15f * position.y, 0);
			position = new Vector3(Mathf.Clamp(position.x, 75, Screen.width - 560),
				Mathf.Clamp(position.y, 50, Screen.height - 320));
			var panel = Instantiate(contentPanel, position, Quaternion.identity);
			panel.transform.SetParent(canvas.transform);
			panel.transform.SetAsLastSibling();
			panel.rectTransform.anchoredPosition = position;

			foreach (var item in items) {
				var tempReference = item;
				var button = Instantiate(item.button);
				var buttonText = button.GetComponentInChildren<TMP_Text>();
				buttonText.text = item.text;
				button.onClick.AddListener(delegate { tempReference.action(panel); });
				button.transform.SetParent(panel.transform);
			}
		}
	}
}