using UnityEngine;
using System.Collections;

public class WeaponHolder : MonoBehaviour {

	public Animator animator;

	public Pawn owner;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		owner = transform.parent.GetComponent<Pawn> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnAnimatorIK()
	{
		
	//Debug.Log ("animIK");
		if(animator) {
			
			//if the IK is active, set the position and rotation directly to the goal. 
			//weight = 1.0 for the right hand means position and rotation will be at the IK goal (the place the character wants to grab)
			animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1.0f);
			
			
			//set the position and the rotation of the right hand where the external object is
			
			animator.SetIKPosition(AvatarIKGoal.LeftHand,owner.CurWeapon.leftHandHolder.position);
			
			
			//weight = 1.0 for the right hand means position and rotation will be at the IK goal (the place the character wants to grab)
			animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1.0f);
			animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1.0f);
			
			//set the position and the rotation of the right hand where the external object is
			
			animator.SetIKPosition(AvatarIKGoal.RightHand,owner.CurWeapon.curTransform.position);
			animator.SetIKRotation(AvatarIKGoal.RightHand,owner.CurWeapon.curTransform.rotation);
			
		}
		
		
		
	}	
}
