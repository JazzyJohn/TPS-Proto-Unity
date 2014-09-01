using UnityEngine;
using System.Collections;

public class TurretPawn : Pawn {

	public Transform headTransform;
    public Quaternion startRotation;


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
		
		//		Debug.Log (characterState);
		UpdateAnimator ();
		DpsCheck ();
	}
    void LateUpdate()
    {
        base.LateUpdate();
        if (headTransform != null&&!isDead)
        {
            headTransform.rotation = Quaternion.LookRotation(aimRotation - myTransform.position) * startRotation;
        }
    }
}
