using UnityEngine;
using System.Collections;

public class AIDirector_Runner : AIDirector {

    public void NextBlock()
    {
        swarms = FindObjectsOfType<AISwarm>();
        for (int i = 0; i < swarms.Length; i++)
        {
            swarms[i].Init(i);


        }
        selfRespawns = FindObjectsOfType<SelfSpawnPoint>();
    }
}
