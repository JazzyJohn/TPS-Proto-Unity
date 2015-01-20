using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;


//We don't want to our projectile fly infinite time
[RequireComponent(typeof(DelayDestroyedObject))]
public class FlashBang : BaseProjectile
{

	
	public override void SpawnAfterEffect(Vector3 position){
		base.SpawnAfterEffect(position);
		Vector3 direction = position -Camera.main.transform.position;
		if(direction.magnitude<splashRadius&& Vector3.Angle(direction.normalized,Camera.main.transform.forward)>0&&!Physics.Raycast(Camera.main.transform.position,direction,direction.magnitude-0.2f,explosionLayerBlock)){
            Debug.Log("Flash");
			FlashTween tween =Camera.main.GetComponent<FlashTween>();
			if(tween!=null){
				tween.Restart();		
			}
		}
		
	
	}
}