using UnityEngine;
using System.Collections;

public class SimpleShooter : MonoBehaviour {

	public BaseDamage damageAmount;
	
	public GameObject projectilePrefab;

	public float FireRate;

	private float fireTimer=0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		fireTimer += Time.deltaTime;
		if (fireTimer > FireRate) {
			fireTimer=0;
			Fire();
		}
	}

	void Fire(){
		GameObject proj=Instantiate(projectilePrefab,transform.position,transform.rotation) as GameObject;
	
		BaseProjectile projScript =proj.GetComponent<BaseProjectile>();
		projScript.damage =new BaseDamage(damageAmount) ;
		projScript.owner = gameObject;


	}
}
