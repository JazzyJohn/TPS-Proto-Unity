using UnityEngine;
using System.Collections;

public class AISwarm_SimpleWave : AISwarm
{
    public int maxSpawnCount;

    public int needToKill;

    private int _alreadySpawn;

    private int _alredyDead;

   
	public override  void DrawCheck(){
		base. DrawCheck();
        if (guiComponent != null)
        {
            guiComponent.SetTitle((needToKill - _alredyDead) + "/" + needToKill);
        }
	}
}
