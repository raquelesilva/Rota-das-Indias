using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System;
namespace FancyCrab.CustomPackages.FirstPersonController
{
    public class FirstPersonController : MonoBehaviour
    {
       public static FirstPersonController instance;

        [BoxGroup("Input"), SerializeField] private InputReader inputReader;
        #region Movement Variables

        [BoxGroup("Movement"), SerializeField] private float walkSpeed = 5f;
        [BoxGroup("Movement"), SerializeField] private float maxVelocityChange = 10f;

        // Internal Variables
        private bool isWalking = false;

        #region Sprint


        [BoxGroup("Movement/Sprint")] public bool enableSprint = true;
        [BoxGroup("Movement/Sprint"), ShowIf(nameof(enableSprint)), SerializeField] private bool unlimitedSprint = false;
        [BoxGroup("Movement/Sprint"), ShowIf(nameof(enableSprint)), SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
        [BoxGroup("Movement/Sprint"), ShowIf(nameof(enableSprint)), SerializeField] private float sprintSpeed = 7f;
        [BoxGroup("Movement/Sprint"), ShowIf(nameof(enableSprint)), SerializeField] private float sprintDuration = 5f;
        [BoxGroup("Movement/Sprint"), ShowIf(nameof(enableSprint)), SerializeField] private float sprintCooldown = .5f;
        [BoxGroup("Movement/Sprint"), ShowIf(nameof(enableSprint)), SerializeField] private float sprintFOV = 80f;
        [BoxGroup("Movement/Sprint"), ShowIf(nameof(enableSprint)), SerializeField] private float sprintFOVStepTime = 10f;

        // Sprint Bar
        [BoxGroup("Movement/Sprint/Bar"), ShowIf(nameof(enableSprint)), SerializeField] private bool useSprintBar = true;
        [BoxGroup("Movement/Sprint/Bar"), ShowIf(nameof(useSprintBar)), SerializeField] private bool hideBarWhenFull = true;
        [BoxGroup("Movement/Sprint/Bar"), ShowIf(nameof(useSprintBar)), SerializeField] private Image sprintBar;
        [BoxGroup("Movement/Sprint/Bar"), ShowIf(nameof(useSprintBar)), SerializeField] private float sprintBarWidthPercent = .3f;
        [BoxGroup("Movement/Sprint/Bar"), ShowIf(nameof(useSprintBar)), SerializeField] private float sprintBarHeightPercent = .015f;

        // Internal Variables
        private bool isSprinting = false;
        private float sprintRemaining;
        private float sprintBarWidth;
        private float sprintBarHeight;
        private bool isSprintCooldown = false;
        private float sprintCooldownReset;

        #endregion

        #region Jump

        [BoxGroup("Movement/Jump")] public bool enableJump = true;
        [BoxGroup("Movement/Jump"), ShowIf(nameof(enableJump)), SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [BoxGroup("Movement/Jump"), ShowIf(nameof(enableJump)), SerializeField] private float jumpPower = 5f;

        // Internal Variables
        private bool isGrounded = false;

        #endregion

        #region Crouch

        [BoxGroup("Movement/Crouch")] public bool enableCrouch = true;
        [BoxGroup("Movement/Crouch"), ShowIf(nameof(enableCrouch)), SerializeField] private bool holdToCrouch = true;
        [BoxGroup("Movement/Crouch"), ShowIf(nameof(enableCrouch)), SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
        [BoxGroup("Movement/Crouch"), ShowIf(nameof(enableCrouch)), SerializeField] private float crouchHeight = .75f;
        [BoxGroup("Movement/Crouch"), ShowIf(nameof(enableCrouch)), SerializeField] private float speedReduction = .5f;

        // Internal Variables
        private bool isCrouched = false;
        private float originalHeight = 2;

        #endregion
        #endregion

        #region Head Bob

        #endregion

        private Rigidbody rb;
        private CapsuleCollider capsuleCollider;
        private bool canMove;
        // Exposed read-only states for CameraController
        public bool CanMove => canMove;
        public bool IsWalking => isWalking;
        public bool IsSprinting => isSprinting;
        public bool IsCrouched => isCrouched;
        // Expose a few tuning values used by head-bob
        public float SprintSpeed => sprintSpeed;
        public float SpeedReduction => speedReduction;

        private Vector2 movement;
        private bool sprintPressed;
        private bool crouchHeld;

        private void OnEnable()
        {
            PlayerStateHandler.OnPlayerStateChanged += PlayerStateCallback;
            inputReader.Movement += MovementCallback;
            inputReader.Jump += JumpCallback;
            inputReader.Sprint += SprintCallback;
            inputReader.Crouch += CrouchCallback;
        }

        private void CrouchCallback()
        {
            if (!canMove || !enableCrouch) return;

            if (!holdToCrouch)
            {
                Crouch();
            }
            else
            {
                crouchHeld = !crouchHeld;

                if (crouchHeld)
                {
                    if (!isCrouched)
                    {
                        Crouch();
                    }
                }
                else
                {
                    if (isCrouched)
                    {
                        Crouch();
                    }
                }
            }
        }

        private void SprintCallback(bool state)
        {
            sprintPressed = state;
        }
        private void JumpCallback()
        {
            if (!canMove) return;

            if (enableJump && isGrounded)
            {
                Jump();
            }
        }
        private void MovementCallback(Vector2 vector)
        {
            movement = vector;
        }

        private void OnDestroy()
        {
            PlayerStateHandler.OnPlayerStateChanged -= PlayerStateCallback;
            inputReader.Movement -= MovementCallback;
            inputReader.Jump -= JumpCallback;
            inputReader.Sprint -= SprintCallback;
            inputReader.Crouch -= CrouchCallback;
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;

            rb = GetComponent<Rigidbody>();
            capsuleCollider = GetComponent<CapsuleCollider>();

            // Set internal variables
            originalHeight = capsuleCollider.height;

            if (!unlimitedSprint)
            {
                sprintRemaining = sprintDuration;
                sprintCooldownReset = sprintCooldown;
            }
        }

        private void TeleportPlayerCallback(Transform transform)
        {
            this.transform.position = transform.position;
        }

        
        private void PlayerStateCallback(PlayerStates newState)
        {
            if(!newState.Equals(PlayerStates.Playing)) rb.angularVelocity = Vector3.zero;
            canMove = newState == PlayerStates.Playing;
        }

        void Start()
        {
            // Camera / crosshair handled by CameraController

            #region Sprint Bar

            if (useSprintBar)
            {
                sprintBar.gameObject.SetActive(true);

                float screenWidth = Screen.width;
                float screenHeight = Screen.height;

                sprintBarWidth = screenWidth * sprintBarWidthPercent;
                sprintBarHeight = screenHeight * sprintBarHeightPercent;

                sprintBar.rectTransform.sizeDelta = new Vector3(sprintBarWidth - 2, sprintBarHeight - 2, 0f);
            }
            else
            {
              //  sprintBar.gameObject.SetActive(false);
            }

            #endregion
        }

        private void Update()
        {
            // Camera movement and zoom moved to CameraController

            #region Sprint

            if (enableSprint)
            {
                if (isSprinting)
                {
                    // Drain sprint remaining while sprinting
                    if (!unlimitedSprint)
                    {
                        sprintRemaining -= 1 * Time.deltaTime;
                        if (sprintRemaining <= 0)
                        {
                            isSprinting = false;
                            isSprintCooldown = true;
                        }
                    }
                }
                else
                {
                    // Regain sprint while not sprinting
                    sprintRemaining = Mathf.Clamp(sprintRemaining += 1 * Time.deltaTime, 0, sprintDuration);
                }

                // Handles sprint cooldown 
                // When sprint remaining == 0 stops sprint ability until hitting cooldown
                if (isSprintCooldown)
                {
                    sprintCooldown -= 1 * Time.deltaTime;
                    if (sprintCooldown <= 0)
                    {
                        isSprintCooldown = false;
                    }
                }
                else
                {
                    sprintCooldown = sprintCooldownReset;
                }

                // Handles sprintBar 
                if (useSprintBar && !unlimitedSprint)
                {
                    float sprintRemainingPercent = sprintRemaining / sprintDuration;
                    sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
                }
            }

            #endregion

            CheckGround();

            // Head bob handled by CameraController
        }

        void FixedUpdate()
        {
            #region Movement

            if (canMove)
            {
                // Calculate how fast we should be moving
                Vector3 targetVelocity = new Vector3(movement.x, 0, movement.y);

                // Checks if player is walking and isGrounded
                // Will allow head bob
                if (targetVelocity.x != 0 || targetVelocity.z != 0 && isGrounded)
                {
                    isWalking = true;
                }
                else
                {
                    isWalking = false;
                }

                // All movement calculations while sprint is active
                if (enableSprint && sprintPressed && sprintRemaining > 0f && !isSprintCooldown)
                {
                    targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed;

                    // Apply a force that attempts to reach our target velocity
                    Vector3 velocity = rb.linearVelocity;
                    Vector3 velocityChange = (targetVelocity - velocity);
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                    velocityChange.y = 0;

                    // Player is only moving when valocity change != 0
                    // Makes sure fov change only happens during movement
                    if (velocityChange.x != 0 || velocityChange.z != 0)
                    {
                        isSprinting = true;

                        if (isCrouched)
                        {
                            Crouch();
                        }

                    }

                    rb.AddForce(velocityChange, ForceMode.VelocityChange);
                }
                // All movement calculations while walking
                else
                {
                    isSprinting = false;

                    targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

                    // Apply a force that attempts to reach our target velocity
                    Vector3 velocity = rb.linearVelocity;
                    Vector3 velocityChange = (targetVelocity - velocity);
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                    velocityChange.y = 0;

                    rb.AddForce(velocityChange, ForceMode.VelocityChange);
                }
            }

            #endregion
        }

        // Sets isGrounded based on a raycast sent straigth down from the player object
        private void CheckGround()
        {
            Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
            Vector3 direction = transform.TransformDirection(Vector3.down);
            float distance = .75f;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
            {
                Debug.DrawRay(origin, direction * distance, Color.red);
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }

        private void Jump()
        {
            if (isGrounded)
            {
                rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
                isGrounded = false;
            }

            if (isCrouched && !holdToCrouch)
            {
                Crouch();
            }
        }

        private void Crouch()
        {
            if (isCrouched)
            {
                capsuleCollider.height = originalHeight;
                walkSpeed /= speedReduction;

                isCrouched = false;
            }
            else
            {
                capsuleCollider.height = crouchHeight;
                walkSpeed *= speedReduction;

                isCrouched = true;
            }
        }

        
    }
}