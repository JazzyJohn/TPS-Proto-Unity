using UnityEngine;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;
public struct KillInfo{
	public int weaponId;
    public bool isHeadShoot;
    public bool isLongShoot;
    public bool isMelee;
   
    public bool isOnMount;
    public bool killerMount;

    public KillInfo(int weaponId, bool isHeadShoot, bool isMelee, bool isOnMount, bool isLongShoot)
    {
        
        this.weaponId = weaponId;
        this.isHeadShoot = isHeadShoot;
        this.isMelee = isMelee;      
        this.isOnMount = isOnMount;
        this.isLongShoot = isLongShoot;
        this.killerMount = false;
    }

}
public class DamagebleObject : DestroyableNetworkObject {

    private ObscuredFloat _health;
	
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
                killInfo.isMelee = damage.isMelee;
                killInfo.isHeadShoot = damage.isHeadshoot;
                killInfo.isLongShoot = damage.isLongShoot;
                
				KillIt(killer);

			}
		}
	}
	public virtual void KillIt(GameObject killer){
		Destroy(gameObject);

	}
}
