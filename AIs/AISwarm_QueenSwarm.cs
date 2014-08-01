using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AISwarm_QueenSwarm : AISwarm
{
	public QueenPawn queen;
	public string queenPawn;
    public override void DecideCheck() {
        if (queen==null|| queen.isDead) {
            isActive = false;
        }
    }
	
	public List<Transform> eggs = new List<Transform>();
	public override void Init(int i)
    {

       
		AISpawnPoint respawn = respawns[(int)(UnityEngine.Random.value * respawns.Length)];
		
		 GameObject obj = NetworkController.PawnSpawnRequest(queenPawn, respawn.transform.position, respawn.transform.rotation, true, new int[]);
                 
		obj.GetComponent<Pawn>().SetTeam(0);
		AIBase ai = obj.GetComponent<AIBase>();
        ai.Init(aiGroup, this, -1);
        AfterSpawnAction(ai);
		base.Init(i);
    }
	public override void SwarmTick()
    {
        if (isActive && Bots.Length > 0)
        {
            for (int i = 0; i < eggs.Count; i++)
            {
              
                   // GameObject obj = PhotonNetwork.InstantiateSceneObject(Bots[(int)(UnityEngine.Random.value * Bots.Length)],eggs[i].position,eggs[i].rotation, 0, null) as GameObject;
					GameObject obj = NetworkController.PawnSpawnRequest(Bots[(int)(UnityEngine.Random.value * Bots.Length)], eggs[i].position,eggs[i].rotation, true, new int[]);
                                  
				 //	GameObject obj = PhotonNetwork.Instantiate (Bots[(int)(UnityEngine.Random.value*Bots.Length)].name, go.transform.position, go.transform.rotation, 0,null) as GameObject;
                    // go.Spawned(obj.GetComponent<Pawn>());
					obj.GetComponent<Pawn>().SetTeam(0);
                    //  Debug.Log("Group before set" + this.aiGroup + "  " + aiGroup);
                    AIBase ai = obj.GetComponent<AIBase>();
                    ai.Init(aiGroup, this, -1);
                    AfterSpawnAction(ai);
                
            }
        }
        DecideCheck();
    }
	public void AddEgg(Transform egg){
		eggs.Add(egg);
	
	}
}
