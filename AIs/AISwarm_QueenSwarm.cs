using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AISwarm_QueenSwarm : AISwarm
{
	public QueenPawn queen;
	public string queenPawn;
    public int maxSwarmling;
    public override void DecideCheck() {
        if (isActive&&(queen == null || queen.isDead))
        {
            SendMessage("SwarmEnd", SendMessageOptions.DontRequireReceiver);
            Hunt_PVPGameRule huntGameRule = GameRule.instance as Hunt_PVPGameRule;
            if (huntGameRule != null)
            {
                huntGameRule.LastWaveAnonce();
            }
            DeActivate();
        }
    }

    public List<QueenEgg> eggs = new List<QueenEgg>();

    public override void Activate()
    {
        AISpawnPoint respawn = respawns[(int)(UnityEngine.Random.value * respawns.Length)];

        GameObject obj = NetworkController.Instance.PawnSpawnRequest(queenPawn, respawn.transform.position, respawn.transform.rotation, true, new int[0], true);
        queen =(QueenPawn) obj.GetComponent<Pawn>();
        queen.SetTeam(0);
      
        AIBase ai = obj.GetComponent<AIBase>();
        ai.Init(aiGroup, this, -1);
        AfterSpawnAction(ai);
        base.Activate();
    }
	public override void Init(int i)
    {


		base.Init(i);

        if (isActive)
        {

            Activate();
        }
    }
	public override void SwarmTick(float delta)
    {
        if (isActive && Bots.Length > 0)
        {

            while (eggs.Count > 0 && eggs[0].ready && allPawn.Count <= maxSwarmling)
                {
            


                    // GameObject obj = PhotonNetwork.InstantiateSceneObject(Bots[(int)(UnityEngine.Random.value * Bots.Length)],eggs[i].position,eggs[i].rotation, 0, null) as GameObject;
                    GameObject obj = NetworkController.Instance.PawnSpawnRequest(Bots[(int)(UnityEngine.Random.value * Bots.Length)], eggs[0].transform.position, eggs[0].transform.rotation, true, new int[0], true);

                    //	GameObject obj = PhotonNetwork.Instantiate (Bots[(int)(UnityEngine.Random.value*Bots.Length)].name, go.transform.position, go.transform.rotation, 0,null) as GameObject;
                    // go.Spawned(obj.GetComponent<Pawn>());
                    obj.GetComponent<Pawn>().SetTeam(0);
                    //  Debug.Log("Group before set" + this.aiGroup + "  " + aiGroup);
                    AIBase ai = obj.GetComponent<AIBase>();
                    ai.Init(aiGroup, this, -1);
                    AfterSpawnAction(ai);
                    Destroy(eggs[0].gameObject);
                    eggs.RemoveAt(0);

                }
            
        }
        DecideCheck();
    }
	public void AddEgg(Transform egg){
		eggs.Add(egg.GetComponent<QueenEgg>());
	
	}
}
