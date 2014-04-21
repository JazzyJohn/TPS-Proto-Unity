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

public class Pawn : DamagebleObject {

	public LayerMask groundLayers = -1;
	public LayerMask wallRunLayers = -1;
	public LayerMask climbLayers = 1 << 9; // Layer 9

	public BaseWeapon CurWeapon;

	public Transform weaponSlot;

	public Transform myTransform;

	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this

	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

	public Vector3 weaponOffset;

	public Vector3 weaponRotatorOffset;

	public bool isDead=false;

	public string publicName;

	private Vector3 aimRotation;
	//rotation for moment when rotation of camera and pawn can be different e.t.c wall run	
	private Vector3 forwardRotation;

	public AnimationManager animator;

	private CharacterState characterState;

	private WallState wallState;

	private CharacterState nextState;

	private Vector3 nextMovement;

	public ThirdPersonCamera cameraController;

	public float pitchAngle;

	public bool isAi;

	public Pawn enemy;

	private Rigidbody _rb;

	private float distToGround;

	private CapsuleCollider capsule;

	public bool canWallRun;

	public float wallRunSpeed;

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

	private ContactPoint[] contacts;

	public Vector3 centerOffset;

	public Vector3 headOffset;

	public static float gravity = 20.0f;

	public InventoryManager ivnMan;

	public bool isAiming=false;

	public float aimModCoef = -10.0f;
	// Use this for initialization
	void Start () {
		maxHealth = health;
		 photonView = GetComponent<PhotonView>();
		if (!photonView.isMine) {
						Destroy (GetComponent<ThirdPersonController> ());
						Destroy (GetComponent<ThirdPersonCamera> ());
						Destroy (GetComponent<MouseLook> ());
						GetComponent<Rigidbody> ().isKinematic = true;
		} else {
			cameraController=GetComponent<ThirdPersonCamera> ();
			isAi = cameraController==null;
		}
		ivnMan =GetComponent<InventoryManager> ();
		myTransform = transform;
		correctPlayerPos = transform.position;
		myCollider = collider;
		_rb  = GetComponent<Rigidbody>();
		capsule = GetComponent<CapsuleCollider> ();
		centerOffset = capsule.bounds.center - myTransform.position;
		headOffset = centerOffset;
		headOffset.y = capsule.bounds.max.y - myTransform.position.y;

		distToGround = capsule.height/2-capsule.center.y;
		Debug.Log (distToGround);
	}
	
	public override void Damage(float damage,GameObject killer){
		if (!PhotonNetwork.isMasterClient){
			return;
		}
		Pawn killerPawn =killer.GetComponent<Pawn> ();
		if (killerPawn != null && killerPawn.team == team &&! PlayerManager.instance.frendlyFire) {
			return;
		}
		//Debug.Log ("DAMAGE");
		base.Damage(damage,killer);
	}

	public void Heal(float damage,GameObject Healler){
		health += damage;
		if (maxHealth < health) {
			health=maxHealth;		
		}

	}

	public override void KillIt(GameObject killer){
		if (isDead) {
			return;		
		}
		isDead = true;
		Pawn killerPawn =killer.GetComponent<Pawn> ();
		Player killerPlayer = null;
		if (killerPawn != null) {
			killerPlayer = killerPawn.player;
			if(killerPlayer!=null){
				killerPlayer.PawnKill(player);
			}
		}
		Debug.Log ("KILLL IT" + player);
		if (player != null) {
			if(player.inBot){
				player.RobotDead(killerPlayer);
			}else{
				player.PawnDead(killerPlayer);
			}
		}

		if (CurWeapon != null) {
			CurWeapon.	RequestKillMe();
		}

		RequestKillMe();
		
	}


