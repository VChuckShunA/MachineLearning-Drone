using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public GameObject packagePrefab; // The prefab of the package to spawn
	public float respawnTime = 60f; // The time in seconds before the next package is spawned

	private GameObject currentPackage; // The currently spawned package

	private Vector3 startingPosition;
	private Quaternion startingRotation;
	private bool isPackageMoved = false;
	// Start is called before the first frame update
	void Start() {
		SpawnPackage();
	}

	private void Update() {
		// Check if the current package has been moved
		if (currentPackage != null && !isPackageMoved && Vector3.Distance(currentPackage.transform.position, startingPosition) > 0.1f) {
			isPackageMoved = true;
			Invoke("SpawnPackage", respawnTime);
		}
	}


	// Spawn the package
	void SpawnPackage() {
		isPackageMoved = false;
		currentPackage = Instantiate(packagePrefab, transform.position + Vector3.up * 0.05f, Quaternion.Euler(-90f, 90f, 0f));
	}

}
