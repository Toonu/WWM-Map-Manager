using TMPro;
using UnityEngine;

public class UITextFloatAppender : MonoBehaviour {
	private TextMeshProUGUI text;
	private string originalText = "";
	public float Value = 0;
	//Loads the text component on awake.
	public void Initiate() {
		text = gameObject.GetComponent<TextMeshProUGUI>();
		originalText = text.text;
	}

	/// <summary>
	/// Updates the text ending with a new number.
	/// </summary>
	/// <param name="replacementNumber">float value</param>
	public void UpdateText(float replacementNumber) {
		if (text == null) {
			Initiate();
		}
		Value = replacementNumber;
		text.text = originalText + Value;
	}
}