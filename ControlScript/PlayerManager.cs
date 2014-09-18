using UnityEngine;
using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;

public class PlayerManager : MonoBehaviour {

	public String[] pawnName;

	public List<Pawn> cachedPawns;
	
	public List<Player> cachedPlayers;

	public RobotPawn[] avaibleBots;

	public GameObject[] ghostsBots;

	private int curTeam =1;

	public int MaxTeam = 2;
	
	public Player LocalPlayer;

	public bool frendlyFire =false;

	public string version = "0.0.8";
	
	  public float updateDelay = 0.1f;

    public float updateTimer = 0.0f;
	
	public int maxPawnSend = 5;

	
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
 
            
 
            return s_Instance;
        }
    }
    public void Awake(){
        version = FindObjectOfType<ServerHolder>().version;
    }
    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit() {
        s_Instance = null;
    }
 
    // Add the rest of the code here...
    public Pawn SpawmPlayer(String newPalyerClass,int team,int[] stims) {
		Pawn localPlayer;
		Transform targetPos = GetSpamPosition (team);


        localPlayer = (Pawn)NetworkController.Instance.PawnSpawnRequest(newPalyerClass, targetPos.position, targetPos.rotation, false, stims,false).GetComponent<Pawn>();

		return localPlayer;
    }
	

    public Pawn SpawmBot(Pawn newPalyerClass, Vector3 position,Quaternion rotation ,int[] stims) {
		Pawn localPlayer;
	

		localPlayer =NetworkController.Instance.PawnSpawnRequest(newPalyerClass.name,position,rotation, false, stims,false).GetComponent<Pawn>();
		
		return localPlayer;
    }

	public Transform GetSpamPosition(int team){

        List<DispenserSpawn> listDist = new List<DispenserSpawn>();
        DispenserSpawn[] disspamPoints = FindObjectsOfType<DispenserSpawn>();
        for (int i = 0; i < disspamPoints.Length; i++)
        {
            if (disspamPoints[i].team == 0 || team == disspamPoints[i].team)
            {
                listDist.Add(disspamPoints[i]);
            }
        }
        if (listDist.Count > 0)
        {
            return listDist[(int)(UnityEngine.Random.value * listDist.Count)].spawnTransform;
        }
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

	public void addPawn(Pawn target){
		cachedPawns.Add (target);
	}
	public void FixedUpdate(){
        cachedPawns.RemoveAll (delegate(Pawn v) {
			return v==null;
		});
		cachedPlayers.RemoveAll (delegate(Player v) {
			return v==null;
		});
	}
	public List<Pawn> FindAllPawn(){
		return cachedPawns;
	}
	public void addPlayer(Player target){
		if(target.playerView.isMine){
			LocalPlayer= target;
		}
		cachedPlayers.Add (target);
	}
	public List<Player> FindAllPlayer(){
		return cachedPlayers;

	}


    public void ClearAll()
    {
        foreach(Player player in cachedPlayers){
            Destroy(player.gameObject);
        }
      
    }
	public void Update(){
		updateTimer += Time.deltaTime;
        if (updateTimer > updateDelay)
        {
            updateTimer = 0.0f;
            PawnUpdate();
        }
	}
	
	 public void PawnUpdate(){
		
		ISFSObject data = new SFSObject();
		ISFSArray pawnsArr = new SFSArray();
		
		foreach(Pawn pawn in cachedPawns){
			if(pawn.NeedUpdate()){
				pawnsArr.AddClass(pawn.GetSerilizedData());
			
			}
			
			if(pawnsArr.Size()>=maxPawnSend){
				data.PutSFSArray("pawns",pawnsArr);
				NetworkController.Instance.PawnUpdateRequest(data);
				data = new SFSObject();
				pawnsArr = new SFSArray();
			
			}
		}
		if(pawnsArr.Size()==0){
			return;
		}
		data.PutSFSArray("pawns",pawnsArr);
		NetworkController.Instance.PawnUpdateRequest(data);
	}
}
	


