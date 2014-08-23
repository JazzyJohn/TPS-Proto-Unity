using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AISwarmBuffTYPE{
	INT,
	FLOAT,
	BOOl
	
}

public class AISwarmBuffComponent : MonoBehaviour {
	public CharacteristicList characteristic;

	public int intValue;
	
	public float floatValue;
	
	public bool boolValue;
	
	public AISwarmBuffTYPE type;
	
	public AISwarm[]  swarm;

    public void StartUse(int owner)
    {
		switch(type){
			case AISwarmBuffTYPE.INT:
                swarm[owner - 1].AddBuffOnAll((int)characteristic, intValue);
			break;
			case AISwarmBuffTYPE.FLOAT:
            swarm[owner - 1].AddBuffOnAll((int)characteristic, floatValue);
			break;
			case AISwarmBuffTYPE.BOOl:
            swarm[owner - 1].AddBuffOnAll((int)characteristic, boolValue);
			break;
		
		}
	}
    public void StopUse(int owner)
    {
		switch(type){
			case AISwarmBuffTYPE.INT:
                swarm[owner-1].RemoveBuffOnAll((int)characteristic, intValue);
			break;
			case AISwarmBuffTYPE.FLOAT:
            swarm[owner - 1].RemoveBuffOnAll((int)characteristic, floatValue);
			break;
			case AISwarmBuffTYPE.BOOl:
            swarm[owner - 1].RemoveBuffOnAll((int)characteristic, boolValue);
			break;
		
		}
	}

  
}