// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;


public class ThirdPersonController : MonoBehaviour 
{

// Require a character controller to be attached to the same game object
//@script RequireComponent(CharacterController)

public AnimationClip idleAnimation;
public AnimationClip walkAnimation;
public AnimationClip runAnimation;
public AnimationClip jumpPoseAnimation;

public float walkMaxAnimationSpeed = 0.75f;
public float trotMaxAnimationSpeed = 1.0f;
public float runMaxAnimationSpeed = 1.0f;
public float jumpAnimationSpeed = 1.15f;
public float landAnimationSpeed = 1.0f;



private Animation _animation;

private Pawn pawn;

private Transform myTransform;

private CharacterState characterState;


// when pressing "Fire3" button (cmd) we start running


public float inAirControlAcceleration= 3.0f;


// The gravity for the character
public float gravity= 20.0f;
// The gravity in controlled descent mode
public float speedSmoothing= 10.0f;
public float rotateSpeed= 500.0f;
public float trotAfterSeconds= 3.0f;

public bool canJump= true;

private float jumpRepeatTime= 0.05f;
private float jumpTimeout= 0.05f;
private float groundedTimeout= 0.25f;

// The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
private float lockCameraTimer= 0.0f;

// The current move direction in x-z
private Vector3 moveDirection= Vector3.zero;
// The current vertical speed
private float verticalSpeed= 0.0f;
// The current x-z move speed
private float moveSpeed= 0.0f;
	
// Are we jumping? (Initiated with jump button and not grounded yet)
private bool jumping= false;
private bool doubleJump= false;
private bool jumpingReachedApex= false;

// Are we moving backwards (This locks the camera to not do a 180 degree spin)
private bool movingBack= false;
// Is the user pressing any keys?
private bool isMoving= false;
// When did the user start walking (Used for going into trot after a while)
private float walkTimeStart= 0.0f;
// Last time the jump button was clicked down
private float lastJumpButtonTime= -10.0f;
// Last time we performed a jump
private float lastJumpTime= -1.0f;


// the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
private float lastJumpStartHeight= 0.0f;


private Vector3 inAirVelocity= Vector3.zero;

private float lastGroundedTime= 0.0f;


private bool isControllable= true;

void  Awake ()
{
	moveDirection = transform.TransformDirection(Vector3.forward);
    
	
	_animation = GetComponent<Animation>();
	pawn= GetComponent<Pawn>();
	myTransform = transform;
		canJump = pawn.canJump;
	/*if(!_animation)
		//Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
	
	/*
public AnimationClip idleAnimation;
public AnimationClip walkAnimation;
public AnimationClip runAnimation;
public AnimationClip jumpPoseAnimation;	
	*/
	/*
	if(!idleAnimation) {
		_animation = null;
		Debug.Log("No idle animation found. Turning off animations.");
	}
	if(!walkAnimation) {
		_animation = null;
		Debug.Log("No walk animation found. Turning off animations.");
	}
	if(!runAnimation) {
		_animation = null;
		Debug.Log("No run animation found. Turning off animations.");
	}
	if(!jumpPoseAnimation && canJump) {
		_animation = null;
		Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
	}
	*/		
}


void  UpdateSmoothedMovementDirection ()
{
	Transform cameraTransform= Camera.main.transform;
	
	
	// Forward vector relative to the camera along the x-z plane	
	Vector3 forward= cameraTransform.TransformDirection(Vector3.forward);
	forward.y = 0;
	forward = forward.normalized;

	// Right vector relative to the camera
	// Always orthogonal to the forward vector
	Vector3 right= new Vector3(forward.z, 0, -forward.x);

	float v= Input.GetAxisRaw("Vertical");
	float h= Input.GetAxisRaw("Horizontal");



	// Are we moving backwards or looking backwards
	if (v < -0.2f)
		movingBack = true;
	else
		movingBack = false;
	
	bool wasMoving= isMoving;
	isMoving = Mathf.Abs (h) > 0.1f || Mathf.Abs (v) > 0.1f;

			
	// Target direction relative to the camera
		Vector3 targetDirection=h*right + v * forward;

	
	
		// Lock camera for short period when transitioning moving & standing still
		lockCameraTimer += Time.deltaTime;
		if (isMoving != wasMoving)
			lockCameraTimer = 0.0f;

		// We store speed and direction seperately,
		// so that when the character stands still we still have a valid forward direction
		// moveDirection is always normalized, and we only update it if there is user input.
		if (targetDirection != Vector3.zero)
		{
			// If we are really slow, just snap to the target direction
			if (moveSpeed < pawn.groundWalkSpeed * 0.9f )
			{
				moveDirection = targetDirection.normalized;
			}
			// Otherwise smoothly turn towards it
			else
			{
				moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
				
				moveDirection = moveDirection.normalized;
			}
		}
		
		// Smooth the speed based on the current target direction
		float curSmooth= speedSmoothing * Time.deltaTime;
		
		// Choose target speed
		//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
		float targetSpeed= Mathf.Min(targetDirection.magnitude, 1.0f);
	
		
		
		// Pick speed modifier
		//Debug.Log (!Input.GetKey (KeyCode.LeftShift) &&! Input.GetKey (KeyCode.RightShift));
		if (!Input.GetKey (KeyCode.LeftShift) &&! Input.GetKey (KeyCode.RightShift))
		{
			targetSpeed *= pawn.groundRunSpeed;
		
				if(isMoving){
					characterState = CharacterState.Running;
				}else{
					characterState = CharacterState.Idle;
				}
		}
		else if (Time.time - trotAfterSeconds > walkTimeStart)
		{
			targetSpeed *= pawn.groundTrotSpeed;
			characterState = CharacterState.Trotting;
		}
		else
		{
			targetSpeed *= pawn.groundWalkSpeed;
				if(isMoving){
					characterState = CharacterState.Walking;
				}else{
					characterState = CharacterState.Idle;
				}
		}
		
		moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
	
		// Reset walk time start when we slow down
		if (moveSpeed < pawn.groundWalkSpeed * 0.3f)
			walkTimeStart = Time.time;
	
	

		
}


void  ApplyJumping (){
	// Prevent jumping too fast after each other
		if (lastJumpTime + jumpRepeatTime > Time.time) {
			//Debug.Log("timeOutReturn"+lastJumpTime+jumpRepeatTime +Time.time);
			return;
		}

		// Jump
		// - Only when pressing the button down
		// - With a timeout so you can press the button slightly before landing		

		if (canJump && Time.time <= lastJumpButtonTime + jumpTimeout) {
			//Debug.Log(lastJumpButtonTime+jumpTimeout);
			verticalSpeed = CalculateJumpVerticalSpeed (pawn.jumpHeight);
			SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
		}else{
			verticalSpeed=0;
		}
	
}


public float CalculateJumpVerticalSpeed ( float targetJumpHeight  )
{
	// From the jump height and gravity we deduce the upwards speed 
	// for the character to reach at the apex.
	return Mathf.Sqrt(2 * targetJumpHeight * Pawn.gravity);
}

