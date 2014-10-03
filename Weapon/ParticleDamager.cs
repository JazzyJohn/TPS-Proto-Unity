using UnityEngine;
using System.Collections;

public class ParticleDamager : MonoBehaviour {
	
		List<DamagebleObject> objectToClean = new List<DamagebleObject>();
		
		List<DamagebleObject> objectToDmg = new List<DamagebleObject>();
	
		public ContiniusGun gun;
		
		
		

		void FidexUpdate(){
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