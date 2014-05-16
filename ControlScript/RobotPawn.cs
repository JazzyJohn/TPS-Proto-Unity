using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotPawn : Pawn {
	public float ActivationTime=2.0f;
	public Transform playerExitPositon;
	
	//Player get in robot
	public new void  Activate(){
		((RobotAnimationManager)animator).Activation();
		characterState=CharacterState.Activate;
		GetComponent<ThirdPersonCamera>().enabled = true;
		GetComponent<ThirdPersonCamera>().Reset ();
		StartCoroutine(WaitBeforeActive(ActivationTime));
	}
	
	public IEnumerator WaitBeforeActive(float waitTime) {
        yield return new WaitForSeconds(waitTime);
		GetComponent<ThirdPersonController>().enabled = true;
		ivnMan.GenerateWeaponStart();
		base.Activate();
	}
	//Player have left robot
	public new void  Deactivate(){
		characterState=CharacterState.DeActivate;
		((RobotAnimationManager)animator).DeActivation();
		GetComponent<ThirdPersonController>().enabled = false;
		GetComponent<ThirdPersonCamera>().enabled = false;
		ivnMan.TakeWeaponAway();
		StopMachine ();
	}
	public override AfterSpawnAction(){
		nextState = CharacterState.Jumping;
	}
	public new void DidLand(){
		if(!isActive){
			characterState=CharacterState.DeActivate;
		}
		//Debug.Log ("LAND");
		lastTimeOnWall = -10.0f;
		//photonView.RPC("JumpChange",PhotonTargets.OthersBuffered,false);
	}
	protected override UpdateAnimator(){
		if (animator != null && animator.gameObject.activeSelf) {
		if (photonView.isMine) {
			switch(nextState){
				case CharacterState.DeActivate:
					if(characterState!=nextState){
						((RobotAnimationManager)animator).DeActivation();
					}
				break;
				case CharacterState.Activate:
					if(characterState!=nextState){
						((RobotAnimationManager)animator).Activation();
					}
				break;
			}
					
		
		}
		}
		base.UpdateAnimator();
	}
}