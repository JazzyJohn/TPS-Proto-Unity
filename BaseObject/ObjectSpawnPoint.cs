using UnityEngine;
using System.Collections;

public class ObjectSpawnPoint : MonoBehaviour {

	public float respawnTime= 10.0f;

	private float respawnTimer = 0.0f;

	public bool isAvalable = true;

	protected GameObject spawnedObject;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!isAvalable && spawnedObject == null){
			respawnTimer+= Time.deltaTime;
			if(respawnTimer > respawnTime) {
				respawnTimer=0.0f;
				isAvalable= true;
			}
		}
	}
	
}