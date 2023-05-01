using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;

/// <summary>
/// This scrip is for generating trees using LSystems
/// </summary>
public class TransformInfo {
	public Vector3 position;
	public Quaternion rotation;
}


public class LSystemScript : MonoBehaviour {
	[SerializeField] private float iterations;
	[SerializeField] private GameObject branch;
	[SerializeField] private GameObject leaf;
	[SerializeField] private float length;
	[SerializeField] private float angle;
	private float rotationAngle;
	private Vector3 initialPosition;

	private const string axiom = "X";

	private Stack<TransformInfo> transformStack;
	private Dictionary<char, string> rules;
	private string currentString = string.Empty;
	void Start() {
		transformStack = new Stack<TransformInfo>();
		angle = UnityEngine.Random.Range(10, 60);
		iterations = UnityEngine.Random.Range(3, 5);
		rotationAngle = UnityEngine.Random.Range(0, 90);
		length = UnityEngine.Random.Range(0.01f, 0.1f);
		rules = new Dictionary<char, string> {
			{ 'X',"[F-[[X]+X]+F[+FX]-X]" },
             //{ 'X', "[FX[+F[-FX]FX][-F-FXFX]]" }, //This generates interesting shrubs
            { 'F',"FF"}
		};

		Generate();
		//ReplaceWithMesh(); //This slows the whole thing down for some reason

	}

	private void Generate() {
		currentString = axiom;
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < iterations; i++) {
			//loop through current string and create a new string based on the rules
			foreach (char c in currentString) {
				sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
			}

			//Set currentString to the new string we just generated.
			currentString = sb.ToString();
			sb = new StringBuilder();
		}




		//foreach (char c in currentString)
		for (int c = 0; c < currentString.Length; c++) {
			switch (currentString[c]) {
				case 'F':
					//Draw a straight line
					initialPosition = transform.position;
					transform.Translate(Vector3.up * length);
					GameObject treeSegment;
					if (currentString[(c + 1) % currentString.Length] == 'X' ||
						currentString[(c + 3) % currentString.Length] == 'F' && currentString[(c + 4) % currentString.Length] == 'X') {
						treeSegment = Instantiate(leaf);
					} else {
						treeSegment = Instantiate(branch);
						transform.Rotate(0, rotationAngle, 0);
					}
					treeSegment.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
					treeSegment.GetComponent<LineRenderer>().SetPosition(1, transform.position);
					break;

				case 'X':
					//does nothing, generate more Fs
					break;

				case '+':
					//Rotates clockwise
					transform.Rotate(Vector3.back * angle);
					break;

				case '-':
					//Rotates counter-clockwise
					transform.Rotate(Vector3.forward * angle);
					break;

				case '[':
					//Save current transform info
					transformStack.Push(new TransformInfo() {
						position = transform.position,
						rotation = transform.rotation
					});
					break;

				case ']':
					//Return to our previously saved transform info
					TransformInfo ti = transformStack.Pop();
					transform.position = ti.position;
					transform.rotation = ti.rotation;
					break;

				default:
					throw new InvalidOperationException("Invalid L-tree operation");
			}
		}
	}
}
