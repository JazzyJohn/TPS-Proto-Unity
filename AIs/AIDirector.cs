using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
[System.Serializable]
public class AISwarm{
	
	public GameObject[] Bots;

	public AISpawnPoint[] respawns;
 
	public int[] enemyIndex;
	
	public List<Transform>  pointOfInterest;
	
	public int aiGroup;
	

	protected List<Transform> avaiblePoints = new List<Transform>();
	
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

	public void SwarmTick(){
		if(Bots.Length>0){
				for(int i =0;i<respawns.Length;i++) {
					AISpawnPoint go  = respawns[i];
					if(go.IsAvalable()){
						GameObject obj = PhotonNetwork.InstantiateSceneObject (Bots[(int)(UnityEngine.Random.value*Bots.Length)].name, go.transform.position, go.transform.rotation, 0,null) as GameObject;
					//	GameObject obj = PhotonNetwork.Instantiate (Bots[(int)(UnityEngine.Random.value*Bots.Length)].name, go.transform.position, go.transform.rotation, 0,null) as GameObject;
						go.Spawned(obj.GetComponent<Pawn>());
						go.GetComponent<AIBase>().Init(aiGroup,this,i);
					}
				}
			}
	}
	public void Init(int i ){
			
			aiGroup  = i;
			ReloadList ();
	}
	public bool IsEnemy(int enemyGroup){
		foreach(int enemy in enemyIndex){
			if(enemy ==enemyGroup){
				return true;
			}
		}
		return false;
	}
}


public class AIDirector : MonoBehaviour {

	private SelfSpawnPoint[] selfRespawns;

	public AISwarm[] swarms;

	public float directorTick= 0.5f;
	
	private float _directorTick = 0.0f;
	
	private bool start = false;
	
	

	public void StartDirector(){
	
		for(int i = 0;i<swarms.Length;i++){
			swarms[i].Init(i);
			
			
		}
		start = true;
		selfRespawns = FindObjectsOfType<SelfSpawnPoint> ();
    }
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		if(start){
			_directorTick+=Time.deltaTime;
			if(_directorTick>directorTick){
				
				foreach(AISwarm swarm in swarms){
					swarm.SwarmTick();
				}
				
				foreach (SelfSpawnPoint go in selfRespawns) {
					if(go.IsAvalable()){
						go.SpawObject();
					}
				}
				
				//Debug.Log("Work");
			}
		}
	
	}



	private static AIDirector s_Instance = null;
	
	public static AIDirector instance {
		get {
			if (s_Instance == null) {
				//Debug.Log ("FIND");
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (AIDirector)) as AIDirector;
			}		
			
			return s_Instance;
		}
	}

}
