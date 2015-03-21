using UnityEngine;
using System.Collections;

public class dog_ThirdPersonController : ThirdPersonController {

	public bool canSprint;
	protected DogAnimationManager Anim;


	public KeyCode[] key = new KeyCode[4]{KeyCode.None, KeyCode.None, KeyCode.None, KeyCode.None};

	// Use this for initialization
	void Start () 
	{
		Anim = GetComponentInChildren<DogAnimationManager>();
	}

	public override	void UpdateSmoothedMovementDirection ()
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

		if (v<0f)
		{
			h = 0f;
		}
		else if (v>0f)
		{
			h = 0f;
		}
		else if (h>0f)
		{
			v = 0f;
		}
		else if (h>0)
		{
			v = 0f;
		}


		// Are we moving backwards or looking backwards
		if (v < -0.2f)
			MovingBack = true;
		else
			MovingBack = false;
		
		bool wasMoving= isMoving;
		isMoving = Mathf.Abs (h) > 0.1f || Mathf.Abs (v) > 0.1f;
		
		
		// Target direction relative to the camera
		Vector3 targetDirection=h*right + v * forward;
		
		
		
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
			if (MoveSpeed < pawn.groundWalkSpeed * 0.9f )
			{
				MoveDirection = targetDirection.normalized;
			}
			// Otherwise smoothly turn towards it
			else
			{
				MoveDirection = Vector3.RotateTowards(MoveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
				
				MoveDirection = MoveDirection.normalized;
			}
		}
		
		// Smooth the speed based on the current target direction
		float curSmooth= speedSmoothing * Time.deltaTime;
		
		// Choose target speed
		//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
		float targetSpeed= Mathf.Min(targetDirection.magnitude, 1.0f);

		
		// Pick speed modifier
		//Debug.Log (!Input.GetKey (KeyCode.LeftShift) &&! Input.GetKey (KeyCode.RightShift));
		if (pawn.isAiming ) 
		{
			targetSpeed *= pawn.groundWalkSpeed;
			if(isMoving){
				characterState = CharacterState.Running;
			}else{
				characterState = CharacterState.Idle;
			}
			
		} else if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && canSprint && h==0)
		{
			targetSpeed *= pawn.groundSprintSpeed;
			if(isMoving){
				characterState = CharacterState.Sprinting;
			}else{
				characterState = CharacterState.Idle;
			}
			
			
		}
		else
		{
			targetSpeed *= pawn.groundRunSpeed;
			if(isMoving){
				characterState = CharacterState.Walking;
			}else{
				characterState = CharacterState.Idle;
			}
		}
		
		MoveSpeed = Mathf.Lerp(MoveSpeed, targetSpeed, curSmooth);
		
		
		
		// Reset walk time start when we slow down
		if (MoveSpeed < pawn.groundWalkSpeed * 0.3f)
			walkTimeStart = Time.time;

		//Debug.Log (moveSpeed);
	}

	// Update is called once per frame
	void Update () 
	{

			if (Input.GetButtonDown ("Fire1")) {
				pawn.StartFire();
			}
			if(Input.GetButton("SpawnBot")){
				((AlphaDogPawn)pawn).Kick(2);
			}
			if(Input.GetButton("Jump")){
				((AlphaDogPawn)pawn).Kick(3	);
			}
	}


}
