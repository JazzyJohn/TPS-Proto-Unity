using UnityEngine;
using System.Collections;

public class RobotAnimationManager : AnimationManager
{
	public new string DeactiveName="Ready";
	public void DeActivation(){
		animator.SetLayerWeight (1, 0.0f);
		animator.SetLayerWeight (2, 0.0f);
		animator.SetBool ("TakeToIdle", false);
		 animator.CrossFade(DeactiveName,1.0f);

	}
	public new void Activation(){
		//Debug.Log ("ctivate");
		animator.SetBool ("TakeToIdle", true);
		animator.SetLayerWeight (1, 1.0f);
		animator.SetLayerWeight (2, 1.0f);
	}
	void OnAnimatorMove() {
	
		if (animator) {
			//Debug.Log (animator.deltaPosition);
		}	

	}

}