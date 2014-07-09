using UnityEngine;
using System.Collections;

public class PartAi : Part {
	void Started(){
		AISwarm[] ChildComp = PartTransform.GetComponentsInChildren<AISwarm>();
		foreach (AISwarm Ai in ChildComp)
			Ai.Init(Numb);
	}
}
