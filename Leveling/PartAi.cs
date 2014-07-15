using UnityEngine;
using System.Collections;

public class PartAi : Part {
     public AISwarm[] AISwarms;
	 public override void Started(){
         base.Started();
         AISwarms = PartTransform.GetComponentsInChildren<AISwarm>();
         foreach (AISwarm Ai in AISwarms)
        {
            Ai.respawns = new AISpawnPoint[Spawner.SpawnedPrefabs.Count];
            for(int i =0;i<Ai.respawns.Length;i++)
            {
                Ai.respawns[i] = Spawner.SpawnedPrefabs[i].GameObj.GetComponent<AISpawnPoint>();
            }
            Ai.Init(Numb);
        }
	}

    
     public override void PlayerEnter()
     {
         base.PlayerEnter();
         foreach (AISwarm Ai in AISwarms)
         {
             Ai.Activate();
         }
         generator.MovePathTo(PartTransform);
     }
    
}
