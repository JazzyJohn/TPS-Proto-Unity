using UnityEngine;
using System.Collections;

using System;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Logging;
using System.Collections.Generic;
using Sfs2X.Protocol.Serialization;
using System.Reflection;

public enum RPCTransitTargetType
{
    OTHER,
    MASTER,
    ALL
}

public class NetworkController : MonoBehaviour {
	public static int MAX_VIEW_IDS = 1000;


    private static Dictionary<int, FoxView> foxViewList = new Dictionary<int, FoxView>();
    static int lastUsedViewSubId = 0;  // each player only needs to remember it's own (!) last used subId to speed up assignment
    static int lastUsedViewSubIdStatic = 0;  // per room, the master is able to instantiate GOs. the subId for this must be unique too
   

    public string serverName = "127.0.0.1";
    public int serverPort = 9933;
    public string zone = "BasicJugger";
    public bool debug = true;
    public bool pause = false;

    private static NetworkController instance;
    public static NetworkController Instance
    {
        get
        {
            return instance;
        }
    }

    public static SmartFox smartFox
    {
        get
        {

            return instance._smartFox;
        }
    }
    private SmartFox _smartFox;  // The reference to SFS client

	public static SmartFox GetClient() {
        return  SmartFoxConnection.Connection;
	}

    public ServerHolder serverHolder;
	
	public static FoxView GetView(int id){
		if(foxViewList.	ContainsKey(id)){
			return foxViewList[id];
		}
		return null;
	}
	
	
		// We start working from here
    void Awake()
    {
        if (instance != null)
        {
            return;

        }
        instance = this;
        Application.runInBackground = true; // Let the application be running whyle the window is not active.
        // In a webplayer (or editor in webplayer mode) we need to setup security policy negotiation with the server first
        if (Application.isWebPlayer || Application.isEditor)
        {
            if (!Security.PrefetchSocketPolicy(serverName, serverPort, 500))
            {
                Debug.LogError("Security Exception. Policy file load failed!");
            }
        }
        DefaultSFSDataSerializer.RunningAssembly = Assembly.GetExecutingAssembly(); 
        Connect();
    }
    public void Connect(){
        if (SmartFoxConnection.IsInitialized)
        {
            _smartFox = SmartFoxConnection.Connection;
        }
        else
        {
            _smartFox = new SmartFox(debug);
        }
     
        if (!_smartFox.IsConnected)
        {

            // Register callback delegate


               Debug.Log("startFoxCOnnection");
          
            _smartFox.AddEventListener(SFSEvent.CONNECTION, OnConnection);
            _smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
            _smartFox.AddEventListener(SFSEvent.LOGIN, OnLogin);
            _smartFox.AddEventListener(SFSEvent.UDP_INIT, OnUdpInit);
            _smartFox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
            _smartFox.AddLogListener(LogLevel.DEBUG, OnDebugMessage);
            _smartFox.Connect(serverName, serverPort);
        }
	}
    	
	// This is needed to handle server events in queued mode
	void FixedUpdate() {
        if (pause)
        {
            return;
        }
        while(lateEvents.Count>0){
            OnExtensionResponse(lateEvents.Dequeue());
        }
        _smartFox.ProcessEvents();
        
	}
    string UID = "";
    public void SetLogin(string username)
    {
        UID = username;
        if (_smartFox.IsConnected)
        {
            Login(username);
        }
    }
	public void Login(string username){
        _smartFox.Send(new LoginRequest(username, "", zone));

    }
	private void UnsubscribeDelegates() {
        _smartFox.RemoveAllEventListeners();
	}
    void OnApplicationQuit()
    {
        UnsubscribeDelegates();
        _smartFox.Disconnect();
    }
    //SMARTFOX LOGIC

    /************
    * Callbacks from the SFS API
     ************/

