using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class IKcontroller : MonoBehaviour {
	
	public AimIK aim;
	protected float targetWeight;
	void Start(){
		aim = gameObject.GetComponent<AimIK>();
	}
	
	public Vector3 aimPosition{
		get{return aim.solver.GetIKPosition();}
		set{aim.solver.IKPosition = value;}
	}
	public void SetWeight(float weight){
		aim.solver.IKPositionWeight = weight;
		targetWeight = weight;
	}
	public void EvalToWeight(float weight){
		//aim.solver.IKPositionWeight = weight;
		targetWeight = weight;
	}
	void Update(){
		if (Mathf.Abs (targetWeight - aim.solver.IKPositionWeight) > 0.05f) {
			
			aim.solver.IKPositionWeight= Mathf.Lerp( aim.solver.IKPositionWeight,targetWeight,Time.deltaTime);
		}
		
	}
}	