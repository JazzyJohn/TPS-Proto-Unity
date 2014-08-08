using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class AIDirector : MonoBehaviour {

	protected SelfSpawnPoint[] selfRespawns;

	public AISwarm[] swarms;

	public float directorTick= 0.5f;
	
	private float _directorTick = 0.0f;
	
	private bool start = false;
	
	

	public virtual void StartDirector(){
	
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
                _directorTick = 0;
				foreach(AISwarm swarm in swarms){
                    swarm.SwarmTick(directorTick);
				}
				
				foreach (SelfSpawnPoint go in selfRespawns) {
					if(go.IsAvalable()){
						go.SpawObject();
					}
				}
				
				
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
