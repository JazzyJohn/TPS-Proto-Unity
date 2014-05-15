using UnityEngine;
using System.Collections;

public class RobotAnimationManager : AnimationManager
{
	public new string DeactiveName="Ready";
	public void DeActivation(){
		 animator.CrossFade(DeactiveName, 2.0f);
	}
	public new void Activation(){
		Debug.Log ("ctivate");
		animator.SetBool ("Pilotin", true);
	}

}