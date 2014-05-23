using UnityEngine;
using System.Collections;

public class AISpawnPoint : ObjectSpawnPoint {

	public int  team
	public void Spawned(Pawn newPawn){
		isAvalable = false;
		newPawn.team = team;
		spawnedObject = newPawn.gameObject;
			
	}
}
