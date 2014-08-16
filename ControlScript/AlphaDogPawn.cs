using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlphaDogPawn : Pawn {


	



	// Update is called once per frame
	void FixedUpdate () 
	{
		base.FixedUpdate();

	}

	protected override void UpdateAnimator()
	{
		float strafe = 0;
		//Debug.Log (strafe);	
		float speed =0 ;
		//

		if (animator != null && animator.gameObject.activeSelf) {
            if (foxView.isMine)
            {
				
				
				strafe = CalculateStarfe();
				//Debug.Log(characterState);
				speed =CalculateSpeed();
				//Debug.Log (speed);
				switch(characterState){
				case CharacterState.Idle:
					animator.SetSome("Any", false);
					animator.ApllyMotion (0.0f, speed, strafe);
					break;
				case CharacterState.Running:
					sControl.playFullClip (stepSound);
					animator.ApllyMotion (2.0f, speed, strafe);
					break;
				case CharacterState.Sprinting:
					animator.Sprint();
					sControl.playFullClip (stepSound);
					animator.ApllyMotion (2.0f, speed, strafe);	
					break;
				case CharacterState.Walking:
					sControl.playFullClip (stepSound);
					animator.ApllyMotion (1.0f, speed, strafe);	
					break;					
				}
				//
			}else{
				strafe = CalculateRepStarfe();
				//Debug.Log (strafe);	
				speed =	CalculateRepSpeed();
				
				switch(nextState)
				{
				case CharacterState.Idle:
					animator.SetSome("Any", false);
					animator.ApllyMotion (0.0f, speed, strafe);
					break;
				case CharacterState.Running:
					sControl.playFullClip (stepSound);
					animator.ApllyMotion (2.0f, speed, strafe);

					break;
				case CharacterState.Sprinting:
					sControl.playFullClip (stepSound);
					animator.ApllyMotion (2.0f, speed, strafe);
					break;
				case CharacterState.Walking:
					sControl.playFullClip (stepSound);
					animator.ApllyMotion (1.0f, speed, strafe);
					break;					
				}
				characterState = nextState;
			}
			if (isLookingAt) {
				Vector3 laimRotation = aimRotation;
				/*if(animator.isWeaponAimable()){
					Quaternion diference = Quaternion.FromToRotation(CurWeapon.muzzlePoint.forward,myTransform.forward);

					Vector3 direction= laimRotation-myTransform.position;
				
					laimRotation =(diference *direction.normalized)*direction.magnitude +myTransform.position;
				}*/
				
				animator.SetLookAtPosition (laimRotation);
				//animator.animator.SetLookAtWeight (1, 0.5f, 0.7f, 0.0f, 0.5f);
				
			}
		}
	}

	
	
	
}
