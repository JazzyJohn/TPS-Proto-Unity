using UnityEngine;
using System.Collections;
public struct KillInfo{
	public int weaponId;
    public bool isHeadShoot;


    public KillInfo(int weaponId, bool isHeadShoot)
    {
        
        this.weaponId = weaponId;
        this.isHeadShoot = isHeadShoot;
    }
}
public class DamagebleObject : DestroyableNetworkObject {

	private float _health;
	
	protected KillInfo killInfo;
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
	public virtual  void clearDps(GameObject killer){
	
		
	}
    public virtual void addDPS(BaseDamage damage, GameObject killer, float fireInterval = 1.0f)
    {
	
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
			if(health<=0){
				
				killInfo.weaponId=damage.shootWeapon;
                killInfo.isHeadShoot = damage.isHeadshoot;
				KillIt(killer);

			}
		}
	}
	public virtual void KillIt(GameObject killer){
		Destroy(gameObject);

	}
}
