using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CharacterState {
	Idle = 0,
	Walking = 1,
	Trotting = 2,
	Running = 3,
	Jumping = 4,
	WallRunning = 5,
	PullingUp=6,
	DoubleJump= 7
}
public enum WallState{
	WallL,
	WallR,
	WallF
}

public struct singleDPS
{
	public BaseDamage damage;
	public GameObject killer;
	public float lastTime;
	public void SetTime(float time){
		lastTime = time;
	}
}	

public class Pawn : DamagebleObject {

	public List<singleDPS> activeDPS = new List<singleDPS> ();

	public const int SYNC_MULTUPLIER = 5;

	public LayerMask groundLayers = -1;
	public LayerMask wallRunLayers = -1;
	public LayerMask climbLayers = 1 << 9; // Layer 9

	public bool isActive =true;

	public BaseWeapon CurWeapon;

	public Transform weaponSlot;

	public Transform myTransform;

	protected Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this

	protected Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

	public Vector3 weaponOffset;

	public Vector3 weaponRotatorOffset;

	public bool isDead=false;

	public string publicName;

	protected Vector3 aimRotation;
	//rotation for moment when rotation of camera and pawn can be different e.t.c wall run	
	protected Vector3 forwardRotation;

	public AnimationManager animator;

	private CharacterState _characterState;

	protected CharacterState characterState
	{
		
		get {
			return _characterState;
		}
		
		
		set {
			if(_characterState!=value&&photonView.isMine){
				//photonView.RPC("SendCharacterState",PhotonTargets.Others,value,wallState);
				
			}
			_characterState = value;
			
		}
		
	}

	private WallState wallState;

	private CharacterState nextState;

	private int jetPackCharge;

	private float jetPackTimer= 0.0f;

	public float jetPackTime=5.0f;

	private Vector3 nextMovement;

	public ThirdPersonCamera cameraController;

	public AIBase mainAi;	

	public bool isAi;

	public Pawn enemy;

	private Rigidbody _rb;

	private Vector3 pushingForce;

	private const float  FORCE_MULIPLIER=10.0f;

	private float distToGround;

	private CapsuleCollider capsule;

	public bool canWallRun;

	public bool canPullUp;

	public bool canJump;

	public float wallRunSpeed;

	public const float  WALL_TIME_UP=1.5f;

	public const float  WALL_TIME_SIDE=3.0f;

	public float groundRunSpeed;

	public float groundTrotSpeed;

	public float groundWalkSpeed;

	public float jumpHeight;
	
	public float climbSpeed;

	public float climbCheckRadius = 0.1f;

	public float climbCheckDistance = 0.5f;

	public float heightOffsetToEdge = 2.0f;

	public float PullUpStartTimer= 0.0f;
	
	public float PullUpTime=2.0f;
	
	private float v;

	public Transform curLookTarget= null;

	public Player player=null;

	public int team;

	private List<Pawn> seenPawns=new List<Pawn>();

	public float seenDistance;

	private Collider myCollider;

	private bool _isGrounded;

	private bool netIsGround;

	public bool isGrounded
	{
		
		get {
			return _isGrounded;
		}

		
		set {
			if(_isGrounded!=value&& value){
				SendMessage ("DidLand", SendMessageOptions.DontRequireReceiver);

			}
			_isGrounded = value;

		}
		
	}
	private float lastTimeOnWall;

	private float lastJumpTime;

	private Vector3 floorNormal;

	private ContactPoint[] contacts;

	public Vector3 centerOffset;

	public Vector3 headOffset;

	public static float gravity = 20.0f;

	public InventoryManager ivnMan;

	public bool isAiming=false;

	public float aimModCoef = -10.0f;

	public bool isLookingAt = true;

	public bool initialReplication= true;

	public class BasePawnStatistic{
		//Shoot Counter
		public int shootCnt=0;
	
	};

	private CharacteristicManager charMan;
	public BasePawnStatistic statistic = new BasePawnStatistic();
	//effects

	public GameObject bloodSplash;

	public ParticleEmitter emitter;

	private bool isSpawn=false;//флаг респавна

	protected void Awake(){
		myTransform = transform;
		ivnMan =GetComponent<InventoryManager> ();
		_rb  = GetComponent<Rigidbody>();
		capsule = GetComponent<CapsuleCollider> ();
		photonView = GetComponent<PhotonView>();
	}
	// Use this for initialization
	protected void Start () {

		if (emitter != null) {
				emitter.Emit ();//запускаем эмиттер
				isSpawn = true;//отключаем движения и повреждения
		}
		if (!photonView.isMine) {

						Destroy (GetComponent<ThirdPersonController> ());
						Destroy (GetComponent<ThirdPersonCamera> ());
						Destroy (GetComponent<MouseLook> ());
						GetComponent<Rigidbody> ().isKinematic = true;
		} else {
			cameraController=GetComponent<ThirdPersonCamera> ();
			isAi = cameraController==null;
		}
		mainAi =  GetComponent<AIBase> ();
		isAi = mainAi!=null;

		correctPlayerPos = transform.position;
		myCollider = collider;

		centerOffset = capsule.bounds.center - myTransform.position;
		headOffset = centerOffset;
		headOffset.y = capsule.bounds.max.y - myTransform.position.y;

		distToGround = capsule.height/2-capsule.center.y;
		charMan = GetComponent<CharacteristicManager> ();
		charMan.Init ();
		health= charMan.GetIntChar(CharacteristicList.MAXHEALTH);
		if (canJump) {
			jetPackCharge = charMan.GetIntChar(CharacteristicList.JETPACKCHARGE);
		}
		//Debug.Log (distToGround);
	}
	
