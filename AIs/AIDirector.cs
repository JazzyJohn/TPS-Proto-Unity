using UnityEngine;
using System.Collections;

public class AIDirector : MonoBehaviour {

	public GameObject[] Bots;

	public void SpawnBot(){
		GameObject[] respawns = GameObject.FindGameObjectsWithTag("Respawn");
		foreach (GameObject go in respawns) {
				
			PhotonNetwork.Instantiate (Bots[(int)(UnityEngine.Random.value*Bots.Length)].name, go.transform.position, go.transform.rotation, 0);
		}

	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
