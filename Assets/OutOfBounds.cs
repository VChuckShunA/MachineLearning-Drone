using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{

	GameObject drone; 
	// Start is called before the first frame update
	void Start()
    {
		drone = GameObject.FindWithTag("Drone");
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Drone") {
			Debug.Log("OUT OF BOUNDS");
			drone.GetComponent<DroneController>().AddReward(-10f);
			drone.GetComponent<DroneController>().EndEpisode();
		}
	}
}
