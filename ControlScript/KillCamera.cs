
using UnityEngine;
using System.Collections;

public class KillCamera : ThirdPersonCamera
{
	private float killCamTime = 3.0f;
	
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
		killCamTime-= Time.deltaTime;
		if(killCamTime<0){
			FinishKillCam();
		}
		
	}
	void FinishKillCam(){
	
		PlayerMainGui.instance.StopKillCam();	
		Destroy(this);
	}
	public bool Init(Player Killer){
		
		_pawn  =Killer.GetActivePawn();
		killpalyer = Killer;
		return Init (_pawn);
		
	}
	public bool Init(Pawn Killer){
		
		_pawn  =Killer;
		
		if(_pawn !=null){
			_target =_pawn.myTransform;
			return false;
		}
	
		InitOffsets();
		return   true;
	}
	
	


}