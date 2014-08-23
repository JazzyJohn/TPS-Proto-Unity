using UnityEngine;
using System.Collections;

public class AISwarm_SimpleWave : AISwarm
{
    public int maxSpawnCount;

    public int needToKill;

    private int _alreadySpawn;

    private int _alredyDead;

    public override void SwarmTick(float delta)
    {
        if (isActive && Bots.Length > 0 && _alreadySpawn < maxSpawnCount)
        {
            for (int i = 0; i < respawns.Length; i++)
            {
                AISpawnPoint go = respawns[i];
                if (go.IsAvalable())
                {
                    GameObject obj = NetworkController.Instance.PawnSpawnRequest(Bots[(int)(UnityEngine.Random.value * Bots.Length)], go.transform.position, go.transform.rotation, true, new int[0], true);
                    //	GameObject obj = PhotonNetwork.Instantiate (Bots[(int)(UnityEngine.Random.value*Bots.Length)].name, go.transform.position, go.transform.rotation, 0,null) as GameObject;
                    Pawn pawn = obj.GetComponent<Pawn>();
                    go.Spawned(pawn);

                    //  Debug.Log("Group before set" + this.aiGroup + "  " + aiGroup);
                    AIBase ai = obj.GetComponent<AIBase>();
                    ai.Init(aiGroup, this, i);

                    AfterSpawnAction(ai);

                }
                if (_alreadySpawn >= maxSpawnCount)
                {
                    break;
                }
            }
        }
        DecideCheck();
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
            DeActivate();
        }
    }
	public override  void DrawCheck(){
		base. DrawCheck();
        if (guiComponent != null)
        {
            guiComponent.SetTitle((needToKill - _alredyDead) + "/" + needToKill);
        }
	}
}
