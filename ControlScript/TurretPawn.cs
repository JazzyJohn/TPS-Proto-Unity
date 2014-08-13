using UnityEngine;
using System.Collections;

public class TurretPawn : Pawn {

	public Transform headTransform;
	private Quaternion startRotation;

    public override void StartPawn()
    {
		base.StartPawn ();
		startRotation =Quaternion.Inverse( myTransform.rotation)* headTransform.rotation;
	}

	public void FixedUpdate () {

	}
	void Update () {
		//Debug.Log (photonView.isSceneView);
		if (!isActive&&!isDead) {
			return;		
		}
//		Debug.Log (characterState);
        if (foxView.isMine)
        {

				UpdateSeenList();


				//if(aimRotation.sqrMagnitude==0){
				getAimRotation();
				/*}else{
					aimRotation = Vector3.Lerp(aimRotation,getAimRotation(CurWeapon.weaponRange), Time.deltaTime*10);
				}*/
				//Quaternion eurler = Quaternion.LookRotation(aimRotation-myTransform.position);
				

			//TODO: TEMP SOLUTION BEFORE NORMAL BONE ORIENTATION
			
			//animator.SetFloat("Pitch",pitchAngle);
			 SendNetUpdate();
		} else {
			ReplicatePosition();

			

		}
		if(headTransform!=null){
			headTransform.rotation=Quaternion.LookRotation(aimRotation-myTransform.position)* startRotation ; 
		}
		//		Debug.Log (characterState);
		UpdateAnimator ();
		DpsCheck ();
	}
}
