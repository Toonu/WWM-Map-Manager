using TMPro;
using UnityEngine;

public class UILabelTextAppender : MonoBehaviour {
	private TextMeshProUGUI textLabelUI;    //UI element containing the changeable textLabelUI.
	private string originalText = "";       //Original textLabelUI of the element.
	public string Value = "0";              //Current float value.
	/// <summary>
	/// Method loads the textLabelUI component on awake.
	/// </summary>
	public void Initiate() {
		textLabelUI = gameObject.GetComponent<TextMeshProUGUI>();
		originalText = textLabelUI.text;
	}

	/// <summary>
	/// Method updates the textLabelUI ending with a new number.
	/// </summary>
	/// <param name="replacementNumber">float value</param>
	public void UpdateText(float replacementNumber) {
		UpdateText(replacementNumber.ToString());
	}

	public void UpdateOrigin(string replacementString) {
		if (textLabelUI == null && gameObject != null) textLabelUI = gameObject.GetComponent<TextMeshProUGUI>();
		originalText = replacementString;
		textLabelUI.text = originalText + Value;
	}

	/// <summary>
	/// Method updates the textLabelUI ending with a new number.
	/// </summary>
	/// <param name="replacementNumber">float value</param>
	public void UpdateText(string replacementString) {
		if (textLabelUI == null && gameObject != null) {
			Initiate();
		}
		Value = replacementString;
		textLabelUI.text = originalText + Value;
	}
}