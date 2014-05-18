using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class IKcontroller : MonoBehaviour {

	public AimIK aim;

	void Start(){
		aim = gameObject.GetComponent<AimIK>();
	}

	public Vector3 aimPosition{
		get{return aim.solver.GetIKPosition();}
		set{aim.solver.IKPosition = value;}
	}
	
}
