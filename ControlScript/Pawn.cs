using UnityEngine;
using System.Collections;

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

public class Pawn : DamagebleObject {

	public LayerMask groundLayers = -1;
	public LayerMask wallRunLayers = -1;
	public LayerMask climbLayers = 1 << 9; // Layer 9

	private BaseWeapon CurWeapon;

	public Transform weaponSlot;

	public Transform myTransform;

	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this

	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this


	public Vector3 weaponOffset;

	public bool isDead=false;

	public string publicName;

	private Vector3 aimRotation;

	public Animator animator;
	//rEplication Section

	private CharacterState characterState;

	public ThirdPersonCamera cameraController;

	public float pitchAngle;

	public bool isAi;

	public Pawn enemy;

	private Rigidbody _rb;

	private float distToGround;

	private CapsuleCollider capsule;

	public bool canWallRun;

	public float wallRunSpeed;

	public float climbSpeed;

	public float climbCheckRadius = 0.1f;

	public float climbCheckDistance = 0.5f;

	public float heightOffsetToEdge = 2.0f;

	public float PullUpStartTimer= 0.0f;
	
	public float PullUpTime=2.0f;
	
	private float v;

	public Transform curLookTarget= null;

	public Player player=null;

	// Use this for initialization
	void Start () {
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
		myTransform = transform;
		_rb  = GetComponent<Rigidbody>();
		capsule = GetComponent<CapsuleCollider> ();

		distToGround = capsule.height/2-capsule.center.y;
		Debug.Log (distToGround);
	}
	
	public override void Damage(float damage){
		if (!PhotonNetwork.isMasterClient){
			return;
		}
		Debug.Log ("DAMAGE");
		base.Damage(damage);
	}


