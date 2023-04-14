using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class TextFloatAppender : MonoBehaviour {
	private TextMeshProUGUI text;
	//Loads the text component on awake.
	void Awake() {
		text = GetComponent<TextMeshProUGUI>();
	}

	/// <summary>
	/// Updates the text ending with a new number.
	/// </summary>
	/// <param name="replacementNumber">float value</param>
	public void UpdateText(float replacementNumber) {
		try {
			text.text = string.Join(" ", text.text.Split(' ').Take(text.text.Split(' ').Length - 1).ToArray()) + " " + replacementNumber;
		} catch (System.NullReferenceException) { }
	}
}