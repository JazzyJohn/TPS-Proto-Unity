using UnityEngine;
using System.Collections;

public class AISpawnPoint : ObjectSpawnPoint {

	public int  team;
	public void Spawned(Pawn newPawn){
		isAvalable = false;
		newPawn.SetTeam(team);
		spawnedObject = newPawn.gameObject;
			
	}
}
