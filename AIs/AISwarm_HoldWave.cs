using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

public class AISwarm_HoldWave : AISwarm_QuantizeWave
{
	[Serializable]
	public class WaveData{
		public int waveCnt;
		
		public string[] botsOnWave;
		
		public StartIntCharacteristic[] startIntCharacteristic;
		
		public StartBoolCharacteristic[] startBoolCharacteristic;
		
		public StartFloatCharacteristic[] startFloatCharacteristic;
		
		public Init(){
			bonusData = new List<CharacteristicToAdd>();
			for(int  i=0; i<startIntCharacteristic.Length;i++){
				CharacteristicToAdd add = new CharacteristicToAdd();
				add.characteristic =startIntCharacteristic[i].characteristic;
				add.effect = new Effect<int>(startIntCharacteristic[i].startValue));
				bonusData.Add(add);
		
			}
			for(int  i=0; i<startBoolCharacteristic.Length;i++){
				CharacteristicToAdd add = new CharacteristicToAdd();
				add.characteristic =startBoolCharacteristic[i].characteristic;
				add.effect = new Effect<bool>(startBoolCharacteristic[i].startValue));
				bonusData.Add(add);
			}
			for(int  i=0; i<startFloatCharacteristic.Length;i++){
				CharacteristicToAdd add = new CharacteristicToAdd();
				add.characteristic =startFloatCharacteristic[i].characteristic;
				add.effect = new Effect<float>(startFloatCharacteristic[i].startValue));
				bonusData.Add(add);
			}
		}
		List<CharacteristicToAdd> bonusData;
	}

	protected WaveData[] waveData;
	
	protected int curData = -1;
	
	protected int oldData = -1;
	
	protected void Awake()
    {
        base.Awake();
	    for(int i =0; i<waveData.Lenght;i++){
			data.Init();
			if(data.waveCnt==0){
				curData= i;
				oldData=i;
			}
	    }
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
			obj= NetworkController.Instance.PawnForSwarmSpawnRequest(prefabName, position, respawns[point].transform.rotation, new int[0], aiGroup, point,waveData[curData].bonusData);
		}else{
			obj= NetworkController.Instance.PawnForSwarmSpawnRequest(prefabName, position, respawns[point].transform.rotation, new int[0], aiGroup, point);

		}
		
        Pawn pawn = obj.GetComponent<Pawn>();
        pawn.SetTeam(0);

        AIBase ai = obj.GetComponent<AIBase>();
        ai.Init(aiGroup, this, point);
        AfterSpawnAction(ai);
    }
    
    public void NextSwarmWave()
    {
        _curWave++;
		if(oldData==waveData.Lenght-1){
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
            rule.NextWave(_curWave);
        }
	}
   
}
