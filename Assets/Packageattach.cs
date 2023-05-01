using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Packageattach : MonoBehaviour
{

	public bool hasPackage = false;
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasPackage) {
			StartCoroutine(ReactivateTrigger());

		}
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Package" && !hasPackage) {
			// Attach the package to the drone at the same position and rotation
            hasPackage = true;
			other.transform.parent = transform;
			other.transform.localPosition = Vector3.zero;
			other.transform.localRotation = Quaternion.identity;
		}
    }

	IEnumerator ReactivateTrigger() {
		yield return new WaitForSeconds(1);
        this.gameObject.GetComponent<BoxCollider>().isTrigger = true;
	}
}
