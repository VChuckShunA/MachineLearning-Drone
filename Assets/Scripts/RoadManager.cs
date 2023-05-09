using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// This script is used for navigation by the pedestrian AIs
/// </summary>
public class RoadManager : MonoBehaviour {
	[SerializeField] public List<Marker> pedestrianMarkers;
	[SerializeField] protected bool isCorner;
	[SerializeField] protected bool hasCrosswalks;

	private float appropriateThresholdCorner = 0.265f;


	/// <summary>
	/// Gets position for pedestrian to spawn
	/// </summary>
	/// <param name="structurePosition"></param>
	/// <returns></returns>
	public virtual Marker GetPositionForPedestrianToSpawn(Vector3 structurePosition) {
		return GetClosestMarkerTo(structurePosition, pedestrianMarkers);

	}

	/// <summary>
	/// Each Road prefab comes with a pedestrian marker. When each road prefab is instantiated, this function is 
	/// called and the pedestrian markers are added to the list
	/// </summary>
	/// <param name="PedesMarkers"></param>
	public void PopulatePedestrianMarkers(List<Marker> PedesMarkers) {

		//foreach(Marker p in PedesMarkers)
		//{pedestrianMarkers.Add(p);}
		pedestrianMarkers = PedesMarkers;

		//Debug.Log("pedestrian markers " + pedestrianMarkers.Count);
	}

	/// <summary>
	/// This function returns the closest pedestrian marker
	/// </summary>
	/// <param name="structurePosition"></param>
	/// <param name="pedestrianMarkers"></param>
	/// <returns></returns>
	private Marker GetClosestMarkerTo(Vector3 structurePosition, List<Marker> pedestrianMarkers) {
		if (isCorner) {
			foreach (var marker in pedestrianMarkers) {
				var direction = marker.Position - structurePosition;
				direction.Normalize();
				if (Mathf.Abs(direction.x) < appropriateThresholdCorner || Mathf.Abs(direction.z) < appropriateThresholdCorner) {
					return marker;
				}
			}

			return null;
		} else {
			Marker closestMarker = null;
			float distance = float.MaxValue;
			foreach (var marker in pedestrianMarkers) {
				var markerDistance = Vector3.Distance(structurePosition, marker.Position);
				if (distance > markerDistance) {
					distance = markerDistance;
					closestMarker = marker;
				}
			}

			return closestMarker;
		}
	}

	/// <summary>
	/// Returns closest pedestrian postion to a given position
	/// </summary>
	/// <param name="currentPosition"></param>
	/// <returns></returns>
	public Vector3 GetClosestPedestrianPositionTo(Vector3 currentPosition) {
		return GetClosestMarkerTo(currentPosition, pedestrianMarkers).Position;
	}

	/// <summary>
	/// Returns all pedestrian markers
	/// </summary>
	/// <returns></returns>
	public List<Marker> GetAllPedestrianMarkers() {
		return pedestrianMarkers;
	}
}
