using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple coroutine class for offsetting a tile's y pos
[System.Serializable]
public class OffsetOnHover : MonoBehaviour {
//Variables for offset calculation
	public float yOffset = 0.25f;
	public float duration = 0.125f;
	
	float yOrigin;

	void Start() {
		yOrigin = transform.position.y;
	}
//
//Offset y 
	public IEnumerator Activate() {
		float time = 0;
		while(time < duration) {
			//float t = time / duration;
			//t = t * t * (3f - 2f * t); this was an easing calc I found but didn't bother to implement		
			transform.position = 
				new Vector3(
				transform.position.x, 
				Mathf.Lerp(yOrigin, yOrigin + yOffset, time/duration), 
				transform.position.z);
			time += Time.deltaTime;
			yield return null;
		}	
	}
//
//Return y to origin
	public IEnumerator Deactivate() {
		float time = 0;
		while(time <= duration) {				
			transform.position = 
				new Vector3(
				transform.position.x, 
				Mathf.Lerp(transform.position.y, yOrigin, time/duration), 
				transform.position.z);
			time += Time.deltaTime;
			yield return null;
		}	
	}
//
}
