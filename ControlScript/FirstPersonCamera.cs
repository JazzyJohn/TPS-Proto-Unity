using UnityEngine;
using System.Collections;

public class FirstPersonCamera : PlayerCamera {


	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationX = 0F;
	float rotationY = 0F;
	
	Quaternion originalRotation;
    void Awake()
    {

        if (!cameraTransform && Camera.main)
            cameraTransform = Camera.main.transform;

        if (!cameraTransform)
        {
            //Debug.Log("Please assign a camera to the ThirdPersonCamera script.");
            enabled = false;
        }
       
        //minimapTransform = GameObject.FindGameObjectWithTag ("MinimapCamera").GetComponent<Transform> ();
        //minimapCamera = minimapTransform.camera;
        _target = transform;
        _pawn = GetComponent<Pawn>();
        
        //TODO: Learn about wtf this do here
        //EventHolder.instance.Bind (this);

    }

	void Update ()
	{
        if (PlayerMainGui.IsMouseAV)
        {
            if (axes == RotationAxes.MouseXAndY)
            {
                // Read the mouse input axis
                rotationX += InputManager.instance.GetMouseAxis("Mouse X") * sensitivityX;
                rotationY += InputManager.instance.GetMouseAxis("Mouse Y") * sensitivityY;

                rotationX = ClampAngle(rotationX, minimumX, maximumX);
                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);

                cameraTransform.rotation = originalRotation * xQuaternion * yQuaternion;
            }
            else if (axes == RotationAxes.MouseX)
            {
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationX = ClampAngle(rotationX, minimumX, maximumX);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                cameraTransform.rotation = originalRotation * xQuaternion;
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
                cameraTransform.rotation = originalRotation * yQuaternion;
            }
       
        }
        Vector3 targetHead = _target.position + cameraTransform.rotation * normalOffset;
        cameraTransform.position = targetHead;
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
		originalRotation = transform.localRotation;
	}
	
	public static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}
}
