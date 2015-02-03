using UnityEngine;
using System;
using System.Collections;
using RootMotion.FinalIK;

public enum AnimType {TAUNT};
public enum AnimDirection {Front,Back};

public delegate void UpdateFinished();
public class AnimationManager : MonoBehaviour
{
    private float
        directionAxisZ,
        directionAxisX;

    public Animator animator;
	public IKcontroller aimPos;
	private bool shouldAim= true;
	public Rigidbody rb;
    protected RaggdollRoot raggdollRoot;
    public bool isDead = false;
    public Pawn pawn;
	protected void Awake()
    {
        animator = GetComponent<Animator>();
		if (aimPos == null) {
			aimPos = gameObject.GetComponentInChildren<IKcontroller> ();
            aimPos.AddAction(FinisedIKUpdate);
		}
     
        pawn =transform.root.GetComponent<Pawn>();
        if (animator == null)
            Debug.LogError("Animator not find!", this);

		animator.logWarnings = false;
        raggdollRoot = gameObject.GetComponentInChildren<RaggdollRoot>();
    }


    public void FinisedIKUpdate()
    {
		if(pawn!=null&&pawn.CurWeapon!=null){
			pawn.CurWeapon.UpdateCahedPosition();
			
		}
    }
   /* /// <summary>
    /// Задает или возвращает значение воспроизведения скорости анимации
    /// </summary>
    public float AnimationSpeed
    {
        get
        {
            return animator.speed;
        }
        set
        {
            animator.speed = value;
        }
    }*/
    /// <summary>
    /// Служит для определения направления анимации, значения принимаются от -2 до +2,
    /// при этом от -1 до +1 - хотьба, от -2 до +2 - бег
    /// </summary>
    /// <param name="directionAxisX">Анимация влево/вправо</param>
    /// <param name="directionAxisZ">Анимация вперед/назад</param>
    public void ApllyMotion(float Speed,float forward, float direction)
    {
        //so... this line makes advanced idle
        //axis return 0f when it not pressed
        //animator.SetBool("Idle", (directionAxisX == 0f && directionAxisZ == 0f) ? true : false);
		//Debug.Log (directionAxisZ);
		animator.SetFloat("Speed", Speed);
		animator.SetFloat("Direction", -direction);
		animator.SetFloat ("Forward", forward);
    }
    /// <summary>
    /// Служит для определения выбора одно из вариантов позы смерти.
    /// Принимает числа больше ноля. Каждое число соотсветсвует позе. 
    /// </summary>
    /// <param name="poseDeath"></param>
  /*  public void ApllyDeath(int poseDeath)
    {
        if (poseDeath < 0)
            Debug.LogError("Posture of death can only be non-negative", this);
        animator.SetInteger("Death", poseDeath);
    }
    /// <summary>*/
    /// Служит для определения выбора одно из вариантов перелазанья.
    /// Может быть вызвано только при движении. Принимает числа больше ноля.
    /// Каждое число соотсветсвует позе. 
    /// </summary>
    /// <param name="climbLedgePose"></param>
 /*   public void ApllyClimbLedge(int climbLedgePose)
    {
        if (climbLedgePose < 0)
            Debug.LogError("Posture of clim Ledge can only be non-negative", this);
        animator.SetInteger("ClimbLedge", climbLedgePose);
    }*/
    /// <summary>
    /// Служит для начала анимаций прыжка. Принимает значение true или false
    /// </summary>
    /// <param name="jump"></param>
    public virtual void ApllyJump(bool jump)
    {	
		//if (animator.layerCount >= 3) {
						//AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo ();
		switch(jump)
		{
		case true:
			animator.SetBool ("StandUp", false);
			animator.SetBool ("Jump", true);
			animator.SetBool ("Grounded", false);
			break;
		case false:
			animator.SetBool ("Jump", false);
			animator.SetBool ("Grounded", true);
			animator.SetBool ("StandUp", true);
			animator.SetFloat ("TimerFree", 0);
			break;
		}
			//	}
				
    }
	public virtual void DoubleJump(){
		animator.SetBool ("StandUp", false);
		animator.SetBool ("Jump", true);
		animator.SetBool ("Grounded", false);
	
	}
	/*public bool GetJump(){
		return animator.GetBool("Jump");
	} */
	
	
    /// <summary>
    /// Reset all animation to default
    /// </summary>
   /* public void ResetAnimation()
    {
        animator.SetInteger("Death", 0);
        animator.SetInteger("ClimbLedge", 0);
        animator.SetBool("Jump", false);
        animator.Play("Motion");
    }*/
	/// <summary>
	/// Wall run animation
	/// </summary>
	public virtual void WallAnimation(bool leftW,bool rightW,bool frontW)
	{

		if (leftW || rightW || frontW) {
			animator.SetBool ("Jump", false);
			animator.SetBool ("Grounded", false);
			animator.SetBool ("StandUp", false);
		}
		animator.SetBool("WallRunL", leftW);
		
		animator.SetBool("WallRunR", rightW);
		
		animator.SetBool("WallRunUp", frontW);
	}
	//for Sprint Additional animation like jetpack in subclass
	public virtual void Sprint(){

	}
	//for freefall no DoubleJump Additional animation like jetpack in subclass
	public virtual void FreeFall(){
		
	}
	//Pulling weapon up near wall;
	public void WeaponDown(bool value){
		/*if (aimPos != null) {
						if (value && shouldAim) {
								shouldAim = false;

								aimPos.SetWeight(0.0f);

						}
						if (!value && !shouldAim) {
								shouldAim = true;
								aimPos.EvalToWeight(1.0f);
						}
		}
		animator.SetBool("wall_stop", value);	*/
	}
	//HTH attack anim switch
	public virtual void StartAttackAnim(string name){
		//Debug.Log (name);

		animator.SetTrigger(name.ToString());	 //Заменил с була на тригер
	}
	public void StopAttackAnim(string name){
		animator.SetBool(name, false);	
		
	}
	/// <summary>
    /// IS we under IK controll
    /// </summary>
	/*public bool IsIk(){
        return aimPos != null && aimPos.IsIk();
	
	}*/
    /// <summary>
    /// Turn on and of IK of aiming
    /// </summary>
    public void ToggleAimPos(bool state){
       // Debug.Log(state);
		return;
       /* if (aimPos != null) {

            if (state)
            {
                aimPos.EvalToWeight(1.0f);
            }else{
                aimPos.SetWeight(0.0f);
            }
        }*/
    }
	/// <summary>
    /// Short cut to turn off IK
    /// </summary>
	public virtual  void IKOff(){
		ToggleAimPos(false);
	}
	/// <summary>
    /// Short cut to turn on IK
    /// </summary>
    public virtual void IKOn()
    {
		ToggleAimPos(true);
	}
	
