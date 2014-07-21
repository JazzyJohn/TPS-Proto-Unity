using UnityEngine;
using System;
using System.Collections;

public class QueenAnimationManager : AnimationManager{



	public void CreateEgg(){
		 transform.parent.SendMessage("CreateEgg", SendMessageOptions.DontRequireReceiver);
	}
		

	public void LayedFinished(){
		 transform.parent.SendMessage("LayedFinished", SendMessageOptions.DontRequireReceiver);
	}
		



}