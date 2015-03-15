using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
using nstuff.juggerfall.extension.models;
using Sfs2X.Entities.Variables;
using System.Threading;

public enum RPCTransitTargetType
{
    OTHER,
    MASTER,
    ALL
}

public class NetworkController : MonoBehaviour
{
    public static int MAX_VIEW_IDS = 1000;


    private static Dictionary<int, FoxView> foxViewList = new Dictionary<int, FoxView>();
    static int lastUsedViewSubId = 0;  // each player only needs to remember it's own (!) last used subId to speed up assignment
    static int lastUsedViewSubIdStatic = 0;  // per room, the master is able to instantiate GOs. the subId for this must be unique too


    public string serverName = "127.0.0.1";
    public bool serverResolved = false;
    public string serverResolverName = "127.0.0.1";
    public int serverPort = 9933;
    public int udpPort = 9934;
    public string zone = "BasicJugger";
    public bool debug = true;
    public bool pause = false;
    public string PlayerPrefab = "Player";
    ConterIdleRequest conterIdleRequest;
    private static NetworkController instance;

    List<int> deleteIdLate = new List<int>();
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
            if (instance != null)
            {
                return instance._smartFox;
            }
            return null;
        }
    }
    private SmartFox _smartFox;  // The reference to SFS client

    public static SmartFox GetClient()
    {
        return SmartFoxConnection.Connection;
    }

    public ServerHolder serverHolder;

    public static FoxView GetView(int id)
    {
        if (foxViewList.ContainsKey(id))
        {
            return foxViewList[id];
        }
        return null;
    }

    public static void ClearView(int id)
    {
        foxViewList.Remove(id);
    }
    public void MasterViewUpdate()
    {
        foreach (FoxView view in foxViewList.Values)
        {

            if (view.IsOnMasterControll())
            {
                Debug.Log("MasterViewUpdate" + view);
                if (view.viewID == 0)
                {
                    NetworkController.RegisterSceneView(view);
                }
                view.SetMine(true);
                view.SendMessage("OnMasterClientSwitched", SendMessageOptions.DontRequireReceiver);
            }

        }
        Debug.Log("IMMASTER" + smartFox.MySelf.ContainsVariable("Master"));
    }

    public Player GetLocalPlayer()
    {
        foreach (PlayerView view in PlayerView.allPlayer.Values)
        {
            if (view.isMine)
            {
                return view.observed;
            }
        }
        return null;
    }

    public static bool IsMaster()
    {
        return smartFox.MySelf.ContainsVariable("Master") && smartFox.MySelf.GetVariable("Master").GetBoolValue();
    }

    public void SetNetworkLvl(int lvl)
    {
        List<UserVariable> userVars = new List<UserVariable>();
        userVars.Add(new SFSUserVariable("lvl", lvl));
        smartFox.Send(new SetUserVariablesRequest(userVars));
    }
    public User GetUser(int id)
    {
        return serverHolder.gameRoom.GetUserById(id);
    }
    public void OnVariablesUpdate(BaseEvent evt)
    {
        //   evt.Params["user"];
        /*   foreach (System.Object obj in evt.Params.Keys)
           {
               Debug.Log(obj +"  "+obj.GetType());
           }
      
           */
        ArrayList changedVars = (ArrayList)evt.Params["changedVars"];
        if (changedVars != null)
        {
            Debug.Log("OnVariablesUpdate: " + changedVars.ToArray());
        }



        User user = (User)evt.Params["user"];
        if (user == smartFox.MySelf)
        {
            if (changedVars.Contains("Master"))
            {
                Debug.Log(smartFox.MySelf.GetVariable("Master").GetBoolValue());
                if (smartFox.MySelf.GetVariable("Master").GetBoolValue())
                {
                    if (GameRule.instance != null)
                    {
                        GameRule.instance.StartGame();
                        MasterViewUpdate();
                    }
                }
            }

        }

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
    public void Connect()
    {
        serverResolved = true;
        if (_smartFox != null)
        {

            _smartFox.Disconnect();
        }
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
            _smartFox.ThreadSafeMode = true;
            _smartFox.AddEventListener(SFSEvent.CONNECTION, OnConnection);
            _smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
            _smartFox.AddEventListener(SFSEvent.LOGIN, OnLogin);
            _smartFox.AddEventListener(SFSEvent.UDP_INIT, OnUdpInit);
            _smartFox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);

            _smartFox.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnVariablesUpdate);
            _smartFox.AddEventListener(SFSEvent.USER_EXIT_ROOM, HandlePlayerLeaveEvent);

            _smartFox.AddLogListener(LogLevel.DEBUG, OnDebugMessage);
            /*  myThread = new Thread(new ThreadStart(this.SendLoop));
              myThread.Start();	*/
            _smartFox.Connect(serverName, serverPort);
        }
    }

    // This is needed to handle server events in queued mode
    void Update()
    {
        if (!serverResolved)
        {
            return;
        }
        if (serverResolved && smartFox == null)
        {
            Connect();
        }
        if (pause)
        {
            return;
        }
        while (lateEvents.Count > 0)
        {
            OnExtensionResponse(lateEvents.Dequeue());
        }
        if (_smartFox != null)
        {
            _smartFox.ProcessEvents();
        }
        FoxView.SendShoot();
    }

    private string UID = "";
    private Thread pyThread;
    private TcpClient client;
    private NetworkStream stream;
    private StreamReader reader;
    private StreamWriter writer;
    public void SetLogin(string username)
    {
        UID = username;
        /* Thread  pyThread = new Thread(new ThreadStart(this.ServerResolvedLoop));
      
         pyThread.Start();		*/
        if (_smartFox.IsConnected)
        {
            Login(username);
        }

    }

    public void ServerResolvedLoop()
    {
        Debug.Log("Starting CLIENT");
        client = new TcpClient(serverResolverName, 9991);
        Debug.Log("Started CLIENT" + client.Connected);
        stream = client.GetStream();

        writer = new StreamWriter(stream);
        reader = new StreamReader(stream);
        try
        {


            while (true)
            {
                writer.WriteLine(UID);

                writer.Flush();

                //Debug.Log("BUFER SIZEclient.ReceiveBufferSize);
                string serverName = reader.ReadLine();
                if (!serverResolved)
                {
                    Debug.Log(serverName);
                    serverResolved = true;
                    this.serverName = serverName;
                }
                Thread.Sleep(1000);

            }
        }
        catch (Exception e)
        {
            Debug.LogError("socket Erorro");
        }
    }
    public void Login(string username)
    {
        ISFSObject data = new SFSObject();
        data.PutUtfString("playerName", GlobalPlayer.instance.PlayerName);
        _smartFox.Send(new LoginRequest(username, "", zone, data));

    }
    private void UnsubscribeDelegates()
    {
        _smartFox.RemoveAllEventListeners();
    }
    public void BackToMenu()
    {

        Screen.lockCursor = false;
        try
        {
            LeaveRoomReuqest();
        }
        catch (Exception e)
        {

            Debug.LogError("Exception handling response: " + e.Message + " >>> " + e.StackTrace);
        }

        Application.LoadLevel(0);
    }
    void OnDestroy()
    {
        if (client != null)
        {
            client.Close();
        }
        if (stream != null)
        {
            stream.Close();
        }
        if (pyThread != null)
        {
            pyThread.Abort();
        }
        _smartFox.Disconnect();
        UnsubscribeDelegates();
        /*if (myThread != null)
        {
            myThread.Abort();
        }*/
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
            Debug.Log(UID);
            if (UID != "")
            {
                Login(UID);
            }

        }
        else
        {
            UnsubscribeDelegates();
            this.Connect();
        }

    }

    public void OnConnectionLost(BaseEvent evt)
    {

        Debug.Log("On Connection lost");

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

        if (evt.Params.Contains("success") && !(bool)evt.Params["success"])
        {
            // Login failed - lets display the error message sent to us

            Debug.Log("Login error: " + (string)evt.Params["errorMessage"]);
        }
        else
        {
            // Startup up UDP
            Debug.Log("Login ok");
            conterIdleRequest = FindObjectOfType<ConterIdleRequest>();
            if (conterIdleRequest != null)
            {
                conterIdleRequest.enabled = true;
            }
            List<UserVariable> userVars = new List<UserVariable>();

            smartFox.InitUDP(serverName, serverPort);
        }
    }

    public void OnUdpInit(BaseEvent evt)
    {
        if (evt.Params.Contains("success") && !(bool)evt.Params["success"])
        {
            smartFox.InitUDP(serverName, serverPort);
            Debug.Log("UDP error: " + (string)evt.Params["errorMessage"]);
        }
        else
        {
            Debug.Log("UDP ok");
            GlobalPlayer.instance.loadingStage++;
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
            switch (cmd)
            {
                case "getTime":
                    if (TimeManager.Instance != null)
                    {
                        TimeManager.Instance.Synchronize(dt.GetLong("t"));
                    }
                    break;
                case "ConterIdleRequest":
                    conterIdleRequest.Recived();
                    break;
                case "playersSpawm":

                    foreach (PlayerModel player in dt.GetSFSArray("owners"))
                    {
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


                    //TODO Think maybe faster would be to sort it?
                    foreach (SFSObject dtIt in dt.GetSFSArray("views"))
                    {
                        //Yes try catch is not best solution we mustn't rely on it. but this is our insurance that player can play anyhow further 
                        try
                        {
                            if (dtIt.ContainsKey("pawn"))
                            {
                                HandlePawnSpawn(dtIt);
                            }
                            else if (dtIt.ContainsKey("model"))
                            {
                                HandleSimplePrefabSpawn(dtIt);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Exception handling playersSpawm in views section: " + e.Message + " >>> " + e.StackTrace);

                        }

                    }
                    foreach (SFSObject dtIt in dt.GetSFSArray("views"))
                    {
                        try
                        {
                            if (dtIt.ContainsKey("weapon"))
                            {
                                HandleWeaponSpawn(dtIt);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Exception handling playersSpawm in views section: " + e.Message + " >>> " + e.StackTrace);

                        }

                    }

                    //Delete scene view that was deleted before player join
                    foreach (int id in dt.GetIntArray("deleteSceneView"))
                    {
                        try
                        {
                            FoxView view = GetView(id);
                            //	Debug.Log("ALREDY DESTROY ID" + id);
                            if (view != null)
                            {
                                DestroyableNetworkObject obj = view.GetComponent<DestroyableNetworkObject>();

                                if (obj != null)
                                {
                                    //Debug.Log("ALREDY DESTROY" + obj);
                                    obj.KillMe();

                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Exception handling playersSpawm in delete views section: " + e.Message + " >>> " + e.StackTrace);

                        }
                    }
                    if (dt.ContainsKey("Master") && dt.GetBool("Master"))
                    {
                        HandleMasterStart(dt);
                        SendMapData();
                    }
                    if (dt.ContainsKey("swarms"))
                    {
                        ReadMapData(dt);
                    }
                    GameRule.instance.ReadData(dt);

                    break;
                case "updatePlayerInfo":
                    {
                        PlayerModel player = (PlayerModel)dt.GetClass("player");
                        Debug.Log("updatePlayerInfo team" + player.team);
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
                case "pawnChangeShootAnimState":
                    HandlePawnChangeShootAnimState(dt);
                    break;
                case "pawnStartKick":
                    HandlePawnKick(dt);
                    break;
                case "pawnActiveState":
                    HandlePawnActiveState(dt);
                    break;
                case "pawnUpdate":
                    HandlePawnUpdate(dt);
                    break;
                case "pawnTaunt":
                    HandlePawnTaunt(dt);
                    break;
                case "pawnKnockOut":
                    HandlePawnKnockOut(dt);
                    break;
                case "deleteView":
                    HandleDeleteView(dt);
                    break;
                case "weaponSpawn":
                    HandleWeaponSpawn(dt);
                    break;
                case "weaponShoot":
                    HanldeWeaponShoot(dt);
                    break;
                case "pawnSkillCastEffect":
                    HandlePawnSkillCastEffect(dt);
                    break;
                case "pawnSkillActivate":
                    HandleSkillActivate(dt);
                    break;
                case "pawnDetonate":
                    HandlePawnDetonate(dt);
                    break;

                case "invokeProjectileCall":
                    HandleInvokeProjectileCall(dt);
                    break;
                case "simplePrefabSpawn":
                    HandleSimplePrefabSpawn(dt);
                    break;
                case "gameStart":
                    HandleGameStart();
                    break;
                case "gameUpdate":
                    HandleGameUpdate(dt);
                    break;
                case "nextMap":
                    HandleNextMap(dt);
                    break;
                case "pawnDiedByKill":
                    HandlePawnDiedByKill(dt);
                    break;
                case "newMaster":
                    HandleMasterStart(dt);
                    break;
                case "playerLeave":
                    HandlePlayerLeave(dt);
                    break;
                case "pawnInPilotChange":
                    HandlePawnInPilotChange(dt);
                    break;
                case "updateSimpleDestroyableObject":
                    HandleUpdateSimpleDestroyableObject(dt);
                    break;
                case "remoteDamageOnPawn":
                    HandleRemoteDamageOnPawn(dt);
                    break;
                case "enterRobot":
                    HandleEnterRobot(dt);
                    break;
                case "enterRobotSuccess":
                    HandleEnterRobotSuccess(dt);
                    break;
                case "customAnimStart":
                    HandleCustomAnimStar(dt);
                    break;
                case "AISwarmUpdate":
                    HandleAISwarmUpdate(dt);
                    break;
                case "AINextWave":
                    HandleAINextWave(dt);
                    break;
                case "AISpawnBot":
                    HandleAISpawnBot(dt);
                    break;
                case "changeWeaponShootState":
                    HandleChangeWeaponShootState(dt);
                    break;
                case "changeWeaponState":
                    HandleChangeWeaponState(dt);
                    break;
                case "gamePointData":
                    HandleGamePointData(dt);
                    break;
                case "sendMark":
                    HandleSendMark(dt);
                    break;

            }
        }
        catch (Exception e)
        {

            Debug.LogError("Exception handling response: " + e.Message + " >>> " + e.StackTrace);
        }

    }

    public Player GetPlayer(int uid)
    {
        if (!PlayerView.allPlayer.ContainsKey(uid))
        {
            SpawnPlayer(uid);
        }

        return PlayerView.allPlayer[uid].GetComponent<Player>();
    }


    private GameObject InstantiateNetPrefab(string prefab, Vector3 vector3, Quaternion quaternion, ISFSObject data, bool Scene)
    {
        GameObject newObject = _SpawnPrefab(prefab, vector3, quaternion);
        if (newObject == null)
        {
            return null;
        }

        FoxView view = newObject.GetComponent<FoxView>();
        if (!Scene)
        {
            view.viewID = AllocateViewID(_smartFox.MySelf.Id);
        }
        else
        {
            view.viewID = AllocateViewID(FoxView.SCENE_OWNER_ID);
        }
        foxViewList.Add(view.viewID, view);

        return newObject;
    }
    private GameObject RemoteInstantiateNetPrefab(string prefab, Vector3 vector3, Quaternion quaternion, int viewId)
    {
        if (foxViewList.ContainsKey(viewId))
        {
            return foxViewList[viewId].gameObject;

        }
        GameObject newObject = _SpawnPrefab(prefab, vector3, quaternion);
        if (newObject == null)
        {
            return null;
        }
        FoxView view = newObject.GetComponent<FoxView>();

        view.viewID = viewId;

        foxViewList.Add(view.viewID, view);
        return newObject;
    }

    private GameObject _SpawnPrefab(string prefabName, Vector3 vector3, Quaternion quaternion)
    {
        if (prefabName == null || prefabName == "")
        {
            return null;
        }
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
        if (resourceGameObject == null)
        {
            Debug.LogError("FoxNetwork error: Could not Instantiate the prefab [" + prefabName + "]. Please verify you have this gameobject in a Resources folder.");
            return null;
        }
        return Instantiate(resourceGameObject, vector3, quaternion) as GameObject;
    }
    public static void RegisterSceneView(FoxView view)
    {

        view.viewID = AllocateViewID(FoxView.SCENE_OWNER_ID);
        foxViewList.Add(view.viewID, view);
    }

    // use 0 for scene-targetPhotonView-ids
    // returns viewID (combined owner and sub id)
    private static int AllocateViewID(int ownerId)
    {
        if (ownerId == FoxView.SCENE_OWNER_ID)
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
                if (!foxViewList.ContainsKey(newViewId))
                {
                    lastUsedViewSubId = newSubId;
                    return newViewId;
                }
            }

            throw new Exception(string.Format("AllocateViewID() failed. User {0} is out of subIds, as all viewIDs are used.", ownerId));
        }
    }

    /// <summary>
    ///Prevent player form diconect in main menu
    /// </summary>	

    public void ConterIdleRequestSend()
    {
        ISFSObject data = new SFSObject();

        ExtensionRequest request = new ExtensionRequest("conterIdle", data);
        smartFox.Send(request);
    }
    /// <summary>
    /// Spawn current player object
    /// </summary>	
    public void SpawnPlayer(PlayerModel player)
    {

        //Debug.Log( "CREATE PLAYER " + player.userId );
        GameObject newObject = _SpawnPrefab(PlayerPrefab, Vector3.zero, Quaternion.identity);
        PlayerView view = newObject.GetComponent<PlayerView>();
        view.SetId(player.userId);
        view.NetUpdate(player);

    }
    public void SpawnPlayer(int uid)
    {

        //Debug.Log("CREATE PLAYER " + uid);
        GameObject newObject = _SpawnPrefab(PlayerPrefab, Vector3.zero, Quaternion.identity);
        PlayerView view = newObject.GetComponent<PlayerView>();
        view.SetId(uid);

    }


    public void SendMapData()
    {
        ISFSObject data = new SFSObject();
        if (AIDirector.instance != null)
        {
            AIDirector.instance.SendData(data);
            ExtensionRequest request = new ExtensionRequest("mapData", data, serverHolder.gameRoom);
            smartFox.Send(request);
        }
    }
    public void ReadMapData(ISFSObject data)
    {
        AIDirector.instance.ReadData(data);
    }

    //REQUEST SECTION

    /// <summary>
    /// Request the current server time. Used for time synchronization
    /// </summary>	
    public void TimeSyncRequest()
    {
        Sfs2X.Entities.Room room = _smartFox.LastJoinedRoom;
        ExtensionRequest request = new ExtensionRequest("getTime", new SFSObject(), room, true);
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
    public ISFSObject pawnSpawnData;

    public List<WeaponModel> pawnWeapons = new List<WeaponModel>();
    /// <summary>
    /// pawnSpawn request to server
    /// 
    /// </summary>	
    public GameObject BeginPawnSpawnRequest(string prefab, Vector3 vector3, Quaternion quaternion, bool isAI, int[] stims, bool scene)
    {
        pawnSpawnData = new SFSObject();
        pawnWeapons.Clear();
        pawnSpawnData.PutBool("Scene", scene);
        pawnSpawnData.PutBool("isAI", isAI);
        pawnSpawnData.PutIntArray("stims", stims);
        GameObject go = InstantiateNetPrefab(prefab, vector3, quaternion, pawnSpawnData, scene);
        PawnModel pawn = go.GetComponent<Pawn>().GetSerilizedData();
        pawn.type = prefab;
        pawnSpawnData.PutClass("pawn", pawn);

        return go;


    }
    /// <summary>
    /// pawnSpawn request to server
    /// </summary>	
    public GameObject BeginPawnForSwarmSpawnRequest(string prefab, Vector3 vector3, Quaternion quaternion, int[] stims, int swarm, int home, int team = 0, List<CharacteristicToAdd> bonusData = null)
    {
        pawnSpawnData = new SFSObject();
        pawnWeapons.Clear();
        if (bonusData != null)
        {
            ISFSArray sendBonuses = new SFSArray();
            foreach (CharacteristicToAdd bonus in bonusData)
            {
                ISFSObject bonusSfs = new SFSObject();
                bonusSfs.PutInt("characteristic", (int)bonus.characteristic);
                Effect<float> floatEffect = bonus.addEffect as Effect<float>;
                if (floatEffect != null)
                {
                    bonusSfs.PutUtfString("type", "float");
                    bonusSfs.PutFloat("value", floatEffect.value);
                    sendBonuses.AddSFSObject(bonusSfs);
                    continue;
                }
                Effect<int> intEffect = bonus.addEffect as Effect<int>;
                if (intEffect != null)
                {
                    bonusSfs.PutUtfString("type", "int");
                    bonusSfs.PutInt("value", intEffect.value);
                    sendBonuses.AddSFSObject(bonusSfs);
                    continue;
                }
                Effect<bool> boolEffect = bonus.addEffect as Effect<bool>;
                if (boolEffect != null)
                {
                    bonusSfs.PutUtfString("type", "bool");
                    bonusSfs.PutBool("value", boolEffect.value);
                    sendBonuses.AddSFSObject(bonusSfs);
                    continue;
                }

            }
            pawnSpawnData.PutSFSArray("bonus", sendBonuses);
        }

        pawnSpawnData.PutBool("Scene", true);
        pawnSpawnData.PutBool("isAI", true);
        pawnSpawnData.PutIntArray("stims", stims);
        GameObject go = InstantiateNetPrefab(prefab, vector3, quaternion, pawnSpawnData, true);
        PawnModel pawn = go.GetComponent<Pawn>().GetSerilizedData();
        pawn.type = prefab;
        pawn.team = team;
        pawnSpawnData.PutClass("pawn", pawn);
        pawnSpawnData.PutInt("group", swarm);
        pawnSpawnData.PutInt("home", home);
        //        Debug.Log(serverHolder.gameRoom);


        if (bonusData != null)
        {
            go.GetComponent<Pawn>().AddExternalCharacteristic(bonusData);
        }
        return go;
    }
    /// <summary>
    /// weaponSpawn request to server
    /// </summary>	

    public GameObject WeaponSpawn(string prefab, Vector3 vector3, Quaternion quaternion, bool isAI, int pawnId)
    {
        ISFSObject data = new SFSObject();


        GameObject go = InstantiateNetPrefab(prefab, vector3, quaternion, data, isAI);
        WeaponModel weapon = go.GetComponent<BaseWeapon>().GetSerilizedData();
        weapon.type = prefab;
        pawnWeapons.Add(weapon);

        return go;


    }
    /// <summary>
    /// weaponSpawn request to server
    /// </summary>	

    public GameObject InstantWeaponSpawnRequest(string prefab, Vector3 vector3, Quaternion quaternion, bool isAI, int pawnId)
    {
        ISFSObject data = new SFSObject();


        GameObject go = InstantiateNetPrefab(prefab, vector3, quaternion, data, isAI);
        WeaponModel weapon = go.GetComponent<BaseWeapon>().GetSerilizedData();
        weapon.type = prefab;
        data.PutClass("weapon", weapon);
        data.PutInt("pawnId", pawnId);
        ExtensionRequest request = new ExtensionRequest("weaponSpawn", data, serverHolder.gameRoom);
        smartFox.Send(request);
        return go;


    }

    public void ThisSpawnWeaponMakeInHand(int i)
    {
        foreach (WeaponModel model in pawnWeapons)
        {
            model.state = false;
        }
        pawnWeapons[i].state = true;
    }
    public void LastSpawnWeaponMakeInHand()
    {
        foreach (WeaponModel model in pawnWeapons)
        {
            model.state = false;
        }
        pawnWeapons[pawnWeapons.Count - 1].state = true;
    }

    public void EndPawnSpawnRequest()
    {
        if (pawnSpawnData != null)
        {
            SFSArray sendWeapon = new SFSArray();


            foreach (WeaponModel model in pawnWeapons)
            {
                sendWeapon.AddClass(model);
            }
            pawnSpawnData.PutSFSArray("weapons", sendWeapon);
            ExtensionRequest request = new ExtensionRequest("pawnSpawn", pawnSpawnData, serverHolder.gameRoom);
            smartFox.Send(request);
        }
    }
    /// <summary>
    /// pawnChangeShootAnimState request to server
    /// </summary>	
    public void PawnChangeShootAnimStateRequest(int id, string animName, bool state)
    {
        ISFSObject data = new SFSObject();

        data.PutInt("id", id);
        data.PutBool("state", state);
        data.PutUtfString("animName", animName);
        ExtensionRequest request = new ExtensionRequest("pawnChangeShootAnimState", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }
    /// <summary>
    /// pawnStartKick request to server
    /// </summary>	
    public void PawnKickRequest(int id, int kick, bool state)
    {
        ISFSObject data = new SFSObject();

        data.PutInt("id", id);
        data.PutBool("state", state);
        data.PutInt("kick", kick);
        ExtensionRequest request = new ExtensionRequest("pawnStartKick", data, serverHolder.gameRoom);
        smartFox.Send(request);
    }
    /// <summary>
    /// pawnActiveState request to server
    /// </summary>	
    public void PawnActiveStateRequest(int id, bool state)
    {
        ISFSObject data = new SFSObject();

        data.PutInt("id", id);
        data.PutBool("active", state);
        ExtensionRequest request = new ExtensionRequest("pawnActiveState", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }
    /*ConcurrentQueue<ExtensionRequest> outQueue = new ConcurrentQueue<ExtensionRequest>();

    private Thread myThread;

    private void SendLoop()
    {
        while (true)
        {
            //Debug.Log (incomeQueue.Count	);
            while (outQueue.Count > 0)
            {
                ExtensionRequest mess = outQueue.Dequeue();
                Debug.Log("SEND");
                smartFox.Send(mess);
            }
           
            //	

            Thread.Sleep(10);
        }
    }*/
    /// <summary>
    /// pawnUpdate request to server
    /// </summary>	
    public void PawnUpdateRequest(ISFSObject data)
    {

        ExtensionRequest request = new ExtensionRequest("pawnUpdate", data, serverHolder.gameRoom, true);
        //outQueue.Enqueue(request);
        smartFox.Send(request);

    }
    /// <summary>
    /// pawnTaunt request to server
    /// </summary>	
    public void PawnTauntRequest(int id, string animName)
    {
        ISFSObject data = new SFSObject();

        data.PutInt("id", id);
        data.PutUtfString("animName", animName);
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
        //Debug.Log("DeleteView Request");
        data.PutInt("id", id);
        ExtensionRequest request = new ExtensionRequest("deleteView", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }

    /// <summary>
    /// weaponShoot request to server
    /// </summary>	



    public void WeaponShootRequest(ISFSObject data)
    {
        ExtensionRequest request = new ExtensionRequest("weaponShoot", data, serverHolder.gameRoom);
        smartFox.Send(request);


    }
    /// <summary>
    /// pawnSkillCastEffect request to server
    /// </summary>	

    public void SkillCastEffectRequest(int id, string name)
    {
        ISFSObject data = new SFSObject();

        data.PutInt("id", id);
        data.PutUtfString("name", name);
        ExtensionRequest request = new ExtensionRequest("pawnSkillCastEffect", data, serverHolder.gameRoom);
        smartFox.Send(request);


    }

    /// <summary>
    /// pawnSkillActivate request to server
    /// </summary>	

    public void SkillActivateRequest(ISFSObject data)
    {
        ExtensionRequest request = new ExtensionRequest("pawnSkillActivate", data, serverHolder.gameRoom);
        smartFox.Send(request);


    }

    /// <summary>
    /// pawnDetonate request to server
    /// </summary>	

    public void DetonateRequest(int id)
    {
        ISFSObject data = new SFSObject();

        data.PutInt("id", id);
        ExtensionRequest request = new ExtensionRequest("pawnDetonate", data, serverHolder.gameRoom);
        smartFox.Send(request);


    }


    /// <summary>
    /// invokeProjectileCall request to server
    /// </summary>	

    public void InvokeProjectileCallRequest(ISFSObject data)
    {
        ExtensionRequest request = new ExtensionRequest("invokeProjectileCall", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }
    /// <summary>
    /// simplePrefabSpawn request to server
    /// </summary>	

    /*
     * type is descriptor for logic on server 0 mean no logic
     * 
     * */

    public enum PREFABTYPE { SIMPLE, EGG, PLAYERBUILDING };
    public GameObject SimplePrefabSpawn(string prefab, Vector3 vector3, Quaternion quaternion, ISFSObject data, bool isScene, PREFABTYPE type = PREFABTYPE.SIMPLE)
    {

        GameObject go = InstantiateNetPrefab(prefab, vector3, quaternion, data, true);
        SimpleNetModel model = new SimpleNetModel();
        model.id = go.GetComponent<FoxView>().viewID;
        model.type = prefab;
        model.position.WriteVector(vector3);
        model.rotation.WriteQuat(quaternion);
        data.PutClass("model", model);
        data.PutInt("preftype", (int)type);
        ExtensionRequest request = new ExtensionRequest("simplePrefabSpawn", data, serverHolder.gameRoom);
        smartFox.Send(request);
        return go;

    }
    public GameObject SimplePrefabSpawn(string prefab, Vector3 vector3, Quaternion quaternion, ISFSObject data, PREFABTYPE type = PREFABTYPE.SIMPLE)
    {

        return SimplePrefabSpawn(prefab, vector3, quaternion, data, true, type);

    }
    public GameObject SimplePrefabSpawn(string prefab, Vector3 vector3, Quaternion quaternion, PREFABTYPE type = PREFABTYPE.SIMPLE)
    {
        ISFSObject data = new SFSObject();
        return SimplePrefabSpawn(prefab, vector3, quaternion, data, type);

    }


    public GameObject QuuenEggSpawn(string prefab, Vector3 vector3, Quaternion quaternion, int spawnId, long delay)
    {
        ISFSObject data = new SFSObject();
        data.PutInt("spawnid", spawnId);
        data.PutLong("delay", delay);
        return SimplePrefabSpawn(prefab, vector3, quaternion, data, PREFABTYPE.EGG);

    }
    /// <summary>
    /// pawnDiedByKill request to server
    /// </summary>	

    public void PawnDiedByKillRequest(ISFSObject data)
    {


        ExtensionRequest request = new ExtensionRequest("pawnDiedByKill", data, serverHolder.gameRoom);
        smartFox.Send(request);
    }
    /// <summary>
    /// vipSpawned request to server
    /// </summary>	

    public void VipSpawnedRequest(PawnModel pawn)
    {
        ISFSObject data = new SFSObject();

        data.PutClass("pawn", pawn);
        ExtensionRequest request = new ExtensionRequest("vipSpawned", data, serverHolder.gameRoom);
        smartFox.Send(request);
    }
    /// <summary>
    /// registerSceneView request to server
    /// </summary>	

    public void RegisterSceneViewRequest(SimpleNetModel model)
    {
        ISFSObject data = new SFSObject();

        data.PutClass("model", model);
        ExtensionRequest request = new ExtensionRequest("registerSceneView", data, serverHolder.gameRoom);
        smartFox.Send(request);
    }
    /// <summary>
    /// nextRoom request to server
    /// </summary>	

    public void NextRoomRequest()
    {
        ISFSObject data = new SFSObject();

        ExtensionRequest request = new ExtensionRequest("nextRoom", data, serverHolder.gameRoom);
        smartFox.Send(request);
    }
    /// <summary>
    /// nextRoute request to server
    /// </summary>	

    public void NextRouteRequest(int nextRoute)
    {
        ISFSObject data = new SFSObject();
        data.PutInt("nextRoute", nextRoute);
        ExtensionRequest request = new ExtensionRequest("nextRoute", data, serverHolder.gameRoom);
        smartFox.Send(request);
    }

    /// <summary>
    /// gameRuleArrived request to server
    /// </summary>	

    public void GameRuleArrivedRequest()
    {
        ISFSObject data = new SFSObject();

        ExtensionRequest request = new ExtensionRequest("gameRuleArrived", data, serverHolder.gameRoom);
        smartFox.Send(request);
    }
    /// <summary>
    /// leaveRoom request to server
    /// </summary>	

    public void LeaveRoomReuqest()
    {


        LeaveRoomRequest request = new LeaveRoomRequest(serverHolder.gameRoom);
        PlayerManager.instance.ClearAll();
        PlayerView.allPlayer.Clear();
        smartFox.Send(request);
    }
    /// <summary>
    /// pawnInPilotChange request to server
    /// </summary>	


    public void InPilotChangeRequest(int id, bool isPilotIn)
    {
        ISFSObject data = new SFSObject();

        data.PutInt("id", id);
        data.PutBool("isPilotIn", isPilotIn);
        ExtensionRequest request = new ExtensionRequest("pawnInPilotChange", data, serverHolder.gameRoom);
        smartFox.Send(request);
    }
    /// <summary>
    /// pawnBattleJuggerSpawn request to server
    /// </summary>	
    public GameObject BattleJuggerSpawnRequest(string prefab, Vector3 vector3, Quaternion quaternion, int[] stims)
    {
        ISFSObject data = new SFSObject();

        data.PutBool("Scene", true);
        data.PutIntArray("stims", stims);
        GameObject go = InstantiateNetPrefab(prefab, vector3, quaternion, data, true);
        PawnModel pawn = go.GetComponent<Pawn>().GetSerilizedData();
        pawn.type = prefab;
        data.PutClass("pawn", pawn);
        // Debug.Log(serverHolder.gameRoom);
        ExtensionRequest request = new ExtensionRequest("pawnBattleJuggerSpawn", data, serverHolder.gameRoom);
        smartFox.Send(request);
        return go;


    }

    /// <summary>
    /// baseSpawned request to server
    /// </summary>	

    public void BaseSpawnedRequest(BaseModel model)
    {
        ISFSObject data = new SFSObject();

        data.PutClass("model", model);
        ExtensionRequest request = new ExtensionRequest("baseSpawned", data, serverHolder.gameRoom);
        //Debug.Log("BaseSpawnedRequest");
        smartFox.Send(request);
    }
    /// <summary>
    /// gameRuleDamageBase request to server
    /// </summary>	

    public void BaseDamageRequest(int team, int damage)
    {
        ISFSObject data = new SFSObject();

        data.PutInt("team", team);
        data.PutInt("damage", damage);
        ExtensionRequest request = new ExtensionRequest("gameRuleDamageBase", data, serverHolder.gameRoom);

        smartFox.Send(request);
    }
    /// <summary>
    /// updateSimpleDestroyableObject request to server
    /// </summary>	

    public void UpdateSimpleDestroyableObjectRequest(SimpleDestroyableModel model)
    {
        ISFSObject data = new SFSObject();

        data.PutClass("model", model);
        ExtensionRequest request = new ExtensionRequest("updateSimpleDestroyableObject", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }
    /// <summary>
    /// updateConquestPoint request to server
    /// </summary>	

    public void UpdateConquestPointRequest(ConquestPointModel model)
    {
        ISFSObject data = new SFSObject();

        data.PutClass("model", model);
        ExtensionRequest request = new ExtensionRequest("updateConquestPoint", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }
    /// <summary>
    /// deleteSceneView request to server
    /// </summary>	

    public void DeleteSceneViewRequest(int id)
    {
        ISFSObject data = new SFSObject();

        data.PutInt("id", id);
        ExtensionRequest request = new ExtensionRequest("deleteSceneView", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }

    /// <summary>
    /// remoteDamageOnPawn request to server
    /// </summary>	

    public void RemoteDamageOnPawnRequest(BaseDamageModel model, int pawnId, int killerId)
    {
        ISFSObject data = new SFSObject();

        data.PutClass("model", model);
        data.PutInt("pawnId", pawnId);
        data.PutInt("killerId", killerId);
        ExtensionRequest request = new ExtensionRequest("remoteDamageOnPawn", data, serverHolder.gameRoom);
        smartFox.Send(request);
    }
    /// <summary>
    /// enterRobot request to server
    /// </summary>	

    public void EnterRobotRequest(int robotId)
    {
        ISFSObject data = new SFSObject();
        data.PutInt("robotId", robotId);
        //Debug.Log("enter");
        ExtensionRequest request = new ExtensionRequest("enterRobot", data, serverHolder.gameRoom);
        if (smartFox.MySelf.ContainsVariable("Master") && NetworkController.smartFox.MySelf.GetVariable("Master").GetBoolValue())
        {
            data.PutInt("userId", smartFox.MySelf.Id);
            HandleEnterRobot(data);
        }
        else
        {
            smartFox.Send(request);
        }
    }
    /// <summary>
    /// bossHit request to server
    /// </summary>	

    public void BossHitRequest(float damage, int pawnId)
    {
        ISFSObject data = new SFSObject();
        data.PutInt("pawnId", pawnId);
        data.PutFloat("damage", damage);
        ExtensionRequest request = new ExtensionRequest("bossHit", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }
    /// <summary>
    /// lastWave request to server
    /// </summary>	

    public void LastWaveRequest()
    {

        ExtensionRequest request = new ExtensionRequest("lastWave", new SFSObject(), serverHolder.gameRoom);
        smartFox.Send(request);

    }
    /// <summary>
    /// customAnimStart request to server
    /// </summary>	

    public void CustomAnimStartRequest(int pawnId, string animName)
    {
        ISFSObject data = new SFSObject();
        data.PutInt("pawnId", pawnId);
        data.PutUtfString("anim", animName);
        ExtensionRequest request = new ExtensionRequest("customAnimStart", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }
    /// <summary>
    /// changeWeaponShootState request to server
    /// </summary>	

    public void ChangeWeaponShootStateRequest(int weaponId, bool state)
    {
        ISFSObject data = new SFSObject();
        data.PutInt("id", weaponId);
        data.PutBool("state", state);
        ExtensionRequest request = new ExtensionRequest("changeWeaponShootState", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }

    /// <summary>
    /// changeWeaponState request to server
    /// </summary>	

    public void ChangeWeaponStateRequest(int weaponId, bool state)
    {
        ISFSObject data = new SFSObject();
        data.PutInt("id", weaponId);
        data.PutBool("state", state);
        ExtensionRequest request = new ExtensionRequest("changeWeaponState", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }
    /// <summary>
    /// gamePointData request to server
    /// </summary>	

    public void GamePointDataRequest(ISFSArray sendPoint)
    {
        ISFSObject data = new SFSObject();
        data.PutSFSArray("points", sendPoint);

        ExtensionRequest request = new ExtensionRequest("gamePointData", data, serverHolder.gameRoom);
        smartFox.Send(request);

    }

    /// <summary>
    /// sendMark request to server
    /// </summary>
    public void SendMarkRequest(int pawnId)
    {
        ISFSObject data = new SFSObject();
        data.PutInt("id", pawnId);
        ExtensionRequest request = new ExtensionRequest("sendMark", data, serverHolder.gameRoom);
        smartFox.Send(request);
    }











    //Handler SECTION
    /// <summary>
    /// handle pawnSpawn  from Server
    /// </summary>	

    public void HandlePawnSpawn(ISFSObject dt)
    {
        PawnModel sirPawn = (PawnModel)dt.GetClass("pawn");
        if (deleteIdLate.Remove(sirPawn.id))
        {
            Debug.Log("ALredy delete");
            return;
        }
        //        Debug.Log("Pawn Spawn" + sirPawn.type + "ID:"+ sirPawn.id);
        GameObject go = RemoteInstantiateNetPrefab(sirPawn.type, sirPawn.position.GetVector(), sirPawn.rotation.GetQuat(), sirPawn.id);
        if (go == null)
        {
            return;
        }
        Pawn pawn = go.GetComponent<Pawn>();
        if (dt.ContainsKey("ownerId"))
        {
            Player player = GetPlayer(dt.GetInt("ownerId"));

            if (dt.ContainsKey("isAI") && dt.GetBool("isAI"))
            {
                player.AISpawnSetting(pawn, dt.GetIntArray("stims"));
            }
            else
            {
                player.AfterSpawnSetting(pawn, dt.GetIntArray("stims"));
            }
        }
        else
        {
            if (dt.ContainsKey("isAI") && dt.GetBool("isAI"))
            {
                pawn.RemoteSetAI(dt.GetInt("group"), dt.GetInt("home"));
            }
        }
        pawn.NetUpdate(sirPawn);
        if (dt.ContainsKey("bonus"))
        {
            ISFSArray sendBonuses = dt.GetSFSArray("bonus");
            List<CharacteristicToAdd> effects = new List<CharacteristicToAdd>();
            foreach (ISFSObject bonus in sendBonuses)
            {
                CharacteristicToAdd add = new CharacteristicToAdd();
                add.characteristic = (CharacteristicList)bonus.GetInt("characteristic");
                string type = bonus.GetUtfString("type");
                BaseEffect effect = null;
                if (type == "float")
                {
                    effect = new Effect<float>(bonus.GetFloat("value"));
                }
                else if (type == "int")
                {
                    effect = new Effect<int>(bonus.GetInt("value"));
                }
                else
                {
                    effect = new Effect<bool>(bonus.GetBool("value"));
                }
                effect.initalEffect = true;
                add.addEffect = effect;
                effects.Add(add);


            }
            pawn.AddExternalCharacteristic(effects);
        }

        ISFSArray weapons = dt.GetSFSArray("weapons");

        foreach (WeaponModel weaponModel in weapons)
        {
            GameObject wepGo = RemoteInstantiateNetPrefab(weaponModel.type, Vector3.zero, Quaternion.identity, weaponModel.id);
            if (wepGo == null)
            {
                continue;
            }

            BaseWeapon weapon = wepGo.GetComponent<BaseWeapon>();
            weapon.NetUpdate(weaponModel);
            //			Debug.Log("PAwn" + pawn + " View" + GetView(dt.GetInt("pawnId")) + "ID" + dt.GetInt("pawnId"));
            weapon.RemoteAttachWeapon(pawn, weaponModel.state);
        }

    }
    /// <summary>
    /// handle pawnChangeShootAnimState(  from Server
    /// </summary>	

    public void HandlePawnChangeShootAnimState(ISFSObject dt)
    {

        Pawn pawn = GetView(dt.GetInt("id")).pawn;
        if (dt.GetBool("state"))
        {
            pawn.StartShootAnimation(dt.GetUtfString("animName"));
        }
        else
        {
            pawn.StopShootAnimation(dt.GetUtfString("animName"));
        }

    }
    /// <summary>
    /// handle pawnStartKick  from Server
    /// </summary>	

    public void HandlePawnKick(ISFSObject dt)
    {

        Pawn pawn = GetView(dt.GetInt("id")).pawn;
        if (dt.GetBool("state"))
        {
            pawn.Kick(dt.GetInt("kick"));
        }
        else
        {
            pawn.StopKick();
        }

    }
    /// <summary>
    /// handle pawnActiveState  from Server
    /// </summary>	

    public void HandlePawnActiveState(ISFSObject dt)
    {

        Pawn pawn = GetView(dt.GetInt("id")).pawn;
        if (dt.GetBool("active"))
        {
            pawn.RemoteActivate();
        }
        else
        {
            pawn.RemoteDeActivate();
        }

    }
    /// <summary>
    /// handle pawnUpdate  from Server
    /// </summary>	

    public void HandlePawnUpdate(ISFSObject allDt)
    {
        ISFSArray pawns = allDt.GetSFSArray("pawns");
        foreach (PawnModel pawnModel in pawns)
        {
            FoxView view = GetView(pawnModel.id);
            if (view != null)
            {
                Pawn pawn = view.pawn;
                pawn.NetUpdate(pawnModel);
            }
            else
            {
                Debug.LogError("NOT FOUND" + pawnModel.id);
            }
        }
    }
    /// <summary>
    /// handle pawnTaunt  from Server
    /// </summary>	

    public void HandlePawnTaunt(ISFSObject dt)
    {
        Pawn pawn = GetView(dt.GetInt("id")).pawn;
        pawn.RemotePlayTaunt(dt.GetUtfString("animName"));

    }
    /// <summary>
    /// handle pawnKnockOut  from Server
    /// </summary>	

    public void HandlePawnKnockOut(ISFSObject dt)
    {
        Pawn pawn = GetView(dt.GetInt("id")).pawn;
        pawn.StartKnockOut();

    }
    /// <summary>
    /// handle deleteView  from Server
    /// </summary>	

    public void HandleDeleteView(ISFSObject dt)
    {

        if (GetView(dt.GetInt("id")) == null)
        {
            deleteIdLate.Add(dt.GetInt("id"));
            return;
        }
        DestroyableNetworkObject obj = GetView(dt.GetInt("id")).GetComponent<DestroyableNetworkObject>();

        obj.KillMe();

    }
    /// <summary>
    /// handle weaponSpawn  from Server
    /// </summary>	

    public void HandleWeaponSpawn(ISFSObject dt)
    {
        WeaponModel sirWeapon = (WeaponModel)dt.GetClass("weapon");
        if (deleteIdLate.Remove(sirWeapon.id))
        {

            return;
        }
        if (foxViewList.ContainsKey(sirWeapon.id))
        {
            return;
        }
        GameObject go = RemoteInstantiateNetPrefab(sirWeapon.type, Vector3.zero, Quaternion.identity, sirWeapon.id);
        if (go == null)
        {
            return;
        }

        BaseWeapon weapon = go.GetComponent<BaseWeapon>();
        weapon.NetUpdate(sirWeapon);
        Pawn pawn = GetView(dt.GetInt("pawnId")).pawn;
        Debug.Log("PAwn" + pawn + " View" + GetView(dt.GetInt("pawnId")) + "ID" + sirWeapon.id);
        weapon.RemoteAttachWeapon(pawn, sirWeapon.state);



    }
    /// <summary>
    /// handle weaponShoot  from Server
    /// </summary>	


    public void HanldeWeaponShoot(ISFSObject allDt)
    {
        ISFSArray shoots = allDt.GetSFSArray("shoots");
        foreach (SFSObject dt in shoots)
        {
            FoxView view = GetView(dt.GetInt("id"));
            if (view == null)
            {
                continue;
            }
            BaseWeapon weapon = GetView(dt.GetInt("id")).weapon;

            weapon.RemoteShot(((Vector3Model)dt.GetClass("position")).GetVector(),
                                ((QuaternionModel)dt.GetClass("direction")).GetQuat(),
                                dt.GetFloat("power"), dt.GetFloat("range"), dt.GetFloat("minRange"), dt.GetInt("viewId"), dt.GetInt("projId"), dt.GetLong("timeShoot"));
        }


    }
    /// <summary>
    /// handle pawnSkillCastEffect  from Server
    /// </summary>	

    public void HandlePawnSkillCastEffect(ISFSObject dt)
    {
        Pawn pawn = GetView(dt.GetInt("id")).pawn;
        pawn.skillManager.GetSkill(dt.GetUtfString("name")).CasterVisualEffect();

    }
    /// <summary>
    /// handle pawnSkillActivate  from Server
    /// </summary>	

    public void HandleSkillActivate(ISFSObject dt)
    {
        Pawn pawn = GetView(dt.GetInt("id")).pawn;
        SkillBehaviour skill = pawn.skillManager.GetSkill(dt.GetUtfString("name"));
        switch (skill.type)
        {
            case TargetType.SELF:
            case TargetType.GROUPOFPAWN_BYSELF:
                skill.RemoteActivateSkill();
                break;
            case TargetType.PAWN:
            case TargetType.GROUPOFPAWN_BYPAWN:
                skill.RemoteActivateSkill(dt.GetInt("viewId"));
                break;
            case TargetType.POINT:
            case TargetType.GROUPOFPAWN_BYPOINT:
                skill.RemoteActivateSkill(((Vector3Model)dt.GetClass("position")).GetVector());
                break;
        }


    }
    /// <summary>
    /// handle pawnDetonate  from Server
    /// </summary>	

    public void HandlePawnDetonate(ISFSObject dt)
    {
        SuicidePawn pawn = GetView(dt.GetInt("id")).pawn as SuicidePawn;
        pawn.RemoteDetonate();

    }

    /// <summary>
    /// handle invokeProjectileCall  from Server
    /// </summary>	

    public void HandleInvokeProjectileCall(ISFSObject dt)
    {

        ProjectileManager.instance.RemoteInvoke(dt);

    }

    /// <summary>
    ///handle  simplePrefabSpawn  from server
    /// </summary>	

    public void HandleSimplePrefabSpawn(ISFSObject dt)
    {
        SimpleNetModel model = dt.GetClass("model") as SimpleNetModel;
        if (model != null)
        {
            if (deleteIdLate.Remove(model.id))
            {
                return;
            }
            GameObject go = RemoteInstantiateNetPrefab(model.type, model.position.GetVector(), model.rotation.GetQuat(), model.id);
            if (go == null)
            {
                return;
            }
            switch ((PREFABTYPE)dt.GetInt("preftype"))
            {
                case PREFABTYPE.PLAYERBUILDING:
                    go.GetComponent<Building>().SetOwner(GetPlayer(dt.GetInt("ownerId")));
                    break;
            }
            return;
        }

        SimpleDestroyableModel destrModel = dt.GetClass("model") as SimpleDestroyableModel;
        if (model != null)
        {
            if (deleteIdLate.Remove(destrModel.id))
            {
                return;
            }

            FoxView view = GetView(destrModel.id);
            if (view != null)
            {
                view.GetComponent<DestroyableNetworkObject>().SetHealth(destrModel.health);
            }
        }


    }

    /// <summary>
    ///handle  gameStart  from server
    /// </summary>	

    public void HandleGameStart()
    {
        //Debug.Log("GAME START");
        GameRule.instance.StartGame();

    }

    /// <summary>
    ///handle masterStart  from server
    /// </summary>	

    public void HandleMasterStart(ISFSObject dt)
    {
        Debug.Log("HandleMasterStart: " + string.Join("|", dt.GetKeys()));
        GameRule.instance.ReadMasterInfo(dt);
        //MasterViewUpdate();      

    }
    /// <summary>
    ///handle gameUpdate  from server
    /// </summary>	

    public void HandleGameUpdate(ISFSObject dt)
    {
        //Debug.Log("gameeUPDATE");
        GameRuleModel model = (GameRuleModel)dt.GetClass("game");
        GameRule.instance.SetFromModel(model);

    }
    /// <summary>
    ///handle nextMap  from server
    /// </summary>	

    public void HandleNextMap(ISFSObject dt)
    {

        string map = dt.GetUtfString("map");
        serverHolder.LoadNextMap(map);

    }
    /// <summary>
    ///handle pawnDiedByKill  from server
    /// </summary>	

    public void HandlePawnDiedByKill(ISFSObject dt)
    {
        //        Debug.Log("PAWN DIED");
        if (GetView(dt.GetInt("viewId")) == null)
        {
            deleteIdLate.Add(dt.GetInt("viewId"));
            Debug.Log("Add to Late");
            return;
        }
        Pawn pawn = GetView(dt.GetInt("viewId")).pawn;

        int player = dt.GetInt("player");
        if (player == _smartFox.MySelf.Id)
        {
            bool headShoot = false;
            if (dt.ContainsKey("headShot"))
            {
                headShoot = dt.GetBool("headShot");
            }
            KillInfo info = new KillInfo(dt.GetInt("weaponId"), headShoot);
            GetPlayer(player).PawnKill(pawn, pawn.player, pawn.myTransform.position, info);
            pawn.PawnKill(GetPlayer(player));

        }
        else
        {
            if (player > 0)
            {
                pawn.PawnKill(GetPlayer(player));
            }
            else
            {
                pawn.PawnKill(null);
            }



        }



    }
    /// <summary>
    ///handle playerLeave  from server
    /// </summary>	

    public void HandlePlayerLeave(ISFSObject dt)
    {

        int playerID = dt.GetInt("playerId");
        //Debug.Log("PLAYER LEAVESERVER"+playerID);
        if (!PlayerView.allPlayer.ContainsKey(playerID))
        {
            return;
        }
        Destroy(PlayerView.allPlayer[playerID].gameObject);
        PlayerView.allPlayer.Remove(playerID);
        foreach (int viewId in dt.GetSFSArray("views"))
        {
            FoxView view = GetView(viewId);
            if (view != null)
            {
                Destroy(view.gameObject);
            }
        }


    }
    /// <summary>
    ///handle player leave by std Event;
    /// </summary>	

    public void HandlePlayerLeaveEvent(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];
        User user = (User)evt.Params["user"];
        if (room.Id != serverHolder.gameRoom.Id)
        {
            return;
        }

        int playerID = user.Id;
        //Debug.Log("PLAYER LEAVE"+playerID);
        if (!PlayerView.allPlayer.ContainsKey(playerID))
        {
            return;
        }
        //
        Destroy(PlayerView.allPlayer[playerID].gameObject);

        foreach (FoxView view in foxViewList.Values)
        {
            if (view.IsOwner(playerID))
            {
                Destroy(view.gameObject);
            }
        }
        Debug.Log("LEave romme");

    }


    /// <summary>
    /// handle pawnInPilotChange  from Server
    /// </summary>	

    public void HandlePawnInPilotChange(ISFSObject dt)
    {

        RobotPawn pawn = GetView(dt.GetInt("id")).pawn as RobotPawn;
        pawn.isPilotIn = dt.GetBool("isPilotIn");

    }
    /// <summary>
    /// handle updateSimpleDestroyableObject from Server
    /// </summary>	

    public void HandleUpdateSimpleDestroyableObject(ISFSObject dt)
    {

        SimpleDestroyableModel model = (SimpleDestroyableModel)dt.GetClass("model");
        DamagebleObject target = GetView(model.id).GetComponent<DamagebleObject>();
        target.health = model.id;

    }

    /// <summary>
    /// handle updateConquestPoint from Server
    /// </summary>	

    public void HandleUpdateConquestPoint(ISFSObject dt)
    {

        ConquestPointModel model = (ConquestPointModel)dt.GetClass("model");
        ConquestPoint target = GetView(model.id).conquestPoint;
        target.UpdateFromModel(model);

    }

    /// <summary>
    ///handle  remoteDamageOnPawn request from server
    /// </summary>	

    public void HandleRemoteDamageOnPawn(ISFSObject dt)
    {
        Pawn pawn = GetView(dt.GetInt("pawnId")).pawn;
        GameObject killer = GetView(dt.GetInt("killerId")).gameObject;

        pawn.LowerHealth((BaseDamageModel)dt.GetClass("model"), killer);

    }
    /// <summary>
    ///handle enterRobot request from server
    /// </summary>	

    public void HandleEnterRobot(ISFSObject dt)
    {

        RobotPawn pawn = (RobotPawn)GetView(dt.GetInt("robotId")).pawn;
        if (pawn.isEmpty)
        {
            pawn.isEmpty = false;
            ExtensionRequest request = new ExtensionRequest("enterRobotSuccess", dt, serverHolder.gameRoom);

            smartFox.Send(request);
        }

    }
    /// <summary>
    ///handle enterRobotSuccess request from server
    /// </summary>	

    public void HandleEnterRobotSuccess(ISFSObject dt)
    {
        ISFSObject data = new SFSObject();
        RobotPawn pawn = (RobotPawn)GetView(dt.GetInt("robotId")).pawn;
        pawn.isEmpty = false;
        Player player = GetPlayer(dt.GetInt("userId"));
        player.AfterSpawnSetting(pawn, new int[] { });
        if (_smartFox.MySelf.Id == dt.GetInt("userId"))
        {
            pawn.foxView.SetMine(true);
            //Debug.Log("enterRobotSuccess");
            player.EnterBotSuccess(pawn);
        }
        else
        {
            //Debug.Log("enterRobotSuccess");
            pawn.foxView.SetMine(false);
            pawn.AnotherEnter();
        }
        pawn.foxView.SetOwner(dt.GetInt("userId"));
        if (player.team == 1)
        {
            PlayerMainGui.instance.Annonce(AnnonceType.INTEGRATAKEJUGGER);
        }
        else
        {
            PlayerMainGui.instance.Annonce(AnnonceType.RESTAKEJUGGER);
        }


    }


    /// <summary>
    /// handle customAnimStart  request from server
    /// </summary>	

    public void HandleCustomAnimStar(ISFSObject dt)
    {
        ISFSObject data = new SFSObject();
        GetView(dt.GetInt("pawnId")).pawn.PlayCustomAnimRemote(dt.GetUtfString("anim"));



    }

    /// <summary>
    /// handle AISwarmUpdate  request from server
    /// </summary>	


    public void HandleAISwarmUpdate(ISFSObject dt)
    {
        ISFSObject data = new SFSObject();
        AIDirector.instance.RemoteStateChange(dt.GetInt("swarmId"), dt.GetBool("isActive"));



    }
    /// <summary>
    /// handle AINextWave  request from server
    /// </summary>	


    public void HandleAINextWave(ISFSObject dt)
    {
        ISFSObject data = new SFSObject();
        AISwarm_QuantizeWave swarm = AIDirector.instance.swarms[dt.GetInt("swarmId")] as AISwarm_QuantizeWave;
        if (swarm != null)
        {
            swarm.NextSwarmWave();
        }


    }
    /// <summary>
    /// handle AISpawnBot  request from server
    /// </summary>	


    public void HandleAISpawnBot(ISFSObject dt)
    {

        //        Debug.Log("HandleAISpawnBot: " + string.Join("|", dt.GetKeys()));
        if (dt.ContainsKey("team"))
        {
            AIDirector.instance.swarms[dt.GetInt("swarmId")].SpawnBot(dt.GetUtfString("prefabName"), dt.GetInt("id"), ((Vector3Model)dt.GetClass("position")).GetVector(), dt.GetInt("team"));
        }
        else
        {
            AIDirector.instance.swarms[dt.GetInt("swarmId")].SpawnBot(dt.GetUtfString("prefabName"), dt.GetInt("id"), ((Vector3Model)dt.GetClass("position")).GetVector());
        }



    }
    /// <summary>
    /// handle changeWeaponShootState  request from server
    /// </summary>	
    public void HandleChangeWeaponShootState(ISFSObject dt)
    {
        BaseWeapon weapon = GetView(dt.GetInt("id")).weapon;
        if (dt.GetBool("state"))
        {
            weapon.StartFireRep();
        }
        else
        {
            weapon.StopFire();
        }
    }
    /// <summary>
    /// handle changeWeaponState  request from server
    /// </summary>	
    public void HandleChangeWeaponState(ISFSObject dt)
    {
        FoxView view = GetView(dt.GetInt("id"));
        if (view == null)
        {
            return;
        }
        BaseWeapon weapon = GetView(dt.GetInt("id")).weapon;
        Debug.Log("HandleChangeWeaponState");
        if (dt.GetBool("state"))
        {
            weapon.TakeInHand();
        }
        else
        {
            weapon.PutAway();
        }
    }

    /// <summary>
    ///handle  gamePointData request from server
    /// </summary>	

    public void HandleGamePointData(ISFSObject dt)
    {

        ISFSArray points = dt.GetSFSArray("points");
        foreach (AssaultPointModel point in points)
        {
            ((PointGameRule)GameRule.instance).PointUpdate(point);
        }

    }
    /// <summary>
    ///handle  sendMark request from server
    /// </summary>	
    public void HandleSendMark(ISFSObject dt)
    {
        Pawn pawn = GetView(dt.GetInt("id")).pawn;
        pawn.MarkMe();
    }
}

