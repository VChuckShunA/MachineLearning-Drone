using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class DroneController : Agent
{

	public float moveSpeed = 1f;
	public float verticalSpeed = 1f;
	public Transform cameraTransform;
	public float cameraDistance = 5f;
	public float cameraHeight = 2f;
	public float cameraSmoothness = 5f;
	public float zoomSpeed = 5f;
	public float zoomMin = -5f;
	public float zoomMax = 2f;
	public GameObject AttachPoint;

	private Vector3 cameraOffset;
	// Initialize closest object and distance
	GameObject closestPackage = null;
	GameObject closestDropzone = null;
	float distanceToClosestPackage = Mathf.Infinity;
	float distanceToClosestDropZone = Mathf.Infinity;

	void Start() {
		cameraOffset = new Vector3(0f, cameraHeight, -cameraDistance);
	}

	void Update() {
		

		//speed up
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			moveSpeed = 5f;
			verticalSpeed = 5f;
		} else {
			moveSpeed = 0.1f;
			verticalSpeed = 5f;
		}
		// Zoom in and out
		float zoomAmount = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
		cameraDistance = Mathf.Clamp(cameraDistance - zoomAmount, zoomMin, zoomMax);

		// Find all game objects with the target tag
		GameObject[] packages = GameObject.FindGameObjectsWithTag("Package");
		GameObject[] dropzones = GameObject.FindGameObjectsWithTag("DropZone");

	

		// Iterate through each target
		foreach (GameObject package in packages) {
			if (package.GetComponent<Package>().delivered != true) {
				// Calculate distance between this object and target
				float distanceToPackage = Vector3.Distance(transform.position, package.transform.position);

				// Update closest object if distance is shorter than current closest distance
				if (distanceToPackage < distanceToClosestPackage) {
					closestPackage = package;
					distanceToClosestPackage = distanceToPackage;
				}
			}
			
		}

		// Iterate through each target
		foreach (GameObject dropzone in dropzones) {
			if (dropzone.GetComponent<DropZone>().hasPackage != true) {
				// Calculate distetweeance bn this object and target
				float distanceToDropZone = Vector3.Distance(transform.position, dropzone.transform.position);

				// Update closest object if distance is shorter than current closest distance
				if (distanceToDropZone < distanceToClosestDropZone) {
					closestDropzone = dropzone;
					distanceToClosestDropZone = distanceToDropZone;
				}
			}

		}

		// Check if closest object was found
		if (closestPackage != null) {
			//Debug.Log("Closest Package is: " + closestPackage.name);
		}

		if (closestDropzone != null) {
			//Debug.Log("Closest DroZone is: " + closestDropzone.name);
		}
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
		if (other.gameObject.tag == "Package" || other.gameObject.tag == "Bounds"
			 ) {
			return;
			//Debug.Log(other.tag);
		} else if (other.gameObject.tag == "Spawner" || other.gameObject.tag == "DropZone") {
			AddReward(1f);
		} 
		else{
			Debug.Log("Hit");
			Debug.Log(other.tag);
			AddReward(-1f);
			EndEpisode();
		}
		}
	private void OnTriggerExit(Collider other) {
		if (other.gameObject.GetComponent<OutOfBounds>()) {
			Debug.Log("Drone has exited the box collider!");
			AddReward(-1f);
		}
	}

	public override void CollectObservations(VectorSensor sensor) {
		//does the closest pickup point contain a  package?
		sensor.AddObservation(closestPackage.GetComponent<Package>().delivered?1:0);
		//does the closest Dropzone contain a  package?
		sensor.AddObservation(closestDropzone.GetComponent<DropZone>().hasPackage ? 1 : 0);
		//distance to closest package & drop zone
		sensor.AddObservation(distanceToClosestPackage);
		sensor.AddObservation(distanceToClosestDropZone);
		//Does the drone have the package
		sensor.AddObservation(AttachPoint.GetComponent<Packageattach>().hasPackage);

	
	}

	public override void OnActionReceived(ActionBuffers actions) {
		float moveX = actions.ContinuousActions[0]; 
		float moveY = actions.ContinuousActions[1];
		float moveZ = actions.ContinuousActions[2];

		transform.position+=new Vector3(moveX, moveY, moveZ)*Time.deltaTime*moveSpeed;

	}

	public override void OnEpisodeBegin() {
		transform.localPosition = new Vector3(500,1,500);
		Destroy(AttachPoint.GetComponentInChildren<Package>());
		AttachPoint.GetComponent<Packageattach>().hasPackage = false;
	}
	public override void Heuristic(in ActionBuffers actionsOut) {
		ActionSegment<float> continousActions = actionsOut.ContinuousActions;
		continousActions[0] = Input.GetAxisRaw("Horizontal");
		continousActions[1] = Input.GetAxisRaw("YAxis");
		continousActions[2] = Input.GetAxisRaw("ZAxis");

	}
	
	

}
