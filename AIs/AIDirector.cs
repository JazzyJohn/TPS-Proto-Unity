using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using nstuff.juggerfall.extension.models;



public class AIDirector : MonoBehaviour {

	protected SelfSpawnPoint[] selfRespawns;

	public AISwarm[] swarms;

    public int[] chain;

	public float directorTick= 0.5f;
	
	private float _directorTick = 0.0f;
	
	private bool start = false;
	
	
	
	protected void Awake(){
		if (swarms.Length == 0)
        {
            swarms = FindObjectsOfType<AISwarm>();
          
        }
        for (int i = 0; i < swarms.Length; i++)
        {
            swarms[i].Init(i);
        }
	}

	public virtual void StartDirector(){
        if (swarms.Length == 0)
        {
            swarms = FindObjectsOfType<AISwarm>();
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


    public void RemoteStateChange(int id, bool state)
    {
        swarms[id].ChangeState(state);
    }

    public void SendData(ISFSObject data)
    {
        ISFSArray swarmsSend = new SFSArray();
        List<int> active = new List<int>();
       foreach(AISwarm swarm in swarms){

           ISFSObject swarmSend = new SFSObject();
            swarm.SendData(swarmSend);
            swarmsSend.AddSFSObject(swarmSend);
            if (swarm.isActive)
            {
                active.Add(swarm.aiGroup);
            }
       }
       data.PutSFSArray("swarms",swarmsSend);
       data.PutIntArray("chain", chain);
       data.PutIntArray("active", active.ToArray());
    }

    public void ReadData(ISFSObject data)
    { 
        ISFSArray swarmsGet  =data.GetSFSArray("swarms");
        for (int i = 0; i < swarmsGet.Size();i++ )
        {
            swarms[i].ReadData(swarmsGet.GetSFSObject(i));
        }


    }
}
