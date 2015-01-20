using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;


//We don't want to our projectile fly infinite time
[RequireComponent(typeof(DelayDestroyedObject))]
public class FlashBang : BaseProjectile
{

	public float flashRange;
	public override void SpawnAfterEffect(Vector3 position){
		base.SpawnAfterEffect(position);
		Vector3 direction = position -Camera.main.transform.position;
		if(direction.magnitude<flashRange&& Vector3.Angle(direction.normalized,Camera.main.forward)>0&&!Physics.RayCast(Camera.main.transform.position,direction,direction.magnitude-0.2f,explosionLayerBlock)){
			FlashTween tween =Camera.main.GetComponent<FlashTween>();
			if(tween!=null){
				tween.Restart();		
			}
		}
		
	
	}
}