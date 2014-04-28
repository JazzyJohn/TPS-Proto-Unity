using UnityEngine;
using System;
using System.Collections.Generic;


public class PlayerManager : MonoBehaviour {

	public Pawn[] avaiblePawn;

	public Pawn[] avaibleBots;

	public GameObject[] ghostsBots;

	private int curTeam =1;

	public int MaxTeam = 2;

	public bool frendlyFire =false;

	public string version = "0.0.8";
	
	public InventoryManager.AmmoBag[] AllTypeInGame;
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static PlayerManager s_Instance = null;
 
    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static PlayerManager instance {
        get {
            if (s_Instance == null) {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance =  FindObjectOfType(typeof (PlayerManager)) as PlayerManager;
            }
 
            // If it is still null, create a new instance
            if (s_Instance == null) {
                GameObject obj = new GameObject("PlayerManager");
                s_Instance = obj.AddComponent(typeof (PlayerManager)) as PlayerManager;
               
            }
 
            return s_Instance;
        }
    }
 
    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit() {
        s_Instance = null;
    }
 
    // Add the rest of the code here...
    public Pawn SpawmPlayer(Pawn newPalyerClass,int team) {
		Pawn localPlayer;
		Transform targetPos = GetSpamPosition (team);


		localPlayer =(Pawn) PhotonNetwork.Instantiate (newPalyerClass.name, targetPos.position, targetPos.rotation, 0).GetComponent<Pawn>();

		return localPlayer;
    }
	

    public Pawn SpawmPlayer(Pawn newPalyerClass, Vector3 position,Quaternion rotation ) {
		Pawn localPlayer;
	

		localPlayer =(Pawn) PhotonNetwork.Instantiate (newPalyerClass.name,position,rotation,0).GetComponent<Pawn>();
		
		return localPlayer;
    }

	public Transform GetSpamPosition(int team){
		List<SpawnPoint> list  = new List<SpawnPoint>();
		SpawnPoint[] spamPoints = FindObjectsOfType <SpawnPoint>();
		for(int i=0; i<spamPoints.Length;i++){
			if(spamPoints[i].team==0||team==spamPoints[i].team)
			{
				list.Add(spamPoints[i]);
			}
		}
//		Debug.Log ( list.Count);
		return list[(int)(UnityEngine.Random.value*list.Count)].transform;
	}
	public int NextTeam(){
		curTeam++;
		if(curTeam>MaxTeam){
			curTeam=1;
		}
		return curTeam;
	}
	public Pawn[] FindAllPawn(){
		return GameObject.FindObjectsOfType<Pawn>();
	}
	public Player[] FindAllPlayer(){
		return GameObject.FindObjectsOfType<Player>();

	}
 
 }
	