	public override void Damage(BaseDamage damage,GameObject killer){
		if (isSpawn||killer==null) {//если только респавнились, то повреждений не получаем
			return;
		}
		bool isVs =( damage.isVsArmor && charMan.GetBoolChar (CharacteristicList.ARMOR))||( !damage.isVsArmor && !charMan.GetBoolChar (CharacteristicList.ARMOR));
		if (!isVs) {
			damage.Damage*=0.5f;		
		}

		Pawn killerPawn =killer.GetComponent<Pawn> ();
		if (killerPawn != null && killerPawn.team == team &&! PlayerManager.instance.frendlyFire) {
			return;
		}
		if (killerPawn != null){
			Player killerPlayer =  killerPawn.player;
			if(killerPlayer!=null){
				killerPlayer.DamagePawn(damage);
			}
		}
		if (photonView.isMine) {
			AddEffect(damage.hitPosition);
			float forcePush =  charMan.GetFloatChar(CharacteristicList.STANDFORCE);
			///Debug.Log(forcePush +" "+damage.pushForce);
			forcePush =damage.pushForce-forcePush;
			//Debug.Log(forcePush);
			if(forcePush>0){
				damage.pushDirection.y=0;
				AddPushForce(forcePush*damage.pushDirection*FORCE_MULIPLIER);


			}	
		
		}
		if (!PhotonNetwork.isMasterClient){
			return;
		}
		if (isAi) {
			mainAi.WasHitBy(killer);

		}

		
		//Debug.Log ("DAMAGE");
		base.Damage(damage,killer);
	}

	public void Heal(float damage,GameObject Healler){
		int maxHealth =charMan.GetIntChar(CharacteristicList.MAXHEALTH);
		health += damage;
		if (maxHealth < health) {
			health=maxHealth;		
		}

	}

	public void HPChange(){
		if (PhotonNetwork.isMasterClient) {
			if(photonView.owner!=null){
				photonView.RPC ("RPCSetHealth", photonView.owner, health);
			}else{
				photonView.RPC ("RPCSetHealth", PhotonTargets.MasterClient, health);
			}
		}
	}
	public override void KillIt(GameObject killer){
		if (isDead) {
			return;		
		}
		isDead = true;
		if (CurWeapon != null) {
			CurWeapon.	RequestKillMe();
		}

		StartCoroutine (CoroutineRequestKillMe ());
		Pawn killerPawn =killer.GetComponent<Pawn> ();
		Player killerPlayer = null;
		if (killerPawn != null) {
			killerPlayer = killerPawn.player;
			if(killerPlayer!=null){
				killerPlayer.PawnKill(player,myTransform.position);
			}
		}
		//Debug.Log ("KILLL IT" + player);
		if (player != null) {
			if(player.inBot){
				player.RobotDead(killerPlayer);
			}else{
				player.PawnDead(killerPlayer);
			}
		}

	

		
	}
	//EFFECCT SECTION
	void AddEffect(Vector3 position){
		if (bloodSplash != null) {
				Instantiate (bloodSplash, position, Quaternion.LookRotation (UnityEngine.Random.onUnitSphere));
		}

	}


	//END OF EFFECT SECTIOn

	// Update is called once per frame

