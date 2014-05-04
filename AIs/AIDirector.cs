using UnityEngine;
using System.Collections;

public class AIDirector : MonoBehaviour {

	public GameObject[] Bots;

	private AISpawnPoint[] respawns;

	public float directorTick= 0.5f;

	public void StartDirector(){
		respawns = FindObjectsOfType<AISpawnPoint> ();
		StartCoroutine("Tick");

	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
	private IEnumerator Tick()
	{
		while (true)
		{
			
			foreach (AISpawnPoint go in respawns) {
				if(go.isAvalable){
					GameObject obj = PhotonNetwork.Instantiate (Bots[(int)(UnityEngine.Random.value*Bots.Length)].name, go.transform.position, go.transform.rotation, 0) as GameObject;
					go.Spawned(obj.GetComponent<Pawn>());
				}
			}
			yield return new WaitForSeconds(directorTick);
			//Debug.Log("Work");
		}
	}
}
