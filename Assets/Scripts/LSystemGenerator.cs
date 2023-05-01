using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

/// <summary>
/// This scrupt is used to Generate a town using LSystems. 
/// It should not be confused with the script used to generate Trees
/// </summary>
public class LSystemGenerator : MonoBehaviour {
	public Rule[] rules;
	public string rootSentence;
	[Range(0, 10)]
	public int iterationLimit = 1;


	public bool randomIgnoreRuleModifier = true;
	[Range(0, 1)]
	public float chanceToIgnoreRule = 0.3f;
	private void Start() {
		Debug.Log(GenerateSentence());
	}

	/// <summary>
	/// Generates a sentence to base the L System upon
	/// </summary>
	/// <param name="word"></param>
	/// <returns></returns>
	public string GenerateSentence(string word = null) {
		if (word == null) {
			word = rootSentence;
		}
		return GrowRecursive(word);
	}

	/// <summary>
	/// Grows Recursiverly based on the word
	/// </summary>
	/// <param name="word"></param>
	/// <param name="iterationIndex"></param>
	/// <returns></returns>
	private string GrowRecursive(string word, int iterationIndex = 0) {
		if (iterationIndex >= iterationLimit) {
			return word;
		}

		StringBuilder newWord = new StringBuilder();

		foreach (var c in word) {
			newWord.Append(c);
			ProcessRulesRecursively(newWord, c, iterationIndex);
		}

		return newWord.ToString();
	}

	/// <summary>
	/// Processes rules recursively
	/// </summary>
	/// <param name="newWord"></param>
	/// <param name="c"></param>
	/// <param name="iterationIndex"></param>
	private void ProcessRulesRecursively(StringBuilder newWord, char c, int iterationIndex) {
		foreach (var rule in rules) {
			if (rule.letter == c.ToString()) {
				if (randomIgnoreRuleModifier && iterationIndex > 1) {
					if (Random.value < chanceToIgnoreRule) {
						return;
					}
				}
				newWord.AppendLine(GrowRecursive(rule.GetResult(), iterationIndex + 1));
			}
		}
	}

}
