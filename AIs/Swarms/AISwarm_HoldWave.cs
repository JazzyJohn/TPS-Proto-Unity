using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;

public class AISwarm_HoldWave : AISwarm_QuantizeWave
{
	[System.Serializable]
	public class WaveData{
		public int waveCnt;
		
		public string[] botsOnWave;
		
		public StartIntCharacteristic[] startIntCharacteristic;
		
		public StartBoolCharacteristic[] startBoolCharacteristic;
		
		public StartFloatCharacteristic[] startFloatCharacteristic;
		
		public void Init(){
			bonusData = new List<CharacteristicToAdd>();
			for(int  i=0; i<startIntCharacteristic.Length;i++){
				CharacteristicToAdd add = new CharacteristicToAdd();
				add.characteristic =startIntCharacteristic[i].characteristic;
                add.addEffect = new Effect<int>(startIntCharacteristic[i].startValue);
				bonusData.Add(add);
		
			}
			for(int  i=0; i<startBoolCharacteristic.Length;i++){
				CharacteristicToAdd add = new CharacteristicToAdd();
				add.characteristic =startBoolCharacteristic[i].characteristic;
                add.addEffect = new Effect<bool>(startBoolCharacteristic[i].startValue);
				bonusData.Add(add);
			}
			for(int  i=0; i<startFloatCharacteristic.Length;i++){
				CharacteristicToAdd add = new CharacteristicToAdd();
				add.characteristic =startFloatCharacteristic[i].characteristic;
                add.addEffect = new Effect<float>(startFloatCharacteristic[i].startValue);
				bonusData.Add(add);
			}
		}
		public List<CharacteristicToAdd> bonusData;
	}

	public  WaveData[] waveData;

    public string[] bosses;
	
	protected int curData = -1;
	
	protected int oldData = -1;

    public int waveWithBossCount;
	
	protected void Awake()
    {
        base.Awake();
        for (int i = 0; i < waveData.Length; i++)
        {
            waveData[i].Init();
            if (waveData[i].waveCnt == 0)
            {
                curData = i;
                oldData = i;
            }
        }
    }
    public override void SendData(ISFSObject swarmSend)
    {
        base.SendData(swarmSend);
        swarmSend.PutUtfStringArray("bosses", bosses);
        swarmSend.PutInt("waveWithBossCount", waveWithBossCount);
    }
    public override void AgentKilled(AIBase ai)
    {
        base.AgentKilled(ai);
        _alreadyDead++; 
    }

    public override void SpawnBot(string prefabName, int point, Vector3 position)
    {
		
		position =NormalizePositon(position);
        GameObject obj;
		if(curData!=-1){
            obj = NetworkController.Instance.BeginPawnForSwarmSpawnRequest(prefabName, position, respawns[point].transform.rotation, new int[0], aiGroup, point, 0, waveData[curData].bonusData);
		}else{
            obj = NetworkController.Instance.BeginPawnForSwarmSpawnRequest(prefabName, position, respawns[point].transform.rotation, new int[0], aiGroup, point);

		}
		
        Pawn pawn = obj.GetComponent<Pawn>();
        pawn.SetTeam(0);
        pawn.AfterAwake();
        AIBase ai = obj.GetComponent<AIBase>();
        ai.Init(aiGroup, this, point);
        AfterSpawnAction(ai);

        NetworkController.Instance.EndPawnSpawnRequest();
    }
    
    public  override void NextSwarmWave()
    {
        _curWave++;
		if(oldData==waveData.Length-1){
			curData = oldData;
		}else{
			if(waveData[oldData+1].waveCnt== _curWave){
				oldData++;
				curData++;
			}else{
				curData =-1;
			}
		}
		
        _alreadyDead = 0;
		SendMessage("NextWave", SendMessageOptions.DontRequireReceiver);
        HoldPosition_PVEGameRule rule=  GameRule.instance as HoldPosition_PVEGameRule;
        if (rule != null)
        {
            rule.NextWaveGame(_curWave);
        }
	}
   
}
