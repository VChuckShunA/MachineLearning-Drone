using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script has functions that performs calculations for the Adjacency graph
/// </summary>
public class Vertex : IEquatable<Vertex> {
	public Vector3 Position { get; private set; }
	public Vertex(Vector3 position) {
		this.Position = position;
	}
	public bool Equals(Vertex other) {
		return Vector3.SqrMagnitude(Position - other.Position) < 0.0001f;
	}

	public override string ToString() {
		return Position.ToString();
	}
}
