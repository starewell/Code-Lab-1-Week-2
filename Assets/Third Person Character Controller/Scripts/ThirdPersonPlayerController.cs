using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class ThirdPersonPlayerController : MonoBehaviour
{
    /// <summary>
    /// This is a 3D platformer controller made by Josh
    /// It uses the Unity proprietary character controller
    /// It's main purpose is to swap positions with enemies to put them into harms way
    /// </summary>
    /// 

    // our variables 
    [Header("Referenced Variables")]
    public static ThirdPersonPlayerController instance; // the current instance of our player controller
    public enum PlayerState { Alive, Dead, Win }; // our three player states
    public PlayerState playerState; // our current player state
    private bool alive, hasWon; // used to trigger player states only once

    public delegate void OnPlayerState(int dumby);
    public event OnPlayerState PlayerDeathCallback;
    public event OnPlayerState PlayerWinCallback;

    [Header("References")]
    [SerializeField] CharacterController characterController; // our character controller
    //[SerializeField] PlayerAnimationController animationController; // our animation controller
    [SerializeField] Transform cameraTransform; // our current camera transform

    [Header("Movement Variables")]
    [SerializeField] float moveSpeed; // things we can adjust
    [SerializeField] float jumpVelocity; // how HARD WE JUMP!
    Vector3 moveH, moveV; // our horizontal and forward movement values on the controller stick
    public Vector3 move; // our final movement direction in local space converted to world space
    [SerializeField] bool canMove; // can we move right now?

    [Header("Gravity Variables")]
    [SerializeField] float gravity; // what is our base gravity
    [SerializeField] float gravityValue, playerJumpVelocity, verticalVelocity; // current gravity variables 
    [SerializeField] float upJumpMultiplier, fallMultiplier, standMultiplier; // what is the gravity when we are grounded

    [Header("Acrobatic Variables")]
    [SerializeField] float staminaCharges; // the amount of current stamina we have
    [SerializeField] float staminaChargesMax, staminaCooldownTime; // how many charges we currently have (3?), how many we can have at one time, the cooldown for recharge
    [SerializeField] Slider staminaSlider; // the slider displaying our stamina values
    [SerializeField] float dashCoolDown, dashCoolDownMax, dashTime, dashTimeMax, dashIntensity, dashJumpVelocity; // all the variables relating to our dash
    Vector3 dashDir; // our dash direction, calculated off of our current movement direction
    int remainingJumps; // this is for our double jump

    [Header("Teleport Variables")]
    public Transform teleportTarget; // the target enemy we want to swap with
    [SerializeField] float teleportCooldown, teleportCooldownMax; // current and maximum of our teleport cooldown
    [SerializeField] float teleportCharges, teleportChargesMax, teleportRechargeRate; // the amount of charges and recharge rate of our teleporter
    [SerializeField] Slider teleportSlider; // our teleporter charge slider display
    [SerializeField] GameObject teleportParticle; // the particle effect that we spawn on movement

    // runs when the object is loaded into the memory
    void Awake()
    {
        // create our instance
        if (instance != null)
        {
            Debug.Log("More than one instance of ThirdPersonPlayerController found!");
            return;
        }
        instance = this;
    }

    // runs whent he object is loaded into the scene
    private void Start() {
        playerState = PlayerState.Alive;
        alive = true;
        hasWon = false;
    }

    // Update is called once per frame
    void Update() {
        switch(playerState) {
            case PlayerState.Alive:
                Movement(); // run the player control here
                break;
            case PlayerState.Dead:
                if (alive) {
                    Debug.Log("Calling event");
                    PlayerDeathCallback?.Invoke(0);
                    alive = false;
                }
                //Ragdoll?
                break;
            case PlayerState.Win:
                if (!hasWon) { 
                    PlayerWinCallback?.Invoke(0);
                    hasWon = true;
                }
                //Victory dance?
                break;
        }               
    }

    private void FixedUpdate()
    {

        // decrease our cooldown amount once per frame
        if (teleportCooldown > 0) { teleportCooldown--; }
        if (teleportCharges < teleportChargesMax) { teleportCharges += Time.deltaTime / teleportRechargeRate; }
        if (teleportSlider) { teleportSlider.value = teleportCharges; }

        // manage our stamina slider
        if (staminaSlider) { staminaSlider.value = staminaCharges; }

        // make sure we reduce dash cooldown
        if (dashCoolDown > 0) { dashCoolDown -= 1; }

        if (dashTime > 0) { dashTime--; }
        if (dashTime <= 0) { dashDir = Vector3.zero; }
    }

    // movement
    void Movement()
    {
        // our players movement based off of the raw controller inputs
        if (canMove)
        {
            // declare our movement directions
            float pAxisV = Input.GetAxis("Vertical");
            float pAxisH = Input.GetAxis("Horizontal");

            // turn those into vector3 movement directions relative to the direction that the camera is facing
            moveV = cameraTransform.forward * pAxisV; moveH = cameraTransform.right * pAxisH;

            // rotate our body
            Vector3 rotMove = new Vector3(move.x, 0, move.z);
            if (Mathf.Abs(rotMove.x) > 0.1f || Mathf.Abs(rotMove.z) > 0.1f)
            {
                // playerRotationTransform.rotation = Quaternion.LookRotation((rotMove + playerRotationTransform.position) - playerRotationTransform.position, Vector3.up); }
                transform.rotation = Quaternion.LookRotation((rotMove + transform.position) - transform.position, Vector3.up); }
            // gravity modifications
            if (characterController.isGrounded && !Input.GetButtonDown("Jump"))
            {
                // normal gravity
                gravityValue = gravity * standMultiplier;
                // if we are grounded, set our acrobatic stamina charges to staminaChargesMax
                staminaCharges = staminaChargesMax;

                // reset velocity
                if (playerJumpVelocity < 0)
                { playerJumpVelocity = 0; }

                // make sure we know we're grounded on the animator
                //animationController.isGrounded = true;
            }
            else if (characterController.isGrounded && Input.GetButtonDown("Jump"))
            {
                // jump falling
                gravityValue = gravity * upJumpMultiplier;
                // note that we do not decrease remaining jumps because this was a free jump from the ground
            }
            else if (!characterController.isGrounded && Input.GetButtonDown("Jump") && staminaCharges > 0)
            {
                // jump falling
                gravityValue = gravity * upJumpMultiplier;
            }
            else if (characterController.velocity.y <= 0 && !characterController.isGrounded)
            {
                // normal falling
                gravityValue = gravity * fallMultiplier;
                // make sure we know we're grounded on the animator
                //animationController.isGrounded = false;
            } // upwards velocity custom gravity
            else if (characterController.velocity.y > 0)
            {
                // jump falling
                gravityValue = gravity * upJumpMultiplier;
                // make sure we know we're grounded on the animator
                //animationController.isGrounded = false;
            }

            // jumping
            if (Input.GetButtonDown("Jump") && characterController.isGrounded)
            {
                Debug.Log("Jump");
                playerJumpVelocity = Mathf.Sqrt(jumpVelocity * -3.0f * gravity);
            } else if (Input.GetButtonDown("Jump") && !characterController.isGrounded && staminaCharges > 0)
            {
                // if we have a jump remaining and are in the air, jump
                playerJumpVelocity = Mathf.Sqrt(jumpVelocity * -3.0f * gravity);
                // then remove a stamina charge
                staminaCharges--;
            }

            // dashing
            // horizontal dash
            if (Input.GetButtonDown("Dash"))
            {
                Debug.Log("dashing");
                // make sure our dash has cooled down, our stick is not idle, and we have a stamina charge to use
                if (dashCoolDown <= 0 && ((Mathf.Abs(pAxisV) > 0.1f) || (Mathf.Abs(pAxisH) > 0.1f)) && staminaCharges > 0)
                {
                    // spend a stamina charge
                    staminaCharges--;
                    // manage our dash cooldown
                    dashCoolDown = dashCoolDownMax;
                    // manage our dash time
                    dashTime = dashTimeMax;
                    // define our dash direction using our intensity
                    dashDir = new Vector3((moveV.x + moveH.x) * dashIntensity, 0f, (moveV.z + moveH.z) * dashIntensity);
                    // zero out gravity to make sure player can recover after their dash
                    verticalVelocity = 0; playerJumpVelocity = 0;
                    // give our dash a little bit of vertical force 
                    if (!characterController.isGrounded)
                    playerJumpVelocity = Mathf.Sqrt(dashJumpVelocity * -3.0f * gravity);
                }
            }


            // jump calculations
            playerJumpVelocity += gravityValue * Time.deltaTime;
            verticalVelocity = playerJumpVelocity;
            // final movement calculation
            move = new Vector3((moveH.x + moveV.x), verticalVelocity / moveSpeed, (moveH.z + moveV.z));
            move += dashDir; // add our dash
            // apply to the character controller
            characterController.Move(move * Time.deltaTime * moveSpeed);
            // set our speed on our animator
            //animationController.currentSpeed = move.x + move.z;
        }

        // swapping positions
        if (Input.GetAxis("RightTrigger") > 0.5f && teleportCooldown <= 0)
        {
            //Check if we have teleport charges and if valid target
            if (teleportCharges >= 1 && teleportTarget) {

                teleportCooldown = teleportCooldownMax; // set cooldown to max
                teleportCharges -= 1; // reduce charges by 1

                characterController.enabled = false; // disable the character controller
                Vector3 previousPos = transform.position; // store our position         
                GameObject swapTarget = teleportTarget.GetComponent<Targetable>().swapTarget;
                transform.position = swapTarget.transform.position; // move us to the swap target as defined by the targetable class               
                //swapTarget.GetComponent<GenericEnemyAI>().agent.enabled = false; // disable our nav agent so that it can be moved without reason
                //swapTarget.GetComponent<Rigidbody>().isKinematic = false;
                swapTarget.transform.position = previousPos; // target to previous pos and move the enemy                
                characterController.enabled = true; // enable character controller

                // spawn our two particle ffects
                if (teleportParticle) { 
                    Instantiate(teleportParticle, transform.position, Quaternion.identity, null);
                    Instantiate(teleportParticle, swapTarget.transform.position, Quaternion.identity, null);
                }
            }
        }
    }

    /// based on the tag of the thing we hit, react differently
    void OnCollisionEnter(Collision collision) { 
        if (collision.transform.tag == "Deadly") {
            playerState = PlayerState.Dead;
        }
    }

    private void OnTriggerEnter(Collider other) { 
        if (other.transform.tag == "Deadly") {
            playerState = PlayerState.Dead;
        }
        if (other.tag == "OOB") {
            playerState = PlayerState.Dead;
        }
        if (other.tag == "Win Zone") {
            playerState = PlayerState.Win;
        }
    }
}
