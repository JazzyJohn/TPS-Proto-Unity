using UnityEngine;
using System.Collections;

public class JetAnimationManager : AnimationManager
{
	public JetPackController jetController;
	public override void ApllyJump(bool jump)
    {	
		if (!jump) {
				
			jetController.StopAll();
		}
				
    }
	public override void DoubleJump(){
		jetController.StartAll();
	
	}
	public override void WallAnimation(bool leftW,bool rightW,bool frontW)
	{

		//This part is little confused, cause we activate opposite to wall jet
		if (leftW ) {
			jetController.StartRight();
		}
		if (rightW ) {
			jetController.StartLeft();
		}
		if (frontW ) {
			jetController.StartMiddle();
		}
		
	}
	public override void FreeFall(){
		jetController.StopAll();
	}

}