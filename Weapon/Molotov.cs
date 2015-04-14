using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;


//We don't want to our projectile fly infinite time
[RequireComponent(typeof(DelayDestroyedObject))]
public class Molotov : BaseProjectile
{



    public override void SpawnAfterEffect(Vector3 forward)
    {
	
			if (hitParticle != null)
            {
                GameObject flame = hitParticle[0].Spawn(effectPosition,Quaternion.LookRotation(forward));
			   
			   StationaryDamager damager = flame.GetComponent<StationaryDamager>(); 
			   if(damager!=null){
					damager.owner =owner;
			   }
            }
	}
}