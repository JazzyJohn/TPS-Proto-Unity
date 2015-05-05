
using UnityEngine;
using System.Collections;
using System;

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
    protected override void Apply()
    {
        // Early out if we don't have a target
        /*if (!controller)
            return;
        */
        Vector3 targetCenter = _target.position + centerOffset;
        Vector3 targetHead = _target.position + headOffset;


        Vector3 centerPoint = Vector3.zero;
        centerPoint.z = minimapOffset.y;
        centerPoint.x = 0.5f;
        centerPoint.y = 0.5f;
        /*
            if (minimapTransform != null) {
                            minimapTransform.position = targetCenter + minimapTransform.position - minimapCamera.ViewportToWorldPoint (centerPoint);
                    }*/
        //	DebugDrawStuff();

        // Calculate the current & target rotation angles
        float originalTargetAngle = _target.eulerAngles.y;
        float currentAngle = cameraTransform.eulerAngles.y;

        // Adjust real target angle when camera is locked
        float targetAngle = originalTargetAngle;

        // When pressing Fire2 (alt) the camera will snap to the target direction real quick.
        // It will stop snapping when it reaches the target
        /*if (Input.GetButton("Fire2"))
            snap = true;
		
        if (snap)
        {
            // We are close to the target, so we can stop snapping now!
            if (AngleDistance (currentAngle, originalTargetAngle) < 3.0f)
                snap = false;
			
            currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref angleVelocity, snapSmoothLag, snapMaxSpeed);
        }
        // Normal camera motion
        else
        {
            if (controller.GetLockCameraTimer () < lockCameraTimeout)
            {
                targetAngle = currentAngle;
            }
	
            // Lock the camera when moving backwards!
            // * It is really confusing to do 180 degree spins when turning around.
            if (AngleDistance (currentAngle, targetAngle) > 160 && controller.IsMovingBackwards ())
                targetAngle += 180;
	
            currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref angleVelocity, angularSmoothLag, angularMaxSpeed);
        }
	
    */
        currentAngle = targetAngle;
        // When jumping don't move camera upwards but only down!
        /*if (controller.IsJumping ())
        {
            // We'd be moving the camera upwards, do that only if it's really high
            float newTargetHeight= targetCenter.y + height;
            if (newTargetHeight < targetHeight || newTargetHeight - targetHeight > 5)
                targetHeight = targetCenter.y + height;
        }
        // When walking always update the target height
        else
        {
            targetHeight = targetCenter.y + height;
        }*/
        targetHeight = targetCenter.y + height;
        // Damp the height

        if (aiming)
        {
            targetOffset = aimingOffset;
            Camera.main.fieldOfView = aimFov;
        }
        else
        {
            targetOffset = normalOffset;
            Camera.main.fieldOfView = startFov;
        }

        // Convert the angle into a rotation, by which we then reposition the camera
        Quaternion currentRotation = Quaternion.identity;

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        float vert = InputManager.instance.GetMouseAxis("Mouse Y");
        float horizont = InputManager.instance.GetMouseAxis("Mouse X");
        if (PlayerMainGui.IsMouseAV)
        {
            yAngle -= vert * angularMaxSpeed;

            xAngle += horizont * angularMaxSpeed;
        }
        if (yAngle > MaxYAngle)
        {
            yAngle = MaxYAngle;
        }
        if (yAngle < MinYAngle)
        {
            yAngle = MinYAngle;
        }
        float distance = -targetOffset.z;
        Vector3 FlatOffset = targetOffset;
        FlatOffset.z = 0;
        //Quaternion pitchRotation= Quaternion.Euler (yAngle, xAngle, 0);
        Vector3 localCamOffset = Quaternion.Euler(yAngle, xAngle, 0) * Vector3.back * distance;
        Vector3 localTargetOffset = Quaternion.Euler(0, xAngle, 0) * FlatOffset;
        FlatOffset.x = 0;
        Vector3 localFlatOffset = Quaternion.Euler(0, xAngle, 0) * FlatOffset;
        Vector3 resultcameraPos = targetCenter;
        Vector3 targetforCamera = targetCenter;
        //Debug.Log(pitchRotation* Vector3.back );
        resultcameraPos.y = targetHeight;
        localCamOffset += localTargetOffset;
        //localCamOffset =  localCamOffset;
        resultcameraPos += localCamOffset;
        //		Debug.Log (localTargetOffset);
        Vector3 direction = (resultcameraPos - targetforCamera - localFlatOffset);
        Ray wallRay = new Ray(targetforCamera + FlatOffset, direction.normalized);
        //Debug.DrawLine (targetCenter+ localTargetOffset, targetCenter+ localTargetOffset + direction.normalized*distance);
        direction = (resultcameraPos - targetforCamera - localTargetOffset);
        //	Debug.DrawRay (wallRay.origin, wallRay.direction);
        float magnitude = distance * distance + 10.0f;
        //Debug.DrawLine (wallRay.origin,wallRay.origin+ wallRay.direction*distance);

        foreach (RaycastHit target in Physics.RaycastAll(wallRay, distance, cameraLayer))
        {


            if (target.distance < magnitude)
            {
                magnitude = target.distance;
            }
            else
            {
                //	Debug.Log(magnitude+ " " +target.distance);
                continue;
            }
            if (target.transform != _target)
            {
                //Debug.Log(target.collider);
                //Vector3 newPostion  = 	target.point-direction.normalized*1.0f;
                Vector3 offsetDirection = (target.point - wallRay.origin);

                resultcameraPos = target.point - offsetDirection.normalized * 1.0f;
            }

        }

        foreach (RaycastHit target in Physics.SphereCastAll(wallRay, 0.5f, distance, cameraLayer))
        {


            if (target.distance < magnitude)
            {
                magnitude = target.distance;
            }
            else
            {
                //Debug.Log(magnitude+ " " +target.distance);
                continue;
            }
            if (target.transform != _target)
            {
                //Debug.Log(target.collider);
                //Vector3 newPostion  = 	target.point-direction.normalized*1.0f;
                Vector3 offsetDirection = (target.point - wallRay.origin);

                resultcameraPos = target.point - offsetDirection.normalized * 1.0f;
            }

        }

        cameraTransform.position = resultcameraPos + GetShaker();
        // Always look at the target	
        Vector3 relativePos = (targetforCamera + localTargetOffset) - cameraTransform.position;

        cameraTransform.rotation = Quaternion.LookRotation(-direction);

        ///SetUpRotation(targetCenter+ localTargetOffset, targetHead);
    }
	void Update(){
			
		
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
    void LateUpdate()
    {
        Apply();
    }
	void FinishKillCam(){
      
		PlayerMainGui.instance.StopKillCam();	
		this.enabled = false;
	}
	public bool Init(Player Killer){
		
		_pawn  =Killer.GetActivePawn();

//        Debug.Log("KILLER PAWWN"+_pawn);
		killpalyer = Killer;
        return _Init(_pawn);
		
	}
    public bool Init(Pawn Killer)
    {
        killpalyer = null;
        return _Init(Killer);
    }
	bool _Init(Pawn Killer){

      
            _pawn  =Killer;
//            Debug.Log("KILLER PAWWN" + _pawn);
            killpawn = Killer;
		    killCamTimer = 0.0f;
            if (Player.localPlayer != null)
            {
                killCamTime = Player.localPlayer.respawnTime;
            }
		  
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