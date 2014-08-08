using UnityEngine;
using System.Collections;

public class AISwarm_SimpleWave : AISwarm
{
    public int maxSpawnCount;

    public int needToKill;

    private int _alreadySpawn;

    private int _alredyDead;

    public override void DecideCheck() {
        if (_alreadySpawn >= maxSpawnCount) {
            isActive = false;
        }
    }
    public override void AfterSpawnAction(AIBase ai)
    {
		base.AfterSpawnAction(ai);
        _alreadySpawn++;
    }
    public override void AgentKilled(AIBase ai)
    {
		base.AgentKilled(ai);
        _alredyDead++;
        if (_alredyDead >= maxSpawnCount || _alredyDead >= needToKill) {
            SendMessage("SwarmEnd", SendMessageOptions.DontRequireReceiver);
        }
    }
}
