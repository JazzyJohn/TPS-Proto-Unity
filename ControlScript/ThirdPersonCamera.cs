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



    protected Vector3 targetOffset = Vector3.zero;
	//public Vector3 normalOffset  = Vector3.zero;
    public Vector3 aimingOffset = Vector3.zero;

	public Vector3 minimapOffset  = new Vector3(0.0f,40.0f,0.0f);

    protected bool aiming = false;
	
	protected bool isFps =false;

    public float inWallDrop;

    public float maxDistance;
    public float xOffset = 0.0f;
	
	private float heightVelocity= 0.0f;
	private float angleVelocity= 0.0f;
	private bool snap= false;
	private ThirdPersonController controller;
	protected float targetHeight= 100000.0f; 

	public float yAngle =0.0f;
	public float xAngle = 0.0f;

	public const float RECOIL_GLOBAL_MOD = 3.0f;

	private bool closeFOV= false;
    protected float startFov = 60.0f;
	public float sprintFov =60.0f;
    public float aimFov = 60.0f;

    public static LayerMask cameraLayer = -123909;
	Vector3 curAddShake= Vector3.zero;

    Camera mainCamera;
	
	void  Awake (){
		
		if(!cameraTransform && Camera.main)
			cameraTransform = Camera.main.transform;
	
		if(!cameraTransform) {
			//Debug.Log("Please assign a camera to the ThirdPersonCamera script.");
			enabled = false;	
		}
		startFov = Camera.main.fieldOfView;
        mainCamera = Camera.main;
		//minimapTransform = GameObject.FindGameObjectWithTag ("MinimapCamera").GetComponent<Transform> ();
		//minimapCamera = minimapTransform.camera;
		_target = transform;
		_pawn = GetComponent<Pawn> ();
		InitOffsets();
		
		//TODO: Learn about wtf this do here
		//EventHolder.instance.Bind (this);

	}

	

	public override void ToggleAim(bool value,bool isFps){
		aiming = value;
		this.isFps = isFps;
		
	}
	void  DebugDrawStuff (){
		//Debug.DrawLine(_target.position, _target.position + headOffset);
	
	}
	
	public float AngleDistance ( float a ,   float b  ){
		a = Mathf.Repeat(a, 360);
		b = Mathf.Repeat(b, 360);
		
		return Mathf.Abs(b - a);
	}
	
	protected virtual void  Apply (   ){
		// Early out if we don't have a target
		/*if (!controller)
			return;
		*/
		float lXOffset;
        if (aiming)
        {
			if(isFps){
				targetOffset= Vector3.Zero;
				lXOffset =0;
			}else{
				lxOffset = xOffset;
				targetOffset = aimingOffset;
			}
            Camera.main.fieldOfView = aimFov;
        }
        else
        {
			lxOffset = xOffset;
            targetOffset = normalOffset;
            Camera.main.fieldOfView = startFov;
        }
		
        Vector3 targetHead = _target.position + _pawn.headOffset +_pawn.GetDesireRotation()*Vector3.right*lxOffset;
        Vector3 lOffset = _pawn.GetDesireRotation()* targetOffset;

        Debug.DrawLine(targetHead, targetHead + lOffset, Color.red);

        Vector3 resultcameraPos = targetHead + lOffset;

        Ray wallRay = new Ray(targetHead, lOffset.normalized);
        float distance = lOffset.magnitude;
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
       


        Vector3 fromHeadDirection = targetHead - (resultcameraPos + GetShaker());
        Vector3 direction = _pawn.getAimpointForCamera() - (resultcameraPos + GetShaker());
       
        if (fromHeadDirection.sqrMagnitude < maxDistance|| Vector3.Dot(direction,fromHeadDirection)<0)
        {
            resultcameraPos = targetHead + lOffset.normalized * inWallDrop;
            direction = _pawn.getAimpointForCamera() - (resultcameraPos + GetShaker());
            if (mainCamera.useOcclusionCulling)
            {
                mainCamera.useOcclusionCulling = false;
            }
        }
        else
        {
            if (!mainCamera.useOcclusionCulling)
            {
                mainCamera.useOcclusionCulling = true;
            }
        }


        cameraTransform.position = resultcameraPos + GetShaker();
        // Always look at the target	
     
        cameraTransform.rotation = Quaternion.LookRotation(direction);
	}

    protected Vector3 GetShaker()
    {

	
		curAddShake= Vector3.Lerp (curAddShake,Vector3.zero,Time.deltaTime*10);
    //    Debug.Log(curAddShake);
		return curAddShake;

	}
	public Vector3 CurrrentOffset(){
		return (cameraTransform.position - _target.position + centerOffset);
	}
	public override void AddShake(float mod){
		if (_pawn != null) {
//            Debug.Log(mod);
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