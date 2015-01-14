using UnityEngine;
using System.Collections;

public class JuggerSpawnPoint : MonoBehaviour {
    public float respawnTime= 10.0f;

	private float respawnTimer = 0.0f;

 	protected GameObject spawnedObject;
	
	public string juggerName;
	// Use this for initialization
	void Awake() {
       
		respawnTimer = 0;
	}
	// Update is called once per frame
	void Update () {
        if (GameRule.instance.start&& spawnedObject == null&&NetworkController.IsMaster())
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
        spawnedObject = NetworkController.Instance.BeginPawnSpawnRequest(juggerName, transform.position, transform.rotation, false, new int[0], true);
        NetworkController.Instance.EndPawnSpawnRequest();      
	}
	
	void OnMasterClientSwitched(){
		spawnedObject = FindObjectOfType<RobotPawn>().gameObject;
	
	}
	
	
}
