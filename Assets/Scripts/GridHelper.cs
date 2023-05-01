using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridHelper : MonoBehaviour {
	public string cellStr;

	public GameObject pM_GO;
	public PlacementManager placementManager;

	public CellType parsed_enum;

	private RoadManager rm;

	/// <summary>
	/// In this script we access the placement Manager 
	/// and the Road manager and add the town objects with generated into our Grid
	/// The Grid is used for pathfinding
	/// </summary>
	void Start() {
		pM_GO = GameObject.FindWithTag("placementManager");
		rm = gameObject.GetComponent<RoadManager>();
		placementManager = pM_GO.GetComponent<PlacementManager>();
		parsed_enum = (CellType)System.Enum.Parse(typeof(CellType), cellStr);
		placementManager.AddToGrid(new Vector3Int(Mathf.FloorToInt(transform.position.x),
			Mathf.FloorToInt(transform.position.y),
			Mathf.FloorToInt(transform.position.z)), cellStr, rm);
	}
}