using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour {

    public enum CameraState { FollowCam, FreeCam, TargetCam };


    //Variables used in every camera mode
    [Header("All Camera Modes")]
    public CameraState camState; // Our current camera mode

    public Transform playerTransform; // Reference to player
    private ThirdPersonPlayerController playerController;

    public Transform cameraTransform; // Reference to camera 
    private Camera camera;
    
    [SerializeField] Transform targetTransform; // Target transform for the camera to lerp to
    [SerializeField] Vector3 rootOffset; // The Y offset of the character for the camera to LookAt
    
    [SerializeField] float dollySpeed; // How fast the camera translates
    [SerializeField] float rotationSpeed; // How fast the camera rotates

    [SerializeField] float zOffset, yOffset; // How far away the camera should be from the player
    [SerializeField] float zOffsetMultiplier, yOffsetMultiplier; // Multipliers for camera offset, used for zooming in and out

    [SerializeField] private Vector3 lookDir; // Direction for the calculated targetPos's forward

    private float lTrigger, rJoystickH, rJoystickV; // Input variables

    [Header("Follow Cam")]


    //Variables used in the FreeCam mode
    [Header("Free Cam")]
    [SerializeField] Transform targetForward; // Child object of rig, moves to player and rotates based on right joystick
    bool orbiting; // used to reset targetForward when free cam is activated

    //Variables used in the TargetCam Mode
    [Header("Target Cam")]
    [SerializeField] float targetSwapCooldown;
    [SerializeField] float targetSwapCooldownMax; //Timer for switching targets with the right joystick
    public List<Targetable> teleportTargets; // Targets that are currently targetable, updated by the Targetable class
    public Targetable currentTarget; // Our current target
    private float closestTargetDistance; // reference variable for switching targets with right joystick
    public float targetableRange = 10; // How close enemies have to be to be targeted
   
    //Start Coroutine?! Called when scene loaded, runs camera state machine
    IEnumerator Start() {
        //assign references
        camera = cameraTransform.gameObject.GetComponent<Camera>();
        playerController = FindObjectOfType<ThirdPersonPlayerController>();
        //Start input check coroutine
        StartCoroutine(CheckInputs());

        while (true) { //..... endless loop, prone to breaking, run state machine
            switch(camState) {
                case CameraState.FollowCam:
                    orbiting = false;
                    yield return StartCoroutine(FollowCam());                
                    break;
                case CameraState.FreeCam:
                    yield return StartCoroutine(FreeCam());
                    break;
                case CameraState.TargetCam:
                    orbiting = false;
                    yield return StartCoroutine(TargetCam());
                    break;
            }          
        }
    }

    //Get inputs, trigger state changes and other functions
    IEnumerator CheckInputs() {
        while (true) { //.... more endless loops
            lTrigger = Input.GetAxis("LeftTrigger");
            rJoystickH = Input.GetAxis("HorizontalR");
            rJoystickV = Input.GetAxis("VerticalR");

            if (lTrigger > 0.5f) {
                camState = CameraState.TargetCam;
                //Debug.Log("Target Cam");
            } else if (Mathf.Abs(rJoystickH) > 0f) {
                camState = CameraState.FreeCam;
                //Debug.Log("Free Cam");
            } else if (camState != CameraState.FreeCam) {
                camState = CameraState.FollowCam;
                //Debug.Log("Follow Cam");
            }

            if (Mathf.Abs(rJoystickV) > 0f)
                ZoomCamera();

            yield return new WaitForEndOfFrame();
        }     
    }

    //Coroutine that runs while current CameraState is followCam
    IEnumerator FollowCam() {
        Vector3 characterOffset = playerTransform.position + rootOffset;

        //Set look direction to follow the player
        lookDir = characterOffset - cameraTransform.position;
        lookDir.y = 0;
        lookDir.Normalize();

        //Set camera's targetTransform relative to the players position, lookDir, and y and z Offsets
        targetTransform.position = playerTransform.position + playerTransform.up * yOffset - lookDir * zOffset;

        //Adjust targetTransform to compensate for wall collisions
        Vector3 adjustedTargetPos = targetTransform.position;
        CompensateForWallClipping(playerTransform.position, ref adjustedTargetPos);
        targetTransform.position = adjustedTargetPos;
        //Move camera to target
        MoveCamToTargetPos();
     
        yield return new WaitForEndOfFrame();
        CameraCenterTargeting();
    }

    //Coroutine that runs while current CameraState is freeCam
    IEnumerator FreeCam() {
        //Check if this is the first call of the function, reset targetForward to current camera forward if so
        if (!orbiting) { 
            targetForward.forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z);
            orbiting = true;
        }

        targetForward.position = playerTransform.position; // Set targetForward transform position to player
        targetForward.Rotate(new Vector3(0, rotationSpeed * rJoystickH * Time.deltaTime, 0)); // Rotate the forward based on right joystick

        //Set look direction to targetFoward
        lookDir = new Vector3 (targetForward.forward.x, 0, targetForward.forward.z);       
        lookDir.Normalize();

        //Set camera's targetTransform relative to the players position, lookDir, and y and z Offsets
        targetTransform.position = playerTransform.position + playerTransform.up * yOffset - lookDir * zOffset;
        Debug.DrawRay(cameraTransform.position, targetTransform.position, Color.magenta);

        //Adjust targetTransform to compensate for wall collisions
        Vector3 adjustedTargetPos = targetTransform.position;
        CompensateForWallClipping(playerTransform.position, ref adjustedTargetPos);
        targetTransform.position = adjustedTargetPos;
        //Move camera to target
        MoveCamToTargetPos();

        yield return new WaitForEndOfFrame();
        CameraCenterTargeting();
        
    }

    //Coroutine that runs while current CameraState is targetCam
    IEnumerator TargetCam() {
        //First, snap camera to behind player forward
        /*if (!camLock) {            
            triggeredDir = playerTransform.forward;
            dollySpeed *= 2;
            while (Vector3.Distance(cameraTransform.position, targetTransform.position) > 1) {
                targetTransform.position = playerTransform.position + playerTransform.up * yOffset - triggeredDir * zOffset;
                MoveCamToTargetPos();
                yield return new WaitForEndOfFrame();              
            }
            dollySpeed /= 2;
            camLock = true;
        }*/
        //Then, if we have a target, align camera to look in the dir between it and player
        if (currentTarget) { 
            //Set the look direction to focus the difference in player and enemy transforms
            lookDir = Vector3.Normalize(currentTarget.transform.position - playerTransform.position);

            //Set camera's targetTransform relative to the players position, lookDir, and y and z Offsets
            targetTransform.position = playerTransform.position + playerTransform.up * yOffset - lookDir * zOffset;

            //Adjust targetTransform to compensate for wall collisions
            Vector3 adjustedTargetPos = targetTransform.position;
            CompensateForWallClipping(playerTransform.position, ref adjustedTargetPos);
            targetTransform.position = adjustedTargetPos;
            MoveCamToTargetPos();
            
        } /* { //Otherwise, continue locking the camera behind the player's forward

            //Set camera's targetTransform relative to the players position, lookDir, and y and z Offsets
            targetTransform.position = playerTransform.position + playerTransform.up * yOffset * yOffsetMultiplier - triggeredDir * zOffset * zOffsetMultiplier;

            //Adjust targetTransform to compensate for wall collisions
            Vector3 adjustedTargetPos = targetTransform.position;
            CompensateForWallClipping(playerTransform.position, ref adjustedTargetPos);
            targetTransform.position = adjustedTargetPos;
            MoveCamToTargetPos();
        }*/
        yield return new WaitForEndOfFrame();
        DirectTargeting();
    }

    //Function to lerp camera to target position
    void MoveCamToTargetPos() {
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetTransform.position, Time.deltaTime * dollySpeed);
        cameraTransform.LookAt(playerTransform);
    }

    //Function to detect wall collisions, adjust the targetTransform to not clip into them
    private void CompensateForWallClipping(Vector3 fromObject, ref Vector3 toTarget) {
        RaycastHit wallHit;

        Debug.DrawRay(fromObject, toTarget - fromObject, Color.cyan);
        if (Physics.Raycast(fromObject, toTarget - fromObject, out wallHit)) {
            toTarget = new Vector3(wallHit.point.x, playerTransform.position.y + yOffset, wallHit.point.z);;
        }
        

    }

    void ZoomCamera() {

    }

    //Function for targeting when not locking on, finds the central most targetable in the camera
    void CameraCenterTargeting() {
        if (teleportTargets.Count > 0)
        {
            // this function works by identifying the enemy closest to the center of the player's screen, and highlights them
            closestTargetDistance = Mathf.Infinity;
            // loop through the list, foreach targetable object, 
            foreach (Targetable targetable in teleportTargets)
            {
                // find their position to the center of the screen
                Vector3 targetScreenPos = camera.WorldToScreenPoint(targetable.gameObject.transform.position);
                // store that distance
                float currentTargetDistance = Vector2.Distance(targetScreenPos, new Vector2(Screen.width / 2, Screen.height / 2)); // our current target's distance
                // if that distance is lower than any other distance
                if (currentTargetDistance < closestTargetDistance || closestTargetDistance == 0)
                {
                    // set that to the new minimum distance
                    closestTargetDistance = currentTargetDistance;
                    // make it the new highlight target
                    currentTarget = targetable;
                }
            }
            // update our character's target each frame
            playerController.teleportTarget = currentTarget.gameObject.transform;
        }
        else
            currentTarget = null;
    }

    //Function for targeting while locking on, checks list of viable targets and switches between them based on camera position and right joystick
    void DirectTargeting() {
        if(teleportTargets.Count > 0) { 
            if (targetSwapCooldown <= 0) {
                // run while left trigger is held
                // if we move the right stick to the right, find the closest enemy to the right of the camera center and lock on
                if (Input.GetAxis("HorizontalR") < -0.25f) {
                    // create a new list of targetables
                    List<Targetable> rightwardsTargets = new List<Targetable>();
                    // loop through the enemies and get the enemies to the right
                    foreach (Targetable targetable in teleportTargets) {
                        // get the screenspace of the target
                        Vector2 targetScreenspace = camera.WorldToScreenPoint(targetable.gameObject.transform.position);
                        // if the targetable's X screenspace minus the center of the screen is negative, that means it is to the right of us, so add it to the rightwards target
                        if (targetScreenspace.x - Screen.width / 2 < 0) {
                            // make sure not to add our current target
                            if (targetable != currentTarget)
                                rightwardsTargets.Add(targetable);
                        }
                    }

                    float closestRightTargetDistance = Mathf.Infinity;
                    // now flip through the rightwards targets and pick out the nearest one
                    foreach (Targetable targetable in rightwardsTargets) {
                        // find their position to the center of the screen
                        Vector3 targetScreenPos = camera.WorldToScreenPoint(targetable.gameObject.transform.position);
                        // store that distance
                        float currentTargetDistance = Vector2.Distance(targetScreenPos, new Vector2(Screen.width / 2, Screen.height / 2)); // our current target's distance
                        // if that distance is lower than any other distance
                        if (currentTargetDistance < closestRightTargetDistance || closestRightTargetDistance == 0) {
                            // set that to the new minimum distance
                            closestRightTargetDistance = currentTargetDistance;
                            // make it the new highlight target
                            currentTarget = targetable;
                        }
                    }
                    // update our character's target
                    if (currentTarget)
                        playerController.teleportTarget = currentTarget.swapTarget.transform;

                    // reset targeting cooldown
                    targetSwapCooldown = targetSwapCooldownMax;
                }
                // if we move to the stick to the left, find the closest enemy to the left and lock on
                if (Input.GetAxis("HorizontalR") > 0.25f) {
                    // create a new list of targetables
                    List<Targetable> leftwardsTargets = new List<Targetable>();
                    // loop through the enemies and get the enemies to the right
                    foreach (Targetable targetable in teleportTargets) {
                        // get the screenspace of the target
                        Vector2 targetScreenspace = camera.WorldToScreenPoint(targetable.gameObject.transform.position);
                        // if the targetable's X screenspace minus the center of the screen is negative, that means it is to the right of us, so add it to the rightwards target
                        if (targetScreenspace.x - Screen.width / 2 > 0) {
                            // make sure not to add our current target
                            if (targetable != currentTarget)
                                leftwardsTargets.Add(targetable);
                        }
                    }

                    float closestLeftTargetDistance = Mathf.Infinity;
                    // now flip through the rightwards targets and pick out the nearest one
                    foreach (Targetable targetable in leftwardsTargets) {
                        // find their position to the center of the screen
                        Vector3 targetScreenPos = camera.WorldToScreenPoint(targetable.gameObject.transform.position);
                        // store that distance
                        float currentTargetDistance = Vector2.Distance(targetScreenPos, new Vector2(Screen.width / 2, Screen.height / 2)); // our current target's distance
                        // if that distance is lower than any other distance
                        if (currentTargetDistance < closestLeftTargetDistance || closestLeftTargetDistance == 0) {
                            // set that to the new minimum distance
                            closestLeftTargetDistance = currentTargetDistance;
                            // make it the new highlight target
                            currentTarget = targetable;
                        }
                    }
                    // update our character's target
                    if (currentTarget)
                        playerController.teleportTarget = currentTarget.swapTarget.transform;

                    // reset targeting cooldown
                    targetSwapCooldown = targetSwapCooldownMax;
                }
            } else if (targetSwapCooldown > 0) { targetSwapCooldown--; }
        }
        else
            currentTarget = null;
    }
}
