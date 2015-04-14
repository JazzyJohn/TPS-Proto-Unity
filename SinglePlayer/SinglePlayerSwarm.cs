using UnityEngine;
using System.Collections;

public class SinglePlayerSwarm : AISwarm
{
    public int botToSpawn;

    public bool spawned= false;
    public void Activate()
    {
        if (spawned)
        {
            return;
        }
        spawned =true;
        for (int i = 0; i < botToSpawn; i++)
        {
            int randKey = (int)(UnityEngine.Random.value * Bots.Length);
            SpawnBot(Bots[randKey], i, respawns[i].position);
        }
    }
    public void Update()
    {
        
    }

    public virtual void SpawnBot(string prefabName, int point, Vector3 position)
    {

        position = NormalizePositon(position);
        GameObject obj = NetworkController.Instance.SpawnSinglePlayerPrefab(prefabName, position, respawns[point].transform.rotation);
        Pawn pawn = obj.GetComponent<Pawn>();
        pawn.foxView.SetMine(true);
        pawn.SetTeam(0);

        AIBase ai = obj.GetComponent<AIBase>();
        ai.Init(aiGroup, this, point);
        AfterSpawnAction(ai);
        pawn.AfterAwake();
       
    }
}
