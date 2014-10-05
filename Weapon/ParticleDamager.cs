using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleDamager : MonoBehaviour {
	
		List<DamagebleObject> objectToClean = new List<DamagebleObject>();
		
		List<DamagebleObject> objectToDmg = new List<DamagebleObject>();
	
		public ContiniusGun gun;
		
		
		

		void FixedUpdate(){
			foreach(DamagebleObject obj in objectToClean){
				gun.ClearDps(obj);
			}
			objectToClean.Clear();
			foreach(DamagebleObject obj in objectToDmg){
				gun.fireDamage(obj);
				objectToClean.Add(obj);
			}
			objectToDmg.Clear();
		}
	   void OnParticleCollision(GameObject other) {
           //Debug.Log("hit " + other);
			DamagebleObject obj =other.GetComponent<DamagebleObject>();
			if(obj!=null){
				objectToDmg.Add(obj);
			}
	   }
	   
	   void OnDestroy(){
			foreach(DamagebleObject obj in objectToClean){
				gun.ClearDps(obj);
			}
	   }
}