using UnityEngine;
using System.Collections;

public class JuggerSpawnPoint : ObjectSpawnPoint {
public float respawnTime= 10.0f;

	private float respawnTimer = 0.0f;

 	protected GameObject spawnedObject;
	
	public string juggerName;
	// Use this for initialization
	void Awake() {
       
		respawnTimer = respawnTime;
	}
	// Update is called once per frame
	void Update () {
        if (spawnedObject == null&&NetworkController.IsMaster())
		{
			
			
			respawnTimer += Time.deltaTime;
			if (respawnTimer > respawnTime)
			{
				respawnTimer = 0.0f;
				SpawnJugger();
				
			
			}
		}
	}	
	void SpawnJugger(){
		spawnedObject = NetworkController.Instance.PawnSpawnRequest(juggerName, transform.position, transform.rotation, false, new int[0],true);
                 
	}
	
	void OnMasterClientSwitched(){
		spawnedObject = FindObjectOfType<RobotPawn>().gameObject;
	
	}
	
	
}
