using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple class for raycasting clicks onto the hex grid's colliders
public class CameraController : MonoBehaviour {

	public GameObject focus;

	Camera cam;
	public LayerMask interactionMask;

	void Start() {
		//idk why I didn't define it in the inspector, when should one use this kind of fail safe? I define a lot of things in the inspector...
		if(cam == null) {
			if (GetComponent<Camera>() != null) {
				cam = GetComponent<Camera>();
			}
		}
	}

	void FixedUpdate() {
		
    }

	void Update() {
	//Check for clicks, raycast, execute Interacable class function
	    if(Input.GetMouseButtonDown(0)) {
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 100f, interactionMask)) {
				hit.collider.GetComponent<Interactable>().OnClicked();
			}
		}
	}

}
