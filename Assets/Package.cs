using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour
{

	Collider myCollider;
	Renderer myRenderer;
	// Start is called before the first frame update
	void Start()
    {
		myCollider = GetComponent<Collider>();
		myRenderer = GetComponent<Renderer>();
		myRenderer.material.color = Color.yellow;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "DropZone" && other.gameObject.GetComponent<DropZone>().hasPackage!=true) {
			Debug.Log("DROP ZONE");
			// Drop off package
            DetachPackage();

			other.gameObject.GetComponent<DropZone>().hasPackage = true;
			myRenderer.material.color = Color.green;
			StartCoroutine(DestroyAfterSeconds(60f,other));
		}
	}



	void DetachPackage() {
		this.transform.parent.gameObject.GetComponent<BoxCollider>().isTrigger = false;
		this.transform.parent.gameObject.GetComponent<Packageattach>().hasPackage=false;
		this.transform.parent = null;
		myCollider.isTrigger = false;
	}

	// Coroutine to destroy the object after a certain amount of time
	IEnumerator DestroyAfterSeconds(float seconds,Collider other) {
		yield return new WaitForSeconds(seconds);
		other.gameObject.GetComponent<DropZone>().hasPackage=false;
		Destroy(gameObject);
	}

	
}
