using UnityEngine;
using System.Collections;

public class DogAnimationManager : AnimationManager {

	public float timer;
	public float TimerMax;

	protected string State;

	private int Idle = Animator.StringToHash("Base.IDLE");

	void Start()
	{
		animator = GetComponent<Animator>();
		timer = 0;
	}

	public override void ApllyJump(bool jump)
	{

	}

	public override void FreeFall(){
		
	}

	public override void Sprint()
	{

	}

	void Update()
	{
		float v= Input.GetAxisRaw("Vertical");
		float h= Input.GetAxisRaw("Horizontal");

		animator.SetFloat("RightOrLeft", h);
		animator.SetFloat("MoveF", v);

		if (v != 0f || h != 0f)
			AnyDo();

		timer+=Time.deltaTime;
		if (timer >= TimerMax)
		{
			string rand = "Idle"+Random.Range(1, 6);
			animator.SetTrigger(rand);
			timer=0;
		}
	}

	public void AnyDo()
	{
		animator.SetBool("Any", true);
		timer=0;
	}
}
