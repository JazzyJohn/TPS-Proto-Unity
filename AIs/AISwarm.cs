using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AISwarm:MonoBehaviour
{

    public string[] Bots;

    public AISpawnPoint[] respawns;

    public int[] enemyIndex;

    public List<Transform> pointOfInterest;

    public int aiGroup;

    public bool isActive = false;


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

    public virtual void SwarmTick()
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
                    go.Spawned(obj.GetComponent<Pawn>());
                    //  Debug.Log("Group before set" + this.aiGroup + "  " + aiGroup);
                    AIBase ai = obj.GetComponent<AIBase>();
                    ai.Init(aiGroup, this, i);
                    AfterSpawnAction(ai);
                }
            }
        }
        DecideCheck();
    }
    public virtual void Init(int i)
    {

        aiGroup = i;
        ReloadList();
    }
    public virtual void DecideCheck() { 
            
    }
    public virtual void AfterSpawnAction(AIBase ai) { 
    
    }
    public virtual void AgentKilled() { 
    
    }
    public virtual void Activate() {
        Debug.Log("Activate");
        isActive = true;
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
}
