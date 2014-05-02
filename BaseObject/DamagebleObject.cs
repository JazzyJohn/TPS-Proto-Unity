using UnityEngine;
using System.Collections;

public class DamagebleObject : DestroyableNetworkObject {

	private float _health;

	public float health{
		
		get {
			return _health;
		}
		
		
		set {
			if(_health!=value){
				_health = value;
				SendMessage ("HPChange", SendMessageOptions.DontRequireReceiver);
				
			}
		
			
		}
		
	}



	public bool destructableObject = true;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void Damage(BaseDamage damage,GameObject killer){
		if (destructableObject){
			health-= damage.Damage;
			if(health<0){
				KillIt(killer);

			}
		}
	}
	public virtual void KillIt(GameObject killer){
		Destroy(gameObject);

	}
}
