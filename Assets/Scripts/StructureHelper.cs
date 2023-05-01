using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script is used when placing trees and buildings on the side of the road
/// </summary>
public class StructureHelper : MonoBehaviour {

	public HouseType[] buildingTypes;
	public GameObject[] naturePrefabs;
	public bool randomNaturePlacement = false;
	[Range(0, 1)]
	public float randomNaturePlacementThreshold = 0.3f;
	public Dictionary<Vector3Int, GameObject> structuresDictionary = new Dictionary<Vector3Int, GameObject>();
	public Dictionary<Vector3Int, GameObject> natureDictionary = new Dictionary<Vector3Int, GameObject>();

	/// <summary>
	/// This function places structures around a given road position
	/// </summary>
	/// <param name="roadPositions"></param>
	public void PlaceStructuresAroundRoad(List<Vector3Int> roadPositions) {
		Dictionary<Vector3Int, Direction> freeEstateSpots = FindFreeSpaceAroundRoad(roadPositions);
		List<Vector3Int> blockedPositions = new List<Vector3Int>();
		foreach (var freeSpot in freeEstateSpots) {
			if (blockedPositions.Contains(freeSpot.Key)) {
				continue;
			}
			var rotation = Quaternion.identity;
			switch (freeSpot.Value) {
				case Direction.Up:
					rotation = Quaternion.Euler(0, 90, 0);
					break;
				case Direction.Down:
					rotation = Quaternion.Euler(0, -90, 0);
					break;
				case Direction.Right:
					rotation = Quaternion.Euler(0, 180, 0);
					break;
				default:
					break;
			}
			//We need to loop through each building types
			for (int i = 0; i < buildingTypes.Length; i++) {
				if (buildingTypes[i].quantity == -1) {
					if (randomNaturePlacement) {
						var random = UnityEngine.Random.value;
						if (random < randomNaturePlacementThreshold) {
							var nature = SpawnPrefab(naturePrefabs[UnityEngine.Random.Range(0, naturePrefabs.Length)], freeSpot.Key, rotation);
							natureDictionary.Add(freeSpot.Key, nature);
							break;
						}
					}
					var building = SpawnPrefab(buildingTypes[i].GetPrefab(), freeSpot.Key, rotation);
					structuresDictionary.Add(freeSpot.Key, building);
					break;
				}
				if (buildingTypes[i].IsBuildingAvailable()) {
					if (buildingTypes[i].sizeRequired > 1) {
						var halfSize = Mathf.CeilToInt(buildingTypes[i].sizeRequired / 2.0f);
						List<Vector3Int> tempPositionsBlocked = new List<Vector3Int>();
						//check if there's space to place a building
						if (VerifyIfBuildingFits(halfSize, freeEstateSpots, freeSpot, blockedPositions, ref tempPositionsBlocked)) {
							blockedPositions.AddRange(tempPositionsBlocked);
							var building = SpawnPrefab(buildingTypes[i].GetPrefab(), freeSpot.Key, rotation);
							structuresDictionary.Add(freeSpot.Key, building);
							foreach (var position in tempPositionsBlocked) {
								structuresDictionary.Add(position, building);
							}
							break;
						}
					} else {
						var building = SpawnPrefab(buildingTypes[i].GetPrefab(), freeSpot.Key, rotation);
						structuresDictionary.Add(freeSpot.Key, building);
					}
					break;
				}

			}
		}
	}

	/// <summary>
	/// We use this function to check if the larger buildings(Hospitals, Mainhalls & Apartments) will fit in 
	/// properly alongside the roads
	/// </summary>
	/// <param name="halfSize"></param>
	/// <param name="freeEstateSpots"></param>
	/// <param name="freeSpot"></param>
	/// <param name="blockedPositions"></param>
	/// <param name="tempPositionsBlocked"></param>
	/// <returns></returns>
	private bool VerifyIfBuildingFits(int halfSize,
		Dictionary<Vector3Int,
			Direction> freeEstateSpots,
		KeyValuePair<Vector3Int, Direction> freeSpot,
		List<Vector3Int> blockedPositions,
		ref List<Vector3Int> tempPositionsBlocked) {
		Vector3Int direction = Vector3Int.zero;
		//check up and down
		if (freeSpot.Value == Direction.Down || freeSpot.Value == Direction.Up) {
			direction = Vector3Int.right;
		} else {
			direction = new Vector3Int(0, 0, 1);
		}
		for (int i = 1; i <= halfSize; i++) {
			//checking one position to the right and one position to the left
			var pos1 = freeSpot.Key + direction * i;
			var pos2 = freeSpot.Key - direction * i;
			if (!freeEstateSpots.ContainsKey(pos1) || !freeEstateSpots.ContainsKey(pos2) ||
				blockedPositions.Contains(pos1) || blockedPositions.Contains(pos2)) {
				//this building doesn't fit
				return false;
			}
			tempPositionsBlocked.Add(pos1);
			tempPositionsBlocked.Add(pos2);
		}
		return true;
	}

	/// <summary>
	/// This function spawns a prefab at a given location
	/// </summary>
	/// <param name="prefab"></param>
	/// <param name="position"></param>
	/// <param name="rotation"></param>
	/// <returns></returns>
	private GameObject SpawnPrefab(GameObject prefab, Vector3Int position, Quaternion rotation) {
		var newStructure = Instantiate(prefab, position, rotation, transform);
		return newStructure;
	}

	/// <summary>
	/// This function is called when finding free space around a road placement
	/// </summary>
	/// <param name="roadPositions"></param>
	/// <returns></returns>
	private Dictionary<Vector3Int, Direction> FindFreeSpaceAroundRoad(List<Vector3Int> roadPositions) {
		Dictionary<Vector3Int, Direction> freeSpace = new Dictionary<Vector3Int, Direction>();
		foreach (var position in roadPositions) {
			var neighbourDirections = PlacementHelper.FindNeighbour(position, roadPositions);
			foreach (Direction direction in System.Enum.GetValues(typeof(Direction))) {
				if (neighbourDirections.Contains(direction) == false) {
					var newPosition = position + PlacementHelper.GetOffsetFromDirection(direction);
					if (freeSpace.ContainsKey(newPosition)) {
						continue;
					}
					freeSpace.Add(newPosition, PlacementHelper.GetReverseDirection(direction));
				}
			}
		}
		return freeSpace;
	}

	/// <summary>
	/// This function is used to clear up all the prefabs when regenerating the LSystem
	/// </summary>
	public void Reset() {
		foreach (var item in structuresDictionary.Values) {
			Destroy(item);
		}
		structuresDictionary.Clear();
		foreach (var item in natureDictionary.Values) {
			Destroy(item);
		}
		natureDictionary.Clear();
		foreach (var buildingType in buildingTypes) {
			buildingType.Reset();
		}
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag("Tree");
		foreach (var tree in gos) {
			Destroy(tree);
		}
	}

}