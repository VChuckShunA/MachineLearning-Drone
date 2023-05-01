using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class fetches information regarding the individual structures
/// </summary>
public class StructureModel : MonoBehaviour {
	float yHeight = 0;
	public Vector3Int RoadPosition { get; set; }
	private RoadManager roadManager;
	public GameObject _placementManager;
	private void Start() {
		_placementManager = GameObject.Find("PlacementManager");
	}

	/// <summary>
	/// Returns the closest marker to a specific position
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	public Vector3 GetNearestMarkerTo(Vector3 position) {
		return gameObject.GetComponent<RoadManager>().GetClosestPedestrianPositionTo(position);
	}

	/// <summary>
	/// Returns pedestrian spawn markers at a specific position
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	public Marker GetPedestrianSpawnMarker(Vector3 position) {
		return gameObject.GetComponent<RoadManager>().GetPositionForPedestrianToSpawn(position);
	}

	/// <summary>
	/// Returns a list of pedestrian markers
	/// </summary>
	/// <returns></returns>
	public List<Marker> GetPedestrianMarkers() {
		// var glos = GameObject.FindGameObjectsWithTag("PedestrianMarker");
		return gameObject.GetComponent<RoadManager>().GetAllPedestrianMarkers();
	}
}
