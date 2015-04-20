using UnityEngine;
using System.Collections;

public class AITimedSwarm : AISwarm
{
    private float _timer = 0.0f;

    public float timeToSpawn;

    

    public override void Init(int i)
    {
        _timer = timeToSpawn;
        Debug.Log("init");
        base.Init(i);
    }
}
