using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlphaDogPawn : Pawn {


	private Animator Anim;
	public WeaponOfExtremities naturalWeapon;

	public List<HTHHitter> AttackType = new List<HTHHitter>();

	// Use this for initialization
	void Start () 
	{
		base.Start();
		naturalWeapon = GetComponent<WeaponOfExtremities>();
		Anim = transform.GetComponentInChildren<Animator>(); // Привязка аниматора
		animator = transform.GetComponentInChildren<DogAnimationManager>();
	}

	public override Vector3 getAimRotation()
	{

		if(photonView.isMine){
			if(isAi){
				if(enemy==null){
					aimRotation =myTransform.position+myTransform.forward*10;
				}else{
					aimRotation =Vector3.Lerp( aimRotation,enemy.myTransform.position,Time.deltaTime*10);
				}
			}else{
				if(cameraController.enabled ==false){
					aimRotation= myTransform.position +myTransform.forward*50;
					return aimRotation;
				}
				Camera maincam = Camera.main;
				Ray centerRay= maincam.ViewportPointToRay(new Vector3(.5f, 0.5f, 1f));
				
				Vector3 targetpoint = Vector3.zero;
				bool wasHit = false;
				float magnitude = aimRange;
				float range=aimRange;
				foreach( RaycastHit hitInfo  in Physics.RaycastAll(centerRay, aimRange))				
				{
					if(hitInfo.collider==myCollider)
					{
						continue;
					}
					
					//
					//Debug.DrawRay(centerRay.origin,centerRay.direction);
					targetpoint =hitInfo.point;
					range =(targetpoint-centerRay.origin).magnitude;
					if(range<magnitude){
						magnitude=range;
					}else{
						continue;
					}
					wasHit= true;
					curLookTarget= hitInfo.transform;
					//Debug.Log (curLookTarget);
				}
				
				if(!wasHit){
					//Debug.Log("NO HIT");
					curLookTarget= null;
					targetpoint = maincam.transform.forward*aimRange +maincam.ViewportToWorldPoint(new Vector3(.5f, 0.5f, 1f));
				}else{
					//Debug.Log(range.ToString()+(cameraController.normalOffset.magnitude+5));
					if(magnitude<cameraController.normalOffset.magnitude+1){
						targetpoint =maincam.transform.forward*aimRange +maincam.ViewportToWorldPoint(new Vector3(.5f, 0.5f, 1f));
					}
				}
				aimRotation=targetpoint; 
				
			}
			
			return aimRotation;
		}else{
			return aimRotation;
		}
	}

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
			if (photonView.isMine) {
				
				
				strafe = CalculateStarfe();
				//Debug.Log(characterState);
				speed =CalculateSpeed();
				//Debug.Log (speed);
				switch(characterState){
				case CharacterState.Idle:
					Anim.SetBool("Any", false);
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
					Anim.SetBool("Any", false);
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

	public void Kick(int i)
	{
		naturalWeapon.StartKick(AttackType[i]); 
//		Debug.Log ("ATtack");
		((DogAnimationManager) animator).AnyDo();
		if (photonView.isMine) {
			photonView.RPC("RPCKick",PhotonTargets.OthersBuffered,i);
		}
	}
	[RPC]
	public void RPCKick(int i){

		naturalWeapon.StartKick(AttackType[i]); 
		
		((DogAnimationManager) animator).AnyDo();
	}

	public override void ToggleAim(bool value)
	{
		if (value) {
						Kick (0);
		}
	}
	public override void StartFire(){
		Kick (1);
	}

	
}
