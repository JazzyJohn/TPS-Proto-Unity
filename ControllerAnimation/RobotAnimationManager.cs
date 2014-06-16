using UnityEngine;
using System.Collections;

public class RobotAnimationManager : AnimationManager
{
    protected void Awake()
    {
        base.Awake();
        aimPos.SetWeight(0.0f);
        }
	public new string DeactiveName="Ready";
	public void DeActivation(){
		animator.SetLayerWeight (1, 0.0f);
		animator.SetLayerWeight (2, 0.0f);
		animator.SetBool ("TakeToIdle", false);
		 animator.CrossFade(DeactiveName,1.0f);
         ToggleAimPos(false);	
	}
	public new void Activation(){
		Debug.Log ("ctivate");
		animator.SetBool ("TakeToIdle", true);
		animator.SetLayerWeight (1, 1.0f);
		animator.SetLayerWeight (2, 1.0f);
        ToggleAimPos(true);	
	}
	void OnAnimatorMove() {
	
		if (animator) {
			//Debug.Log (animator.deltaPosition);
		}	

	}
	public bool isActive(){
		return animator.GetBool ("TakeToIdle");
	}

}