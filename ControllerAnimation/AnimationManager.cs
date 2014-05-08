using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour
{
    private float
        directionAxisZ,
        directionAxisX;

    public Animator animator;

	public Rigidbody rb;
    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Animator not find!", this);
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
						if (!jump) {
								animator.SetBool ("Jump", false);
								animator.SetBool ("Grounded", true);
								animator.SetBool ("StandUp", true);
						}else{
								animator.SetBool ("StandUp", false);
								animator.SetBool ("Jump", true);
								animator.SetBool ("Grounded", false);
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
	public virtual void FreeFall(){
		
	}
	public void WeaponDown(bool value){
		animator.SetBool("wall_stop", value);	
	}
	public void StartAttackAnim(string name){
		//Debug.Log (name);
		animator.SetBool(name, true);	

	}
	public void StopAttackAnim(string name){
		animator.SetBool(name, false);	
		
	}
	public void StartPullingUp(){
		animator.SetBool("PullUp", true);	
		SetNotMainLayer (0.0f);
	}
	public void FinishPullingUp(){
		animator.SetBool("PullUp", false);	
		SetNotMainLayer (1.0f);
	}
	private void SetNotMainLayer(float weight){
		for(int i =1; i<animator.layerCount;i++){
			animator.SetLayerWeight (i, weight);
			
			
		}

	}
	public void SetLong(bool longPull){
		animator.SetBool("LongPull", longPull);
	}
	public bool isWeaponAimable(){
		return !animator.GetBool("wall_stop");
	}
}
