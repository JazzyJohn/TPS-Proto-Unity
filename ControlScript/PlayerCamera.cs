using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
    public Vector3 normalOffset = Vector3.zero;

    public Transform cameraTransform;

    /*protected Transform minimapTransform;
    protected Camera minimapCamera;*/
    protected Transform _target;
    protected Pawn _pawn;

    protected Vector3 headOffset = Vector3.zero;
    protected Vector3 centerOffset = Vector3.zero;

    public float MaxYAngle = 90f;
    public float MinYAngle = -90f;
	// Use this for initialization
	void Start () {
	
	}
    protected void InitOffsets()
    {

        CapsuleCollider characterController = _target.GetComponent<CapsuleCollider>();
        if (characterController != null)
        {
            centerOffset = characterController.bounds.center - _target.position;
            headOffset = centerOffset;
            headOffset.y = characterController.bounds.max.y - _target.position.y;

        }
  
        //Cut(_target, centerOffset);
    }
    protected virtual void Cut(Transform dummyTarget, Vector3 dummyCenter)
    {


    }

	// Update is called once per frame
	void Update () {
	
	}
    public virtual void ToggleAim(bool value)
    {
        


    }
    public virtual void AddShake(float mod)
    {
        
    }
    public virtual void Reset()
    {
  

    }


    public virtual void SetAimFOV(float p)
    {
       
    }
}
