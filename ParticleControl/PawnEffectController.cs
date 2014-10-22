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
		
		public GameObject[] effectObject;

        public ParticleSystem[] particleObject;
		
		public bool spawn;
        [HideInInspector] 
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
                    foreach (GameObject effectObject in entity.effectObject)
                    {
                        effectObject.SetActive(false);
                    }
                    foreach (ParticleSystem effectObject in entity.particleObject)
                    {
                        effectObject.Stop();
                    }
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
               // Debug.Log(type + "  " + entity.effectObject + "  " + entity.timer+" "+ entity.spawn);
				if(entity.spawn){
					if (entity._timer <= 0)
					{
                        entity._timer = entity.timer;
                        Instantiate(entity.effectObject[UnityEngine.Random.Range(0,entity.effectObject.Length)], position, rot);
					}
				}else{
                   
					if (entity._timer <= 0)
					{
                        foreach (GameObject effectObject in entity.effectObject)
                        {
                            effectObject.SetActive(true);
                        }
                        foreach (ParticleSystem effectObject in entity.particleObject)
                        {
                            effectObject.Play();
                        }
					}
					entity._timer = entity.timer;
				}
                break;
            }
			
        }
	
	}
	






}