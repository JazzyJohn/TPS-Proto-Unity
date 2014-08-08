using UnityEngine;
using System.Collections;

public class AITimedSwarm : AISwarm
{
    private float _timer = 0.0f;

    public float timeToSpawn;

    public override void SwarmTick(float delta)
    {
        if (isActive && Bots.Length > 0)
        {
            _timer += delta;
            
            if (timeToSpawn < _timer)
            {
                _timer = 0;
                for (int i = 0; i < respawns.Length; i++)
                {
                    AISpawnPoint go = respawns[i];
                   
                        GameObject obj = NetworkController.Instance.PawnSpawnRequest(Bots[(int)(UnityEngine.Random.value * Bots.Length)], go.transform.position, go.transform.rotation, true, new int[0], true);
                        //	GameObject obj = PhotonNetwork.Instantiate (Bots[(int)(UnityEngine.Random.value*Bots.Length)].name, go.transform.position, go.transform.rotation, 0,null) as GameObject;
                        Pawn pawn = obj.GetComponent<Pawn>();
                        go.Spawned(pawn);
                        //  Debug.Log("Group before set" + this.aiGroup + "  " + aiGroup);
                        AIBase ai = obj.GetComponent<AIBase>();
                        ai.Init(aiGroup, this, i);

                        AfterSpawnAction(ai);
                    
                }
            }
        }
        DecideCheck();
    }

    public override void Init(int i)
    {
        _timer = timeToSpawn;
        Debug.Log("init");
        base.Init(i);
    }
}
