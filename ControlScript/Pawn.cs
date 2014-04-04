using UnityEngine;
using System.Collections;

public enum CharacterState {
	Idle = 0,
	Walking = 1,
	Trotting = 2,
	Running = 3,
	Jumping = 4,
	WallRunning = 5
}

public class Pawn : DamagebleObject {

	public LayerMask groundLayers = -1;
	public LayerMask wallRunLayers = -1;

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

	public CharacterState characterState;

	public ThirdPersonCamera cameraController;

	public float pitchAngle;

	public bool isAi;

	public Pawn enemy;

	private Rigidbody _rb;

	private float distToGround;

	private CapsuleCollider capsule;

	public bool canWallRun;
	
	private float v;

	// Use this for initialization
	void Start () {
		 photonView = GetComponent<PhotonView>();
		if (!photonView.isMine) {
						Destroy (GetComponent<ThirdPersonController> ());
						Destroy (GetComponent<ThirdPersonCamera> ());
						Destroy (GetComponent<MouseLook> ());
						Destroy (GetComponent<Rigidbody> ());
		} else {
			cameraController=GetComponent<ThirdPersonCamera> ();
			isAi = cameraController==null;
		}
		myTransform = transform;
		_rb  = GetComponent<Rigidbody>();
		capsule = GetComponent<CapsuleCollider> ();
		distToGround = capsule.bounds.extents.y-capsule.center.y;
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
	public void Movement(Vector3 movement){

		if (IsGrounded ()) {
				_rb.velocity = movement;
		} else {
			v = _rb.velocity.magnitude;
			WallRun ();
		}
	}

	void WallRun ()
	{
		if(!canWallRun) return;
		
		if(v < 0.2f) return;
		
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
				
				_rb.velocity = myTransform.forward * Mathf.Abs(v) + myTransform.up*3;
				if(!(characterState == CharacterState.WallRunning))
				{
					characterState = CharacterState.WallRunning;
					//animator.SetBool("WallRunL", true);
					StartCoroutine( WallRunCoolDown(3f)); // Exclude if not needed
				}
				
				/*if(doJumpDown)
				{
					animator.SetBool("Jump", true);
				}*/
			}
			
			else if(rightW)
			{
				_rb.velocity = myTransform.forward * Mathf.Abs(v) + myTransform.up*3;
				if(!(characterState == CharacterState.WallRunning))
				{
					characterState = CharacterState.WallRunning;
					StartCoroutine( WallRunCoolDown(3f)); // Exclude if not needed
				}
				
				/*if(doJumpDown)
				{
					animator.SetBool("Jump", true);
				}*/
			}
			
			else if(frontW)
			{
				_rb.velocity = myTransform.forward;
				if(!(characterState == CharacterState.WallRunning))
				{
					characterState = CharacterState.WallRunning;
					StartCoroutine( WallRunCoolDown(3f)); // Exclude if not needed
				}
				
				/*if(doJumpDown)
				{
					animator.SetBool("Jump", true);
				}*/
			}
			
			
			animator.SetBool("WallRunL", leftW);
			
			animator.SetBool("WallRunR", rightW);
			
			animator.SetBool("WallRunUp", frontW);
		}
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


	public bool IsGrounded ()
	{
		return Physics.Raycast(myTransform.position, -Vector3.up, distToGround);
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