	// Update is called once per frame
	void Update () {
		//Debug.Log (photonView.isSceneView);

		if (photonView.isMine) {
						if (isAi) {
								Quaternion aimRotation = Quaternion.LookRotation (enemy.myTransform.position - myTransform.position);
								pitchAngle = aimRotation.eulerAngles.x;
						} else {
								pitchAngle = -cameraController.yAngle;

						}
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
					//Debug.DrawLine(startpoint,normalDist*100+startpoint);
					if (allPawn[i].team!=team&&Physics.Raycast(startpoint,normalDist,out hitInfo)) {
						//Debug.Log(hitInfo.collider);
				
						if(allPawn[i].myCollider!=hitInfo.collider){
							continue;
						}
					}
					seenPawns.Add(allPawn[i]);
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
				if(characterState == CharacterState.WallRunning){
					if(forwardRotation.sqrMagnitude>0){
						myTransform.rotation= Quaternion.LookRotation(forwardRotation);
					}
				}else{
					myTransform.rotation= Quaternion.Euler(eurler);
				}
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
				if(characterState == CharacterState.WallRunning){
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

			myTransform.position = Vector3.Lerp(myTransform.position, correctPlayerPos, Time.deltaTime * 5);
			myTransform.rotation = Quaternion.Lerp(myTransform.rotation, correctPlayerRot, Time.deltaTime * 5);

		}
		if (animator != null&&animator.gameObject.activeSelf) {
			if(characterState == CharacterState.Idle) {
				animator.ApllyMotion(0.0f,0.0f);
			}
			else 
			{
				if(characterState == CharacterState.Running) {
					animator.ApllyMotion(2.0f,0.0f);
				}
				else if(characterState == CharacterState.Trotting) {
					animator.ApllyMotion(1.0f,0.0f);	
				}
				else if(characterState == CharacterState.Walking) {
					animator.ApllyMotion(1.0f,0.0f);	
				}
				else if( characterState ==CharacterState.WallRunning){
					//Debug.Log ("INSWITCH");
					switch(wallState){
						case WallState.WallF:
						animator.WallAnimation(false,false,true);
						 	break;
						case WallState.WallR:
							animator.WallAnimation(false,true,false);
							break;
						case WallState.WallL:
							animator.WallAnimation(true,false,false);
							break;
					}

				}
			}

			animator.animator.SetLookAtPosition (aimRotation);
			animator.animator.SetLookAtWeight(1, 0.5f, 0.7f, 0.0f, 0.5f);

		}

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
	public Vector3 getAimRotation(float weaponRange){
		
		if(photonView.isMine){
			if(isAi){
				aimRotation = enemy.myTransform.position;
			}else{
				if(cameraController.enabled ==false){
					aimRotation= myTransform.position +myTransform.forward*50;
					return aimRotation;
				}
				Camera maincam = Camera.main;
				Ray centerRay= maincam.ViewportPointToRay(new Vector3(.5f, 0.5f, 1f));
				RaycastHit hitInfo;
				Vector3 targetpoint = Vector3.zero;
				if (Physics.Raycast (centerRay,out hitInfo, weaponRange)&&hitInfo.collider!=collider) {
					targetpoint =hitInfo.point;
					curLookTarget= hitInfo.transform;
					//Debug.Log (curLookTarget);
				//	Debug.Log((targetpoint-myTransform.position).sqrMagnitude.ToString()+(cameraController.normalOffset.magnitude+5));
					if((targetpoint-myTransform.position).sqrMagnitude<cameraController.normalOffset.magnitude+5){
						targetpoint =maincam.transform.forward*weaponRange +maincam.ViewportToWorldPoint(new Vector3(.5f, 0.5f, 1f));
						animator.WeaponDown(true);
					}else{
						animator.WeaponDown(false);
					}
				}else{
					targetpoint =maincam.transform.forward*weaponRange +maincam.ViewportToWorldPoint(new Vector3(.5f, 0.5f, 1f));
				}
				aimRotation=targetpoint; 
				
			}
			
			return aimRotation;
		}else{
			return aimRotation;
		}
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
	//END WEAPON SECTION
	void OnCollisionEnter(Collision collision) {
		Debug.Log ("COLLISION ENTER PAWN " + this + collision);
	}
	void OnTriggerEnter	(Collider other) {
		Debug.Log ("TRIGGER ENTER PAWN "+ this +  other);
	}

	//TODO: MOVE THAT to PAwn and turn on replication of aiming
	//TODO REPLICATION
	


		

	//Movement section
	public void Movement(Vector3 movement,CharacterState state){
		//Debug.Log (state);

		nextState = state;

		nextMovement  = movement;



	}

	bool WallRun (Vector3 movement,CharacterState state)
	{
		if (!canWallRun&&photonView.isMine) return false;

		//if (isGrounded) return false;
		
		if(v < 0.2f&&photonView.isMine) return false;
	
		//Debug.Log (movement);
		RaycastHit leftH,rightH,frontH;
		
		
		bool leftW = Physics.Raycast (myTransform.position + myTransform.up,
		                              myTransform.right * -1 ,out leftH, capsule.radius + 0.2f,wallRunLayers);
		bool rightW = Physics.Raycast (myTransform.position + myTransform.up,
		                               myTransform.right ,out rightH, capsule.radius + 0.2f, wallRunLayers);
		bool frontW = Physics.Raycast (myTransform.position+ myTransform.up,
		                               myTransform.forward,out frontH, capsule.radius + 0.2f, wallRunLayers);

		/*Debug.DrawRay (myTransform.position + myTransform.up,
		               myTransform.right * -1 );
		
		Debug.DrawRay (myTransform.position + myTransform.up,
		               myTransform.right );
		
		Debug.DrawRay (myTransform.position+ myTransform.up,
		               myTransform.forward);*/
		if (!photonView.isMine) {
			if( leftW||rightW||frontW){
					characterState = CharacterState.WallRunning;
				return true;
			}else
				return false;
			     
		}


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
					StartCoroutine( WallRunCoolDown(3f)); // Exclude if not needed
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
					StartCoroutine( WallRunCoolDown(3f)); // Exclude if not needed
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
					StartCoroutine( WallRunCoolDown(3f)); // Exclude if not needed
				}
				
				if(state == CharacterState.Jumping)
				{
					_rb.velocity = myTransform.up*movement.y  +myTransform.forward*-1*movement.y;
					StartCoroutine( WallJump(1f)); // Exclude if not needed
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
	    contacts = collisionInfo.contacts;
		if (contacts != null) {
			foreach (ContactPoint contact in contacts) {
				/*if(contact.otherCollider.CompareTag("decoration")){
					continue;
				}*/
				Vector3 Direction = contact.point - myTransform.position;
				//Debug.Log (this.ToString()+Vector3.Dot(Direction.normalized ,Vector3.down) );
				if (Vector3.Dot (Direction.normalized, Vector3.down) > 0.75) {
					isGrounded = true;
				}
				///Debug.DrawRay(contact.point, contact.normal, Color.white);
			}
			contacts= null;	
		}


	}
	public void FixedUpdate () {



		if (isGrounded) {
			if (photonView.isMine) {
				if (_rb.isKinematic) _rb.isKinematic= false;
				Vector3 velocity = rigidbody.velocity;
				Vector3 velocityChange = (nextMovement - velocity);
			
				rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
			}
			characterState = nextState;
			if(nextState==CharacterState.Jumping){
				Jump ();

			}

		} else {
			
			v = nextMovement.normalized.magnitude;
			
			switch(nextState)
			{
			case CharacterState.DoubleJump:
				if(characterState!=CharacterState.WallRunning
				   &&characterState!=CharacterState.PullingUp){
					if (photonView.isMine) {
						Vector3 velocity = rigidbody.velocity;
						Vector3 velocityChange = (nextMovement - velocity);
						//Debug.Log("DOUBLE JUMP");
						
						rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
					}
					characterState = nextState;
				}
				break;
			default:

				if(!WallRun (nextMovement,nextState)){
					animator.ApllyJump(true);						
					animator.WallAnimation(false,false,false);
					
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
			
			_rb.AddForce(new Vector3(0,-gravity * rigidbody.mass,0));
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
		animator.ApllyJump(true);		
		//photonView.RPC("JumpChange",PhotonTargets.OthersBuffered,true);
	}

	public void DidLand(){
		animator.ApllyJump(false);
		//photonView.RPC("JumpChange",PhotonTargets.OthersBuffered,false);
	}

	bool PullUpCheck(){
		if (characterState == CharacterState.PullingUp) {
			return true;
		}
		Vector3 p1 = myTransform.position - (myTransform.up * -heightOffsetToEdge) + myTransform.forward;
		Vector3 p2 = myTransform.position - (myTransform.up * -heightOffsetToEdge);
		//Debug.DrawLine (p1, p2);
		RaycastHit hit;
		//Debug.DrawLine (p1-myTransform.up*climbCheckDistance, p2-myTransform.up*climbCheckDistance);
		// Hit nothing and not at edge -> Out
		return Physics.CapsuleCast (p1, p2, climbCheckRadius, -myTransform.up, out hit, climbCheckDistance, climbLayers);
	
	}


	void PullUp ()
	{
			//Debug.Log (characterState);
			if(	characterState != CharacterState.PullingUp){
				characterState = CharacterState.PullingUp;
				_rb.isKinematic = true;
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
				} else { // Animation is finished 
						_rb.isKinematic = false;
						characterState = CharacterState.Idle;
				}
	}

	public void StopMachine(){
		characterState = CharacterState.Idle;

	}
	//end Movement Section

	//NetworkSection
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(aimRotation);
			stream.SendNext(characterState);
			stream.SendNext(health);
			stream.SendNext(wallState);
			stream.SendNext(netIsGround);
			//stream.SendNext(animator.GetJump());

		}
		else
		{
			// Network player, receive data
			Vector3 newPosition= (Vector3) stream.ReceiveNext();
			correctPlayerPos = newPosition;
			correctPlayerRot = (Quaternion) stream.ReceiveNext();
			this.aimRotation = (Vector3) stream.ReceiveNext();
			nextState = (CharacterState) stream.ReceiveNext();
			//Debug.Log (characterState);
			health=(float) stream.ReceiveNext();
			wallState = (WallState) stream.ReceiveNext();
			isGrounded =(bool) stream.ReceiveNext();
			//animator.ApllyJump((bool)stream.ReceiveNext());
			//Debug.Log (wallState);
		}
	}
	public void Activate(){
		if(cameraController!=null){
			_rb.isKinematic = false;
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
		Debug.Log ("RPCActivate");
		if(cameraController!=null){
			cameraController.enabled = true;
			GetComponent<ThirdPersonController> ().enabled= true;

		}
		for (int i =0; i<myTransform.childCount; i++) {
			myTransform.GetChild(i).gameObject.SetActive(true);
		}
	}
	public void DeActivate(){
		if(cameraController!=null){
			_rb.isKinematic = true;
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
		Debug.Log ("RPCDeActivate");
		if(cameraController!=null){
			cameraController.enabled = false;

			GetComponent<ThirdPersonController> ().enabled= false;
		}
		for (int i =0; i<myTransform.childCount; i++) {
			myTransform.GetChild(i).gameObject.SetActive(false);
		}

	}
	//EndNetworkSection

	//Base Seenn Hear work

	public List<Pawn> getAllSeenPawn(){
		return seenPawns;
	}


	//end seen hear work
}
