using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    // this manages all targetable object
    [SerializeField] Material normal, highlight; // our normal and highlighted materials
    [SerializeField] Renderer ourRenderer;
    ThirdPersonPlayerController playerController;
    ThirdPersonCameraController cameraController; // our camera controller
    public GameObject swapTarget; // the object connected to this one which holds the enemy class and information we need to swap properly

    [SerializeField] private bool targeted = false; // whether the player is targeting us or not

    // Start is called before the first frame update
    void Start()
    {
        // find our camera and add ourselves to the list of targetable objects
        cameraController = FindObjectOfType<ThirdPersonCameraController>();
        playerController = FindObjectOfType<ThirdPersonPlayerController>();
    }

    private void FixedUpdate() {
        // check if we are being targeted and adjust our material
        if (cameraController.currentTarget == this) { ChangeShader(true); }
        else { ChangeShader(false); }

        // run a check to see if we can be targeted by the player
        Debug.DrawRay(transform.position, playerController.transform.position - transform.position, Color.green);
        if (Vector3.Distance(playerController.transform.position, transform.position) < cameraController.targetableRange && !targeted) {
            RaycastHit hit;
            if (Physics.Linecast(transform.position, playerController.transform.position, out hit)) {
                Debug.Log(hit.transform.tag);
                if (hit.transform.tag == "Player") {
                    ToggleListReference(true);
                } else {
                    ToggleListReference(false);
                }
            } 
        } // run a check to see if we should be removed from the player's target list
        else if ((Vector3.Distance(playerController.transform.position, transform.position) > cameraController.targetableRange && targeted) || targeted) {
            RaycastHit hit;
            if (Physics.Linecast(transform.position, playerController.transform.position, out hit))
            {
                if (hit.transform.tag != "Player")
                {
                    ToggleListReference(false);
                }
            }
        }
    }

    // function for toggling ourselves in the list
    public void ToggleListReference(bool active) { 
        if (active) 
            cameraController.teleportTargets.Add(this);      
        else 
            cameraController.teleportTargets.Remove(this);
        targeted = active;
            
    }

    // function for changing our shader
    public void ChangeShader(bool isHighlighted)
    {
        // if we are highlighted, ensure our current material is the highlight materials
        if (isHighlighted && ourRenderer.material != highlight) { ourRenderer.material = highlight; }
        else if (!isHighlighted & ourRenderer.material != normal) { ourRenderer.material = normal; }
    }

    // debug gizmo draw to check if we are targeted or not
    private void OnDrawGizmos()
    {
        if (targeted)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 2);
        } else if (!targeted)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 2);
        }
    }
}
