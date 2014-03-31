using UnityEngine;
using System.Collections;

public class DamagebleObject : MonoBehaviour {

	public float health;

	public bool destructableObject = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Damage(float damage){
		if (destructableObject){
			health-= damage;
			if(health<0){
				Destroy(gameObject);
			}
		}
	}
}
