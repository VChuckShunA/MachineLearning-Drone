using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This script was an attempt to implement vehicles into the LSystem, but it wasn't fully implemented
/// due to an obstacle in the AIs
/// </summary>
public class CarSpawner : MonoBehaviour {
	public GameObject[] carPrefabs;

	private void Start() {
		Instantiate(SelectACarPrefab(), transform);
	}

	private GameObject SelectACarPrefab() {
		var randomIndex = Random.Range(0, carPrefabs.Length);
		return carPrefabs[randomIndex];
	}

}
