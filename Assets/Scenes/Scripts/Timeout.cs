using System.Collections;
using UnityEngine;

public class Timeout : MonoBehaviour {
	public void PopUp() {
		gameObject.SetActive(true);
		StartCoroutine(Begone());
	}

	private IEnumerator Begone() {
		yield return new WaitForSeconds(1.75f);
		gameObject.SetActive(false);
	}
}
