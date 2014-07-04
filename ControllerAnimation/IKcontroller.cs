using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class IKcontroller : MonoBehaviour {
	
	public AimIK aim;
	protected float targetWeight=1.0f;
	private float vel = 0.0f;
	/// <summary>
    /// IS we under IK controll
    /// </summary>
	public bool IsIk(){
		return targetWeight>0.5;
	}
	void Start(){
		aim = gameObject.GetComponent<AimIK>();
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
	
}	