	protected void UpdateSeenList(){
		Pawn[] allPawn =PlayerManager.instance.FindAllPawn();
		seenPawns.Clear();

		for(int i=0;i<allPawn.Length;i++){
			if(allPawn[i]==this){
				continue;
			}
			Vector3 distance =(allPawn[i].myTransform.position-myTransform.position);

			if(distance.sqrMagnitude<seenDistance){
				RaycastHit hitInfo;
				Vector3 normalDist = distance.normalized;
				Vector3 startpoint = myTransform.position +normalDist*capsule.radius;

					
				if (allPawn[i].team!=team&&Physics.Raycast(startpoint,normalDist,out hitInfo)) {

					
					if(allPawn[i].myCollider!=hitInfo.collider){
						continue;
					}
				}
				seenPawns.Add(allPawn[i]);
			}
			
		}

	}
	protected void UpdateAnimator(){
		float strafe = 0;
		//Debug.Log (strafe);	
		float speed =0 ;
		//Debug.Log (speed);
		if (animator != null && animator.gameObject.activeSelf) {
			if (photonView.isMine) {
				
				
				strafe = CalculateStarfe();
				//Debug.Log (strafe);	
				speed =CalculateSpeed();
				if (characterState == CharacterState.Idle) {
					animator.ApllyMotion (0.0f, speed, strafe);
				} else {
					if (characterState == CharacterState.Running) {
						animator.ApllyMotion (2.0f, speed, strafe);
					} else if (characterState == CharacterState.Trotting) {
						animator.ApllyMotion (1.0f, speed, strafe);	
					} else if (characterState == CharacterState.Walking) {
						animator.ApllyMotion (1.0f, speed, strafe);	
					} else if (characterState == CharacterState.WallRunning) {
						//Debug.Log ("INSWITCH");
						switch (wallState) {
						case WallState.WallF:
							animator.WallAnimation (false, false, true);
							break;
						case WallState.WallR:
							animator.WallAnimation (false, true, false);
							break;
						case WallState.WallL:
							animator.WallAnimation (true, false, false);
							break;
						}
						
					}
				}
				
				//
			}else{
				strafe = CalculateRepStarfe();
				//Debug.Log (strafe);	
				speed =CalculateRepSpeed();
				switch(nextState){
				case CharacterState.Idle:
					if(characterState == CharacterState.Jumping||characterState == CharacterState.DoubleJump){
						animator.ApllyJump(false);

					}
					animator.ApllyMotion (0.0f, speed, strafe);
					break;
				case CharacterState.Running:
					if(characterState == CharacterState.Jumping||characterState == CharacterState.DoubleJump){
						animator.ApllyJump(false);
					}
					animator.ApllyMotion (2.0f, speed, strafe);
					break;
				case CharacterState.Trotting:
					if(characterState == CharacterState.Jumping||characterState == CharacterState.DoubleJump){
						animator.ApllyJump(false);
					}
					animator.ApllyMotion (1.0f, speed, strafe);
					break;
				case CharacterState.Walking:
					if(characterState == CharacterState.Jumping||characterState == CharacterState.DoubleJump){
						animator.ApllyJump(false);
					}
					animator.ApllyMotion (1.0f, speed, strafe);
					break;
				case CharacterState.WallRunning:
					switch (wallState) {
					case WallState.WallF:
						animator.WallAnimation (false, false, true);
						break;
					case WallState.WallR:
						animator.WallAnimation (false, true, false);
						break;
					case WallState.WallL:
						animator.WallAnimation (true, false, false);
						break;
					}
					break;
				case CharacterState.Jumping:
					animator.ApllyJump(true);
					if(characterState==CharacterState.WallRunning){					
						animator.WallAnimation(false,false,false);
						animator.FreeFall();

					}
					break;
				case CharacterState.DoubleJump:
					animator.ApllyJump(true);
					animator.DoubleJump();
					if(characterState==CharacterState.WallRunning){					
						animator.WallAnimation(false,false,false);
					}
					break;
				case CharacterState.PullingUp:
					if(characterState!=CharacterState.PullingUp){
						StartCoroutine("PullUpEnd",PullUpTime);
						animator.StartPullingUp();
					}
					break;
					
					
				}
				characterState = nextState;
			}
			if (isLookingAt) {
				Vector3 laimRotation =aimRotation;
				/*if(animator.isWeaponAimable()){
					Quaternion diference = Quaternion.FromToRotation(CurWeapon.muzzlePoint.forward,myTransform.forward);

					Vector3 direction= laimRotation-myTransform.position;
				
					laimRotation =(diference *direction.normalized)*direction.magnitude +myTransform.position;
				}*/
				animator.animator.SetLookAtPosition (laimRotation);
				animator.animator.SetLookAtWeight (1, 0.5f, 0.7f, 0.0f, 0.5f);

			}
		}

	}
	void Update () {
		//Debug.Log (photonView.isSceneView);
		if (!isActive) {
			return;		
		}
		if (isSpawn) {//если респавн

			if (emitter==null) {//если все партиклы кончились
				isSpawn=false;//то освобождаем все движения и повреждения
			}
		}

		if (photonView.isMine) {

			UpdateSeenList();
			if (canJump&&jetPackCharge<charMan.GetIntChar(CharacteristicList.JETPACKCHARGE)){
				jetPackTimer+=Time.deltaTime;
				if(jetPackTimer>=jetPackTime){
					jetPackTimer=0.0f;
					jetPackCharge++;
				}
			}


			if(CurWeapon!=null){
				//if(aimRotation.sqrMagnitude==0){
				getAimRotation(CurWeapon.weaponRange);
				/*}else{
					aimRotation = Vector3.Lerp(aimRotation,getAimRotation(CurWeapon.weaponRange), Time.deltaTime*10);
				}*/
				Vector3 eurler = Quaternion.LookRotation(aimRotation-myTransform.position).eulerAngles;
				eurler.z =0;
				eurler.x =0;
				if(characterState == CharacterState.WallRunning||characterState ==CharacterState.PullingUp){
					if(forwardRotation.sqrMagnitude>0){
						myTransform.rotation= Quaternion.LookRotation(forwardRotation);
					}
				}else{
					myTransform.rotation= Quaternion.Euler(eurler);
				}
				//animator.animator.SetLookAtPosition (aimRotation);
				//animator.animator.SetLookAtWeight (1, 0.5f, 0.7f, 0.0f, 0.5f);
				//CurWeapon.curTransform.rotation =  Quaternion.LookRotation(aimRotation-CurWeapon.curTransform.position);
				/*Quaternion diff = Quaternion.identity;
				Vector3 target = (aimRotation-CurWeapon.transform.position).normalized;
				if(!CurWeapon.IsReloading()){
					diff= Quaternion.FromToRotation(CurWeapon.transform.forward,target);
				}

				Debug.DrawLine(CurWeapon.transform.position,aimRotation);
				Vector3 aimRotationWeapon = diff*target*CurWeapon.weaponRange+CurWeapon.transform.position; 
				Debug.DrawLine(CurWeapon.transform.position,aimRotationWeapon);*/

			}else{
				//if(aimRotation.sqrMagnitude==0){
					getAimRotation(50);
				/*}else{
					aimRotation = Vector3.Lerp(aimRotation,getAimRotation(50), Time.deltaTime*10);
				}*/
				Vector3 eurler = Quaternion.LookRotation(aimRotation-myTransform.position).eulerAngles;
				eurler.z =0;
				eurler.x =0;
				if(characterState == CharacterState.WallRunning||characterState ==CharacterState.PullingUp){
					if(forwardRotation.sqrMagnitude>0){
						myTransform.rotation= Quaternion.LookRotation(forwardRotation);
					}
				}else{
					myTransform.rotation= Quaternion.Euler(eurler);
				}
				

				
			}
			//TODO: TEMP SOLUTION BEFORE NORMAL BONE ORIENTATION
			
			//animator.SetFloat("Pitch",pitchAngle);

		} else {
			ReplicatePosition();

		}
//		Debug.Log (characterState);

		if (!Application.isPlaying&&Application.isEditor)
		{
			foreach (BodyHurt bh in this.GetComponentsInChildren<BodyHurt>()) {bh.TargetHarm = this;}
		}
		//		Debug.Log (characterState);
		UpdateAnimator ();
		DpsCheck ();

		
	}
	public void DpsCheck(){
		//Debug.Log ("dps"+this+activeDPS.Count );
		if (activeDPS.Count > 0) {

			foreach (singleDPS key in activeDPS) {
				BaseDamage ldamage = new BaseDamage(key.damage);
				ldamage.hitPosition =myTransform.position;
				ldamage.isContinius = true;
				ldamage.Damage *= Time.deltaTime;
				if(key.lastTime+0.1f<Time.time){
					ldamage.sendMessage = true;
					key.SetTime(Time.time);
				}else{
					ldamage.sendMessage = false;
				}

				Damage(ldamage,key.killer);
			}
		} 
	}
	public void ReplicatePosition(){
		if (initialReplication) {
			myTransform.position = correctPlayerPos;
			myTransform.rotation = correctPlayerRot;
			initialReplication= false;
		}
		myTransform.position = Vector3.Lerp(myTransform.position, correctPlayerPos, Time.deltaTime *SYNC_MULTUPLIER);
		myTransform.rotation = Quaternion.Lerp(myTransform.rotation, correctPlayerRot, Time.deltaTime * SYNC_MULTUPLIER);


	}
	[RPC]
	public void SendCharacterState(int nextrpcState,int nextwallState){
		wallState = (WallState)nextwallState;
		nextState =(CharacterState) nextrpcState;
	}
	//Weapon Section
	public void StartFire(){
		if (CurWeapon != null) {
			CurWeapon.StartFire ();
		} 
	}
	public void StopFire(){
		if (CurWeapon != null) {
			CurWeapon.StopFire ();
		}
	}
	public void setWeapon(BaseWeapon newWeapon){
		CurWeapon = newWeapon;
		//Debug.Log (newWeapon);
		CurWeapon.AttachWeapon(weaponSlot,weaponOffset,Quaternion.Euler (weaponRotatorOffset),this);
	}
	public void ChangeWeapon(int weaponIndex){
		ivnMan.ChangeWeapon (weaponIndex);
	}

