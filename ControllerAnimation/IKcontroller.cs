using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class IKcontroller : MonoBehaviour {
	
	public AimIK aim;
    public FullBodyBipedIK fullBody;
    public GrounderFBBIK grounder;
	protected float targetWeight=1.0f;
    private float motionWeight = 1.0f;
    private float actionWeight = 1.0f;
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
		set{
            if(aim!=null)
                aim.solver.IKPosition = value;
        }
	}
	public void SetWeight(float weight){
       
			
//           Debug.Log("WEIGHT" + weight);
            actionWeight = weight;
           
	}

    public void SetMotionWeight(float weight)
    {
        motionWeight = weight;
  
    }
	public void EvalToWeight(float weight){
		//aim.solver.IKPositionWeight = weight;
       //  Debug.Log("WEIGHT" + weight);
        actionWeight = weight;
	}
	void Update(){
        if (actionWeight == 1.0f)
        {
            targetWeight = motionWeight;
        }
        else
        {
            targetWeight =actionWeight;
        }
     
        if (aim!=null)
            aim.solver.IKPositionWeight = targetWeight; 
		
	}
    public void AddAction(UpdateFinished finished)
    {
        //GetComponentInChildren<AimIK>().solver.action = finished;
    }
    public bool  ActiveAim()
    {
        if (aim != null)
        {
            if (aim.enabled)
            {
                return aim.solver.IKPositionWeight == 1.0f;
            }
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
        if (aim != null)
            aim.solver.transform = point;
       
    }
}