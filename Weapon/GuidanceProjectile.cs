using UnityEngine;
using System;
//We don't want to our projectile fly infinite time
[RequireComponent (typeof (DelayDestroyedObject))]
public class GuidanceProjectile : BaseProjectile {
	public Pawn target;
	protected void Update() {
		Quaternion rotation = Quaternion.LookRotation((target.position -mTransform.position).normalized);
		mRigidBody.velocity =  rotation*mRigidBody.velocity;
		base.Update();
		
	}



}