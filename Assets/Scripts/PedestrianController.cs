using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script sets a pedestrian as the camera's follow target when clicked on
/// It needs some tweaking
/// </summary>
public class PedestrianController : MonoBehaviour {
	private Color startcolor = Color.blue;

	public void OnMouseDown() {
		CameraController.instance.followTransform = transform;
		startcolor = GetComponent<Renderer>().material.color;
		GetComponent<Renderer>().material.color = Color.yellow;
	}
}
