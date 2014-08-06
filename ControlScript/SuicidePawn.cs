using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



public class SuicidePawn : Pawn {
	public float detonateRadius;
	
	public BaseDamage damage;
	
	private bool isDetonate = false;
	
	public GameObject hitParticle;

	public AudioClip exploseSound;

	new void Update () {
		if(isAi){
				if(enemy!=null){
					if(AIAgentComponent.FlatDifference(enemy.myTransform.position,myTransform.position).sqrMagnitude<detonateRadius*detonateRadius/4){
						Detonate();
						//StartCoroutine (CoroutineRequestKillMe ());
					}
				}
		}
		
		base. Update ();
	}
	public override void KillIt(GameObject killer){
		if (isDead) {
			return;		
		}

		Detonate ();
	//	base.KillIt(killer);
		
	}
    protected override void ActualKillMe()
    {
       Destroy(gameObject);
    }
	void Detonate(){
		if(!isDetonate){
            //TODO: DETONATE
           // foxView.RPC("RPCDetonate", PhotonTargets.All);
		   foxView.Detonate();
		   RemoteDetonate();
		   Destroy(gameObject);
		}

	}
	
	public void RemoteDetonate(){
		if(isDetonate){
			return;
		}
		isDetonate = true;
		Instantiate(hitParticle, myTransform.position, myTransform.rotation);
		
		sControl.playClip (exploseSound);//звук взрыва

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, detonateRadius);
		Vector3 Position = transform.position;
		RaycastHit[] hits;
		for(int i=0;i < hitColliders.Length;i++) {

			if(hitColliders[i]==collider){
				continue;
			}
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
				lDamage.pushDirection = myTransform.forward;
				lDamage.hitPosition = myTransform.position;
				if (obj != null) {
					obj.Damage(lDamage,this.gameObject);
				}	
			}
		}
	}
}
