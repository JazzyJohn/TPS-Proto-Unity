using UnityEngine;
using System.Collections;

public class AISpawnPoint : ObjectSpawnPoint {

	
	public void Spawned(Pawn newPawn){
        isAvalable = false;
		spawnedObject = newPawn.gameObject;
				}
}
