using UnityEngine;
using System.Collections;

public class DamagebleObject : DestroyableNetworkObject {

	public float health;

	public bool destructableObject = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void Damage(float damage,GameObject killer){
		if (destructableObject){
			health-= damage;
			if(health<0){
				KillIt(killer);

			}
		}
	}
	public virtual void KillIt(GameObject killer){
		Destroy(gameObject);

	}
}
