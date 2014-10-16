using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;




public class PawnEffectController : MonoBehaviour
{
	public GameObject spawnEffect;
	
	[Serializable]
    public class EffectEntity
    {
		public DamageType type;
		
		public GameObject effectObject;
		
		public float _timer;
		
		public float timer;
	
	}
	
	
	public EffectEntity[] allEffect;
	
	
	
	
	void Update(){
		foreach(EffectEntity entity in allEffect){
			if(entity._timer>0){
				entity._timer-=Time.deltaTime;
				if(entity._timer<=0){
                    entity.effectObject.SetActive(false);
				}
			}
		
		}
	
	}
	public bool IsSpawn(){
		return spawnEffect==null;
	}
	
	public void DamageEffect(DamageType type){
        foreach (EffectEntity entity in allEffect)
        {
            if (entity.type == type)
            {
                if (entity.timer <= 0)
                {
                    entity.effectObject.SetActive(true);
                }
                entity.timer = entity._timer;
            }
        }
	
	}
	






}