	public void DidJump ()
	{
		///Debug.Log ("DidJump");
		jumpingReachedApex = false;
		lastJumpTime = Time.time;

		lastJumpStartHeight = transform.position.y;
		lastJumpButtonTime = -10;
		if (jumping == true && !doubleJump) {
				doubleJump= true;
				characterState = CharacterState.DoubleJump;
		} else {
				characterState = CharacterState.Jumping;	
		}
		jumping = true;
	}
	void Update ()
	{
		if (!isControllable)
		{
			// kill all inputs if not controllable.
			Input.ResetInputAxes();
		}
		
		if (Input.GetButtonDown ("Jump"))
		{
			lastJumpButtonTime = Time.time;
			
			
		}
		if (PlayerMainGui.IsMouseAV) {
						if (Input.GetButtonDown ("Fire1")) {
				
								pawn.StartFire ();
						}
						if (Input.GetButtonUp ("Fire1")) {
				
								pawn.StopFire ();
						}
						if (Input.GetButtonUp ("Reload")) {
							
							pawn.Reload ();
						}
		
						float wheel = Input.GetAxis ("Mouse ScrollWheel");
		
						if (wheel < 0) {
								pawn.GetComponent<InventoryManager> ().PrevWeapon ();
						}
						if (wheel > 0) {
								pawn.GetComponent<InventoryManager> ().NextWeapon ();
						}
		}

}
void FixedUpdate ()
{
	moveDirection = Vector3.zero;
	
	
	UpdateSmoothedMovementDirection();
	
	
	// Apply jumping logic
	ApplyJumping ();
		//Debug.Log (jumping.ToString()+doubleJump);
	// Calculate actual motion
	Vector3 movement= moveDirection * moveSpeed + new Vector3 (0, verticalSpeed, 0) + inAirVelocity;
	
		//Debug.Log (movement.magnitude);
		pawn.Movement (movement, characterState);


			
	

	// ANIMATION sector
	
	


	
}
public void DidLand(){
	//if(lastGroundedTime
	if(	lastJumpTime +jumpRepeatTime>=	 Time.time){
			return;
	}
	lastGroundedTime = 0;
	inAirVelocity = Vector3.zero;
	if (jumping) {
			doubleJump = false;
			jumping = false;
			//
	}
}

public void WallLand(){
		//if(lastGroundedTime
		if(	lastJumpTime +jumpRepeatTime>=	 Time.time){
			return;
		}
		lastGroundedTime = 0;
		inAirVelocity = Vector3.zero;
		if (jumping) {
			doubleJump = false;
			jumping = false;
			//
		}
	}

public float GetSpeed ()
{
	return moveSpeed;
}

public bool IsJumping ()
{
	return jumping;
}


public Vector3 GetDirection ()
{
	return moveDirection;
}

public bool IsMovingBackwards ()
{
	return movingBack;
}

public float GetLockCameraTimer ()
{
	return lockCameraTimer;
}

public bool IsMoving ()
{
	 return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f;
}

public bool HasJumpReachedApex ()
{
	return jumpingReachedApex;
}

public bool IsGroundedWithTimeout ()
{
	return lastGroundedTime + groundedTimeout > Time.time;
}

public void Reset ()
{
	gameObject.tag = "Player";
}

}