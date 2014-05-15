using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotPawn : Pawn {
	public float ActivationTime=2.0f;
	public Transform playerExitPositon;
	//Player get in robot
	public void new Activate(){
		((RobotAnimationManager)animator).Activation();
		StartCoroutine(WaitBeforeActive(ActivationTime));
	}
	
	public void WaitBeforeActive(float waitTime) {
        yield return new WaitForSeconds(waitTime);
		base.Activate();
	}
	//Player have left roboot
	public void new Deactivate(){
		((RobotAnimationManager)animator).DeActivation();
		base.DeActivate();
	}
}