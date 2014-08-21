using UnityEngine;
using System.Collections;

public class AISwarm_QuantizeWave : AISwarm
{
	
	public int[] maxSpawnCount;
	
	public int _curWave=0;
	
    public int[] needToKill;
	
	private int _alreadySpawn;

    private int _alreadyDead;

    public override void SwarmTick(float delta)
    {
        if (isActive && Bots.Length > 0 && _alreadySpawn < maxSpawnCount[_curWave])
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
                if (_alreadySpawn >= maxSpawnCount[_curWave])
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
        _alreadyDead++;
        if (_alreadyDead >= maxSpawnCount[_curWave] || _alreadyDead >= needToKill[_curWave]) {
            
            NextWave();
        }
    }
	public override  void DrawCheck(){
		base. DrawCheck();
        if (guiComponent != null)
        {
            guiComponent.SetTitle((needToKill[_curWave] - _alreadyDead) + "/" + needToKill[_curWave]);
        }
	}
	public void   NextWave(){
		_alreadyDead =0;
		_alreadySpawn = 0;
		_curWave++;
        Hunt_PVPGameRule huntGameRule = GameRule.instance as Hunt_PVPGameRule;
		if(_curWave>=needToKill.Length){
			SendMessage("SwarmEnd", SendMessageOptions.DontRequireReceiver);
			if(huntGameRule!=null){
				huntGameRule.LastWave();
			}
			DeActivate();
		}else{
			SendMessage("NextWave", SendMessageOptions.DontRequireReceiver);
			if(huntGameRule!=null){
				huntGameRule.NextWave(_curWave);
			}
		}
	}
}
