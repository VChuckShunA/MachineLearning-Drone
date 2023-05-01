using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System;


public class AdjacencyGraph {

	// This dictionary stores the adacent graph
	Dictionary<Vertex, List<Vertex>> adjacencyDictionary = new Dictionary<Vertex, List<Vertex>>();

	/// <summary>
	/// This method adds vertexes to the graph
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	public Vertex AddVertex(Vector3 position) {
		if (GetVertexAt(position) != null) {
			return null;
		}

		Vertex v = new Vertex(position);
		AddVertex(v);
		return v;

	}
	/// <summary>
	/// This method adds vertexes to the graph
	/// </summary>
	/// <param name="v"></param>
	private void AddVertex(Vertex v) {
		if (adjacencyDictionary.ContainsKey(v))
			return;
		adjacencyDictionary.Add(v, new List<Vertex>());
	}

	/// <summary>
	/// Retrieving the vertexes at a certain position
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	private Vertex GetVertexAt(Vector3 position) {
		return adjacencyDictionary.Keys.FirstOrDefault(x => CompareVertices(position, x.Position));
	}

	/// <summary>
	/// Comparing two vertices
	/// </summary>
	/// <param name="position1"></param>
	/// <param name="position2"></param>
	/// <returns></returns>
	private bool CompareVertices(Vector3 position1, Vector3 position2) {
		return Vector3.SqrMagnitude(position1 - position2) < 0.0001f;
	}

	/// <summary>
	/// Adding edges to the graph
	/// </summary>
	/// <param name="position1"></param>
	/// <param name="position2"></param>
	public void AddEdge(Vector3 position1, Vector3 position2) {
		if (CompareVertices(position1, position2)) {
			return;
		}
		var v1 = GetVertexAt(position1);
		var v2 = GetVertexAt(position2);
		if (v1 == null) {
			v1 = new Vertex(position1);
		}
		if (v2 == null) {
			v2 = new Vertex(position2);
		}
		AddEdgeBetween(v1, v2);
		AddEdgeBetween(v2, v1);

	}
	/// <summary>
	/// Adding edge between two vertices
	/// </summary>
	/// <param name="v1"></param>
	/// <param name="v2"></param>
	private void AddEdgeBetween(Vertex v1, Vertex v2) {
		if (v1 == v2) {
			return;
		}
		if (adjacencyDictionary.ContainsKey(v1)) {
			if (adjacencyDictionary[v1].FirstOrDefault(x => x == v2) == null) {
				adjacencyDictionary[v1].Add(v2);
			}
		} else {
			AddVertex(v1);
			adjacencyDictionary[v1].Add(v2);
		}

	}
	/// <summary>
	/// Retrieving vertices connected to a specifc vertex
	/// </summary>
	/// <param name="v1"></param>
	/// <returns></returns>
	public List<Vertex> GetConnectedVerticesTo(Vertex v1) {
		if (adjacencyDictionary.ContainsKey(v1)) {
			return adjacencyDictionary[v1];
		}
		return null;
	}
	/// <summary>
	/// Retrieve vertices connected to a certain position
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	public List<Vertex> GetConnectedVerticesTo(Vector3 position) {
		var v1 = GetVertexAt(position);
		if (v1 == null)
			return null;
		return adjacencyDictionary[v1];
	}

	/// <summary>
	/// Clears the graph
	/// </summary>
	public void ClearGraph() {
		adjacencyDictionary.Clear();
	}

	/// <summary>
	/// Retrieves the vertices
	/// </summary>
	/// <returns></returns>
	public IEnumerable<Vertex> GetVertices() {
		return adjacencyDictionary.Keys;
	}

	/// <summary>
	/// Retrieves the data in the the adjacent graph and presents it as a string
	/// </summary>
	/// <returns></returns>
	public override string ToString() {
		StringBuilder builder = new StringBuilder();
		foreach (var vertex in adjacencyDictionary.Keys) {
			builder.AppendLine("Vertex " + vertex.ToString() + " neighbours: " + String.Join(", ", adjacencyDictionary[vertex]));
		}
		return builder.ToString();
	}

	/// <summary>
	/// An implementation of an A Star Search Algorithm
	/// </summary>
	/// <param name="graph"></param>
	/// <param name="startPosition"></param>
	/// <param name="endPosition"></param>
	/// <returns></returns>
	public static List<Vector3> AStarSearch(AdjacencyGraph graph, Vector3 startPosition, Vector3 endPosition) {
		List<Vector3> path = new List<Vector3>();

		Vertex start = graph.GetVertexAt(startPosition);
		Vertex end = graph.GetVertexAt(endPosition);

		List<Vertex> positionsTocheck = new List<Vertex>();
		Dictionary<Vertex, float> costDictionary = new Dictionary<Vertex, float>();
		Dictionary<Vertex, float> priorityDictionary = new Dictionary<Vertex, float>();
		Dictionary<Vertex, Vertex> parentsDictionary = new Dictionary<Vertex, Vertex>();

		positionsTocheck.Add(start);
		priorityDictionary.Add(start, 0);
		costDictionary.Add(start, 0);
		parentsDictionary.Add(start, null);

		while (positionsTocheck.Count > 0) {
			Vertex current = GetClosestVertex(positionsTocheck, priorityDictionary);
			positionsTocheck.Remove(current);
			if (current.Equals(end)) {
				path = GeneratePath(parentsDictionary, current);
				return path;
			}

			foreach (Vertex neighbour in graph.GetConnectedVerticesTo(current)) {
				float newCost = costDictionary[current] + 1;
				if (!costDictionary.ContainsKey(neighbour) || newCost < costDictionary[neighbour]) {
					costDictionary[neighbour] = newCost;

					float priority = newCost + ManhattanDiscance(end, neighbour);
					positionsTocheck.Add(neighbour);
					priorityDictionary[neighbour] = priority;

					parentsDictionary[neighbour] = current;
				}
			}
		}
		return path;
	}

	/// <summary>
	/// Returns the closest vertex
	/// </summary>
	/// <param name="list"></param>
	/// <param name="distanceMap"></param>
	/// <returns></returns>
	private static Vertex GetClosestVertex(List<Vertex> list, Dictionary<Vertex, float> distanceMap) {
		Vertex candidate = list[0];
		foreach (Vertex vertex in list) {
			if (distanceMap[vertex] < distanceMap[candidate]) {
				candidate = vertex;
			}
		}
		return candidate;
	}

	/// <summary>
	/// Returns the distance between two points measured along axes at right angles
	/// </summary>
	/// <param name="endPos"></param>
	/// <param name="position"></param>
	/// <returns></returns>
	private static float ManhattanDiscance(Vertex endPos, Vertex position) {
		return Math.Abs(endPos.Position.x - position.Position.x) + Math.Abs(endPos.Position.z - position.Position.z);
	}

	/// <summary>
	/// Generates a path for the AI agents to travel along
	/// </summary>
	/// <param name="parentMap"></param>
	/// <param name="endState"></param>
	/// <returns></returns>
	public static List<Vector3> GeneratePath(Dictionary<Vertex, Vertex> parentMap, Vertex endState) {
		List<Vector3> path = new List<Vector3>();
		Vertex parent = endState;
		while (parent != null && parentMap.ContainsKey(parent)) {
			path.Add(parent.Position);
			parent = parentMap[parent];
		}
		path.Reverse();
		return path;
	}
}
