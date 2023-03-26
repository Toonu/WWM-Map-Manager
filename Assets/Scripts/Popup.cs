using System.Collections;
using UnityEngine;
using TMPro;

public class Popup : MonoBehaviour {
	public TextMeshProUGUI UI;

	public void PopUp(string title = "Saved!", float duration = 1.75f) {
		UI.text = title;
		gameObject.SetActive(true);
		StartCoroutine(Begone(duration));
	}

	private IEnumerator Begone(float duration) {
		yield return new WaitForSeconds(duration);
		gameObject.SetActive(false);
	}
}
