using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

public class AISwarm_QuantizeWave : AISwarm
{
	
	public int[] maxSpawnCount;
	
	public int _curWave=0;
	
    public int[] needToKill;
	
	private int _alreadyDead;

    public override void AgentKilled(AIBase ai)
    {
        base.AgentKilled(ai);
        _alreadyDead++; 
    }
   
  
	public override  void DrawCheck(){
       
		base. DrawCheck();
        if (!isActive)
        {
            return;
        }
        if (guiComponent != null)
        {
            guiComponent.SetTitle((needToKill[_curWave] - _alreadyDead) + "/" + needToKill[_curWave]);
        }
	}

    public void NextSwarmWave()
    {
        _curWave++;
        _alreadyDead = 0;
		SendMessage("NextWave", SendMessageOptions.DontRequireReceiver);
		
	}
    public override void SendData(ISFSObject swarmSend)
    {
        base.SendData(swarmSend);
        swarmSend.PutIntArray("maxSpawnCount", maxSpawnCount);
        swarmSend.PutIntArray("needToKill", needToKill);
       
    }
    public override void ReadData(ISFSObject iSFSObject)
    {
        _alreadyDead = iSFSObject.GetInt("alreadyDead");
        _curWave = iSFSObject.GetInt("curWave");
    }
}
