using TMPro;
using UnityEngine;

public class UITextFloatAppender : MonoBehaviour {
	[Tooltip("UI element containing the changeable text")]
	private TextMeshProUGUI text;
	[Tooltip("Original text of the element")]
	private string originalText = "";
	[Tooltip("Current float value")]
	public float Value = 0;
	/// <summary>
	/// Loads the text component on awake.
	/// </summary>
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