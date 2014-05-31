using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class AIDirector : MonoBehaviour {

	public GameObject[] Bots;

	private AISpawnPoint[] respawns;

	private SelfSpawnPoint[] selfRespawns;

	public List<Transform>  pointOfInterest;
	
	protected List<Transform> avaiblePoints = new List<Transform>();

	public float directorTick= 0.5f;

	public void StartDirector(){
		respawns = FindObjectsOfType<AISpawnPoint> ();
		selfRespawns = FindObjectsOfType<SelfSpawnPoint> ();
		ReloadList ();
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
			if(Bots.Length>0){
				foreach (AISpawnPoint go in respawns) {
					if(go.isAvalable){
						//GameObject obj = PhotonNetwork.InstantiateSceneObject (Bots[(int)(UnityEngine.Random.value*Bots.Length)].name, go.transform.position, go.transform.rotation, 0,null) as GameObject;
						GameObject obj = PhotonNetwork.Instantiate (Bots[(int)(UnityEngine.Random.value*Bots.Length)].name, go.transform.position, go.transform.rotation, 0,null) as GameObject;
						go.Spawned(obj.GetComponent<Pawn>());
					}
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

	void ReloadList(){
		if (avaiblePoints.Count == 0) {
			foreach(Transform point in  pointOfInterest){
				avaiblePoints.Add(point);
			}
		}

	}
	public Transform[] GetPointOfInterest(int count){
		Transform[] returnTransform =  new Transform[count];
	//	Debug.Log ("patrolPoint" +count);
		ReloadList ();
		for (int i=0; i<count; i++) {
			int randKey =(int)(UnityEngine.Random.value*avaiblePoints.Count);
			returnTransform[i] =avaiblePoints[randKey];
		//	Debug.Log (returnTransform[i]);
			avaiblePoints.RemoveAt(randKey);			
			ReloadList ();
		}
		return returnTransform;
	}

}
