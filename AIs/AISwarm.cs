using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AISwarmBuff{
	public int characteristic;
	public object value;
    
    public AISwarmBuff(int characteristic, object value)
    {
        // TODO: Complete member initialization
        this.characteristic = characteristic;
        this.value = value;
    }
}

public class AISwarm:MonoBehaviour
{

    public string[] Bots;

    public AISpawnPoint[] respawns;

    public int[] enemyIndex;

    public List<Transform> pointOfInterest;
	
	public List<AISwarmBuff> allBuffs = new List<AISwarmBuff>();
	
	public List<AIBase> allPawn;

    public int aiGroup;

    public bool isActive = false;

    public ShowOnGuiComponent guiComponent;

    protected List<Transform> avaiblePoints = new List<Transform>();

    void ReloadList()
    {
        if (avaiblePoints.Count == 0)
        {
            foreach (Transform point in pointOfInterest)
            {
                avaiblePoints.Add(point);
            }
        }

    }
    public Transform[] GetPointOfInterest(int count)
    {
        if (pointOfInterest.Count < count) {
            return pointOfInterest.ToArray();
        }
        Transform[] returnTransform = new Transform[count];
        //	Debug.Log ("patrolPoint" +count);
        ReloadList();
        for (int i = 0; i < count; i++)
        {
            int randKey = (int)(UnityEngine.Random.value * avaiblePoints.Count);
            returnTransform[i] = avaiblePoints[randKey];
            //	Debug.Log (returnTransform[i]);
            avaiblePoints.RemoveAt(randKey);
            ReloadList();
        }
        return returnTransform;
    }

    public virtual void SwarmTick(float delta)
    {
        if (isActive && Bots.Length > 0)
        {
            for (int i = 0; i < respawns.Length; i++)
            {
                AISpawnPoint go = respawns[i];
                if (go.IsAvalable())
                {
                    GameObject obj = NetworkController.Instance.PawnSpawnRequest(Bots[(int)(UnityEngine.Random.value * Bots.Length)], go.transform.position, go.transform.rotation, true, new int[0],true);
                    //	GameObject obj = PhotonNetwork.Instantiate (Bots[(int)(UnityEngine.Random.value*Bots.Length)].name, go.transform.position, go.transform.rotation, 0,null) as GameObject;
                    Pawn pawn =obj.GetComponent<Pawn>();
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
	void Update(){
		DrawCheck();
	}
	public virtual void DrawCheck(){
		if(isActive){
			guiComponent.Show()
		}else{
			guiComponent.Hide()
		}
	}
    public virtual void Init(int i)
    {

        aiGroup = i;
        ReloadList();
		
    }
    public virtual void DecideCheck() { 
            
    }
	public void RemoteAdd(AIBase ai){
		allPawn.Add(ai);
		foreach(AISwarmBuff buff in allBuffs){
            ai.GetPawn().AddBuff(buff.characteristic, buff.value);
		}
	}
    public virtual void AfterSpawnAction(AIBase ai) { 
		allPawn.Add(ai);
		foreach(AISwarmBuff buff in allBuffs){
            ai.GetPawn().AddBuff(buff.characteristic, buff.value);
		}
    }
    public virtual void AgentKilled(AIBase ai) { 
		allPawn.Remove(ai);
    }
    public virtual void Activate() {
        Debug.Log("Activate");
        isActive = true;
		AIDirector.instance.ActivateSwarm(aiGroup);
    }
	public virtual void   DeActivate(){
		Debug.Log("DeActivate");
        isActive = true;
		AIDirector.instance.DeactivateSwarm(aiGroup);
	}
    public bool IsEnemy(int enemyGroup)
    {
        foreach (int enemy in enemyIndex)
        {
            if (enemy == enemyGroup)
            {

                return true;
            }
        }
        return false;
    }

    public Transform[] GetRoutePoint()
    {
        return pointOfInterest.ToArray();
    }
	
	public void AddBuffOnAll(int characteristic, object value){
		allBuffs.Add(new AISwarmBuff(characteristic,value));
      
      
        foreach (AIBase aiBase in allPawn)
        {
			aiBase.GetPawn().AddBuff(characteristic,value);
		}
	}
	public void RemoveBuffOnAll(int characteristic, object value){
		allBuffs.RemoveAt(allBuffs.FindIndex( delegate(AISwarmBuff eff) {
                return eff.characteristic == characteristic&&value.Equals(eff.value) ;
               
			}));
        foreach (AIBase aiBase in allPawn)
        {
			aiBase.GetPawn().RemoveBuff(characteristic,value);
		}
	}
}
