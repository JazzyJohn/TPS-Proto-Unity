using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class IKcontroller : MonoBehaviour {
	
	public AimIK aim;
    public FullBodyBipedIK fullBody;
    public GrounderFBBIK grounder;
	protected float targetWeight=1.0f;
	private float vel = 0.0f;
	/// <summary>
    /// IS we under IK controll
    /// </summary>
	public bool IsIk(){
		return targetWeight>0.5;
	}
	void Awake(){
		aim = gameObject.GetComponent<AimIK>();
        fullBody = gameObject.GetComponent<FullBodyBipedIK>();
        grounder = gameObject.GetComponent<GrounderFBBIK>();
	}
	
	public Vector3 aimPosition{
		get{return aim.solver.GetIKPosition();}
		set{aim.solver.IKPosition = value;}
	}
	public void SetWeight(float weight){
       
			aim.solver.IKPositionWeight = weight;
          // Debug.Log("WEIGHT" + weight);
			targetWeight = weight;
	}
	public void EvalToWeight(float weight){
		//aim.solver.IKPositionWeight = weight;
         //Debug.Log("WEIGHT" + weight);
		targetWeight = weight;
	}
	void Update(){
		if (Mathf.Abs (targetWeight - aim.solver.IKPositionWeight) > 0.01f) {
			
			aim.solver.IKPositionWeight= Mathf.SmoothDamp(aim.solver.IKPositionWeight,targetWeight,ref vel,0.5f); 
		}
		
	}
    public void AddAction(UpdateFinished finished)
    {
        GetComponentInChildren<AimIK>().solver.action = finished;
    }
    public bool  ActiveAim()
    {
        if (aim != null)
        {
            return aim.enabled;
        }
        return false;
    }
    public void AimOff()
    {
        if (aim != null)
        {
            aim.enabled = false;
        }
    }
    public void AimOn()
    {
        if (aim != null)
        {
            aim.enabled = true;
        }
    }
    public void IKShutDown()
    {
        if (aim != null)
        {
            aim.enabled = false;
        }
        if (fullBody != null)
        {
            fullBody.enabled = false;
        }
        if (grounder != null)
        {
            grounder.enabled = false;
        }
    }
    public void IKTurnOn()
    {
        if (aim != null)
        {
            aim.enabled = true;
        }
        if (fullBody != null)
        {
            fullBody.enabled = true;
        }
        if (grounder != null)
        {
            grounder.enabled = true;
        }
    }

     public void SetMuzzle(Transform point)
    {
        aim.solver.transform = point;
       
    }
}