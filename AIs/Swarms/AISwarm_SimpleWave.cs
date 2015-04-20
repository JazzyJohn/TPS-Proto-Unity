using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

public class AISwarm_SimpleWave : AISwarm
{
    public int maxSpawnCount;

    public int needToKill;

    private int _alreadySpawn;

    private int _alreadyDead;

   
	public override  void DrawCheck(){
		base. DrawCheck();
        if (guiComponent != null)
        {
            guiComponent.SetTitle((needToKill - _alreadyDead) + "/" + needToKill);
        }
	}
	
	 public override void SendData(ISFSObject swarmSend)
    {
        base.SendData(swarmSend);
        swarmSend.PutInt("maxSpawnCount", maxSpawnCount);
        swarmSend.PutInt("needToKill", needToKill);
       
    }
    public override void ReadData(ISFSObject iSFSObject)
    {
        base.ReadData(iSFSObject);
        _alreadyDead = iSFSObject.GetInt("alreadyDead");
        
    }
}