	public void StartPullingUp(){



        ToggleAimPos(false);	
		animator.SetBool("PullUp", true);	
		SetNotMainLayer (0.0f);
	}
	public void FinishPullingUp(){
        ToggleAimPos(true);	
		animator.SetBool("PullUp", false);	
		SetNotMainLayer (1.0f);
	}
	private void SetNotMainLayer(float weight){
		for(int i =1; i<animator.layerCount;i++){
			animator.SetLayerWeight (i, weight);
			
			
		}

	}
	//toogle aiming state
	public void ToggleAim(bool aim){
		animator.SetBool("AIM", aim);
	}
	//Setting is that pull long or short
	public void SetLong(bool longPull){
		animator.SetBool("LongPull", longPull);
	}
	//Check if weapon look forward or in air because of near wall
	/*public  bool isWeaponAimable(){
		return !animator.GetBool("wall_stop");
	}*/
	//COntol aim behavieor of object
	public virtual void SetLookAtPosition(Vector3 position){
		if (aimPos != null) {
						aimPos.aimPosition = position;
		}
	}
	public void SetWeaponType(int State){
		if (State == 0) {
			return;
		}
        //Debug.Log(State);
            
		animator.SetInteger ("GunType",State);
	}
	public void ReloadStart(){
	
		animator.SetBool ("Reload",true);
		IKOff ();
	}
    public void ReloadStop()
    {

        animator.SetBool("Reload",false);
        IKOn();
    }

	public void StartDeath(AnimDirection direction){
        SetNotMainLayer(0.0f);

        Debug.Log("StartDeath");
        isDead = true;
      // Debug.Log(direction);
        IKOff();
        aimPos.IKShutDown();
        DollOn();
		switch(direction){
			case AnimDirection.Front:
                animator.SetTrigger("Front_death");
				break;
			case AnimDirection.Back:
                animator.SetTrigger("Back_death");
				break;
              
		
		}
	}
    public void ChangeWeaponNow()
    {
       
        //pawn.ChangeWeapon();
    }
	public void ShootAnim(){
		//animator.SetTrigger ("Shoot");
	}
	public void PlayTaunt(string tauntName){
		SetNotMainLayer (0.0f);
		animator.Play(tauntName);
	}
	public void StopTaunt(){
		SetNotMainLayer (1.0f);
		transform.parent.SendMessage ("StopTaunt", SendMessageOptions.DontRequireReceiver);
		
	}
	//I'm not sure in necessity of this function maybe we should change logic of some script to get rid of it . JazzyJohn.
	public void SetSome(string some, bool value){
		animator.SetBool(some, value);
	}
    //I'm not sure in necessity of this function maybe we should change logic of some script to get rid of it . JazzyJohn.
    public void SetSome(string some)
    {
        animator.SetTrigger(some);
    }
	//This for natural shooting weapons like bug tail, we know about shot only from animation
	public void WeaponShoot(){
		transform.parent.SendMessage ("WeaponShoot", SendMessageOptions.DontRequireReceiver);
	}
	public void StartShootAniamtion(string animName){
		animator.SetBool (animName,true);
	}
	public void StopShootAniamtion(string animName){
		animator.SetBool(animName,false);
	}
	public void KnockOut() {
        Debug.Log("KnockOut");
        DollOn();
    }
    public void DollOn(){
        Debug.Log("DROPDEAD");
        aimPos.IKShutDown();
         animator.enabled = false;
         if (raggdollRoot != null)
         {
             raggdollRoot.Start();
         }
    }
    public void DollOff()
    {

        animator.enabled = true;
        aimPos.IKTurnOn();
        if (raggdollRoot != null)
        {
            raggdollRoot.Stop();
        }
    }
    public void StandUp()
    {
        if (isDead)
        {
            return;
        }
        animator.enabled = true;
        if (raggdollRoot != null)
        {
            raggdollRoot.enabled = false;
        }
        SetNotMainLayer(0.0f);
        animator.Play("GetUp_fromBack");

    }
    public void StandUpFinish()
    {

        DollOff();
        SetNotMainLayer(1.0f);
        transform.parent.SendMessage("StandUpFinish", SendMessageOptions.DontRequireReceiver);
    }
    public void SetMuzzle(Transform point)
    {
        aimPos.SetMuzzle( point);
    }

