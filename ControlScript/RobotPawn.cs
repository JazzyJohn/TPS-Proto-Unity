using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotPawn : Pawn {
	public float ActivationTime=2.0f;
	public Transform playerExitPositon;
	public bool isPilotIn = false;
	//Player get in robot
	public new void  Activate(){
		((RobotAnimationManager)animator).Activation();
		_rb.constraints = RigidbodyConstraints.FreezeRotation;
		isPilotIn = true;
		isActive = false;
		characterState=CharacterState.Activate;
		GetComponent<ThirdPersonCamera>().enabled = true;
		GetComponent<ThirdPersonCamera>().Reset ();
		StartCoroutine(WaitBeforeActive(ActivationTime));
	}
	
	public IEnumerator WaitBeforeActive(float waitTime) {

        yield return new WaitForSeconds(waitTime);
		GetComponent<ThirdPersonController>().enabled = true;
		ivnMan.GenerateWeaponStart();
		_rb.isKinematic = false;
		isActive = true;
		_rb.detectCollisions = true;
		for (int i =0; i<myTransform.childCount; i++) {
			myTransform.GetChild(i).gameObject.SetActive(true);
		}
		photonView.RPC("RPCActivate",PhotonTargets.OthersBuffered);
		//base.Activate();
	}
	//Player have left robot
	public new void  DeActivate(){
		characterState=CharacterState.DeActivate;
		isPilotIn = false;
		((RobotAnimationManager)animator).DeActivation();
		GetComponent<ThirdPersonController>().enabled = false;
		GetComponent<ThirdPersonCamera>().enabled = false;
		ivnMan.TakeWeaponAway();
		StopMachine ();
		_rb.constraints = RigidbodyConstraints.FreezeAll;
		_rb.velocity = Vector3.zero;
		//Debug.Log ("ROBOT");
	}


	public override void  AfterSpawnAction(){
		characterState = CharacterState.Jumping;
	
		//nextState = CharacterState.DeActivate;
	}
	public new void DidLand(){
		if(!isActive){
			characterState=CharacterState.DeActivate;
		}
		//Debug.Log ("LAND");
		lastTimeOnWall = -10.0f;
		//photonView.RPC("JumpChange",PhotonTargets.OthersBuffered,false);
	}
	protected override void UpdateAnimator(){
		//Debug.Log (isGrounded);
		if (animator != null && animator.gameObject.activeSelf) {
			if(!photonView.isMine){

				RobotAnimationManager roboanim =(RobotAnimationManager)animator;
				if(!roboanim.isActive()&&isPilotIn){
					roboanim.Activation();
				}
				if(roboanim.isActive()&&!isPilotIn){
					roboanim.DeActivation();
				}
			}
					
		
		
		}
		base.UpdateAnimator();
	}
	//NetworkSection
	public new void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{

		base.OnPhotonSerializeView (stream,info);
		//Debug.Log (this);
		if (stream.isWriting)
		{


			stream.SendNext(isPilotIn);
			//stream.SendNext(netIsGround);
			//stream.SendNext(animator.GetJump());
			
		}
		else
		{

			isPilotIn =(bool)stream.ReceiveNext();
			//isGrounded =(bool) stream.ReceiveNext();
			//animator.ApllyJump((bool)stream.ReceiveNext());
			//Debug.Log (wallState);
		}
	}
}