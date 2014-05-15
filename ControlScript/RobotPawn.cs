using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotPawn : Pawn {
	public float ActivationTime=2.0f;
	public Transform playerExitPositon;
	//Player get in robot
	public new void  Activate(){
		((RobotAnimationManager)animator).Activation();
		Debug.Log ("ctivate");
		StartCoroutine(WaitBeforeActive(ActivationTime));
	}
	
	public IEnumerator WaitBeforeActive(float waitTime) {
        yield return new WaitForSeconds(waitTime);
		GetComponent<ThirdPersonController>().enabled = true;
		base.Activate();
	}
	//Player have left roboot
	public new void  Deactivate(){
		((RobotAnimationManager)animator).DeActivation();

	}
}