using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

public class AISwarm_HoldWave : AISwarm_QuantizeWave
{
	
	

    public override void AgentKilled(AIBase ai)
    {
        base.AgentKilled(ai);
        _alreadyDead++; 
    }

  
    
    public void NextSwarmWave()
    {
        _curWave++;
        _alreadyDead = 0;
		SendMessage("NextWave", SendMessageOptions.DontRequireReceiver);
        HoldPosition_PVEGameRule rule=  GameRule.instance as HoldPosition_PVEGameRule;
        if (rule != null)
        {
            rule.NextWave(_curWave);
        }
	}
   
}
