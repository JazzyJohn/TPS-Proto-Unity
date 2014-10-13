using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class JuggerHand : MonoBehaviour {

	public AimIK aim;
	public FullBodyBipedIK ik;

	private IKEffector leftHand {get {return ik.solver.leftHandEffector;}}
	private IKEffector rightHand {get {return ik.solver.rightHandEffector;}}

	void Start(){
		aim.Disable();
	}

	void LateUpdate(){
		Vector3 toLeftHand = leftHand.bone.position - rightHand.bone.position;
		Vector3 toLeftHandRelative = rightHand.bone.InverseTransformDirection(toLeftHand);

		aim.solver.Update();

		leftHand.position = rightHand.bone.position + rightHand.bone.TransformDirection(toLeftHandRelative);
		leftHand.positionWeight = 1f;

		aim.solver.Update();
	}
}
