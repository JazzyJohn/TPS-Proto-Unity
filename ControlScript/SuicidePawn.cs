using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



public class SuicidePawn : DamagebleObject {
	public float detonateRadius;
	
	public BaseDamage damage;
	
	private bool isDetonate = false;
	
	public GameObject hitParticle;

	public AudioClip exploseSound;

	new void Update () {
		if(isAi){
				if(enemy!=null){
					if((enemy.myTransform.position-myTransform.position).sqrMagnitude<detonateRadius*detonateRadius/4){
						Detonate();
						
					}
				}
		}
		
		base. Update ();
	}
	public override void KillIt(GameObject killer){
		if (isDead) {
			return;		
		}
		if(!isDetonate){
			Detonate();
		}
		base.KillIt(killer);
		
	}
	
	void Detonate(){
		Instantiate(hitParticle, hit.point, Quaternion.LookRotation(hit.normal));
		
		sControl.playClip (exploseSound);//звук взрыва

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, detonateRadius);
		Vector3 Position = transform.position;
		RaycastHit[] hits;
		for(int i=0;i < hitColliders.Length;i++) {

			//Debug.Log(hitColliders[i]);
			bool isHit = false;
			hits = Physics.RaycastAll(Position, hitColliders[i].transform.position);
			for (int j = 0; j < hits.Length; j++) {
				if(hits[j].collider.tag != "Player")
				{
					isHit = true;
					break;
				}
			}
			if (isHit) {
				DamagebleObject obj = hitColliders[i].GetComponent <DamagebleObject>();
				BaseDamage lDamage  = new BaseDamage(damage);
				lDamage.pushDirection = mTransform.forward;
				lDamage.hitPosition = mTransform.position;
				if (obj != null&&obj!=shootTarget) {
					obj.Damage(lDamage,owner);
				}	
			}
		}
	}
}