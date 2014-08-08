using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AISwarmBuffTYPE{
	INT,
	FLOAT,
	BOOl
	
}

public class AISwarmBuff : MonoBehaviour {
	public CharacteristicList characteristic;

	public int intValue;
	
	public float floatValue;
	
	public bool boolValue;
	
	public AISwarmBuffTYPE type;
	
	public AISwarm  swarm;
	
	public StartBuff(){
		switch(type){
			case AISwarmBuffTYPE.INT:
				swarm.AddBuffOnAll((int)characteristic,intValue);
			break;
			case AISwarmBuffTYPE.FLOAT:
				swarm.AddBuffOnAll((int)characteristic,floatValue);
			break;
			case AISwarmBuffTYPE.BOOl:
				swarm.AddBuffOnAll((int)characteristic,boolValue);
			break;
		
		}
	}
	public StopBuff(){
		switch(type){
			case AISwarmBuffTYPE.INT:
				swarm.RemoveBuff((int)characteristic,intValue);
			break;
			case AISwarmBuffTYPE.FLOAT:
				swarm.RemoveBuff((int)characteristic,floatValue);
			break;
			case AISwarmBuffTYPE.BOOl:
				swarm.RemoveBuff((int)characteristic,boolValue);
			break;
		
		}
	}
}