    public void OnConnection(BaseEvent evt)
    {
        bool success = (bool)evt.Params["success"];
        string error = (string)evt.Params["error"];

        Debug.Log("On Connection callback got: " + success + " (error : <" + error + ">)");

      
        if (success)
        {
            SmartFoxConnection.Connection = _smartFox;
        }
        if (UID!="")
        {
            Login(UID);
        }
       
    }

    public void OnConnectionLost(BaseEvent evt)
    {
        
        UnsubscribeDelegates();
        this.Connect();
    }

    public void OnDebugMessage(BaseEvent evt)
    {
        string message = (string)evt.Params["message"];
        Debug.Log("[SFS DEBUG] " + message);
    }

    public void OnLogin(BaseEvent evt)
    {
        bool success = true;
        if (evt.Params.Contains("success") && !(bool)evt.Params["success"])
        {
            // Login failed - lets display the error message sent to us
 
            Debug.Log("Login error: " + (string)evt.Params["errorMessage"]);
        }
        else
        {
            // Startup up UDP
            Debug.Log("Login ok");
            smartFox.InitUDP(serverName, serverPort);
        }
    }

    public void OnUdpInit(BaseEvent evt)
    {
        if (evt.Params.Contains("success") && !(bool)evt.Params["success"])
        {
      
            Debug.Log("UDP error: " + (string)evt.Params["errorMessage"]);
        }
        else
        {
            Debug.Log("UDP ok");

            // On to the lobby
            serverHolder = GetComponent<ServerHolder>();
            serverHolder.Connect();
        }
    }