    void Update()
    {
        if (!aimPos.ActiveAim()&& pawn != null && pawn.CurWeapon != null)
        {
            pawn.CurWeapon.UpdateCahedPosition();

        }
    }
    public void PutGrenadeAway()
    {
        Debug.Log("PutGrenadeAway");
        pawn.StopGrenadeThrow();
    }
	/*[Serializable]
	public class Leg
	{
		public LegBone[] LegBones;
		public Transform ZeroPoint;
		[HideInInspector]
		public float FootHeight;
		[HideInInspector]
		public float BonesLenght;
		public void LegStep(){
		
			LegBone LegBoneFirst = LegBones[0];
			LegBone LegBoneEnd = LegBones[LegBones.Length - 1];
			Vector3 RayPoint = new Vector3(LegBoneEnd.Bone.position.x, LegBoneFirst.Bone.position.y, LegBoneEnd.Bone.position.z);
			Ray ray = new Ray(RayPoint, Vector3.down);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, BonesLenght + FootHeight)){
				float Distance = Vector3.Distance(RayPoint, hit.point);
				float LegDistance = Vector3.Distance(LegBoneFirst.Bone.position, LegBoneEnd.Bone.position)+FootHeight;
				float Angle;
				Angle = 90f*(LegDistance-Distance)/LegDistance;
				LegBoneFirst.Bone.rotation *= Quaternion.AngleAxis(Angle, LegBoneFirst.GetWorldDirection());
				for (int j = 1; j < LegBones.Length-1; j++){
					LegBones[j].Bone.rotation *= Quaternion.AngleAxis(-2f*Angle, LegBones[j].GetWorldDirection());
				}
				LegBoneEnd.Bone.rotation = Quaternion.FromToRotation(LegBoneEnd.GetDirection(), hit.normal) * LegBoneEnd.Bone.rotation;
              //  Debug.Log(LegBoneEnd.Bone.rotation);
			}
		}
		public void LegSet(){
			int Lenght = LegBones.Length-1;
			float[] BoneLength = new float[Lenght];
			for (int j = 0; j < Lenght; j++) {
				BoneLength[j] = Vector3.Distance(LegBones[j].Bone.position, LegBones[j+1].Bone.position);
				BonesLenght+=BoneLength[j];

			}
			for (int j = 0; j < Lenght; j++)
				LegBones[j].Factor = 90f/BonesLenght;
			//			LegBones[0].Factor = (BoneLength[0]/BonesLenght)*90f/BonesLenght;
        	this.FootHeight = LegBones[Lenght].Bone.position.y - ZeroPoint.position.y;

		}
	}
	[Serializable]
	public class LegBone
	{
		public Transform Bone;
		[HideInInspector]
		public float Factor;
		public Vector Direction;
		public Vector3 GetDirection(){
			Vector3 Up = new Vector3();
			if (Direction == Vector.Right)Up = Bone.right;
			else if (Direction == Vector.Left)Up = -Bone.right;
			else if (Direction == Vector.Up)Up = Bone.up;
			else if (Direction == Vector.Down)Up = -Bone.up;
			else if (Direction == Vector.Forward)Up = Bone.forward;
			else if (Direction == Vector.Back)Up = -Bone.forward;
			return Up;
		}
		public Vector3 GetWorldDirection(){
			Vector3 Up = new Vector3();
			if (Direction == Vector.Right)Up = Vector3.right;
			else if (Direction == Vector.Left)Up = Vector3.left;
			else if (Direction == Vector.Up)Up = Vector3.up;
			else if (Direction == Vector.Down)Up = Vector3.down;
			else if (Direction == Vector.Forward)Up = Vector3.forward;
			else if (Direction == Vector.Back)Up = Vector3.back;
			return Up;
		}
	}
	public enum Vector {Up, Down, Left, Right, Forward, Back};
    */
}
