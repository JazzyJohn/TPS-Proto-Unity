using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StationaryDamager : MonoBehaviour {
	
		List<DamagebleObject> objectToClean = new List<DamagebleObject>();
		
		List<DamagebleObject> objectToDmg = new List<DamagebleObject>();
	
		public BaseDamage damage;
		
		public float fireInterval;
		
		public GameObject owner;
		

		public void fireDamage (DamagebleObject target)
		{
			if (target != null) {
				
				target.addDPS(new BaseDamage(damage), owner, fireInterval);
			}
		}
		public void ClearDps (DamagebleObject target)
		{
			target.clearDps(owner);
		}
		
		void FixedUpdate(){
			foreach(DamagebleObject obj in objectToClean){
				ClearDps(obj);
			}
			objectToClean.Clear();
			foreach(DamagebleObject obj in objectToDmg){
				fireDamage(obj);
				objectToClean.Add(obj);
			}
			objectToDmg.Clear();
		}
	   void OnParticleCollision(GameObject other) {
           Debug.Log("hit " + other);
			DamagebleObject obj =other.GetComponent<DamagebleObject>();
			if(obj!=null){
				objectToDmg.Add(obj);
			}
	   }
	   
	   void OnDestroy(){
			foreach(DamagebleObject obj in objectToClean){
				ClearDps(obj);
			}
	   }
}