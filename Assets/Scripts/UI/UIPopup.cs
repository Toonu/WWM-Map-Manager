using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UIPopup : MonoBehaviour {
	private TextMeshProUGUI textUI;

	void Awake() {
		textUI = GetComponent<TextMeshProUGUI>();
		gameObject.SetActive(false);
	}

	public void PopUp(string title = "Saved!", float duration = 1.75f) {
		textUI.text = title;
		gameObject.SetActive(true);
		StartCoroutine(Begone(duration));
	}

	public async Task PopUpAsync(string title = "Saved!", float duration = 1.75f) {
		textUI.text = title;
		gameObject.SetActive(true);
		await Task.Delay(Convert.ToInt16(duration) * 1000);
		gameObject.SetActive(false);
	}

	private IEnumerator Begone(float duration) {
		yield return new WaitForSeconds(duration);
		gameObject.SetActive(false);
	}
}
