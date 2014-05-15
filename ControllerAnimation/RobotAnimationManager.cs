using UnityEngine;
using System.Collections;

public class RobotAnimationManager : AnimationManager
{
	public string DeactiveName="IDLE";
	public void DeActivation(){
		 animator.CrossFade(DeactiveName, 2.0f);
	}
	public void Activation(){
		animator.SetBool ("StandUp", true);
	}

}