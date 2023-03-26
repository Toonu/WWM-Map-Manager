using System.Collections;
using UnityEngine;

public class Popup : MonoBehaviour {
	public void PopUp(float duration = 1.75f) {
		gameObject.SetActive(true);
		StartCoroutine(Begone(duration));
	}

	private IEnumerator Begone(float duration) {
		yield return new WaitForSeconds(duration);
		gameObject.SetActive(false);
	}
}
