using UnityEngine;
using System.Collections;

public class RayGun : NetSyncGun
{

	/// <summary>
    /// Define Projectile Hit Effect 
    /// </summary>
    

    public BaseProjectile.HITEFFECT projHtEffect;

    public float jumpDistance;

    public int proojHitCnt;

    public int projHitMax;

    private float ammoSpent = 0;

    public RayController rayController;

	public float dmgDelay;
  
	new void Start () {
		base.Start ();
        rayController = rifleParticleController as RayController;
      	sControl.setLooped(true);
        
	}

    void Update()
    {
		if (isShooting)
		{
			if(!foxView.isMine){
				DrawEffectRep();
			}
			owner.animator.StartShootAniamtion("shooting");
		}
		else
		{
			owner.animator.StopShootAniamtion("shooting");

		} 

    }

    protected override void ShootTick(float deltaTime)
    {
		DoRayDamage(deltaTime);
    }
   
	void DoRayDamage(float deltaTime){
        ammoSpent += deltaTime;
        if (curAmmo > 0)
        {
            if (ammoSpent > 1)
            {
                curAmmo--;
                ammoSpent = 0;
            }
           
        }
        else
        {
           ReloadStart();
           return;
              
        }
		Vector3 startPoint  = muzzlePoint.position+muzzleOffset;
		Quaternion startRotation = getAimRotation();
		
		Vector3 direction = startRotation*Vector3.forward;
		Ray centerRay=new Ray(startPoint,direction);
		RaycastHit hitInfo;
		float power=0;
		float range = 0;
		
		switch(prefiretype){
			case PREFIRETYPE.ChargedPower:
				power += _pumpCoef;
			break;
			case PREFIRETYPE.ChargedRange:
				range += _pumpCoef;
			break;		
		}
		if (Physics.Raycast (centerRay, out hitInfo, weaponRange+range)) {
			
			HitEffect(hitInfo,power);
			DrawEffect(startPoint,hitInfo.point,true);
		}else{
			DrawEffect(startPoint,startPoint+(weaponRange+range)*direction,false);
		}
	
	}
	
	void DrawEffectRep(){
		Vector3 startPoint  = muzzlePoint.position+muzzleOffset;
		Quaternion startRotation = getAimRotation();
		Vector3 direction = startRotation*Vector3.forward;
		Ray centerRay=new Ray(startPoint,direction);
		RaycastHit hitInfo;
        Physics.Raycast(centerRay, out hitInfo, weaponRange);
		if (Physics.Raycast (centerRay, out hitInfo, weaponRange)) {
			DrawEffect(startPoint,hitInfo.point,true);
		}else{
			DrawEffect(startPoint,startPoint+(weaponRange)*direction,false);
		}
	}
	
	protected virtual void HitEffect(RaycastHit hitInfo,float power){
		DamagebleObject target =(DamagebleObject) hitInfo.collider.GetComponent(typeof(DamagebleObject));
		if(target!=null){
		
			if( dmgDelay<Time.time){
				dmgDelay=Time.time+fireInterval;
				BaseDamage dmg =new BaseDamage(damageAmount);
				power=power*dmgDelay;
				dmg.Damage += power;
                dmg.hitPosition = hitInfo.point;
				target.Damage(dmg ,owner.gameObject);
			}				
		}
	}
	public void DrawEffect(Vector3 start, Vector3 end,bool isHit){
		rayController.SetRay(start,end,isHit);
	}
    public override void StartFireRep()
    {
		isShooting= true;
   
    }
	public override void StopFire(){
        base.StopFire();
		rayController.StopRay();
	}
	
	public override void AimFix(){
		Vector3 aimTarget =owner.getCachedAimRotation ();
		//Debug.Log ((aimTarget - muzzlePoint.position).normalized);

		muzzlePoint.rotation =Quaternion.LookRotation((aimTarget - muzzlePoint.position).normalized);
	}

}
