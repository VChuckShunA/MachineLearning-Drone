using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is responsible for handling AI movement
/// </summary>

[RequireComponent(typeof(Animator))]
public class AiAgent : MonoBehaviour {
	public event Action OnDeath;

	Animator animator;
	public float speed = 0.2f;
	public float rotationSpeed = 10f;

	List<Vector3> pathToGo = new List<Vector3>();
	bool moveFlag = false;
	int index = 0;
	Vector3 endPosition;

	/// <summary>
	/// These variables are declared within Initialize()
	/// </summary>
	/// <param name="path"></param>
	public void Initialize(List<Vector3> path) {
		pathToGo = path;
		Debug.Log("path:" + path);
		index = 1;
		moveFlag = true;//When this is declared it signals the  PerformMovement()
		endPosition = pathToGo[index]; //Move from current position
		animator = GetComponent<Animator>();
		animator.SetTrigger("Walk");

	}

	private void Update() {
		if (moveFlag) {
			PerformMovement();
		}
	}

	/// <summary>
	/// This function constantly checks if the AI is elligible to move, and destroys the AI gameobject once
	/// it reaches its final destination.
	/// </summary>
	private void PerformMovement() {
		if (pathToGo.Count >= index) {
			float distanceToGo = MoveTheAgent();
			// Debug.Log("Distance to go"+distanceToGo);
			if (distanceToGo < 0.05f) {
				index++;
				// Debug.Log("Index is increasing");
				if (index >= pathToGo.Count) {
					moveFlag = false;
					Destroy(gameObject);
					return;
				}
				endPosition = pathToGo[index];
			}

			//Debug.Log("Didn't Increase the index");
		}
	}

	/// <summary>
	/// This function is responsible actually AI movement.
	/// </summary>
	/// <returns></returns>
	private float MoveTheAgent() {
		float step = speed * Time.deltaTime;
		Vector3 endPositionCorrect = new Vector3(endPosition.x, transform.position.y, endPosition.z);
		transform.position = Vector3.MoveTowards(transform.position, endPositionCorrect, step);

		var lookDirection = endPositionCorrect - transform.position;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * rotationSpeed);
		//Debug.Log("Moved the agent");

		//Debug.Log("Transform.position:"+ transform.position);
		//Debug.Log("End Position:" + endPosition);
		return Vector3.Distance(transform.position, endPositionCorrect);
	}

	private void OnDestroy() {
		OnDeath?.Invoke();
	}
}
