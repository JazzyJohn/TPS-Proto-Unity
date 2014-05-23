
using UnityEngine;
using System.Collections;

public class KillCamera : ThirdPersonCamera
{
	public float killCamTime = 3.0f;

	private float killCamTimer = 0.0f;
	
	private Player killpalyer; 
	
	void  Awake (){
		
		if(!cameraTransform && Camera.main)
			cameraTransform = Camera.main.transform;
	
		if(!cameraTransform) {
			//Debug.Log("Please assign a camera to the ThirdPersonCamera script.");
			enabled = false;	
		}
		
		

	}
	void Update(){
			
		if(killpalyer!=null){
			Pawn pawn  =killpalyer.GetActivePawn();
			if(pawn!=_pawn){
				Init(pawn);
			}
		}
		if(_pawn==null){
			FinishKillCam();
			return;
		}
		killCamTimer+= Time.deltaTime;
		if(killCamTime<killCamTimer){
			FinishKillCam();
		}
		
	}
	void FinishKillCam(){
	
		PlayerMainGui.instance.StopKillCam();	
		this.enabled = false;
	}
	public bool Init(Player Killer){
		
		_pawn  =Killer.GetActivePawn();
		killpalyer = Killer;
		return Init (_pawn);
		
	}
	public bool Init(Pawn Killer){
		
		_pawn  =Killer;
		killCamTimer = 0.0f;
		if(_pawn !=null){
			_target =_pawn.myTransform;
			InitOffsets();
			return true;
		}
	

		return   false;
	}
	
	


}