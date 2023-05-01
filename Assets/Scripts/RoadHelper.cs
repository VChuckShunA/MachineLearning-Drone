using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This script is used to set up Roads along side the path laid out by our LSystem
/// </summary>
public class RoadHelper : MonoBehaviour {
	public GameObject roadStraight, roadCorner, road3way, road4way, roadEnd;
	Dictionary<Vector3Int, GameObject> roadDictionary = new Dictionary<Vector3Int, GameObject>();
	HashSet<Vector3Int> fixRoadCandidates = new HashSet<Vector3Int>();

	/// <summary>
	/// Returns road positions
	/// </summary>
	/// <returns></returns>
	public List<Vector3Int> GetRoadPositions() {
		return roadDictionary.Keys.ToList();
	}

	/// <summary>
	/// This is used to place straight roads along a path when given the parameters
	/// </summary>
	/// <param name="startPosition"></param>
	/// <param name="direction"></param>
	/// <param name="length"></param>
	public void PlaceStreetPositions(Vector3 startPosition, Vector3Int direction, int length) {
		var rotation = Quaternion.identity;
		if (direction.x == 0) {
			rotation = Quaternion.Euler(0, 90, 0);
		}
		for (int i = 0; i < length; i++) {
			var position = Vector3Int.RoundToInt(startPosition + direction * i);
			if (roadDictionary.ContainsKey(position)) {
				continue;
			}
			var road = Instantiate(roadStraight, position, rotation, transform);
			road.name = road.name + " " + i;
			roadDictionary.Add(position, road);
			if (i == 0 || i == length - 1) {
				fixRoadCandidates.Add(position);
			}
		}


	}

	/// <summary>
	/// This function fixes up the instantiated straight roads by connecting them 
	/// using corners, road3ways and road4ways
	/// </summary>
	public void FixRoad() {
		foreach (var position in fixRoadCandidates) {
			List<Direction> neighbourDirections = PlacementHelper.FindNeighbour(position, roadDictionary.Keys);
			Quaternion rotation = Quaternion.identity;

			if (neighbourDirections.Count == 1) {
				Destroy(roadDictionary[position]);
				if (neighbourDirections.Contains(Direction.Down)) {
					rotation = Quaternion.Euler(0, 90, 0);
				} else if (neighbourDirections.Contains(Direction.Left)) {
					rotation = Quaternion.Euler(0, 180, 0);
				} else if (neighbourDirections.Contains(Direction.Up)) {
					rotation = Quaternion.Euler(0, -90, 0);
				}
				roadDictionary[position] = Instantiate(roadEnd, position, rotation, transform);

			} else if (neighbourDirections.Count == 2) {
				if (neighbourDirections.Contains(Direction.Up) && neighbourDirections.Contains(Direction.Down)
					|| neighbourDirections.Contains(Direction.Right) && neighbourDirections.Contains(Direction.Left)) {
					continue;
				}
				Destroy(roadDictionary[position]);
				if (neighbourDirections.Contains(Direction.Up) && neighbourDirections.Contains(Direction.Right)) {
					rotation = Quaternion.Euler(0, 90, 0);
				} else if (neighbourDirections.Contains(Direction.Right) && neighbourDirections.Contains(Direction.Down)) {
					rotation = Quaternion.Euler(0, 180, 0);
				} else if (neighbourDirections.Contains(Direction.Down) && neighbourDirections.Contains(Direction.Left)) {
					rotation = Quaternion.Euler(0, -90, 0);
				}
				roadDictionary[position] = Instantiate(roadCorner, position, rotation, transform);
			} else if (neighbourDirections.Count == 3) {
				Destroy(roadDictionary[position]);
				if (neighbourDirections.Contains(Direction.Right)
					&& neighbourDirections.Contains(Direction.Down)
					&& neighbourDirections.Contains(Direction.Left)
					) {
					rotation = Quaternion.Euler(0, 90, 0);
				} else if (neighbourDirections.Contains(Direction.Down)
					  && neighbourDirections.Contains(Direction.Left)
					  && neighbourDirections.Contains(Direction.Up)) {
					rotation = Quaternion.Euler(0, 180, 0);
				} else if (neighbourDirections.Contains(Direction.Left)
					  && neighbourDirections.Contains(Direction.Up)
					  && neighbourDirections.Contains(Direction.Right)) {
					rotation = Quaternion.Euler(0, -90, 0);
				}
				roadDictionary[position] = Instantiate(road3way, position, rotation, transform);
			} else {
				Destroy(roadDictionary[position]);
				roadDictionary[position] = Instantiate(road4way, position, rotation, transform);
			}
		}
	}

	/// <summary>
	/// This resets the road dictionary
	/// </summary>
	public void Reset() {
		foreach (var item in roadDictionary.Values) {
			Destroy(item);
		}
		roadDictionary.Clear();
		fixRoadCandidates = new HashSet<Vector3Int>();
	}
}
