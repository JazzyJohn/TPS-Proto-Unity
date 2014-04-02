using UnityEngine;
using System.Collections;

public class Pawn : DamagebleObject {

	private BaseWeapon CurWeapon;

	public Transform weaponSlot;

	public Transform myTransform;

	public Vector3 weaponOffset;

	public bool isDead=false;

	public string publicName;

	public PhotonView photonView;

	private Vector3 aimRotation;

	public Animator animator;
	//rEplication Section

	public CharacterState characterState;

	public ThirdPersonCamera cameraController;

	public float pitchAngle;

	public bool isAi;

	public Pawn enemy;
	// Use this for initialization
	void Start () {
		 photonView = GetComponent<PhotonView>();
		if (!photonView.isMine) {
						Destroy (GetComponent<ThirdPersonController> ());
						Destroy (GetComponent<ThirdPersonCamera> ());
						Destroy (GetComponent<MouseLook> ());
		} else {
			cameraController=GetComponent<ThirdPersonCamera> ();
			isAi = cameraController==null;
		}
		myTransform = transform;
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
		photonView.RPC("RPCKillIt",PhotonTargets.All);
		
	}

	[RPC]
	public void RPCKillIt(){
		Debug.Log ("REMOTE KILLL IT" + this);
		if (photonView.isMine) {
			PhotonNetwork.Destroy (photonView);
		}
		
	}
	// Update is called once per frame
	void Update () {
		//Debug.Log (photonView.isSceneView);
		if (isAi) {
				Quaternion aimRotation = Quaternion.LookRotation (enemy.myTransform.position - myTransform.position);
				pitchAngle = aimRotation.eulerAngles.x;
		} else {
				pitchAngle = -cameraController.yAngle;
		}
		if (animator != null) {
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
			animator.SetFloat("Pitch",pitchAngle);
		}

	}

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

	void OnCollisionEnter(Collision collision) {
		Debug.Log ("COLLISION ENTER PAWN " + this + collision);
	}
	void OnTriggerEnter	(Collider other) {
		Debug.Log ("TRIGGER ENTER PAWN "+ this +  other);
	}

	//TODO: MOVE THAT to PAwn and turn on replication of aiming
	//TODO REPLICATION
	

	
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
						Debug.Log(hitInfo.collider);
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

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(aimRotation);
		}
		else
		{
			// Network player, receive data
			Vector3 newPosition= (Vector3) stream.ReceiveNext();
			this.transform.position = newPosition;
			this.transform.rotation = (Quaternion) stream.ReceiveNext();
			this.aimRotation = (Vector3) stream.ReceiveNext();
		}
	}

}
