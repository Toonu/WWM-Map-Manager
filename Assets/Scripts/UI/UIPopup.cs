using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup : MonoBehaviour {
	private GameObject popupObject;
	private TextMeshProUGUI textLabelUI; //Popup UI for the text label.
	private Button acceptButton;
	private Button cancelButton;
	internal TMP_InputField inputField;

	/// <summary>
	/// Method sets up the Components on startup and switches the popup off.
	/// </summary>
	void Awake() {
		popupObject = transform.GetChild(0).gameObject;

		textLabelUI = popupObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		inputField = popupObject.transform.GetChild(1).GetComponent<TMP_InputField>();
		acceptButton = popupObject.transform.GetChild(2).GetChild(0).GetComponent<Button>();
		cancelButton = popupObject.transform.GetChild(2).GetChild(2).GetComponent<Button>();

		acceptButton.onClick.RemoveAllListeners();
		cancelButton.onClick.RemoveAllListeners();
		acceptButton.onClick.AddListener(() => {
			popupObject.SetActive(false);
			HideElements();
		});
		cancelButton.onClick.AddListener(() => {
			popupObject.SetActive(false);
			HideElements();
		});

		HideElements();
	}

	/// <summary>
	/// Method pops up the Popup for the specified duration with textLabelUI.
	/// </summary>
	/// <param name="title">String textLabelUI to show</param>
	/// <param name="duration">Float time duration in seconds</param>
	public void PopUp(string title = "Error!", float duration = 1.75f, bool background = false) {
		textLabelUI.text = title;
		popupObject.GetComponent<Image>().enabled = background;
		popupObject.SetActive(true);
		StartCoroutine(Begone(duration));
	}


	/// <summary>
	/// Method pops up the Popup for the specified duration with textLabelUI.
	/// </summary>
	/// <param name="title">String textLabelUI to show</param>
	/// <param name="duration">Float time duration in seconds</param>
	public void PopUpSticky(Action yesEvent, string title = "Error!", bool input = false) {
		textLabelUI.text = title;
		acceptButton.gameObject.SetActive(true);
		cancelButton.gameObject.SetActive(true);
		inputField.gameObject.SetActive(input);
		if (acceptButton != null) acceptButton.onClick.AddListener(() => { yesEvent(); });
		popupObject.SetActive(true);
		if (acceptButton != null) acceptButton.onClick.RemoveListener(() => { yesEvent(); });
	}

	/// <summary>
	/// Method starts new coroutine for the duration. Used for non-async version of the Popup.
	/// </summary>
	/// <param name="duration">Float time duration in seconds</param>
	/// <returns></returns>
	private IEnumerator Begone(float duration) {
		yield return new WaitForSeconds(duration);
		popupObject.SetActive(false);
		popupObject.GetComponent<Image>().enabled = true;
	}

	/// <summary>
	/// Hides buttons and input.
	/// </summary>
	private void HideElements() {
		acceptButton.gameObject.SetActive(false);
		cancelButton.gameObject.SetActive(false);
		inputField.gameObject.SetActive(false);
	}
}
