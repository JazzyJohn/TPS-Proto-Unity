using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class CharacterControllerM1A1 : MonoBehaviour {
	private float inputMagnitude;
	public float inputMagnitudeDampTime;
	
	private float walkStartAngle;
	public float walkStartAngleDampTime;
	
	private float walkStopAngle;
	public float walkStopAngleDampTime;
	
	private float horAimAngle;
	public float horAimAngleDampTime;
	
	private float inputAngle;
	public float inputAngleDampTime;
	private float inputAngleOld;
	private float inputAngularSpeed;

	private Animator animator;
	
	public Pawn character;
	private Rigidbody characterRigidbody;

	private float IsRU;
	private bool IsStopLU;
	private bool IsStopRU;

	private float Horizontal;
	private float Vertical;
	public float HorizontalDampTime;
	public float VerticalDampTime;

    private bool isStart=false;

	private float speedAimFactor = 1f;
	private float speedSprintFactor = 1f;

	private float IKWeight = 1f;

	private Transform camTransform;

	public IKcontroller ik;

	private Quaternion lookRotation;

	private bool isCrouch;

	private float IsNotAim;
	private float upperWeight;

	// Use this for initialization
	void Awake () {
		animator = GetComponent<Animator> ();
		camTransform = Camera.main.transform;
		characterRigidbody = character.rigidbody;
		isCrouch = false;
	}
	
	void OnAnimatorMove () {
		//Position
       
      
        if (!character.foxView.isMine)
        {
            return;
        }
        switch (character.GetState())
        {
            case CharacterState.Running:
            case CharacterState.Walking:
            case CharacterState.Sprinting:
            case CharacterState.Idle:
                Vector3 velocity = animator.deltaPosition / Time.deltaTime;
		        velocity.y = characterRigidbody.velocity.y;
		        characterRigidbody.velocity = velocity;
                break;
        }
		
	}
	
	// Update is called once per frame
	void Update () {
       
		GetAnimatorVariables ();
		GetInput ();
        if (isStart)
        {
			walkStartAngle = inputAngle;
			//horAimAngle = inputAngle;
		}
		if (inputMagnitude > 0.7f) walkStopAngle = inputAngle;
		
		ChooseStopAnimation ();
		
		SetCharacterPositionAndRotation ();
		
		SetAnimatorVariables ();

		SetAimWeight ();
	}

	void GetInput () {
        Vector3 nextMovement;
            if (character.foxView.isMine)
            {
         
                nextMovement = character.GetNextMovement();
              
            }
            else
            {
                nextMovement = character.GetVelocity();
                if (nextMovement.sqrMagnitude < 0.1f)
                {
                    nextMovement = Vector3.zero;
                }
             
            }

            Horizontal = Vector3.Dot(nextMovement.normalized, character.myTransform.right);
            Vertical = Vector3.Dot(nextMovement.normalized, character.myTransform.forward);
            if (Mathf.Abs(Horizontal) < 0.1f)
            {
                Horizontal = 0.0f;
            }
            if (Mathf.Abs(Vertical)< 0.1f)
            {
                Vertical = 0.0f;
            }
            if (!character.isAi)
            {
                

               // Debug.Log("Horizontal " + Horizontal + " Vertical " + Vertical);
            }
			//set inputMagnitude
			Vector2 speedVec =  new Vector2 (Horizontal, Vertical);
			inputMagnitude = Mathf.Clamp (speedVec.magnitude, 0, 1);
			
			//set inputAngle
			Vector3 stickDirection = new Vector3 (Horizontal, 0, Vertical);
			
			Vector3 CameraDirection = camTransform.forward;
			CameraDirection.y = 0.0f; // kill Y
			Quaternion referentialShift = Quaternion.FromToRotation (Vector3.forward, CameraDirection);
			


			Vector3 moveDirection;

			moveDirection = nextMovement.normalized;
            isStart = false;
            if (moveDirection.sqrMagnitude == 0)
            {
                inputAngle=0;
            }
             else
            {
                Vector3 axis = Vector3.Cross(transform.forward, moveDirection);
                if (inputAngle == 0)
                {
                    isStart = true;
                }
                inputAngle = Vector3.Angle(transform.forward, moveDirection) * (axis.y < 0 ? -1 : 1);
            }
			
			float deltaInputAngle = inputAngleOld - inputAngle;
			inputAngularSpeed = deltaInputAngle/Time.deltaTime;
			inputAngleOld = inputAngle;
			
			//Aim
			if (character.isAiming) {
				speedAimFactor = 0.5f;
				speedSprintFactor = 1f;
			}
			else {
				speedAimFactor = 1f;
				if (character.IsSprinting ()) speedSprintFactor = 2f;
				else speedSprintFactor = 1f;
			}
			
			Horizontal *= speedAimFactor * speedSprintFactor;
			Vertical *= speedAimFactor * speedSprintFactor;
		/*}
		else {
			Horizontal = Input.GetAxis ("Horizontal");
			Vertical = Input.GetAxis ("Vertical");
			
			//set inputMagnitude
			Vector2 speedVec =  new Vector2 (Horizontal, Vertical);
			inputMagnitude = Mathf.Clamp (speedVec.magnitude, 0, 1);
			
			//set inputAngle
			Vector3 stickDirection = new Vector3 (Horizontal, 0, Vertical);
			
			Vector3 CameraDirection = camTransform.forward;
			CameraDirection.y = 0.0f; // kill Y
			Quaternion referentialShift = Quaternion.FromToRotation (Vector3.forward, CameraDirection);
			
			Vector3 moveCompassVector3 = new Vector3 (0f, camTransform.rotation.eulerAngles.y, 0f);
			lookRotation = Quaternion.Euler (moveCompassVector3);
			
			Vector3 moveDirection;
			if (inputMagnitude > 0.2f) moveDirection = referentialShift * stickDirection;
			else moveDirection = referentialShift * Vector3.forward;
			
			Vector3 axis = Vector3.Cross (transform.forward, moveDirection);
			
			inputAngle = Vector3.Angle (transform.forward, moveDirection) * (axis.y < 0 ? -1 : 1);
			float deltaInputAngle = inputAngleOld - inputAngle;
			inputAngularSpeed = deltaInputAngle/Time.deltaTime;
			inputAngleOld = inputAngle;
			
			//Aim
			if (character.isAiming) {
				speedAimFactor = 0.5f;
				speedSprintFactor = 1f;
			}
			else {
				speedAimFactor = 1f;
				if (character.IsSprinting ()) speedSprintFactor = 2f;
				else speedSprintFactor = 1f;
			}
			
			Horizontal *= speedAimFactor * speedSprintFactor;
			Vertical *= speedAimFactor * speedSprintFactor;
			*/
			/*if (Input.GetKeyDown (KeyCode.LeftControl)) {
				isCrouch = !isCrouch;
				if (isCrouch) animator.SetTrigger (Animator.StringToHash ("CrouchTrigger"));
				else animator.SetTrigger (Animator.StringToHash ("UpTrigger"));
			}*/
		
	}
	
	void GetAnimatorVariables () {
		IsRU = animator.GetFloat (Animator.StringToHash ("IsRU"));
		IsNotAim = animator.GetFloat (Animator.StringToHash ("IsNotAim"));
	}
	
	void SetAnimatorVariables () {
		animator.SetFloat (Animator.StringToHash ("InputMagnitude"), inputMagnitude, inputMagnitudeDampTime, Time.deltaTime);
		animator.SetFloat (Animator.StringToHash ("WalkStartAngle"), walkStartAngle, walkStartAngleDampTime, Time.deltaTime);
		animator.SetFloat (Animator.StringToHash ("WalkStopAngle"), walkStopAngle, walkStopAngleDampTime, Time.deltaTime);
		animator.SetFloat (Animator.StringToHash ("HorAimAngle"), horAimAngle, horAimAngleDampTime, Time.deltaTime);
		animator.SetFloat (Animator.StringToHash ("InputAngle"), inputAngle, inputAngleDampTime, Time.deltaTime);
		animator.SetBool (Animator.StringToHash ("IsStopRU"), IsStopRU);
		animator.SetBool (Animator.StringToHash ("IsStopLU"), IsStopLU);
		animator.SetFloat (Animator.StringToHash ("Horizontal"), Horizontal, HorizontalDampTime, Time.deltaTime);
		animator.SetFloat (Animator.StringToHash ("Vertical"), Vertical, VerticalDampTime, Time.deltaTime);
		animator.SetFloat (Animator.StringToHash ("InputAngularSpeed"), inputAngularSpeed, inputAngleDampTime, Time.deltaTime);
	}

	void SetCharacterPositionAndRotation () {
		if (!(character.GetComponent<AIAgentComponent>())) {
			transform.rotation = lookRotation;
		}
	}

	void ChooseStopAnimation() {
		if (inputMagnitude > 0.95) {
			if (IsRU > 0.95) {
				IsStopLU = false;
				IsStopRU = true;
			}
			else {
				IsStopLU = true;
				IsStopRU = false;
			}
		}
	}

	void SetAimWeight () {
		if (Vertical > 1.1f) {
			IKWeight = Mathf.Lerp (IKWeight, 0f, Time.deltaTime * 8f);
			upperWeight = IKWeight;
		}
		else {
			IKWeight = 1f - IsNotAim;
			upperWeight = Mathf.Lerp (upperWeight, 1f, Time.deltaTime * 8f);
		}
		ik.SetWeight (IKWeight);
		animator.SetLayerWeight (1, upperWeight);
	}
}