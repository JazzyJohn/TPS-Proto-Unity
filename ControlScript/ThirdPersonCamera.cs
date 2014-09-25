// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : PlayerCamera
{

	
	// The distance in the x-z plane to the target
	
	//public float distance= 7.0f;
	
	// the height we want the camera to be above the target
	public float height= 3.0f;
	
	public float angularSmoothLag= 0.3f;
	public float angularMaxSpeed= 15.0f;
	
	public float heightSmoothLag= 0.3f;
	
	public float snapSmoothLag= 0.2f;
	public float snapMaxSpeed= 720.0f;
	
	public float clampHeadPositionScreenSpace= 0.75f;
	
	public float lockCameraTimeout= 0.2f;



	private Vector3 targetOffset  = Vector3.zero;
	//public Vector3 normalOffset  = Vector3.zero;
	public Vector3 aimingOffset  = Vector3.zero;

	public Vector3 minimapOffset  = new Vector3(0.0f,40.0f,0.0f);

	private bool aiming = false;

	public float MaxYAngle=90f;
	public float MinYAngle=-90f;

	
	
	private float heightVelocity= 0.0f;
	private float angleVelocity= 0.0f;
	private bool snap= false;
	private ThirdPersonController controller;
	private float targetHeight= 100000.0f; 

	public float yAngle =0.0f;
	public float xAngle = 0.0f;

	public const float RECOIL_GLOBAL_MOD = 3.0f;

	private bool closeFOV= false;
	private float startFov =60.0f;
	public float sprintFov =60.0f;
    public float aimFov = 60.0f;

    private static LayerMask layer = -123909;
	Vector3 curAddShake= Vector3.zero;
	
	void  Awake (){
		
		if(!cameraTransform && Camera.main)
			cameraTransform = Camera.main.transform;
	
		if(!cameraTransform) {
			//Debug.Log("Please assign a camera to the ThirdPersonCamera script.");
			enabled = false;	
		}
		startFov = Camera.main.fieldOfView;
		//minimapTransform = GameObject.FindGameObjectWithTag ("MinimapCamera").GetComponent<Transform> ();
		//minimapCamera = minimapTransform.camera;
		_target = transform;
		_pawn = GetComponent<Pawn> ();
		InitOffsets();
		
		//TODO: Learn about wtf this do here
		//EventHolder.instance.Bind (this);

	}

	

	public override void ToggleAim(bool value){
		aiming = value;
		
	}
	void  DebugDrawStuff (){
		//Debug.DrawLine(_target.position, _target.position + headOffset);
	
	}
	
	public float AngleDistance ( float a ,   float b  ){
		a = Mathf.Repeat(a, 360);
		b = Mathf.Repeat(b, 360);
		
		return Mathf.Abs(b - a);
	}
	
	void  Apply (   ){
		// Early out if we don't have a target
		/*if (!controller)
			return;
		*/
		Vector3 targetCenter= _target.position + centerOffset;
		Vector3 targetHead= _target.position + headOffset;


		Vector3 centerPoint=Vector3.zero;
		centerPoint.z = minimapOffset.y;
		centerPoint.x = 0.5f;
		centerPoint.y = 0.5f;
	
		if (minimapTransform != null) {
						minimapTransform.position = targetCenter + minimapTransform.position - minimapCamera.ViewportToWorldPoint (centerPoint);
				}
	//	DebugDrawStuff();
	
		// Calculate the current & target rotation angles
		float originalTargetAngle= _target.eulerAngles.y;
		float currentAngle= cameraTransform.eulerAngles.y;
	
		// Adjust real target angle when camera is locked
		float targetAngle= originalTargetAngle; 
		
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

		if (aiming) {
				targetOffset = aimingOffset;
                Camera.main.fieldOfView = aimFov;
		} else {
			targetOffset = normalOffset;
            Camera.main.fieldOfView = startFov;
		}
	
		// Convert the angle into a rotation, by which we then reposition the camera
		Quaternion currentRotation = Quaternion.identity;
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		float vert =InputManager.instance.GetMouseAxis("Mouse Y");
        float horizont = InputManager.instance.GetMouseAxis("Mouse X");
		if (PlayerMainGui.IsMouseAV) {
				yAngle -= vert * angularMaxSpeed;

				xAngle += horizont * angularMaxSpeed;
		}
		if (yAngle > MaxYAngle) {
			yAngle = MaxYAngle;
		}
		if(yAngle < MinYAngle) {
			yAngle = MinYAngle;
		}
		float distance = -targetOffset.z;
		Vector3 FlatOffset = targetOffset;
		FlatOffset.z = 0;
		//Quaternion pitchRotation= Quaternion.Euler (yAngle, xAngle, 0);
		Vector3 localCamOffset =  Quaternion.Euler (yAngle, xAngle, 0)* Vector3.back * distance;
		Vector3 localTargetOffset =  Quaternion.Euler (0, xAngle, 0)* FlatOffset;
		FlatOffset.x = 0;
		Vector3 localFlatOffset =  Quaternion.Euler (0, xAngle, 0)* FlatOffset;
		Vector3 resultcameraPos = targetCenter ;
		Vector3 targetforCamera = targetCenter;
		//Debug.Log(pitchRotation* Vector3.back );
		resultcameraPos.y=targetHeight;
		localCamOffset += localTargetOffset;
		//localCamOffset =  localCamOffset;
		resultcameraPos +=localCamOffset;
//		Debug.Log (localTargetOffset);
		Vector3 direction =  (resultcameraPos - targetforCamera -localFlatOffset);
		Ray wallRay = new Ray (targetforCamera+FlatOffset, direction.normalized);
		//Debug.DrawLine (targetCenter+ localTargetOffset, targetCenter+ localTargetOffset + direction.normalized*distance);
		direction =  (resultcameraPos - targetforCamera -localTargetOffset);
	//	Debug.DrawRay (wallRay.origin, wallRay.direction);
		float magnitude = distance*distance+10.0f;
		//Debug.DrawLine (wallRay.origin,wallRay.origin+ wallRay.direction*distance);

        foreach (RaycastHit target in Physics.RaycastAll(wallRay, distance, layer))
        {
			
			
			if(target.distance<magnitude){
				magnitude=target.distance;
			}else{
			//	Debug.Log(magnitude+ " " +target.distance);
				continue;
			}
			if(target.transform!= _target){
				//Debug.Log(target.collider);
				//Vector3 newPostion  = 	target.point-direction.normalized*1.0f;
				Vector3 offsetDirection =  (target.point - wallRay.origin);
				
				resultcameraPos = 	target.point-offsetDirection.normalized*1.0f;
			} 
			
		}
       
        foreach (RaycastHit target in Physics.SphereCastAll(wallRay, 0.5f, distance, layer))
        {


			if(target.distance<magnitude){
				magnitude=target.distance;
			}else{
				//Debug.Log(magnitude+ " " +target.distance);
				continue;
			}
			if(target.transform!= _target){
				//Debug.Log(target.collider);
				//Vector3 newPostion  = 	target.point-direction.normalized*1.0f;
				Vector3 offsetDirection =  (target.point - wallRay.origin);

				resultcameraPos = 	target.point-offsetDirection.normalized*1.0f;
			} 

		}
	
		cameraTransform.position = resultcameraPos + GetShaker ();
		// Always look at the target	
		Vector3 relativePos=(targetforCamera + localTargetOffset)-cameraTransform.position;

			cameraTransform.rotation = Quaternion.LookRotation (-direction);

		///SetUpRotation(targetCenter+ localTargetOffset, targetHead);
	}
	
	Vector3 GetShaker(){

	
		curAddShake= Vector3.Lerp (curAddShake,Vector3.zero,Time.deltaTime*10);
		return curAddShake;

	}
	public Vector3 CurrrentOffset(){
		return (cameraTransform.position - _target.position + centerOffset);
	}
	public override void AddShake(float mod){
		if (_pawn != null) {
			curAddShake = UnityEngine.Random.onUnitSphere/RECOIL_GLOBAL_MOD*mod;
		}
	}

	void  LateUpdate (){
		if(!_pawn.foxView.isMine){
			return;
		}
     
		/*if (_pawn.IsSprinting ()&&!closeFOV) {
			closeFOV=true;
			Camera.main.fieldOfView =sprintFov;

		} 
		if (!_pawn.IsSprinting ()&&closeFOV) {
			closeFOV=false;
			Camera.main.fieldOfView =startFov;
			
		}*/
		Apply ();
	}
	
    protected 	override void  Cut ( Transform dummyTarget ,   Vector3 dummyCenter  ){
		float oldHeightSmooth= heightSmoothLag;
		float oldSnapMaxSpeed= snapMaxSpeed;
		float oldSnapSmooth= snapSmoothLag;
		
		snapMaxSpeed = 10000;
		snapSmoothLag = 0.001f;
		heightSmoothLag = 0.001f;
		
		snap = true;
		Apply ();
		
		heightSmoothLag = oldHeightSmooth;
		snapMaxSpeed = oldSnapMaxSpeed;
		snapSmoothLag = oldSnapSmooth;
	}
	
	public Vector3 GetCenterOffset (){
		return centerOffset;
	}
    public override void Reset()
    {
        yAngle = 0;

        xAngle = 0;

    }
    public override void SetAimFOV(float p)
    {
        if (p == 0)
        {
            aimFov = startFov;
        }
        else
        {
            aimFov = p;
        }
    }

}