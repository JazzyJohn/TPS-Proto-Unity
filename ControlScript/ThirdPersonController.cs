using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    // Require a character controller to be attached to the same game object
    //@script RequireComponent(CharacterController)
    protected Pawn pawn;

    protected CharacterState characterState;

    // when pressing "Fire3" button (cmd) we start running

    // The gravity for the character
    public float gravity = 20.0f;
    // The gravity in controlled descent mode
    public float speedSmoothing = 10.0f;
    public float rotateSpeed = 500.0f;
    public float trotAfterSeconds = 3.0f;

    public bool canJump = true;

    private const float jumpRepeatTime = 0.05f;
    private const float jumpTimeout = 0.05f;
    private const float groundedTimeout = 0.25f;

    // The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
    public float LockCameraTimer { get; protected set; }

    // The current move direction in x-z
    public Vector3 MoveDirection { get; protected set; }
    // The current vertical speed
    private float verticalSpeed;
    // The current x-z move speed
    public float MoveSpeed { get; protected set; }

    // Are we jumping? (Initiated with jump button and not grounded yet)
    public bool Jumping { get; private set; }
    private bool doubleJump;
    public bool JumpingReachedApex { get; private set; }

    // Are we moving backwards (This locks the camera to not do a 180 degree spin)
    public bool MovingBack { get; protected set; }
    // Is the user pressing any keys?
    protected bool isMoving;
    // When did the user start walking (Used for going into trot after a while)
    protected float walkTimeStart;
    // Last time the jump button was clicked down
    protected float lastJumpButtonTime = -10.0f;
    // Last time we performed a jump
    private float lastJumpTime = -1.0f;
    //Last time DoubleJumping
    protected float lastDoubleTime = -1.0f;

    private Vector3 inAirVelocity = Vector3.zero;

    private float lastGroundedTime;


    protected bool isControllable = true;

    private void Awake()
    {
        MoveDirection = transform.TransformDirection(Vector3.forward);

        pawn = GetComponent<Pawn>();
        canJump = pawn.canJump;
    }

    public virtual void UpdateSmoothedMovementDirection()
    {
        Transform cameraTransform = Camera.main.transform;

        // Forward vector relative to the camera along the x-z plane	
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;

        // Right vector relative to the camera
        // Always orthogonal to the forward vector
        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        float v = InputManager.instance.GetAxisRaw("Vertical");
        float h = InputManager.instance.GetAxisRaw("Horizontal");

        // Are we moving backwards or looking backwards
        if (v < -0.2f)
            MovingBack = true;
        else
            MovingBack = false;

        bool wasMoving = isMoving;
        isMoving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;

        // Target direction relative to the camera
        Vector3 targetDirection = h * right + v * forward;

        // Lock camera for short period when transitioning moving & standing still
        LockCameraTimer += Time.deltaTime;
        if (isMoving != wasMoving)
            LockCameraTimer = 0.0f;

        // We store speed and direction seperately,
        // so that when the character stands still we still have a valid forward direction
        // moveDirection is always normalized, and we only update it if there is user input.
        if (targetDirection != Vector3.zero)
        {
            // If we are really slow, just snap to the target direction
            if (MoveSpeed < pawn.groundWalkSpeed * 0.9f)
            {
                MoveDirection = targetDirection.normalized;
            }
            // Otherwise smoothly turn towards it
            else
            {
                MoveDirection = targetDirection.normalized;
            }
        }

        // Smooth the speed based on the current target direction
        //float curSmooth = speedSmoothing * Time.deltaTime;

        // Choose target speed
        //* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
        float targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);

        // Pick speed modifier
        //Debug.Log (!Input.GetKey (KeyCode.LeftShift) &&! Input.GetKey (KeyCode.RightShift));

        bool crouch = InputManager.instance.GetButton("Crouch");

        if (pawn.isAiming)
        {
            if (crouch)
            {
                targetSpeed *= pawn.groundCrouchSpeed;
                characterState = CharacterState.Crouching;
            }
            else
            {
                if (isMoving)
                {
                    targetSpeed *= pawn.groundWalkSpeed;
                    characterState = CharacterState.Walking;
                }
                else
                {
                    targetSpeed *= 0;
                    characterState = CharacterState.Idle;
                }
            }
        }
        else if (InputManager.instance.GetButton("Sprint") && pawn.CanSprint())
        {
            targetSpeed *= pawn.groundRunSpeed;
            if (isMoving)
            {
                characterState = CharacterState.Sprinting;
            }
            else
            {
                characterState = CharacterState.Idle;
            }
        }
        else
        {
            if (crouch)
            {
                targetSpeed *= pawn.groundCrouchSpeed;
                characterState = CharacterState.Crouching;
            }
            else
            {
                if (isMoving)
                {
                    targetSpeed *= pawn.groundRunSpeed;
                    characterState = CharacterState.Running;
                }
                else
                {
                    targetSpeed *= 0;
                    characterState = CharacterState.Idle;
                }
            }
        }

        MoveSpeed = targetSpeed;

        // Reset walk time start when we slow down
        if (MoveSpeed < pawn.groundWalkSpeed * 0.3f)
            walkTimeStart = Time.time;
    }

    private void ApplyJumping()
    {
        // Prevent jumping too fast after each other
        if (lastJumpTime + jumpRepeatTime > Time.time)
        {
            //Debug.Log("timeOutReturn"+lastJumpTime+jumpRepeatTime +Time.time);
            return;
        }

        // Jump
        // - Only when pressing the button down
        // - With a timeout so you can press the button slightly before landing		

        if (canJump && !doubleJump && Time.time <= lastJumpButtonTime + jumpTimeout)
        {
            //Debug.Log(lastJumpButtonTime+jumpTimeout);
            verticalSpeed = CalculateJumpVerticalSpeed(pawn.jumpHeight);
            SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            verticalSpeed = 0;
        }

    }

    public float CalculateJumpVerticalSpeed(float targetJumpHeight)
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * targetJumpHeight * Pawn.gravity);
    }

    public void DidJump()
    {
        ///Debug.Log ("DidJump");
        JumpingReachedApex = false;
        lastJumpTime = Time.time;

        lastJumpButtonTime = -10;
        if (Jumping)
        {
            doubleJump = true;
            characterState = CharacterState.DoubleJump;
        }
        else
        {
            lastDoubleTime = Time.time;
            characterState = CharacterState.Jumping;
        }
        Jumping = true;
    }

    private void Update()
    {
        if (!isControllable)
        {
            // kill all inputs if not controllable.
            Input.ResetInputAxes();
        }

        if (InputManager.instance.GetButtonDown("Jump"))
        {
            lastJumpButtonTime = Time.time;
        }

        if (PlayerMainGui.IsMouseAV)
        {
            if (InputManager.instance.GetButtonDown("Fire1"))
            {
                pawn.StartFire();
            }
            if (InputManager.instance.GetButtonUp("Fire1"))
            {
                pawn.StopFire();
            }
            if (InputManager.instance.GetButtonDown("Grenade"))
            {
                pawn.StartGrenadeThrow();
            }
            /*	 if (InputManager.instance.GetButtonUp("Grenade"))
            {
			    pawn.ThrowGrenade ();
			}*/
            if (InputManager.instance.GetButtonDown("PreFire"))
            {
                pawn.StartPumping();
            }
            if (InputManager.instance.GetButtonUp("PreFire"))
            {
                pawn.StopPumping();
            }
            if (InputManager.instance.GetButtonUp("Reload"))
            {
                pawn.Reload();
            }
            if (InputManager.instance.GetButtonUp("KnockOut"))
            {
                pawn.StartKnockOut();
            }
            if (InputManager.instance.GetButtonDown("LegKick"))
            {
                pawn.Kick(0);
            }

            if (InputManager.instance.GetButtonUp("LegKick"))
            {
                pawn.StopKick();
            }

            float wheel = Input.GetAxis("Mouse ScrollWheel");
            if (wheel < 0)
            {
                pawn.PrevWeapon();
            }
            if (wheel > 0)
            {
                pawn.NextWeapon();
            }

            if (InputManager.instance.GetButtonUp("Taunt"))
            {
                pawn.PlayTaunt();
            }
            if (InputManager.instance.GetButtonDown("Skill1"))
            {
                pawn.UseSkill(0);
            }
            if (InputManager.instance.GetButtonUp("Skill1"))
            {
                pawn.UnUseSkill(0);
            }

            pawn.UpdateRotation(-InputManager.instance.GetMouseAxis("Mouse Y"),
                InputManager.instance.GetMouseAxis("Mouse X"));
        }
    }

    /// <summary>
    /// IS palyer Want DoubleJump
    /// </summary>
    protected virtual void ApplyDoubleJumping()
    {
        if (!pawn.isGrounded)
        {
            switch (pawn.GetState())
            {
                case CharacterState.DoubleJump:
                    if ((InputManager.instance.GetButton("Jump")) || InputManager.instance.GetButton("Sprint"))
                    {
                        characterState = CharacterState.DoubleJump;
                    }
                    else
                    {
                        characterState = CharacterState.Jumping;
                    }
                    break;
                case CharacterState.Jumping:
                case CharacterState.Sprinting:
                    if (lastDoubleTime < Time.time - 1.0f)
                    {
                        if ((InputManager.instance.GetButton("Jump")) || InputManager.instance.GetButton("Sprint"))
                        {
                            characterState = CharacterState.DoubleJump;
                            lastDoubleTime = Time.time;
                        }
                        else
                        {
                            characterState = CharacterState.Jumping;
                        }
                    }
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        MoveDirection = Vector3.zero;

        UpdateSmoothedMovementDirection();

        ApplyJumping();
        ApplyDoubleJumping();

        Vector3 movement = MoveDirection * MoveSpeed + new Vector3(0, verticalSpeed, 0) + inAirVelocity;

        pawn.Movement(movement, characterState);
    }

    public void DidLand()
    {
        //if(lastGroundedTime
        if (lastJumpTime + jumpRepeatTime >= Time.time)
        {
            return;
        }
        lastGroundedTime = 0;
        inAirVelocity = Vector3.zero;
        if (Jumping)
        {
            doubleJump = false;
            Jumping = false;
        }
    }

    public void WallLand()
    {
        //if(lastGroundedTime
        if (lastJumpTime + jumpRepeatTime >= Time.time)
        {
            return;
        }
        lastGroundedTime = 0;
        inAirVelocity = Vector3.zero;
        if (Jumping)
        {
            doubleJump = false;
            Jumping = false;
        }
    }

    public void WallJumpMessage()
    {
        lastDoubleTime = Time.time;
    }

    public bool IsMoving()
    {
        return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f;
    }

    public bool IsGroundedWithTimeout()
    {
        return lastGroundedTime + groundedTimeout > Time.time;
    }

    public void Reset()
    {
        gameObject.tag = "Player";
    }
}