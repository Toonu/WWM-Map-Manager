using System;
using TMPro;
using UnityEngine;

internal class Dice : MonoBehaviour {
	private readonly System.Random random = new();
	public int dices = 1;					//Amount of dices
	public int sides = 6;					//Amount of sides in dice
	public int reduce = 0;					//Amount to reduce from a roll result
	private UITextFloatAppender result;		//UI Label element containing the result textLabelUI.
	private TextMeshProUGUI resultLong;		//UI Label element containing the result formula textLabelUI.

	/// <summary>
	/// Method assigns Components on startup.
	/// </summary>
	private void Awake() {
		result = transform.Find("Result").GetComponent<UITextFloatAppender>();
		resultLong = transform.Find("ResultLong").GetComponent<TextMeshProUGUI>();
	}

	//Setters for the dice Attributes.
	public void UpdateDices(string dices) { int.TryParse(dices, out this.dices); }
	public void UpdateSides(string sides) { int.TryParse(sides, out this.sides); }
	public void UpdateReduce(string reduce) { int.TryParse(reduce, out this.reduce); }

	/// <summary>
	/// Method rolls the dice utilising its sides, dices and reduce attributes.
	/// Dice adds up individual throws of each dice up to each dice sides size and reduces the result at the end.
	/// </summary>
	public void Roll() {
		int result = 0;
		resultLong.text = "Result: ";
		//Roll each dice and add up the result.
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

