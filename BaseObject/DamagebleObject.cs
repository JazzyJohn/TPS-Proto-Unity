using UnityEngine;
using System.Collections;

public class DamagebleObject : DestroyableNetworkObject {

	public float health;
	[HideInInspector] 
	public float maxHealth;

	public bool destructableObject = true;
	// Use this for initialization
	void Start () {
		maxHealth = health;
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
