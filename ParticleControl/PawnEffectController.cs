using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;




public class PawnEffectController : MonoBehaviour
{
	public GameObject spawnEffect;
	
	public Transform myTransform;
	
	[Serializable]
    public class EffectEntity
    {
		public DamageType type;
		
		public GameObject effectObject;
		
		public bool spawn;
		
		public float _timer;
		
		public float timer;
	
	}
	
	
	public EffectEntity[] allEffect;
	
	
	void Start(){
		myTransform= transform;
	
	}
	
	void Update(){
		foreach(EffectEntity entity in allEffect){
			if(entity._timer>0){
				entity._timer-=Time.deltaTime;
				if(entity._timer<=0&&!entity.spawn){
                    entity.effectObject.SetActive(false);
				}
			}
		
		}
	
	}
	public bool IsSpawn(){
		return spawnEffect==null;
	}
	
	public void DamageEffect(DamageType type,Vector3 position,Vector3 direction){
		
		Quaternion rot;
		if(direction.sqrMagnitude==0){
			rot = myTransform.rotation;
		}else{
			rot =  Quaternion.LookRotation( direction.normalized);
		}
		
        foreach (EffectEntity entity in allEffect)
        {
            if (entity.type == type)
            {
				if(entity.spawn){
					if (entity.timer <= 0)
					{
					 Instantiate(effectObject, position, Quaternion.identity)
					}
				}else{
					if (entity.timer <= 0)
					{
						entity.effectObject.SetActive(true);
					}
					entity.timer = entity._timer;
				}
            }
			break;
        }
	
	}
	






}