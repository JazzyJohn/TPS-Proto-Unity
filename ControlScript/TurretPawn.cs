using UnityEngine;
using System.Collections;

public class TurretPawn : Pawn {

	public Transform headTransform;
	private Quaternion startRotation;

	void Awake(){
		base.Awake ();
		startRotation = headTransform.rotation;
	}

	public void FixedUpdate () {

	}
	void Update () {
		//Debug.Log (photonView.isSceneView);
		if (!isActive) {
			return;		
		}

		if (photonView.isMine||(isAi &&PhotonNetwork.isMasterClient)) {

				UpdateSeenList();


				//if(aimRotation.sqrMagnitude==0){
				getAimRotation(CurWeapon.weaponRange);
				/*}else{
					aimRotation = Vector3.Lerp(aimRotation,getAimRotation(CurWeapon.weaponRange), Time.deltaTime*10);
				}*/
				//Quaternion eurler = Quaternion.LookRotation(aimRotation-myTransform.position);
				if(headTransform!=null){
				headTransform.rotation=Quaternion.LookRotation(aimRotation-myTransform.position)* startRotation ; 
				}

			//TODO: TEMP SOLUTION BEFORE NORMAL BONE ORIENTATION
			
			//animator.SetFloat("Pitch",pitchAngle);

		} else {

			myTransform.position = Vector3.Lerp(myTransform.position, correctPlayerPos, Time.deltaTime *SYNC_MULTUPLIER);
			myTransform.rotation = Quaternion.Lerp(myTransform.rotation, correctPlayerRot, Time.deltaTime * SYNC_MULTUPLIER);

		}
//		Debug.Log (characterState);
		UpdateAnimator ();
	}
}
