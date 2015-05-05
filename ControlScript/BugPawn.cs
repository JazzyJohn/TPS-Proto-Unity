using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class BugPawn : Pawn {


	public override void ToggleAim(bool value){
		isAiming = value;
		if (isAiming) {
			animator.AimEvalToWeight(1);
		}else{
			animator.AimSetWeight(0);
		}
	}


}