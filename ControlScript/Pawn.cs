using UnityEngine;
using System.Collections;

public class Pawn : DamagebleObject {

	private BaseWeapon CurWeapon;

	public Transform weaponSlot;

	public Vector3 weaponOffset;

	public bool isDead=false;

	public string publicName;

	private PhotonView photonView;
	// Use this for initialization
	void Start () {
		 photonView = GetComponent<PhotonView>();
		if (!photonView.isMine) {
			Destroy (GetComponent<ThirdPersonController>());
			Destroy (GetComponent<ThirdPersonCamera>());
			Destroy (GetComponent<MouseLook>());
		}
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
		Debug.Log (newWeapon);
		CurWeapon.AttachWeapon(weaponSlot,weaponOffset,gameObject);
	}

	void OnCollisionEnter(Collision collision) {
		Debug.Log ("COLLISION ENTER PAWN " + this + collision);
	}
	void OnTriggerEnter	(Collider other) {
		Debug.Log ("TRIGGER ENTER PAWN "+ this +  other);
	}


	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}
		else
		{
			// Network player, receive data
			this.transform.position = (Vector3) stream.ReceiveNext();
			this.transform.rotation = (Quaternion) stream.ReceiveNext();
		}
	}

}
