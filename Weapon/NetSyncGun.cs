using UnityEngine;
using System.Collections;

public class NetSyncGun : BaseWeapon
{
		
	
    public bool isSend;
	
	new void Start () {
		base.Start ();
        isSend = false;
	}
	
	 void Update()
    {
		if (isShooting)
		{
			owner.animator.StartShootAniamtion("shooting");
		}
		else
		{
			owner.animator.StopShootAniamtion("shooting");

		} 

    }

	
	protected override void  ActualFire(){
		base. ActualFire();
	    if (foxView.isMine && !isSend)
        {
            isSend = true;
			foxView.ChangeWeaponShootState(true);
		}
	}

   
    public override void StartFireRep()
    {
        
   
    }
	public override void StopFire(){
        base.StopFire();
		if (foxView.isMine && isSend)
        {
            isSend = false;
			foxView.ChangeWeaponShootState(false);
		}
	}
	
	
}
