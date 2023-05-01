using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{

	public float moveSpeed = 5f;
	public float verticalSpeed = 5f;
	public Transform cameraTransform;
	public float cameraDistance = 5f;
	public float cameraHeight = 2f;
	public float cameraSmoothness = 5f;
	public float zoomSpeed = 5f;
	public float zoomMin = -5f;
	public float zoomMax = 2f;


	private Vector3 cameraOffset;

	void Start() {
		cameraOffset = new Vector3(0f, cameraHeight, -cameraDistance);
	}

	void Update() {
		// Move forward and backward
		if (Input.GetKey(KeyCode.S)) {
			transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
		} else if (Input.GetKey(KeyCode.W)) {
			transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
		}

		// Move left and right
		if (Input.GetKey(KeyCode.D)) {
			transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
		} else if (Input.GetKey(KeyCode.A)) {
			transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
		}

		// Move up and down
		if (Input.GetKey(KeyCode.Space)) {
			transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime);
		} else if (Input.GetKey(KeyCode.V)) {
			transform.Translate(Vector3.down * verticalSpeed * Time.deltaTime);
		}

		// Zoom in and out
		float zoomAmount = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
		cameraDistance = Mathf.Clamp(cameraDistance - zoomAmount, zoomMin, zoomMax);
	}

	void LateUpdate() {
		// Set the camera's position and rotation to follow the drone
		Vector3 cameraDirection = Quaternion.Euler(45f, 0f, 0f) * Vector3.back;
		Vector3 targetOffset = cameraDirection * cameraDistance + Vector3.up * cameraHeight;
		Vector3 targetPosition = transform.position + transform.TransformDirection(targetOffset);
		cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * cameraSmoothness);
		cameraTransform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position, Vector3.up);
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag != "DropZone"|| other.gameObject.tag != "Package" || other.gameObject.tag != "Spawner") {
			Debug.Log("Hit");
		}
		}
}
