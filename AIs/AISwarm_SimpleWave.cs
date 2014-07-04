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
        _alreadySpawn++;
    }
    public override void AgentKilled()
    {
        _alredyDead++;
        if (_alredyDead >= maxSpawnCount || _alredyDead >= needToKill) {
            SendMessage("SwarmEnd", SendMessageOptions.DontRequireReceiver);
        }
    }
}
