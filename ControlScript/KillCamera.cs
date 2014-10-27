
using UnityEngine;
using System.Collections;

public class KillCamera : ThirdPersonCamera
{
	public float killCamTime = 3.0f;
	
	public Vector3 mediumTargetOffset  = Vector3.zero;
	
	public Vector3 bigTargetOffset  = Vector3.zero;

    public float killCamTimer = 0.0f;

    public Player killpalyer;

    public Pawn killpawn;

    public BaseWeapon killerweapon;
	
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
		/*if(_pawn==null){
			FinishKillCam();
			return;
		}*/
		killCamTimer+= Time.deltaTime;
     //   Debug.Log(killCamTime+" < "+killCamTimer);
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

        Debug.Log("KILLER PAWWN"+_pawn);
		killpalyer = Killer;
		return Init (_pawn);
		
	}
	public bool Init(Pawn Killer){
		
		_pawn  =Killer;
        killpawn = Killer;
		killCamTimer = 0.0f;
		if(_pawn !=null){
			_target =_pawn.myTransform;
			if(_pawn.bigTarget){
				normalOffset =bigTargetOffset;
			}else{
				normalOffset =mediumTargetOffset;
				
			}
			if(!_pawn.isAi){
				killerweapon = ItemManager.instance.weaponPrefabsListbyId[_pawn.CurWeapon.SQLId];
            }
            else
            {
                killerweapon = null;
            }
			InitOffsets();
			return true;
		}
	

		return   false;
	}
	
	


}