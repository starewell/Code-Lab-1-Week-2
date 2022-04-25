using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for all interactable gridbased elements to derrive from
//Doesn't serve much purpose in this execution, but will be helpful later on...
[RequireComponent(typeof(OffsetOnHover))]
public class Interactable : MonoBehaviour {

    public bool active = false;

//Hover effect definitions
    public OffsetOnHover hover;
    public virtual void Start() {
    	hover = GetComponent<OffsetOnHover>();
    }
//
//Function executed through CameraController class, checks if interactable is unlocked and interacts
    public void OnClicked() {
        if(active) {
            Interact();
        }   
    }
//Stop the player from clicking again until flip is complete
    protected virtual void Interact() {
        hover.active = false;
    }
//   
//Mouse Over detection setting OffsetOnHover state
    public void OnMouseOver() {
        if(active) {
            hover.active = true;
        }
    }
    public void OnMouseExit() {
        hover.active = false;
    }
//
}