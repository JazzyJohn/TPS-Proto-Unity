using UnityEngine;
using System;


public class UseObject : DestroyableNetworkObject {

	public bool onTouch;
	
	public bool isOneUse;
	
	public float coolDownTime;
	
	private float coolDownTimer;
	
	public Texture guiIcon;

	public string tooltip;

	public Transform myTransform;

	
	//	Use function check if this object can be used turn on cooldown
	virtual public void Use(Pawn target){
		if(coolDownTimer>0){
			return;
		}
		if(ActualUse(target)){
			if(isOneUse){
				if(!photonView.isMine){
					RequestKillMe();
				}else{
					PhotonNetwork.Destroy(photonView);
				}
			}
			coolDownTimer=coolDownTime;
		}
	
	}
	protected void Awake () {
		photonView = GetComponent<PhotonView> ();
		myTransform = transform;
	}
	protected void Start(){

	}

	//Actual logic of object;
	virtual public bool ActualUse(Pawn target){
		return true;
	}
	void Update(){
		if(coolDownTimer>0){
			
			coolDownTimer-=Time.deltaTime;
		}
	
	}
	void OnTriggerEnter	(Collider other) {
		if(onTouch){
			Pawn pawn =(Pawn)other.GetComponent(typeof(Pawn));
			if(pawn!=null){
				Use(pawn);
			}
		
		}
	}

}