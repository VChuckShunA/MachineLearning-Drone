using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script manages where objects are placed, specifically within the graph set up in Grid.cs
/// </summary>
public class PlacementManager : MonoBehaviour {
	public int width, height;
	Grid placementGrid;

	private Dictionary<Vector3Int, StructureModel> temporaryRoadobjects = new Dictionary<Vector3Int, StructureModel>();

	private Dictionary<Vector3Int, StructureModel> structureDictionary = new Dictionary<Vector3Int, StructureModel>();

	private void Start() {
		placementGrid = new Grid(width, height);
	}

	/// <summary>
	/// This funcation returns the nearest road
	/// This is used when instantiating AIs
	/// </summary>
	/// <param name="position"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <returns></returns>
	public Vector3Int? GetNearestRoad(Vector3Int position, int width, int height) {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				var newPosition = position + new Vector3Int(x, 0, y);
				var roads = GetNeighboursOfTypeFor(newPosition, CellType.Road);
				if (roads.Count > 0) {
					return roads[0];
				}
			}
		}

		return null;
	}


	/// <summary>
	/// This function returns a list of neighbours of a certain type of object
	/// You can get the nearest road objects to a certain house using this
	/// </summary>
	/// <param name="position"></param>
	/// <param name="type"></param>
	/// <returns></returns>
	internal List<Vector3Int> GetNeighboursOfTypeFor(Vector3Int position, CellType type) {
		var neighbourVertices = placementGrid.GetAdjacentCellsOfType(position.x, position.z, type);
		//Debug.Log("NEIGHBOUR VECTOR"+neighbourVertices);
		List<Vector3Int> neighbours = new List<Vector3Int>();

		foreach (var point in neighbourVertices) {
			neighbours.Add(new Vector3Int(point.X, 0, point.Y));
		}

		//Debug.Log(neighbours);
		return neighbours;
	}

	/// <summary>
	/// This function saves the position of the road prefabs as a 'Sturcture Model'
	/// This will be clear when viewing StructureModel.cs
	/// </summary>
	/// <param name="position"></param>
	/// <param name="type"></param>
	/// <param name="rm"></param>
	/// <returns></returns>
	//This is what's causing the issue with the roads

	public StructureModel CreateANewStructureModel(Vector3Int position, CellType type, RoadManager rm) {
		GameObject structure = new GameObject(type.ToString());
		structure.transform.SetParent(transform);
		structure.transform.localPosition = position;
		var structureModel = structure.AddComponent<StructureModel>();
		if (rm) {
			var newRm = structure.AddComponent<RoadManager>();
			newRm.PopulatePedestrianMarkers(rm.pedestrianMarkers);
		}


		return structureModel;
	}

	/// <summary>
	/// This function adds the new instantiated Roads and structures to the Grid
	/// </summary>
	/// <param name="position"></param>
	/// <param name="type"></param>
	/// <param name="rm"></param>
	/// <returns></returns>
	public Grid AddToGrid(Vector3Int position, string type, RoadManager rm) {
		CellType parsed_enum = (CellType)System.Enum.Parse(typeof(CellType), type);
		Vector3Int newPosition;
		StructureModel structure;
		switch (type) {
			case "Structure":
				//placementGrid._houseStructure.Add(new Point((int)position.x, (int)position.z));
				placementGrid[position.x, position.z] = parsed_enum;
				newPosition = new Vector3Int(position.x, 0, position.z);
				structure = CreateANewStructureModel(position, parsed_enum, rm);
				structureDictionary.Add(newPosition, structure);
				break;
			case "SpecialStructure":
				//placementGrid._specialStructure.Add(new Point((int)position.x, (int)position.z));
				placementGrid[position.x, position.z] = parsed_enum;
				newPosition = new Vector3Int(position.x, 0, position.z);
				structure = CreateANewStructureModel(position, parsed_enum, rm);
				structureDictionary.Add(newPosition, structure);
				break;
			case "Road":
				//placementGrid._roadList.Add(new Point((int)position.x, (int)position.z));
				placementGrid[position.x, position.z] = parsed_enum;
				newPosition = new Vector3Int(position.x, 0, position.z);
				structure = CreateANewStructureModel(position, parsed_enum, rm);
				structureDictionary.Add(newPosition, structure);
				break;
		}

		return placementGrid;
	}

	/// <summary>
	/// This function uses the AStarAlgorithm to get the path between two endpoints
	/// it returns the pathas a list of Vector3Ints
	/// </summary>
	/// <param name="startPosition"></param>
	/// <param name="endPosition"></param>
	/// <returns></returns>
	internal List<Vector3Int> GetPathBetween(Vector3Int startPosition, Vector3Int endPosition) {
		var resultPath = GridSearch.AStarSearch(placementGrid, new Point(startPosition.x, startPosition.z),
			new Point(endPosition.x, endPosition.z));
		List<Vector3Int> path = new List<Vector3Int>();
		foreach (Point point in resultPath) {
			path.Add(new Vector3Int(point.X, 0, point.Y));
		}

		return path;
	}

	/// <summary>
	/// Returns a reandom special struction
	/// </summary>
	/// <returns></returns>

	public Point GetRandomSpecialStrucutre() {
		var point = placementGrid.GetRandomSpecialStructurePoint();
		//return GetStructureAt(point);
		return point;
	}

	/// <summary>
	/// Returns a random House structure
	/// </summary>
	/// <returns></returns>
	public Point GetRandomHouseStructure() {
		var point = placementGrid.GetRandomHouseStructurePoint();
		// return GetStructureAt(point);
		return point;
	}


	/// <summary>
	/// Returns alll Houses
	/// </summary>
	/// <returns></returns>
	public List<Point> GetAllHouses() {
		List<StructureModel> returnList = new List<StructureModel>();
		List<Point> housePositions = placementGrid.GetAllHouses();
		Vector3Int i = new Vector3Int(0, 0, 0);

		return housePositions;
	}


	/// <summary>
	/// Returns all special structures
	/// </summary>
	/// <returns></returns>
	internal List<Point> GetAllSpecialStructures() {
		List<StructureModel> returnList = new List<StructureModel>();
		var housePositions = placementGrid.GetAllSpecialStructure();

		return housePositions;
	}

	/// <summary>
	/// Returns the structure at a specific point
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	public StructureModel GetStructureAt(Vector3Int position) {
		if (structureDictionary.ContainsKey(position)) {
			return structureDictionary[position];
		}

		return null;
	}
}