	public override void KillIt(){
		Debug.Log ("KILLL IT" + this);
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
		} else {

			myTransform.position = Vector3.Lerp(myTransform.position, correctPlayerPos, Time.deltaTime * 5);
			myTransform.rotation = Quaternion.Lerp(myTransform.rotation, correctPlayerRot, Time.deltaTime * 5);
		}
		if (animator != null&&animator.gameObject.activeSelf) {
			if(characterState == CharacterState.Idle) {
				animator.SetFloat("Speed",0.0f);
			}
			else 
			{
				if(characterState == CharacterState.Running) {
					animator.SetFloat("Speed",2.0f);
				}
				else if(characterState == CharacterState.Trotting) {
					animator.SetFloat("Speed",1.0f);	
				}
				else if(characterState == CharacterState.Walking) {
					animator.SetFloat("Speed",1.0f);	
				}
			}

			animator.SetLookAtPosition (getAimRotation(CurWeapon.weaponRange));
			animator.SetLookAtWeight(1, 0.5f, 0.7f, 0.0f, 0.5f);
			//animator.SetFloat("Pitch",pitchAngle);
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
		CurWeapon.AttachWeapon(weaponSlot,weaponOffset,this);
	}
	public Vector3 getAimRotation(float weaponRange){
		
		if(photonView.isMine){
			if(isAi){
				aimRotation = enemy.myTransform.position;
			}else{
				Camera maincam = Camera.main;
				Ray centerRay= maincam.ViewportPointToRay(new Vector3(.5f, 0.5f, 1f));
				RaycastHit hitInfo;
				Vector3 targetpoint = Vector3.zero;
				if (Physics.Raycast (centerRay,out hitInfo, weaponRange)) {
					targetpoint =hitInfo.point;
					curLookTarget= hitInfo.transform;
					//Debug.Log(hitInfo.collider);
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
	public bool Movement(Vector3 movement,CharacterState state){
		//Debug.Log (characterState);
		bool isGrounded = IsGrounded ();
		if (isGrounded) {
			_rb.velocity = movement;
			characterState = state;
		} else {
			v = movement.normalized.magnitude;
			switch(state)
			{
				case CharacterState.DoubleJump:
					if(characterState==CharacterState.Jumping){
						_rb.velocity = movement;
						characterState = state;
					}
				break;
				default:
					isGrounded =WallRun (movement,state);
					if(PullUpCheck()){
						isGrounded= false;
						PullUp();
					}
				break;
			}

		}
		return isGrounded;
	}

	bool WallRun (Vector3 movement,CharacterState state)
	{
		if (!canWallRun) return false;
		
		if(v < 0.2f) return false;
		
		bool leftW = Physics.Raycast (myTransform.position + myTransform.up,
		                              myTransform.right * -1 + myTransform.forward/4, capsule.radius + 0.2f, wallRunLayers);
		bool rightW = Physics.Raycast (myTransform.position + myTransform.up,
		                               myTransform.right + myTransform.forward/4, capsule.radius + 0.2f, wallRunLayers);
		bool frontW = Physics.Raycast (myTransform.position+ myTransform.up,
		                               myTransform.forward, capsule.radius + 0.2f, wallRunLayers);
		
		if(!animator.IsInTransition(0) && !_rb.isKinematic)
		{
			if(leftW)
			{
				
				_rb.velocity = movement + myTransform.up*3;
				if(!(characterState == CharacterState.WallRunning))
				{
					characterState = CharacterState.WallRunning;
					//animator.SetBool("WallRunL", true);
					StartCoroutine( WallRunCoolDown(3f)); // Exclude if not needed
				}
				
				if(state == CharacterState.Jumping)
				{
					_rb.velocity = myTransform.up*movement.y  +myTransform.right*movement.y;
					StartCoroutine( WallJump(1f)); // Exclude if not needed
				}
			}
			
			else if(rightW)
			{
				_rb.velocity = movement + myTransform.up*3;
				if(!(characterState == CharacterState.WallRunning))
				{
					characterState = CharacterState.WallRunning;
					StartCoroutine( WallRunCoolDown(3f)); // Exclude if not needed
				}
				
				if(state == CharacterState.Jumping)
				{
					_rb.velocity = myTransform.up*movement.y  +myTransform.right*movement.y;
					StartCoroutine( WallJump(1f)); // Exclude if not needed
				}
			}
			
			else if(frontW)
			{
				_rb.velocity = myTransform.up*wallRunSpeed/1.5f;
				if(!(characterState == CharacterState.WallRunning))
				{
					characterState = CharacterState.WallRunning;
					StartCoroutine( WallRunCoolDown(3f)); // Exclude if not needed
				}
				
				if(state == CharacterState.Jumping)
				{
					_rb.velocity = myTransform.up*movement.y  +myTransform.forward*-1*movement.y;
					StartCoroutine( WallJump(1f)); // Exclude if not needed
				}
			}
			
			
			animator.SetBool("WallRunL", leftW);
			
			animator.SetBool("WallRunR", rightW);
			
			animator.SetBool("WallRunUp", frontW);

			return leftW||rightW||frontW;
		}
		return false;

	}
	// Wall run cool-down
	IEnumerator WallRunCoolDown (float sec)
	{
		canWallRun = true;
		yield return new WaitForSeconds (sec/4);
		canWallRun = false;
		characterState = CharacterState.Jumping;
		yield return new WaitForSeconds (sec);
		canWallRun = true;
	}
	// Wall run cool-down
	IEnumerator WallJump (float sec)
	{
		canWallRun = false;
		characterState = CharacterState.Jumping;
		yield return new WaitForSeconds (sec);
		canWallRun = true;
	}

	public bool IsGrounded ()
	{	
		Vector3 p1 = myTransform.position +myTransform.up;
		RaycastHit hit;
		return Physics.SphereCast(p1,1.0f, -Vector3.up,out hit, distToGround);
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
		}
		else
		{
			// Network player, receive data
			Vector3 newPosition= (Vector3) stream.ReceiveNext();
			correctPlayerPos = newPosition;
			correctPlayerRot = (Quaternion) stream.ReceiveNext();
			this.aimRotation = (Vector3) stream.ReceiveNext();
			characterState = (CharacterState) stream.ReceiveNext();
			health=(float) stream.ReceiveNext();
		}
	}
	public void Activate(){
		if(cameraController!=null){
			cameraController.enabled = true;
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
}
