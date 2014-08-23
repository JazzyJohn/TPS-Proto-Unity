using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotPawn : Pawn {
	public float ActivationTime=2.0f;
	public Transform playerExitPositon;
	public bool isPilotIn = false;
	public bool isMutual;
	public bool isEmpty =true;


    public override void StartPawn()
    {
        Debug.Log("Start PAwn");
      
        _canWallRun = canWallRun;
        //проигрываем звук респавна
        sControl.playClip(spawnSound);
        isActive = true;
        if (emitter != null)
        {
            emitter.Emit();//запускаем эмиттер
            isSpawn = true;//отключаем движения и повреждения
        }

       
        cameraController = GetComponent<PlayerCamera>();
       
        naturalWeapon = GetComponent<WeaponOfExtremities>();
        mainAi = GetComponent<AIBase>();

        isAi = mainAi != null;
        if (isAi)
        {
            if (foxView.isMine)
            {
                mainAi.StartAI();

                foxView.SetAI(mainAi.aiGroup, mainAi.homeIndex);

            }
        }
        GetSize();

        correctPlayerPos = transform.position;

        ivnMan.Init();
        centerOffset = capsule.bounds.center - myTransform.position;
        headOffset = centerOffset;
        headOffset.y = capsule.bounds.max.y - myTransform.position.y;

        distToGround = capsule.height / 2 - capsule.center.y;

        health = charMan.GetIntChar(CharacteristicList.MAXHEALTH);
       

        if (isAi)
        {
            ivnMan.Init();
            AfterSpawnAction();
        }
        //Debug.Log (distToGround);

    }

    public void AnotherEnter()
    {
        Destroy(GetComponent<ThirdPersonController>());
        Destroy(GetComponent<PlayerCamera>());

        GetComponent<Rigidbody>().isKinematic = true;
    }
    public void MySelfEnter()
    {

    }

	protected void Awake(){
		base.Awake();
		if(isMutual){
			PlayerMainGui.instance.Annonce(AnnonceType.JUGGERREADY);
		}
	}
	//Player get in robot
	public new void  Activate(){
		((RobotAnimationManager)animator).Activation();
		_rb.constraints = RigidbodyConstraints.FreezeRotation;
		isPilotIn = true;
        foxView.InPilotChange(isPilotIn);
		isActive = false;
		characterState=CharacterState.Activate;
        GetComponent<PlayerCamera>().enabled = true;
        GetComponent<PlayerCamera>().Reset();
		StartCoroutine(WaitBeforeActive(ActivationTime));
		
	}
	public override void Damage(BaseDamage damage,GameObject killer){
		float reduce =  charMan.GetFloatChar(CharacteristicList.JUGGER_DAMAGE_REDUCE);
		if(reduce!=0){
				damage.Damage-= damage.Damage*reduce;
		}

		base.Damage(damage, killer);
	}
	public IEnumerator WaitBeforeActive(float waitTime) {

        if (isPilotIn)
        {

			yield return new WaitForSeconds(waitTime);
			GetComponent<ThirdPersonController>().enabled = true;
			ivnMan.GenerateWeaponStart();
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
        GetComponent<PlayerCamera>().enabled = false;
		ivnMan.TakeWeaponAway();
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

	}

}