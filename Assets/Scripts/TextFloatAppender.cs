using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class TextFloatAppender : MonoBehaviour {
	private TextMeshProUGUI text;
	// Use this for initialization
	void Awake() {
		text = GetComponent<TextMeshProUGUI>();
	}

	public void UpdateText(float replacementNumber) {
		try {
			text.text = string.Join(" ", text.text.Split(' ').Take(text.text.Split(' ').Length - 1).ToArray()) + " " +replacementNumber;
		} catch (System.NullReferenceException) { }
	}
}

