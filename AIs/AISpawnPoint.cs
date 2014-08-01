using UnityEngine;
using System.Collections;

public class AISpawnPoint : ObjectSpawnPoint {

	public int  team;
	public void Spawned(Pawn newPawn){
		
	
		newPawn.SetTeam(team);
		
	//	newPawn.AfterSpawnAction ();
	}
	 /// <summary>
    /// When Master switched we fix spawnedObject
    /// </summary>
    /// <param name="newPawn"></param>
	public void SpawnedSet(Pawn newPawn){
		spawnedObject=newPawn.gameObject;
	}
	
	
}
