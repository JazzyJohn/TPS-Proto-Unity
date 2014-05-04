using UnityEngine;
using System.Collections;

public class AISpawnPoint : MonoBehaviour {

	public float respawnTime= 10.0f;

	private float respawnTimer = 0.0f;

	public bool isAvalable = true;

	private Pawn spawnedPawn;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!isAvalable && spawnedPawn == null){
			respawnTimer+= Time.deltaTime;
			if(respawnTimer > respawnTime) {
				respawnTimer=0.0f;
				isAvalable= true;
			}
		}
	}
	public void Spawned(Pawn newPawn){
		isAvalable = false;
		spawnedPawn = newPawn;
			
	}
}
