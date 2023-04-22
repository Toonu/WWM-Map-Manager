using System;
using TMPro;
using UnityEngine;

internal class Dice : MonoBehaviour {
	private readonly System.Random random = new();
	public int dices = 1;
	public int sides = 6;
	public int reduce = 0;
	private UITextFloatAppender result;
	private TextMeshProUGUI resultLong;

	private void Awake() {
		result = transform.Find("Result").GetComponent<UITextFloatAppender>();
		resultLong = transform.Find("ResultLong").GetComponent<TextMeshProUGUI>();
	}

	public void UpdateDices(string dices) { this.dices = Convert.ToInt16(dices); }
	public void UpdateSides(string sides) { this.sides = Convert.ToInt16(sides); }
	public void UpdateReduce(string reduce) { this.reduce = Convert.ToInt16(reduce); }

	public void Roll() {
		int result = 0;
		resultLong.text = "Result: ";
		for (int i = 0; i < dices; i++) {
			int j = random.Next(1, sides + 1);
			result += j;
			resultLong.text += $"{j} + ";
		}
		this.result.UpdateText(result - reduce);
		resultLong.text = resultLong.text[..^2] + $"= {result} - {reduce}";
		Debug.Log("Dice rolled " + (result - reduce));
	}
}

