using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nstuff.juggerfall.extension.models;

public class RobotPawn : Pawn {
	public float ActivationTime=2.0f;
	public Transform playerExitPositon;
	public bool isPilotIn = false;
	public bool isMutual;
	public bool isEmpty =true;
    private Pawn passenger;

    public Transform back;
    public override void StartPawn()
    {
        base.StartPawn();
       
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
	//Player get in robot
	public new void  Activate(){
		
		_rb.constraints = RigidbodyConstraints.FreezeRotation;
        _rb.useGravity = false;
		isPilotIn = true;
        foxView.InPilotChange(isPilotIn);
		isActive = false;
		characterState=CharacterState.Activate;
    
		StartCoroutine(WaitBeforeActive(ActivationTime));
		
	}
	public override void Damage(BaseDamage damage,GameObject killer){
        if (passenger != null)
        {
            passenger.Damage(damage, killer);
        }
        else
        {
            base.Damage(damage, killer);
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

        if (isPilotIn)
        {

			yield return new WaitForSeconds(waitTime);
			GetComponent<ThirdPersonController>().enabled = true;
		    _rb.isKinematic = false;
			isActive = true;
			_rb.detectCollisions = true;
			for (int i =0; i<myTransform.childCount; i++) {
				myTransform.GetChild(i).gameObject.SetActive(true);
			}
            foxView.Activate();
		}
		//base.Activate();
	}
	//Player have left robot
	public new void  DeActivate(){
		characterState=CharacterState.DeActivate;
		isPilotIn = false;
        foxView.InPilotChange(isPilotIn);
		((RobotAnimationManager)animator).DeActivation();
		GetComponent<ThirdPersonController>().enabled = false;
      
		StopMachine ();
		_rb.constraints = RigidbodyConstraints.FreezeAll;
		_rb.velocity = Vector3.zero;
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
		if (animator != null && animator.gameObject.activeSelf) {
			if(!foxView.isMine){

				RobotAnimationManager roboanim =(RobotAnimationManager)animator;
				if(!roboanim.isActive()&&isPilotIn){
					roboanim.Activation();
				}
				if(roboanim.isActive()&&!isPilotIn){
					roboanim.DeActivation();
				}
			}
					
			//animator.SetLookAtPosition (getAimRotation());
		
		}
		base.UpdateAnimator();
	}
    public override bool shouldRotate()
    {
        return isPilotIn;
    }
	public override void ChangeDefaultWeapon(int myId){
        ivnMan.AddStandartMelee();

        ivnMan.GenerateWeaponStart();
	}
    public override void NetUpdate(PawnModel pawn)
    {
        nextState = (CharacterState)pawn.characterState;
        Vector3 oldPos = correctPlayerPos;
        correctPlayerPos = pawn.position.MakeVector(correctPlayerPos);
        correctPlayerRot = pawn.rotation.MakeQuaternion(correctPlayerRot);
        aimRotation = pawn.aimRotation.MakeVector(aimRotation);
        ToggleAim(pawn.isAiming);
        team = pawn.team;
        health = pawn.health;
        replicatedVelocity = correctPlayerPos - oldPos;
        float oldTime = lastNetUpdate;
        lastNetUpdate = Time.time;
        replicatedVelocity = replicatedVelocity / (oldTime - lastNetUpdate);
        RestartLocalVisibilite();
        isActive = pawn.active;
       
    }

}