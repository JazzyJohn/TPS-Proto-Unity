using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using nstuff.juggerfall.extension.models;

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

    public Transform[] respawns;

    public int[] enemyIndex;

    public int timeDelay;

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

    protected void Awake()
    {
        guiComponent = GetComponent<ShowOnGuiComponent>();
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

	void Update(){
		DrawCheck();
	}
	public virtual void DrawCheck(){
        if (guiComponent != null)
        {
            if (isActive)
            {
                guiComponent.Show();
            }
            else
            {
                guiComponent.Hide();
            }
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
		foreach (AIBase aiBase in allPawn)
        {
			aiBase.AllyKill();
		}
    }
    public virtual void Activate() {
        Debug.Log("Activate");
        isActive = true;
		
    }
	public virtual void   DeActivate(){
	//	Debug.Log("DeActivate");
        isActive = false;
        SendMessage("SwarmEnd", SendMessageOptions.DontRequireReceiver);
		
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
	public void NewEnemy(Pawn enemy){
		foreach (AIBase aiBase in allPawn)
        {
			aiBase.EnemyFromSwarm(enemy);
		}
	}

    public void ChangeState(bool state)
    {
        if (state)
        {
            Activate();
        }
        else
        {
            DeActivate();
        }
    }

    public void SpawnBot(string prefabName, int point, Vector3 position)
    {

        GameObject obj = NetworkController.Instance.PawnForSwarmSpawnRequest(prefabName, position, respawns[point].transform.rotation, new int[0], aiGroup, point);
        Pawn pawn = obj.GetComponent<Pawn>();
        pawn.SetTeam(0);

        AIBase ai = obj.GetComponent<AIBase>();
        ai.Init(aiGroup, this, point);
        AfterSpawnAction(ai);
    }

    public virtual  void SendData(ISFSObject swarmSend)
    {
        ISFSArray points = new SFSArray();
        foreach (Transform respawn in respawns)
        {
            points.AddClass(new Vector3Model(respawn.position));
        }
        swarmSend.PutLong("timeDelay",timeDelay);
        swarmSend.PutSFSArray("points", points);
        swarmSend.PutUtfStringArray("bots", Bots);
        swarmSend.PutUtfString("class", this.GetType().Name);
    }

    public virtual void ReadData(ISFSObject iSFSObject)
    {
        isActive = iSFSObject.GetBool("active");
    }
}
