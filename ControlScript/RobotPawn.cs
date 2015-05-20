using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nstuff.juggerfall.extension.models;
using System;

public class RobotPawn : Pawn {
	public float ActivationTime=2.0f;
    public float rotateSpeed = 1.0f;

	public Transform playerEnterPositon;
	
	public bool isMutual;
	public bool isEmpty =true;
    private Pawn passenger;

    public Transform back;

    private Transform cameraTransform;
    public override void StartPawn()
    {
        base.StartPawn();
       
    }
    public override bool CanSprint()
    {
        return base.CanSprint() && passenger.CanSprint();
    }
    public override void StartFire()
    {
        passenger.StartFire();
    }
    public override void StopFire()
    {
        passenger.StopFire();
    }
    public override void NextWeapon(){
        passenger.NextWeapon();
    }
    public override void PrevWeapon()
    {
        passenger.PrevWeapon();
    }

    public override void StartPumping()
    {
        passenger.StartPumping();
    }
    public override void StopPumping()
    {
        passenger.StopPumping();
    }

    public override void StopGrenadeThrow()
    {
        passenger.StopGrenadeThrow();
    }

    public override void StartGrenadeThrow()
    {
        passenger.StartGrenadeThrow();


    }
    public override void Reload()
    {
        passenger.Reload();
    }
    public override void ThrowGrenade()
    {
        passenger.ThrowGrenade();
    }
    public override void Init()
    {
        charMan.AddList(player.GetCharacteristick());
        

    }
    public override void AddAmmo(float p)
    {
        passenger.AddAmmo(p);
    }
    public override void Heal(float damage, GameObject Healler)
    {
        passenger.Heal(damage, Healler);
    }
    public override void StartSprint(CharacterState state = CharacterState.Sprinting)
    {

        base.StartSprint();
        passenger.StartSprint(CharacterState.Mount);
    }
    public override void StopSprint()
    {
        base.StopSprint();
        passenger.StopSprint();
    }

    public void AnotherEnter()
    {
        Destroy(GetComponent<ThirdPersonController>());
 
        Destroy(GetComponent<ShowOnGuiComponent>());
        GetComponent<Rigidbody>().isKinematic = true;
    }
    public void MySelfEnter(Pawn passenger)
    {
        this.passenger = passenger;
        passenger.AddBuff((int)CharacteristicList.MAXHEALTH, (int)health);
        ResetDesireRotation(passenger);
        Destroy(GetComponent<ShowOnGuiComponent>());    
    }
    public override void SwitchShoulder()
    {
        passenger.SwitchShoulder();
    }
	protected void Awake(){
		base.Awake();
        cameraTransform = Camera.main.transform;

		if(isMutual){
            try
            {
                PlayerMainGui.instance.Annonce(AnnonceType.JUGGERREADY);
            }
            catch (System.Exception)
            {

                Debug.LogError("exeption error");
            }
		
		}
	}
    protected override bool ShouldRotateTorso()
    {
        return characterState == CharacterState.Jumping ||characterState == CharacterState.Idle || characterState == CharacterState.DoubleJump || characterState == CharacterState.Walking || characterState == CharacterState.Running;
    }
    public override void Movement(Vector3 movement, CharacterState state)
    {
        //Debug.Log (state);
        //Debug.Log (state);
        if (isSpawn)
        {//если только респавнились, то не шевелимся
            return;
        }
      
        Vector3 local = Quaternion.Inverse(cameraTransform.rotation) * movement;
        float yAngle = local.x * Time.deltaTime * rotateSpeed ;
        local.x = 0;
      

        base.Movement(myTransform.rotation*local, state);
     
        UpdateRotation(0, yAngle);
        Vector3 eurler;

        eurler.y = yAngle* fromOldRotationMod;
    


        eurler.z = 0;
        eurler.x = 0;

        if (eurler.y > 360)
        {

            eurler.y -= 360;
        }
        if (eurler.y < -360)
        {

            eurler.y += 360;
        }
        myTransform.rotation *= Quaternion.Euler(eurler);

    }
	//Player get in robot
	public override void  Activate(){
        MySelfEnter(player.GetCurrentPawn());
		_rb.constraints = RigidbodyConstraints.FreezeRotation;
        _rb.useGravity = false;
      
	
      
		isActive = false;
		characterState=CharacterState.Activate;
        if (ActivationTime == 0)
        {
            _ActualMount();
        }
        else
        {
            StartCoroutine(WaitBeforeActive(ActivationTime));
        }
        if (foxView.isMine)
        {
            foxView.Activate();
        }
	}
	public override void Damage(BaseDamage damage,GameObject killer){
        if (damage.isMelee)
        {
            AddEffect(damage.hitPosition, damage.pushDirection, DamageType.MELEE);
            return;
        }

        if (passenger != null)
        {
            passenger.Damage(damage, killer);
        }
        else
        {
            base.Damage(damage, killer);
        }

		
	}
     public override void KillIt(GameObject killer)
    {
      
        if (isDead)
        {
            return;
        }
       
        Player killerPlayer = null;
        try
        {


            isDead = true;

            //StartCoroutine (CoroutineRequestKillMe ());
            Pawn killerPawn = null;

            int killerID = -1;
            string KillerName="";
       

            foxView.PawnDiedByKill(killerID, KillerName);
            player.RobotDead(killerPlayer);
        }
        catch (Exception e)
        {
            Debug.Log("ErrorExeption in Pawn KillIt" + e);
        }
        finally
        {
            PawnKill(killerPlayer);
        }


    }
    public override void UpdateRotation(float xDeltaAngle, float yDeltaAngle)
    {


        yAngle += yDeltaAngle * fromOldRotationMod;
        if (yAngle > 360)
        {
            yAngle -= 360;
        }
        if (yAngle < -360)
        {
            yAngle += 360;
        }




        desiredRotation = Quaternion.Euler(0, yAngle, 0.0f);
        passenger.UpdateRotation(xDeltaAngle, yDeltaAngle); 
    }

	public IEnumerator WaitBeforeActive(float waitTime) {

       
			yield return new WaitForSeconds(waitTime);
            _ActualMount();
       
		//base.Activate();
	}

    private void _ActualMount()
    {
        GetComponent<ThirdPersonController>().enabled = true;
        _rb.isKinematic = false;
        isActive = true;
        _rb.detectCollisions = true;
        for (int i = 0; i < myTransform.childCount; i++)
        {
            myTransform.GetChild(i).gameObject.SetActive(true);
        }
        passenger.Mount();
    }
	//Player have left robot
	public new void  DeActivate(){
		
		//Debug.Log ("ROBOT");
	}
   

	public void OnDestoy(){
		if(isMutual){
			PlayerMainGui.instance.Annonce(AnnonceType.JUGGERKILL);
		}
	}
	public override void  AfterSpawnAction(){
		characterState = CharacterState.Jumping;
	
		//nextState = CharacterState.DeActivate;
	}
	public new void DidLand(){
		if(!isActive){
			characterState=CharacterState.DeActivate;
		}
		//Debug.Log ("LAND");
		lastTimeOnWall = -10.0f;
		//photonView.RPC("JumpChange",PhotonTargets.OthersBuffered,false);
	}
	protected override void UpdateAnimator(){
		//Debug.Log (isGrounded);
	
		base.UpdateAnimator();
	}
  
	public override void ChangeDefaultWeapon(int myId){
        ivnMan.AddStandartMelee();

        ivnMan.GenerateWeaponStart();
	}


}