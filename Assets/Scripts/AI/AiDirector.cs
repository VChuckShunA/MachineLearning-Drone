using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This script is responsible fore creating a new Adjacency graph, instantiating AIs, and assigning them a path
/// </summary>
public class AiDirector : MonoBehaviour {
	public PlacementManager placementManager;
	public GameObject[] pedestrianPrefabs;

	// Creates a new Adjacency Graph
	AdjacencyGraph graph = new AdjacencyGraph();

	/// <summary>
	/// This funcation spawns AI agents on all houses and special structures randomly for each house and structure
	/// </summary>
	public void SpawnAllAagents() {
		foreach (var house in placementManager.GetAllHouses()) {
			TrySpawningAnAgent(house, placementManager.GetRandomSpecialStrucutre());
		}

		foreach (var specialStructure in placementManager.GetAllSpecialStructures()) {
			TrySpawningAnAgent(specialStructure, placementManager.GetRandomHouseStructure());
		}
	}

	/// <summary>
	/// This function is responsible for the actual spawning of the AIs, and once they're spawned,
	/// each AI will be assigned a path
	/// </summary>
	/// <param name="startStructure"></param>
	/// <param name="endStructure"></param>
	private void TrySpawningAnAgent(Point startStructure, Point endStructure) {
		if (startStructure != null && endStructure != null) {
			var startPosition = placementManager.GetNearestRoad(new Vector3Int(startStructure.X,
				0, startStructure.Y), 1, 1).Value;
			var endPosition = placementManager.GetNearestRoad(new Vector3Int(endStructure.X,
				0, endStructure.Y), 1, 1).Value;

			var startMarkerPosition = placementManager.GetStructureAt(startPosition)
				.GetNearestMarkerTo(new Vector3(startStructure.X, 0, startStructure.Y));
			var endMarkeerPosition = placementManager.GetStructureAt(endPosition)
				.GetNearestMarkerTo(new Vector3(endStructure.X, 0, endStructure.Y));

			var agent = Instantiate(GetRandomPedestrian(), new Vector3Int((int)startMarkerPosition.x
				, 0, (int)startMarkerPosition.z), Quaternion.identity);
			//Debug.Log(startPosition);
			var path = placementManager.GetPathBetween(new Vector3Int(startPosition.x,
				0, startPosition.z), new Vector3Int(endPosition.x, 0, endPosition.z));
			Debug.Log("Path count : " + path.Count);
			if (path.Count > 0) {
				path.Reverse();
				Debug.Log("start marker position " + startMarkerPosition + "end marker position " + endMarkeerPosition);
				List<Vector3> agentPath = GetPedestrianPath(path, startMarkerPosition, endMarkeerPosition);
				var aiAgent = agent.GetComponent<AiAgent>();
				Debug.Log("Agent path : " + agentPath);

				aiAgent.Initialize(agentPath);

				// aiAgent.Initialize(new List<Vector3>(path.Select(x => (Vector3)x).ToList()));
			}
		}
	}

	/// <summary>
	/// Retrieves random pedestrians
	/// </summary>
	/// <returns></returns>
	private GameObject GetRandomPedestrian() {
		return pedestrianPrefabs[UnityEngine.Random.Range(0, pedestrianPrefabs.Length)];
	}

	/// <summary>
	/// This function gets a path for the AI to move from point A to B
	/// It returns a list of Vector3s which tells the AI how to navigate from marker to marker.
	/// The markers are placed on the pavement
	/// </summary>
	/// <param name="path"></param>
	/// <param name="startPosition"></param>
	/// <param name="endPosition"></param>
	/// <returns></returns>
	private List<Vector3> GetPedestrianPath(List<Vector3Int> path, Vector3 startPosition, Vector3 endPosition) {
		graph.ClearGraph();
		CreateAGraph(path);
		Debug.Log("path " + path);
		Debug.Log("GRAPH : " + graph);
		Debug.Log("Start position:" + startPosition + "end position " + endPosition);
		//return null;
		return AdjacencyGraph.AStarSearch(graph, startPosition, endPosition);
	}

	/// <summary>
	/// This function creates an Adjacency graph for the AI to move through
	/// </summary>
	/// <param name="path"></param>
	private void CreateAGraph(List<Vector3Int> path) {
		Dictionary<Marker, Vector3> tempDictionary = new Dictionary<Marker, Vector3>();

		for (int i = 0; i < path.Count; i++) {
			var currentPosition = path[i];
			var roadStructure =
				placementManager.GetStructureAt(currentPosition); // I think this might be where it's crashing
			var markersList = roadStructure.GetPedestrianMarkers();

			Debug.Log("Markers List " + markersList.Count);
			Debug.Log("road structure " + roadStructure);
			bool limitDistance = markersList.Count == 4; //use this flag to check if the prefab has 4 markers
			tempDictionary.Clear();

			foreach (var marker in markersList) {
				//loop through each marker in our marker list
				graph.AddVertex(marker.Position); // Create the first marker
												  //Accessing adjacent markers
				foreach (var markerNeighbourPosition in marker.GetAdjacentPositions()) {
					graph.AddEdge(marker.Position, markerNeighbourPosition);
				}

				if (marker.OpenForconnections && i + 1 < path.Count) {
					//Checking to see if we have the next road prefab
					var nextRoadStructure = placementManager.GetStructureAt(path[i + 1]);
					Debug.Log("limit Distance " + limitDistance);
					if (limitDistance) {
						tempDictionary.Add(marker, nextRoadStructure.GetNearestMarkerTo(marker.Position));
					} else {
						graph.AddEdge(marker.Position, nextRoadStructure.GetNearestMarkerTo(marker.Position));
						Debug.Log("ADDING EDGE");
					}
				}
			}

			if (limitDistance && tempDictionary.Count == 4) {
				//Sorting the dictionary
				var distanceSortedMarkers =
					tempDictionary.OrderBy(x => Vector3.Distance(x.Key.Position, x.Value)).ToList();
				for (int j = 0; j < 2; j++) {
					graph.AddEdge(distanceSortedMarkers[j].Key.Position, distanceSortedMarkers[j].Value);
				}
			}
		}

		Debug.Log("Graph vertexes " + graph.GetVertices());
	}

	private void Update() {
		foreach (var vertex in graph.GetVertices()) {
			foreach (var vertexNeighbour in graph.GetConnectedVerticesTo(vertex)) {
				Debug.DrawLine(vertex.Position + Vector3.up, vertexNeighbour.Position + Vector3.up, Color.red);
			}
		}
	}
}