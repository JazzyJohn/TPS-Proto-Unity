using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;

public class AIDirector : MonoBehaviour {

	public GameObject[] Bots;

	private AISpawnPoint[] respawns;

	private SelfSpawnPoint[] selfRespawns;
	
	public float directorTick= 0.5f;

	public void StartDirector(){
		respawns = FindObjectsOfType<AISpawnPoint> ();
		selfRespawns = FindObjectsOfType<SelfSpawnPoint> ();
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
					GameObject obj = PhotonNetwork.InstantiateSceneObject (Bots[(int)(UnityEngine.Random.value*Bots.Length)].name, go.transform.position, go.transform.rotation, 0,null) as GameObject;
					go.Spawned(obj.GetComponent<Pawn>());
				}
			}
			foreach (SelfSpawnPoint go in selfRespawns) {
				if(go.isAvalable){
					go.SpawObject();
				}
			}
			yield return new WaitForSeconds(directorTick);
			//Debug.Log("Work");
		}
	}
}
