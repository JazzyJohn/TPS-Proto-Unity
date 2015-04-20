using UnityEngine;

// Закомментированные строчки кода отключены по подозрению в бесполезности
public class CharacterControllerM1A1 : MonoBehaviour
{
    //private float inputMagnitude;
    //public float inputMagnitudeDampTime;

    //private float walkStartAngle;
    //public float walkStartAngleDampTime;

    //private float walkStopAngle;
    //public float walkStopAngleDampTime;

    //private float horAimAngle;
    //public float horAimAngleDampTime;

    //private float inputAngle;
    //public float inputAngleDampTime;
    //private float inputAngleOld;
    //private float inputAngularSpeed;

    public float IKSetSpeed = 8f;
    public float DelayAfterJump = 0.5f;

    private Animator animator;

    public Pawn character;
    private Rigidbody characterRigidbody;

    private float IsRU;
    private bool IsStopLU;
    private bool IsStopRU;

    private float horizontal;
    private float vertical;
    public float HorizontalDampTime;
    public float VerticalDampTime;

    private float IKWeight = 1f;

    private Transform camTransform;

    [HideInInspector] public IKcontroller ik;

    //private Quaternion lookRotation;


    //private float IsNotAim;
    private float upperWeight;
    private float inputValue;
    private float lastTimeInAir;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        camTransform = Camera.main.transform;
        characterRigidbody = character.rigidbody;
    }

    private void OnAnimatorMove()
    {
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
            case CharacterState.Crouching:
                Vector3 velocity = animator.deltaPosition / Time.deltaTime;
                velocity.y = characterRigidbody.velocity.y;
                characterRigidbody.velocity = velocity;
                break;
        }
    }

    private void Update()
    {
        GetAnimatorVariables();
        GetInput();
        //if (isStart)
        //{
        //    walkStartAngle = inputAngle;
        //}
        //if (inputMagnitude > 0.7f) 
        //    walkStopAngle = inputAngle;

        if (!character.IsGrounded())
        {
            lastTimeInAir = Time.time;
          
        }

        ChooseStopAnimation();

        //SetCharacterPositionAndRotation();

        SetAnimatorVariables();

        SetAimWeight();
    }

    private void GetInput()
    {
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

        horizontal = Vector3.Dot(nextMovement.normalized, character.myTransform.right);
        vertical = Vector3.Dot(nextMovement.normalized, character.myTransform.forward);

        if (Mathf.Abs(horizontal) < 0.1f)
        {
            horizontal = 0.0f;
        }
        if (Mathf.Abs(vertical) < 0.1f)
        {
            vertical = 0.0f;
        }
        
        //set inputMagnitude
        //Vector2 speedVec = new Vector2(Horizontal, Vertical);
        //inputMagnitude = Mathf.Clamp(speedVec.magnitude, 0, 1);
        inputValue = Mathf.Abs(horizontal) + Mathf.Abs(vertical);

        // Чтобы не пытался бежать в воздухе. Проверка на crouch нужна, так как на момент написания кода
        // все еще был баг, что при приседе сразу после прыжка персонаж не был grounded
        if (!character.IsCrouch() && !character.IsGrounded())
        {
            inputValue = 0f;
            vertical = 0f;
            horizontal = 0f;
          //  Debug.Log("RESET GROUNDED");
        }

        Vector3 CameraDirection = camTransform.forward;
        CameraDirection.y = 0.0f; // kill Y

        if (character.isAiming)
        {
            vertical /= 2f;
            horizontal /= 2f;
        }

        //Vector3 moveDirection;

        //moveDirection = nextMovement.normalized;
        //isStart = false;
        //if (moveDirection.sqrMagnitude == 0)
        //{
        //    inputAngle = 0;
        //}
        //else
        //{
        //    Vector3 axis = Vector3.Cross(transform.forward, moveDirection);
        //    if (inputAngle == 0)
        //    {
        //        isStart = true;
        //    }
        //    inputAngle = Vector3.Angle(transform.forward, moveDirection) * Mathf.Sign(axis.y);
        //}

        //float deltaInputAngle = inputAngleOld - inputAngle;
        //inputAngularSpeed = deltaInputAngle / Time.deltaTime;
        //inputAngleOld = inputAngle;

      

        //Aim
        
        animator.SetBool("Sprint", character.IsSprinting());
        animator.SetBool("Crouch", character.IsCrouch());
    }

    private void GetAnimatorVariables()
    {
        IsRU = animator.GetFloat(Animator.StringToHash("IsRU"));
        //IsNotAim = animator.GetFloat(Animator.StringToHash("IsNotAim"));
    }

    private void SetAnimatorVariables()
    {
        //animator.SetFloat(Animator.StringToHash("InputMagnitude"), inputMagnitude, inputMagnitudeDampTime,
            //Time.deltaTime);
        animator.SetFloat(Animator.StringToHash("InputValue"), inputValue);
        //animator.SetFloat(Animator.StringToHash("WalkStartAngle"), walkStartAngle, walkStartAngleDampTime,
        //    Time.deltaTime);
        //animator.SetFloat(Animator.StringToHash("WalkStopAngle"), walkStopAngle, walkStopAngleDampTime, Time.deltaTime);
        //animator.SetFloat(Animator.StringToHash("HorAimAngle"), horAimAngle, horAimAngleDampTime, Time.deltaTime);
        //animator.SetFloat(Animator.StringToHash("InputAngle"), inputAngle, inputAngleDampTime, Time.deltaTime);
        animator.SetBool(Animator.StringToHash("IsStopRU"), IsStopRU);
        animator.SetBool(Animator.StringToHash("IsStopLU"), IsStopLU);
        animator.SetFloat(Animator.StringToHash("Horizontal"), horizontal, HorizontalDampTime, Time.deltaTime);
        animator.SetFloat(Animator.StringToHash("Vertical"), vertical, VerticalDampTime, Time.deltaTime);
        //animator.SetFloat(Animator.StringToHash("InputAngularSpeed"), inputAngularSpeed, inputAngleDampTime,
            //Time.deltaTime);
    }

    //private void SetCharacterPositionAndRotation()
    //{
    //    if (!(character.GetComponent<AIAgentComponent>()))
    //    {
    //        transform.rotation = lookRotation;
    //    }
    //}

    private void ChooseStopAnimation()
    {
        if (inputValue > 0.95)
        {
            IsStopLU = IsRU <= 0.95;
            IsStopRU = IsRU > 0.95;
        }
    }

    private void SetAimWeight()
    {
        if (character.IsSprinting())
        {
            if (lastTimeInAir + DelayAfterJump > Time.time)
            {
                return;
            }

            IKWeight = Mathf.Lerp(IKWeight, 0f, Time.deltaTime * IKSetSpeed);
            upperWeight = IKWeight;
            
        }
        else
        {
            upperWeight = Mathf.Lerp(upperWeight, 1f, Time.deltaTime * IKSetSpeed);
            IKWeight = upperWeight;
        }
        ik.SetMotionWeight(IKWeight);
      //  animator.SetLayerWeight(1, upperWeight);
    }
}