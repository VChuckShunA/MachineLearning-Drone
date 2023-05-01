using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script was an incomplete attempt to generate 3D meshes around a line renderer
/// </summary>
public class TreeMeshRenderer : MonoBehaviour {
	// Start is called before the first frame update
	[SerializeField] private TubeRenderer _tubeRenderer;
	private GameObject[] _gos;
	public Mesh mesh;
	void Start() {

	}

	public void GenerateMesh() {
		_gos = GameObject.FindGameObjectsWithTag("Tree");
		foreach (GameObject g in _gos) {
			Vector3[] gPos = new Vector3[4];
			g.GetComponent<LineRenderer>().GetPositions(gPos);
			Debug.Log(gPos[3]);
			//Vector3 startPos = g.GetComponent<LineRenderer>().GetPosition(0);
			//Vector3 endPos = g.GetComponent<LineRenderer>().GetPosition(g.GetComponent<LineRenderer>().positionCount - 1);
			//Vector3[] positionArray = new Vector3[3];
			//positionArray[0] = new Vector3(startPos.x,startPos.y,startPos.z);            
			//positionArray[1] = new Vector3(0.0f,0.0f,0.0f);
			//positionArray[2] = new Vector3(endPos.x,endPos.y,endPos.z);
			//Debug.Log("Start position : "+startPos + "End Postion : "+ endPos);

			//Instantiate(AllPostitions,branchPosition,Quaternion.Euler(branchRotation.x, branchRotation.y,branchRotation.z));
			//tr.transform.position = g.transform.position;
			//tr.transform.Rotate(new Vector3(g.transform.rotation.x,g.transform.rotation.y, g.transform.rotation.z));
			//Debug.Log();

		}
	}

	// Update is called once per frame
	void Update() {

	}
}
