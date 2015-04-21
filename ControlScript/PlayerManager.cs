using UnityEngine;
using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;

public class PlayerManager : MonoBehaviour {

	public String[] pawnName;

	public List<Pawn> cachedPawns = new List<Pawn>();

    public String[] avaibleBots;
	public List<Player> cachedPlayers = new List<Player>();


	public GameObject[] ghostsBots;

	private int curTeam =1;

	public int MaxTeam = 2;

	public Player LocalPlayer;

	public bool frendlyFire =false;

	public string version = "0.0.8";

	  public float updateDelay = 0.1f;

    public float updateTimer = 0.0f;

	public int maxPawnSend = 5;

	public float radius = 2.0f;


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
			if (s_Instance == null)	//by ssrazor
			{
				var go = new GameObject();
				go.name = "~PlayerManager";
				s_Instance = go.AddComponent<PlayerManager>();
			}

			return s_Instance;
        }
    }
    public void Awake(){
		var serverHolder = FindObjectOfType<ServerHolder>();
		if (serverHolder != null)
			version = serverHolder.version;
    }
    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit() {
        s_Instance = null;
    }

    // Add the rest of the code here...
    public virtual  Pawn SpawmPlayer(String newPalyerClass,int team,int[] stims) {
		Pawn localPlayer;
		Transform targetPos = GetSpamPosition (team);



        localPlayer = (Pawn)NetworkController.Instance.BeginPawnSpawnRequest(newPalyerClass, targetPos.position, targetPos.rotation, false, stims,false).GetComponent<Pawn>();
        localPlayer.AfterAwake();
		return localPlayer;
    }

    public Pawn SpawmBot(String newPalyerClass, Vector3 position,Quaternion rotation ,int[] stims) {
		Pawn localPlayer;


        localPlayer = NetworkController.Instance.BeginPawnSpawnRequest(newPalyerClass, position, rotation, false, stims, false).GetComponent<Pawn>();
		
		return localPlayer;
    }

	public virtual Transform GetSpamPosition(int team){

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
                Collider[] hitColliders = Physics.OverlapSphere(spamPoints[i].transform.position, radius);
				bool move = false;

				foreach(Collider col in hitColliders){
					if(col.transform.root.GetComponent<Pawn>()!=null){
						move= true;
					}
				}
				if(!move||(i==(spamPoints.Length-1)&&list.Count==0)){
					list.Add(spamPoints[i]);
				}
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
	public int playerCount(){
		return cachedPlayers.Count;
	}

    public void ClearAll()
    {
        foreach(Player player in cachedPlayers){
            Destroy(player.gameObject);
        }

    }
	public  void Update(){
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



