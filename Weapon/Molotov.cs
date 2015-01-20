using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;


//We don't want to our projectile fly infinite time
[RequireComponent(typeof(DelayDestroyedObject))]
public class Molotov : BaseProjectile
{

	
	
	public override void SpawnAfterEffect(Vector3 Position){
	
			if (hitParticle != null)
            {
               GameObject flame =  hitParticle.Spawn( Position, mTransform.rotation);
			   
			   StationaryDamager damager = flame.GetComponent<StationaryDamager>(); 
			   if(damager!=null){
					damager.owner =owner;
			   }
            }
	}
}