    private Queue<BaseEvent> lateEvents = new Queue<BaseEvent>();
    private void OnExtensionResponse(BaseEvent evt)
    {
        if (pause)
        {
            lateEvents.Enqueue(evt);
            return;
        }
        try
        {
            string cmd = (string)evt.Params["cmd"];
            ISFSObject dt = (SFSObject)evt.Params["params"];
            Debug.Log("CMD" + cmd);
            switch (cmd)
            {
                case "getTime":
                    long time = dt.GetLong("t");
		            TimeManager.Instance.Synchronize(Convert.ToDouble(time));
                    break;
            
                case "playersSpawm":
                  
                     foreach(nstuff.juggerfall.extension.player.Player  player in dt.GetSFSArray("owners")){
                         if (player.userId != _smartFox.MySelf.Id)
                         {
                             if (PlayerView.allPlayer.ContainsKey(player.userId))
                             {
                                 PlayerView.allPlayer[player.userId].NetUpdate(player);
                             }
                             else
                             {
                                 SpawnPlayer(player);
                             }
                         }
                     }
                    break;
                case "updatePlayerInfo":
                    {
                        nstuff.juggerfall.extension.player.Player player = (nstuff.juggerfall.extension.player.Player)dt.GetClass("player");
                        if (PlayerView.allPlayer.ContainsKey(player.userId))
                        {
                            PlayerView.allPlayer[player.userId].NetUpdate(player);
                        }
                        else
                        {
                            SpawnPlayer(player);
                        }


                    }

                    break;
				case "pawnSpawn":
					{
						HandlePawnSpawn(dt);
					
					}
					break;
				case "pawnChangeShootAnimStateRequest":
					HandlePawnChangeShootAnimState(dt);
					break;
				case "pawnStartKick":
					HandlePawnKick(dt);
					break;
					
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception handling response: " + e.Message + " >>> " + e.StackTrace);
        }

    }
 
    public Player GetPlayer(int uid){
		 if (!PlayerView.allPlayer.ContainsKey(uid))
		{
			SpawnPlayer(uid);
		}
			
		return PlayerView.allPlayer[uid].GetComponent<Player>();
	}


    private GameObject InstantiateNetPrefab(string prefab, Vector3 vector3, Quaternion quaternion, ISFSObject data,bool AI)
    {
        GameObject newObject = _SpawnPrefab(prefab,vector3,quaternion);
        if (newObject == null)
        {
            return null;
        }
        FoxView view =  newObject.GetComponent<FoxView>();
		if(!AI){
			view.viewID = AllocateViewID(_smartFox.MySelf.Id);
		}else{
			view.viewID = AllocateViewID(0);
		}
		
        return newObject;
    }
	 private GameObject RemoteInstantiateNetPrefab(string prefab, Vector3 vector3, Quaternion quaternion, int viewId)
    {
        GameObject newObject = _SpawnPrefab(prefab,vector3,quaternion);
        if (newObject == null)
        {
            return null;
        }
        FoxView view =  newObject.GetComponent<FoxView>();
		
		view.viewID =viewId;
		
		
        return newObject;
    }
	
    private GameObject _SpawnPrefab(string prefabName, Vector3 vector3, Quaternion quaternion)
    {
        GameObject resourceGameObject = null;
        if (PhotonResourceWrapper.allobject.ContainsKey(prefabName))
        {
            resourceGameObject = PhotonResourceWrapper.allobject[prefabName];
        }
        else
        {

            
                resourceGameObject = (GameObject)Resources.Load(prefabName, typeof(GameObject));
            
          

        }
        if (Application.isEditor)
        {
            GameObject tempprefab = (GameObject)Resources.Load(prefabName, typeof(GameObject));
            if (tempprefab != null)
            {
                resourceGameObject = tempprefab;

            }
        }
        if (resourceGameObject==null)
        {
            Debug.LogError("PhotonNetwork error: Could not Instantiate the prefab [" + prefabName + "]. Please verify you have this gameobject in a Resources folder.");
            return null;
        }
       return  Instantiate(resourceGameObject, vector3, quaternion) as GameObject;
    }

    // use 0 for scene-targetPhotonView-ids
    // returns viewID (combined owner and sub id)
    private static int AllocateViewID(int ownerId)
    {
        if (ownerId == 0)
        {
            // we look up a fresh subId for the owner "room" (mind the "sub" in subId)
            int newSubId = lastUsedViewSubIdStatic;
            int newViewId;
            int ownerIdOffset = ownerId * MAX_VIEW_IDS;
            for (int i = 1; i < MAX_VIEW_IDS; i++)
            {
                newSubId = (newSubId + 1) % MAX_VIEW_IDS;
                if (newSubId == 0)
                {
                    continue;   // avoid using subID 0
                }

                newViewId = newSubId + ownerIdOffset;
                if (!foxViewList.ContainsKey(newViewId))
                {
                    lastUsedViewSubIdStatic = newSubId;
                    return newViewId;
                }
            }

            // this is the error case: we didn't find any (!) free subId for this user
            throw new Exception(string.Format("AllocateViewID() failed. Room (user {0}) is out of subIds, as all room viewIDs are used.", ownerId));
        }
        else
        {
            // we look up a fresh SUBid for the owner
            int newSubId = lastUsedViewSubId;
            int newViewId;
            int ownerIdOffset = ownerId * MAX_VIEW_IDS;
            for (int i = 1; i < MAX_VIEW_IDS; i++)
            {
                newSubId = (newSubId + 1) % MAX_VIEW_IDS;
                if (newSubId == 0)
                {
                    continue;   // avoid using subID 0
                }

                newViewId = newSubId + ownerIdOffset;
                if (!foxViewList.ContainsKey(newViewId) )
                {
                    lastUsedViewSubId = newSubId;
                    return newViewId;
                }
            }

            throw new Exception(string.Format("AllocateViewID() failed. User {0} is out of subIds, as all viewIDs are used.", ownerId));
        }
    }
	
	  /// <summary>
    /// Spawn current player object
    /// </summary>	
    public void SpawnPlayer(nstuff.juggerfall.extension.player.Player player)
    {

        Debug.Log( "CREATE PLAYER " + player.userId );
        GameObject newObject = _SpawnPrefab("Player", Vector3.zero, Quaternion.identity);
        PlayerView view = newObject.GetComponent<PlayerView>();
        view.SetId(player.userId);
        view.NetUpdate(player);

    }
	 public void SpawnPlayer(int uid)
    {

        Debug.Log( "CREATE PLAYER " + player.userId );
        GameObject newObject = _SpawnPrefab("Player", Vector3.zero, Quaternion.identity);
        PlayerView view = newObject.GetComponent<PlayerView>();
        view.SetId(uid);
        
    }
	

    //REQUEST SECTION

    /// <summary>
    /// Request the current server time. Used for time synchronization
    /// </summary>	
    public void TimeSyncRequest()
    {
        Sfs2X.Entities.Room room = _smartFox.LastJoinedRoom;
        ExtensionRequest request = new ExtensionRequest("getTime", new SFSObject(), room);
        smartFox.Send(request);
    }
  
    /// <summary>
    /// setNameUID request to server
    /// </summary>	
    public void SetNameUIDRequest(string uid, string name)
    {
        ISFSObject data = new SFSObject();
        data.PutUtfString("uid", uid);
        data.PutUtfString("name", name);
        ExtensionRequest request = new ExtensionRequest("setNameUID", data, serverHolder.gameRoom);
        smartFox.Send(request);


    }
       /// <summary>
    /// setTeam request to server
    /// </summary>	
    public void SetTeamRequest(int team)
    {
        ISFSObject data = new SFSObject();

        data.PutInt("team", team);
        ExtensionRequest request = new ExtensionRequest("setTeam", data, serverHolder.gameRoom);
        smartFox.Send(request);


    }

    /// <summary>
    /// pawnSpawn request to server
    /// </summary>	
    public GameObject PawnSpawnRequest(string prefab, Vector3 vector3,Quaternion quaternion,bool isAI,int[] stims)
    {
        ISFSObject data = new SFSObject();

        if (!isAI)
        {
            data.PutBool("AI", isAI);
        }
		data.PutIntArray("stims", stims);
        GameObject go = InstantiateNetPrefab(prefab, vector3, quaternion, data);
		nstuff.juggerfall.extension.pawn.Pawn pawn  =go.GetComponent<Pawn>().GetSerilizedData();
		pawn.type = prefab;
        data.PutClass("pawn",pawn);
        ExtensionRequest request = new ExtensionRequest("pawnSpawn", data, serverHolder.gameRoom,isAI);
        smartFox.Send(request);
        return go;


    }
	
    /// <summary>
    /// pawnChangeShootAnimStateRequest request to server
    /// </summary>	
    public void PawnChangeShootAnimStateRequest(int id,string animName,bool state)
    {
         ISFSObject data = new SFSObject();

        data.PutInt("id", id);
		data.PutBool("state", state);
		data.PutString("animName", animName);
        ExtensionRequest request = new ExtensionRequest("pawnChangeShootAnimStateRequest", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }
	/// <summary>
    /// pawnStartKick request to server
    /// </summary>	
    public void PawnKickRequest(int id,int kick,bool state)
    {
         ISFSObject data = new SFSObject();

        data.PutInt("id", id);
		data.PutBool("state", state);
		data.PutInt("kick", kick);
        ExtensionRequest request = new ExtensionRequest("pawnStartKick", data, serverHolder.gameRoom);
        smartFox.Send(request);

    /// <summary>
    /// pawnActiveState request to server
    /// </summary>	
    public void PawnActiveStateRequest(int id,bool state)
    {
         ISFSObject data = new SFSObject();

        data.PutInt("id", id);
		data.PutBool("active", state);
	    ExtensionRequest request = new ExtensionRequest("pawnActiveState", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }
    /// <summary>
    /// pawnUpdate request to server
    /// </summary>	
    public void PawnUpdateRequest(nstuff.juggerfall.extension.pawn.Pawn  pawn)
    {
        ISFSObject data = new SFSObject();
     
     	data.PutClass("pawn", pawn);
	    ExtensionRequest request = new ExtensionRequest("pawnUpdate", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }
	/// <summary>
    /// pawnTaunt request to server
    /// </summary>	
    public void PawnTauntRequest(int id,string animName)
    {
        ISFSObject data = new SFSObject();
     
     	data.PutInt("id", id);
		data.PutString("animName", animName);
	    ExtensionRequest request = new ExtensionRequest("pawnTaunt", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }
	/// <summary>
    /// pawnKnockOut request to server
    /// </summary>	
    public void PawnKnockOutRequest(int id)
    {
        ISFSObject data = new SFSObject();
     
     	data.PutInt("id", id);
	    ExtensionRequest request = new ExtensionRequest("pawnKnockOut", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }
	/// <summary>
    /// deleteView request to server
    /// </summary>	
    public void DeleteViewRequest(int id)
    {
        ISFSObject data = new SFSObject();
     
     	data.PutInt("id", id);
	    ExtensionRequest request = new ExtensionRequest("deleteView", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }
	/// <summary>
    /// weaponSpawn request to server
    /// </summary>	
	
	public GameObject WeaponSpawn(string prefab, Vector3 vector3,Quaternion quaternion,bool isAI,int pawnId)
    {
        ISFSObject data = new SFSObject();

       
        GameObject go = InstantiateNetPrefab(prefab, vector3, quaternion, data,isAI);
		nstuff.juggerfall.extension.weapon.Weapon  weapon  = go.GetComponent<BaseWeapon>().GetSerilizedData()
		weapon.type = prefab;
        data.PutClass("weapon",weapon);
		data.PutInt("pawnId",pawnId);
        ExtensionRequest request = new ExtensionRequest("weaponSpawn", data, serverHolder.gameRoom);
        smartFox.Send(request);
        return go;


    }
	/// <summary>
    /// weaponSpawn request to server
    /// </summary>	
	
	public void WeaponSpawn(ISFSObject data )
    {
        ExtensionRequest request = new ExtensionRequest("weaponSpawn", data, serverHolder.gameRoom);
        smartFox.Send(request);


    }
		
		
		
		
		
		
		
		
		
		
	//Handler SECTION
	/// <summary>
    /// handle pawnSpawn  from Server
    /// </summary>	
	
	public void HandlePawnSpawn(ISFSObject dt )
    {
		nstuff.juggerfall.extension.pawn.Pawn sirPawn =  (nstuff.juggerfall.extension.pawn.Pawn)dt.GetClass("pawn");
		GameObject go =RemoteInstantiateNetPrefab(sirPawn.type, Vector3.zero,Quaternion.indentity,sirPawn.id);
		
		Player player  = GetPlayer(dt.GetInt("ownerId"));
		Pawn pawn  = go.GetComponent<Pawn>();
		player.AfterSpawnSetting(pawn,sirPawn.team,dt.GetIntArray("stims"));
		pawn.NetUpdate(sirPawn);
		 
		 
	}
	/// <summary>
    /// handle pawnChangeShootAnimState(  from Server
    /// </summary>	
	
	public void HandlePawnChangeShootAnimState(ISFSObject dt )
    {
			
		Pawn pawn = GetView(dt.GetInt("id")).GetComponent<Pawn>();
		if(dt.GetBool("state")){
			pawn.StartShootAnimation(dt.GeUtfString("animName"));
		}else{
			pawn.StopShootAnimation(dt.GeUtfString("animName"));
		}
		 
	}
	/// <summary>
    /// handle pawnStartKick  from Server
    /// </summary>	
	
	public void HandlePawnKick(ISFSObject dt )
    {
			
		Pawn pawn = GetView(dt.GetInt("id")).GetComponent<Pawn>();
		if(dt.GetBool("state")){
			pawn.Kick(dt.GetInt("kick"));
		}else{
			pawn.StopKick();
		}
		 
	}
}

