using UnityEngine;
using System.Collections;

public enum AnimType {TAUNT};
public class AnimationManager : MonoBehaviour
{
    private float
        directionAxisZ,
        directionAxisX;

    public Animator animator;
	public IKcontroller aimPos;
	private bool shouldAim= true;
	public Rigidbody rb;
	protected void Awake()
    {
        animator = GetComponent<Animator>();
		if (aimPos == null) {
			aimPos = gameObject.GetComponentInChildren<IKcontroller> ();
		}
        if (animator == null)
            Debug.LogError("Animator not find!", this);

		animator.logWarnings = false;
    }

    //debug
    private void Update()
    {
        #region Read and delete this!!!
       /* directionAxisZ = Input.GetAxis("Vertical");
        directionAxisX = Input.GetAxis("Horizontal");
        float runButton = Input.GetAxis("Run") ;

        directionAxisX += directionAxisX > 0 ? runButton : directionAxisX < 0 ? -runButton : 0;
        directionAxisZ += directionAxisZ > 0 ? runButton : directionAxisZ < 0 ? -runButton : 0;

        ApllyMotion(directionAxisX, directionAxisZ);
        ApllyJump(Input.GetKeyDown(KeyCode.Space));

        if (Input.GetKeyDown(KeyCode.Alpha1))
            ApllyDeath(Random.Range(1, 3));
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ApllyClimbLedge(Random.Range(1, 4));
        else
            ApllyClimbLedge(0);

        //reset animation
        if (Input.GetKeyDown(KeyCode.Tab))        
            ResetAnimation();*/
        #endregion
    }
    /// <summary>
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
    }
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
    public void ApllyDeath(int poseDeath)
    {
        if (poseDeath < 0)
            Debug.LogError("Posture of death can only be non-negative", this);
        animator.SetInteger("Death", poseDeath);
    }
    /// <summary>
    /// Служит для определения выбора одно из вариантов перелазанья.
    /// Может быть вызвано только при движении. Принимает числа больше ноля.
    /// Каждое число соотсветсвует позе. 
    /// </summary>
    /// <param name="climbLedgePose"></param>
    public void ApllyClimbLedge(int climbLedgePose)
    {
        if (climbLedgePose < 0)
            Debug.LogError("Posture of clim Ledge can only be non-negative", this);
        animator.SetInteger("ClimbLedge", climbLedgePose);
    }
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
	public bool GetJump(){
		return animator.GetBool("Jump");
	} 
    /// <summary>
    /// Reset all animation to default
    /// </summary>
    public void ResetAnimation()
    {
        animator.SetInteger("Death", 0);
        animator.SetInteger("ClimbLedge", 0);
        animator.SetBool("Jump", false);
        animator.Play("Motion");
    }
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
		if (aimPos != null) {
						if (value && shouldAim) {
								shouldAim = false;

								aimPos.SetWeight(0.0f);

						}
						if (!value && !shouldAim) {
								shouldAim = true;
								aimPos.EvalToWeight(1.0f);
						}
		}
		animator.SetBool("wall_stop", value);	
	}
	//HTH attack anim switch
	public virtual void StartAttackAnim(string name){
		//Debug.Log (name);

		animator.SetTrigger(name.ToString());	 //Заменил с була на тригер
	}
	public void StopAttackAnim(string name){
		animator.SetBool(name, false);	
		
	}
	public void StartPullingUp(){
		if (aimPos != null) {

				aimPos.SetWeight(0.0f);
				
			
		}
		animator.SetBool("PullUp", true);	
		SetNotMainLayer (0.0f);
	}
	public void FinishPullingUp(){
		if (aimPos != null) {
			
			aimPos.EvalToWeight(1.0f);
			
			
		}
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
//		animator.SetBool("AIM", aim);
	}
	//Setting is that pull long or short
	public void SetLong(bool longPull){
		animator.SetBool("LongPull", longPull);
	}
	//Check if weapon look forward or in air because of near wall
	public bool isWeaponAimable(){
		return !animator.GetBool("wall_stop");
	}
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
		animator.SetInteger ("GunType",State);
	}
	public void ReloadStart(){
	
		animator.SetTrigger ("Reload");
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
	//This for natural shooting weapons like bug tail, we know about shot only from animation
	public void WeaponShoot(){
		transform.parent.SendMessage ("WeaponShoot", SendMessageOptions.DontRequireReceiver);
	}
	public void StartShootAniamtion(string animName){
		animator.SetTrigger (animName);
	}
	public void StopShootAniamtion(string animName){
		animator.SetTrigger (animName+"_end");
	}

}