	public bool isAimimngAtEnemy(){
		if (enemy == null) {
						return false;
		} else {
			Vector3 currentDirection =aimRotation- myTransform.position;
			Vector3 desireDirection = enemy.myTransform.position- myTransform.position;
			return (Vector3.Dot (currentDirection.normalized,desireDirection.normalized)>0.9f);
		}
	}
	public Vector3 getAimRotation(float weaponRange){
		
		if(photonView.isMine){
			if(isAi){
				if(enemy==null){
					aimRotation =myTransform.position+myTransform.forward*10;
				}else{
					aimRotation =Vector3.Lerp( aimRotation,enemy.myTransform.position,Time.deltaTime*10);
				}
			}else{
				if(cameraController.enabled ==false){
					aimRotation= myTransform.position +myTransform.forward*50;
					return aimRotation;
				}
				Camera maincam = Camera.main;
				Ray centerRay= maincam.ViewportPointToRay(new Vector3(.5f, 0.5f, 1f));

				Vector3 targetpoint = Vector3.zero;
				bool wasHit = false;
				float magnitude = weaponRange;
				float range=weaponRange;
				foreach( RaycastHit hitInfo  in Physics.RaycastAll(centerRay, weaponRange))				
				{
					if(hitInfo.collider==myCollider)
					{
						continue;
					}

					//
					//Debug.DrawRay(centerRay.origin,centerRay.direction);
					targetpoint =hitInfo.point;
					range =(targetpoint-centerRay.origin).magnitude;
					if(range<magnitude){
						magnitude=range;
					}else{
						continue;
					}
					wasHit= true;
					curLookTarget= hitInfo.transform;
					//Debug.Log (curLookTarget);
				


				}

				if(!wasHit){
					//Debug.Log("NO HIT");
					curLookTarget= null;
					animator.WeaponDown(false);
					targetpoint =maincam.transform.forward*weaponRange +maincam.ViewportToWorldPoint(new Vector3(.5f, 0.5f, 1f));
				}else{
					//Debug.Log(range.ToString()+(cameraController.normalOffset.magnitude+5));
					if(magnitude<cameraController.normalOffset.magnitude+1){
						targetpoint =maincam.transform.forward*weaponRange +maincam.ViewportToWorldPoint(new Vector3(.5f, 0.5f, 1f));
						animator.WeaponDown(true);
					}else{
						animator.WeaponDown(false);
					}
				}
				aimRotation=targetpoint; 
				
			}
			
			return aimRotation;
		}else{
			return aimRotation;
		}
	}
	void OnDrawGizmos() {
		//Gizmos.color = Color.yellow;
		//Gizmos.DrawSphere(aimRotation, 1);
	}
	public Vector3 getCachedAimRotation(){
		return aimRotation;

	}

	public float AimingCoef(){
		if (isAiming) {
			return aimModCoef;		
		}
		return 0.0f;
	}

	public void ToggleAim(){
		isAiming = !isAiming;
		if (cameraController != null) {
			cameraController.ToggleAim();
		}
	}

	public int 	GetAmmoInBag (){
		return ivnMan.GetAmmo (CurWeapon.ammoType);

	}
	public bool IsShooting(){
		if (CurWeapon == null) {
			return false;
		}
		return CurWeapon.IsShooting ();
	}
	public float RecoilMod(){
		if (CurWeapon == null) {
			return 0;
		}
		return CurWeapon.recoilMod;
	}
	public void HasShoot(){
		if (cameraController != null) {
			cameraController.AddShake(RecoilMod());
		}
	}
	public void Reload(){
		if (CurWeapon != null) {
			CurWeapon.ReloadStart();
		}

	}
	//END WEAPON SECTION
	void OnCollisionEnter(Collision collision) {
		//Debug.Log ("COLLISION ENTER PAWN " + this + collision);
	}
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "damageArea") {
			//Debug.Log (other.GetComponent<ContiniusGun> ());
			other.GetComponent<MuzzlePoint>().gun.GetComponent<ContiniusGun> ().fireDamage (this);
		}
	}
	
	void OnTriggerExit (Collider other)
	{
		if (other.tag == "damageArea") {
			singleDPS newDPS = other.GetComponent<MuzzlePoint>().gun.GetComponent<ContiniusGun> ().getId ();
			foreach (singleDPS key in activeDPS) {
				if(newDPS.killer == key.killer){
					activeDPS.Remove(key);
					break;
				}	
			}
		}
	}

	//TODO: MOVE THAT to PAwn and turn on replication of aiming
	//TODO REPLICATION
	


		

	//Movement section

	float CalculateStarfe(){
		return Vector3.Dot (myTransform.right, _rb.velocity.normalized);
				
	
	}
	float CalculateSpeed(){
		float result =Vector3.Project (_rb.velocity,myTransform.forward).magnitude;
		if (result < groundWalkSpeed) {
			return 0.0f;		
		}
		if (result > groundWalkSpeed && result < groundTrotSpeed) {
			return 1.0f*Mathf.Sign(Vector3.Dot(_rb.velocity.normalized,myTransform.forward));	
		}
		if (result > groundTrotSpeed) {
			return 2.0f*Mathf.Sign(Vector3.Dot(_rb.velocity.normalized,myTransform.forward));	
		}
		return 0.0f;		
	}
	float CalculateRepStarfe(){
		Vector3 velocity =  correctPlayerPos-myTransform.position;
		return Vector3.Dot (myTransform.right, velocity.normalized);
				
	
	}
	float CalculateRepSpeed(){
		Vector3 velocity =  correctPlayerPos-myTransform.position;
		velocity = velocity/(Time.deltaTime * SYNC_MULTUPLIER);
		float result =Vector3.Project (velocity,myTransform.forward).magnitude;
		if (result < groundWalkSpeed) {
			return 0.0f;		
		}
		if (result > groundWalkSpeed && result < groundTrotSpeed) {
			return 1.0f*Mathf.Sign(Vector3.Dot(velocity.normalized,myTransform.forward));	
		}
		if (result > groundTrotSpeed) {
			return 2.0f*Mathf.Sign(Vector3.Dot(velocity,myTransform.forward));	
		}
		return 0.0f;		
	}
	public void Movement(Vector3 movement,CharacterState state){
		//Debug.Log (state);
		//Debug.Log (state);
		if (isSpawn) {//если только респавнились, то не шевелимся
			return;
		}

		nextState = state;

		if (nextState != CharacterState.Jumping&&nextState != CharacterState.DoubleJump) {
						movement = (movement - Vector3.Project (movement, floorNormal)).normalized * movement.magnitude;
						//Debug.DrawRay (myTransform.position, movement.normalized);
						//Debug.DrawRay (myTransform.position, floorNormal);
						nextMovement = movement;
		} else {
			nextMovement = movement;
			}

	}

	bool WallRun (Vector3 movement,CharacterState state)
	{
		if (!canWallRun&&photonView.isMine) return false;

		//if (isGrounded) return false;
		if (lastTimeOnWall + 1.0f > Time.time) {
			return false;
		}
		
		/*if (_rb.velocity.sqrMagnitude < 0.02f ) {
			if(characterState == CharacterState.WallRunning){
					characterState = CharacterState.Jumping;
					lastTimeOnWall = Time.time;
			}
			return false;
		}*/
		
		if (_rb.velocity.sqrMagnitude < 0.02f&&characterState == CharacterState.WallRunning) {
			characterState = CharacterState.Jumping;
			lastTimeOnWall = Time.time;

			return false;
		}
		if (characterState != CharacterState.DoubleJump&&characterState != CharacterState.WallRunning) {
			return false;
		}
		//Debug.Log (characterState);
		RaycastHit leftH,rightH,frontH;
		
		
		bool leftW = Physics.Raycast (myTransform.position ,
		                              myTransform.right * -1 ,out leftH, capsule.radius + 0.2f,wallRunLayers);
		bool rightW = Physics.Raycast (myTransform.position,
		                               myTransform.right ,out rightH, capsule.radius + 0.2f, wallRunLayers);
		bool frontW = Physics.Raycast (myTransform.position,
		                               myTransform.forward,out frontH, capsule.radius + 0.2f, wallRunLayers);

		/*Debug.DrawRay (myTransform.position ,
		               myTransform.right * -1 );
		
		Debug.DrawRay (myTransform.position,
		               myTransform.right );
		
		Debug.DrawRay (myTransform.position,
		               myTransform.forward);*/
	
	


		Vector3 tangVect = Vector3.zero;
		
		if(!animator.animator.IsInTransition(0) && !_rb.isKinematic)
		{
			if(leftW)
			{
				
				
				tangVect = Vector3.Cross(leftH.normal,Vector3.up);
				//tangVect = Vector3.Project(movement,tangVect).normalized;
				_rb.velocity = tangVect*wallRunSpeed + myTransform.up*wallRunSpeed/3;
				if(!(characterState == CharacterState.WallRunning))
				{
					wallState =WallState.WallL;
					characterState = CharacterState.WallRunning;
					//animator.SetBool("WallRunL", true);
					StartCoroutine( WallRunCoolDown(WALL_TIME_SIDE)); // Exclude if not needed
				}
				
				if(state == CharacterState.Jumping)
				{
					_rb.velocity = myTransform.up*movement.y  +leftH.normal*movement.y;
					StartCoroutine( WallJump(1f)); // Exclude if not needed
				}
			}
			
			else if(rightW)
			{
				
				tangVect = -Vector3.Cross(rightH.normal,Vector3.up);
				//tangVect = Vector3.Project(movement,tangVect).normalized;
				_rb.velocity = tangVect*wallRunSpeed + myTransform.up*wallRunSpeed/3;
				if(!(characterState == CharacterState.WallRunning))
				{
					wallState =WallState.WallR;
					characterState = CharacterState.WallRunning;
					StartCoroutine( WallRunCoolDown(WALL_TIME_SIDE)); // Exclude if not needed
				}
				
				if(state == CharacterState.Jumping)
				{
					_rb.velocity = myTransform.up*movement.y  +rightH.normal*movement.y;
					StartCoroutine( WallJump(1f)); // Exclude if not needed
				}
			}
			
			else if(frontW)
			{
				_rb.velocity = myTransform.up*wallRunSpeed/1.5f;
				tangVect = frontH.normal*-1;
				if(!(characterState == CharacterState.WallRunning))
				{
					wallState =WallState.WallF;
					characterState = CharacterState.WallRunning;
					StartCoroutine( WallRunCoolDown(WALL_TIME_UP)); // Exclude if not needed
				}
				
				if(state == CharacterState.Jumping)
				{
					_rb.velocity =( myTransform.up+myTransform.forward*-1).normalized*movement.y;
					StartCoroutine( WallJump(1f)); // Exclude if not needed
				}
			}else{
				if(characterState == CharacterState.WallRunning){
					characterState = CharacterState.Jumping;
					lastTimeOnWall = Time.time;
				}

			}

			forwardRotation  =  tangVect*5;
			//Debug.DrawLine(myTransform.position,forwardRotation);
			//animator.WallAnimation(leftW,rightW,frontW);
			

			return leftW||rightW||frontW;
		}
		return false;

	}
	// Wall run cool-down
	IEnumerator WallRunCoolDown (float sec)
	{
		canWallRun = true;
		yield return new WaitForSeconds (sec);
		canWallRun = false;
		characterState = CharacterState.Jumping;
		yield return new WaitForSeconds (sec);
		canWallRun = true;
	}
	// Wall run cool-down
	IEnumerator WallJump (float sec)
	{
		Jump ();
		//Debug.Log ("WALLJUMP");
		canWallRun = false;
		characterState = CharacterState.Jumping;
		yield return new WaitForSeconds (sec);
		canWallRun = true;
	}

	void OnCollisionStay(Collision collisionInfo) {
		if (lastJumpTime + 0.1f > Time.time) {
			return;		
		}
		if(characterState==CharacterState.WallRunning){
		   return;
		}
	    contacts = collisionInfo.contacts;
		if (contacts != null) {
			foreach (ContactPoint contact in contacts) {
				/*if(contact.otherCollider.CompareTag("decoration")){
					continue;
				}*/
				Vector3 Direction = contact.point - myTransform.position;
				//Debug.Log (this.ToString()+collisionInfo.collider+Vector3.Dot(Direction.normalized ,Vector3.down) );
				if (Vector3.Dot (Direction.normalized, Vector3.down) > 0.75) {
					isGrounded = true;
					floorNormal = 	contact.normal;
				}
			
				//Debug.DrawRay(contact.point, contact.normal, Color.white);

			}
			contacts= null;	
		}

	}
	public void FixedUpdate () {

		if (!isActive) {
			return;		
		}
		if (!photonView.isMine) {
			return;
		}
		//Debug.Log (_rb.velocity.magnitude);
		if (isGrounded) {
			//Debug.Log ("Ground"+characterState);

			if (_rb.isKinematic) _rb.isKinematic= false;
			Vector3 velocity = _rb.velocity;
			Vector3 velocityChange = (nextMovement - velocity);
			//Debug.Log (velocityChange);
			rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

			characterState = nextState;
			if(nextState==CharacterState.Jumping){
				Jump ();

			}

		} else {
			//Debug.Log ("Air"+characterState);
			v = nextMovement.normalized.magnitude;
			
			switch(nextState)
			{
			case CharacterState.DoubleJump:
				if(characterState!=CharacterState.WallRunning
				   &&characterState!=CharacterState.PullingUp){
					if(jetPackCharge>0){
						Vector3 velocity = _rb.velocity;
						Vector3 velocityChange = (nextMovement - velocity);
						jetPackCharge--;
						animator.DoubleJump();
						rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

						characterState = nextState;
					}

				}
				break;
			default:

				if(!WallRun (nextMovement,nextState)){

					animator.ApllyJump(true);						
					animator.WallAnimation(false,false,false);
					if(characterState!=CharacterState.DoubleJump){
						animator.FreeFall();
					}
					//Debug.Log ("My Name" +this +"  "+nextState+"  "+isGrounded);
				}else{
					SendMessage ("WallLand", SendMessageOptions.DontRequireReceiver);
				}
				if(PullUpCheck()){

					PullUp();
				}
				break;
			}
			
		}
		//Debug.Log(_rb.isKinematic);

		if (!_rb.isKinematic) {
			
			_rb.AddForce(new Vector3(0,-gravity * rigidbody.mass,0)+pushingForce);
		}	
		netIsGround = isGrounded;
		if (photonView.isMine) {
						isGrounded = false;
		}
	}
	public bool IsGrounded ()
	{	

		return isGrounded;
	}
	public void Jump(){
		if (animator != null) {
						animator.ApllyJump (true);	
		}
		lastJumpTime = Time.time;
		//photonView.RPC("JumpChange",PhotonTargets.OthersBuffered,true);
	}
	public int GetJetPackCharges(){
		return jetPackCharge;
	
	}
	public void DidLand(){
		if (animator != null) {
						animator.ApllyJump (false);
		}
		//Debug.Log ("LAND");
		lastTimeOnWall = -10.0f;
		//photonView.RPC("JumpChange",PhotonTargets.OthersBuffered,false);
	}

	bool PullUpCheck(){
		if (!canPullUp) {
			return false;
		}
		if (characterState == CharacterState.PullingUp) {
			return true;
		}
		RaycastHit frontH;
		bool upCol = Physics.Raycast (myTransform.position,
				                               Vector3.up,out frontH, capsule.height, wallRunLayers);
		if (upCol) {
					return false;
		}
		
		bool frontW = Physics.Raycast (myTransform.position,
		                               myTransform.forward,out frontH, capsule.radius + 0.2f, wallRunLayers);
		bool middleAir = Physics.Raycast (myTransform.position+ myTransform.up/2,
		                                  myTransform.forward,out frontH, capsule.radius + 0.2f, wallRunLayers);
		if(frontW||middleAir){
			bool frontAir = Physics.Raycast (myTransform.position+ myTransform.up,
		                               myTransform.forward,out frontH, capsule.radius + 0.2f, wallRunLayers);
			forwardRotation= frontH.normal*-1;
	

			animator.SetLong(!middleAir);

			return !frontAir;
			
		}
		/*Debug.DrawRay (myTransform.position ,
		               myTransform.forward * -1 );
		Debug.DrawRay (myTransform.position+ myTransform.up ,
		               myTransform.forward * -1 );*/
		return false;
		//Deprecated system of collider pullup system
		/*Vector3 p1 = myTransform.position - (myTransform.up * -heightOffsetToEdge) + myTransform.forward;
		Vector3 p2 = myTransform.position - (myTransform.up * -heightOffsetToEdge);
		//Debug.DrawLine (p1, p2);
		RaycastHit hit;
		//Debug.DrawLine (p1-myTransform.up*climbCheckDistance, p2-myTransform.up*climbCheckDistance);
		// Hit nothing and not at edge -> Out
		return Physics.CapsuleCast (p1, p2, climbCheckRadius, -myTransform.up, out hit, climbCheckDistance, climbLayers);*/
		
	}
	// Wall run cool-down
	IEnumerator PullUpEnd (float sec)
	{
	
		yield return new WaitForSeconds (sec);
		_rb.isKinematic = false;
		animator.FinishPullingUp();
		characterState = CharacterState.Idle;
		isGrounded = true;
		SendMessage ("DidLand", SendMessageOptions.DontRequireReceiver);

	}

	void PullUp ()
	{
			//Debug.Log (characterState);
			if(	characterState != CharacterState.PullingUp){
				characterState = CharacterState.PullingUp;
				_rb.isKinematic = true;
				StartCoroutine("PullUpEnd",PullUpTime);
				animator.StartPullingUp();
				PullUpStartTimer = 0.0f;
			}
			PullUpStartTimer += Time.deltaTime;
			float nT = PullUpStartTimer/PullUpTime;

			if (nT <= 1.0f) {
						if (nT <= 0.4f) { // Step up
								myTransform.Translate (Vector3.up * Time.deltaTime * climbSpeed);
						} else { // Step forward
								if (nT <= 0.6f)
										myTransform.Translate (Vector3.forward * Time.deltaTime * climbSpeed);
								else if (nT >= 0.6f && _rb.isKinematic) // fall early
										_rb.isKinematic = false;
								if (!_rb.isKinematic)
										_rb.velocity = new Vector3 (0, _rb.velocity.y, 0);
						}
				}
						
				
	}

	public void StopMachine(){
		characterState = CharacterState.Idle;
		nextMovement = Vector3.zero;
	}

	void AddPushForce(Vector3 force){
		pushingForce += force;
		StartCoroutine (RemoveForce(force));
	}
	public IEnumerator RemoveForce(Vector3 force){
		yield return new WaitForSeconds (0.1f);
		pushingForce -= force;

	}
	                        
	//end Movement Section


	
	//NetworkSection
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			ServerHolder.WriteVectorToShort(stream,transform.position);
			stream.SendNext(transform.rotation.eulerAngles.y);
			ServerHolder.WriteVectorToShort(stream,aimRotation);
			
			stream.SendNext(characterState);
			//stream.SendNext(health);
			stream.SendNext(wallState);
			stream.SendNext(health);
			//stream.SendNext(netIsGround);
			//stream.SendNext(animator.GetJump());

		}
		else
		{
			// Network player, receive data
		
			correctPlayerPos = ServerHolder.ReadVectorFromShort(stream);
			correctPlayerRot =  Quaternion.Euler(0,(float)stream.ReceiveNext(),0);
		
			this.aimRotation = ServerHolder.ReadVectorFromShort(stream);
			//Debug.Log(aimRotation);
			nextState = (CharacterState) stream.ReceiveNext();
			//Debug.Log (characterState);
			//health=(float) stream.ReceiveNext();
			wallState = (WallState) stream.ReceiveNext();
			float newHealth =(float)stream.ReceiveNext();
			if(!PhotonNetwork.isMasterClient){
				health =newHealth;
			}
			//isGrounded =(bool) stream.ReceiveNext();
			//animator.ApllyJump((bool)stream.ReceiveNext());
			//Debug.Log (wallState);
		}
	}
	public void Activate(){
		if(cameraController!=null){
			_rb.isKinematic = false;
			isActive = true;
			_rb.detectCollisions = true;
			cameraController.enabled = true;
			cameraController.Reset();
			GetComponent<ThirdPersonController> ().enabled= true;
		}

		for (int i =0; i<myTransform.childCount; i++) {
			myTransform.GetChild(i).gameObject.SetActive(true);
		}
		photonView.RPC("RPCActivate",PhotonTargets.OthersBuffered);
	}
	[RPC]
	public void RPCActivate(){
		//Debug.Log ("RPCActivate");
		if(cameraController!=null){
			cameraController.enabled = true;
			isActive = true;
			GetComponent<ThirdPersonController> ().enabled= true;

		}
		for (int i =0; i<myTransform.childCount; i++) {
			myTransform.GetChild(i).gameObject.SetActive(true);
		}
	}
	public void DeActivate(){
		if(cameraController!=null){
			_rb.isKinematic = true;
			isActive = false;
			_rb.detectCollisions = false;
			cameraController.enabled = false;

			GetComponent<ThirdPersonController> ().enabled= false;
		}
		for (int i =0; i<myTransform.childCount; i++) {
			myTransform.GetChild(i).gameObject.SetActive(false);
		}
		photonView.RPC("RPCDeActivate",PhotonTargets.OthersBuffered);
		
	}
	[RPC]
	public void RPCDeActivate(){
		//Debug.Log ("RPCDeActivate");
		if(cameraController!=null){
			cameraController.enabled = false;
			isActive = false;
			GetComponent<ThirdPersonController> ().enabled= false;
		}
		for (int i =0; i<myTransform.childCount; i++) {
			myTransform.GetChild(i).gameObject.SetActive(false);
		}

	}
	[RPC]
	public void RPCSetHealth(float value){
		health = value;
	}

	//EndNetworkSection

	//Base Seenn Hear work

	public List<Pawn> getAllSeenPawn(){
		return seenPawns;
	}


	//end seen hear work

	public void addDPS (BaseDamage damage, GameObject killer)
	{
		singleDPS newDPS = new singleDPS ();
		newDPS.damage = damage;
		newDPS.killer = killer;
		
		activeDPS.Add (newDPS);
	}
}
