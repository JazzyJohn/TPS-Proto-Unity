using UnityEngine;
using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;

public enum PawnType{PAWN,BOT};

//BIT MASK 
public enum GameClassEnum{ENGINEER,ASSAULT,SCOUT,MEDIC,ANY,ROBOTHEAVY,ROBOTMEDIUM,ROBOTLIGHT,ANYROBOT};

public class Player : MonoBehaviour {

	public LayerMask robotLayer =1 ;

	public List<string> friendsInfo = new List<string>();

	public float respawnTime = 10.0f;
	
	private float robotTime = 60.0f;
	
	private float robotTimer;
	
	private float robotKillReduce  =5.0f;

	public bool isStarted = false;

	public int selected;

	public int selectedBot;
	
	private Pawn currentPawn;
	
	private RobotPawn robotPawn;
	
	public bool inBot;
	
	private GhostObject ghostBot;
	
	private bool canSpawnBot=false;
	
	public int team =0;

	private RobotPawn prefabBot;
	
	private GameObject prefabGhostBot;

	public string PlayerName="VK NAME";

	public string UID;

	public Texture2D vkAvavtar;
	
	 // Declare your serializable data.
	[System.Serializable]
	public class PlayerScore
		{
		public int Kill =0;
		public int Death=0;
		public int Assist=0;
		public int RobotKill=0;
	}
 
	
	
	private float respawnTimer;
	
	public PlayerScore Score = new PlayerScore();
	
	private Camera myCamera;

	private bool isDead=true;

	private  PlayerMainGui.PlayerStats stats = new PlayerMainGui.PlayerStats ();
	
	private bool canSpamBot = true;

    public PlayerView playerView;

    public bool isMine { get { return playerView.isMine; } }

	public UseObject useTarget;
	//Func name for delayed external call
	public string delayedExternalCallName;
	//param for delayed external call
	public string delayedExternalCallData;
	
	public bool wasWallPost = false;
	
	public const float SQUERED_RADIUS_OF_ACTION = 16.0f;
	
	public GlobalPlayer globalPlayer;

    private bool robotAnnonce =false;
    
    public int killInRow = 0;
	
	private CharacteristicPlayerManager charMan;

    private List<int> activeSteampacks = new List<int>();

    private List<int> activeBuff = new List<int>();

    public DamagebleObject building;

	public bool isPlayerFriend(string playerId)
	{
	 	foreach (string id in friendsInfo) 
		{
			if(id.Equals(playerId))
				return true;
		}

		return false;
	}
    void Awake()
    {
		DontDestroyOnLoad(gameObject);
        playerView = GetComponent<PlayerView>();
		
    }
    void Start()
    {
        Init();
    }
    public void Restart()
    {
        Init();
    }
	void Init(){
		
		PlayerManager.instance.addPlayer(this);
       
        if (playerView.isMine)
        {
            if (charMan == null)
            {
                charMan = GetComponent<CharacteristicPlayerManager>();
                charMan.Init();
            }
            myCamera = Camera.main;
            ((PlayerMainGui)myCamera.GetComponent(typeof(PlayerMainGui))).SetLocalPlayer(this);
            robotTimer = 0;

            //this.name = "Player";		
      
            //	photonView.RPC ("ASKTeam", PhotonTargets.MasterClient);
            globalPlayer = FindObjectOfType<GlobalPlayer>();
            UID = globalPlayer.GetUID();
            PlayerName = globalPlayer.GetPlayerName();
            //vkAvavtar= globalPlayer.GetPlayerAvatar();
            friendsInfo = globalPlayer.friendsInfo;
            playerView.SetNameUID(UID, PlayerName);
            EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventAppear", this);
            //StatisticHandler.StartStats(UID,PlayerName);
        }
        else
        {
            if (charMan == null)
            {
                charMan = GetComponent<CharacteristicPlayerManager>();
                charMan.Init();
            }
        }
        DeathUpdate();
        Score.Assist = 0;
        Score.Death = 0;
        Score.Kill = 0;
        Score.RobotKill = 0;
        isStarted = false;
        respawnTimer = 0;
	}
	
	
	public void SetTeam(int intTeam){
		team = intTeam;	
        playerView.SetTeam(intTeam);
        

	}
	

	public void GameEnd(){
		if (currentPawn != null) {
			currentPawn.RequestKillMe ();
		}
		if(robotPawn!= null) {
			robotPawn.RequestKillMe ();
		}
	}
	public void Respawn(Pawn newPawn){
       

	}
	public int[] GetBuffs(){
		List <int> allbuff = new List<int>();
        allbuff.AddRange(PassiveSkillManager.instance.GetSkills(selected));
		allbuff.AddRange(ItemManager.instance.GetImplants());
        foreach (int stimId in activeSteampacks)
        {
            allbuff.Add(ItemManager.instance.GetBuffFromStim(stimId));
        }
     
		return allbuff.ToArray();
	}
	void Update(){
        if (!playerView.isMine)
        {
			return;
		}
		isDead =currentPawn==null||currentPawn.isDead;
		
		if(isDead){
		
			SendDelayedExternal();
			respawnTimer-=Time.deltaTime;
			
//			Debug.Log ("Dead");
			if(respawnTimer<=0&&isStarted){
				respawnTimer=respawnTime;
				currentPawn =PlayerManager.instance.SpawmPlayer(PlayerManager.instance.pawnName[selected],team,GetBuffs());
				currentPawn.ChangeDefaultWeapon(Choice._Player);
				ItemManager.instance.SaveItemForSlot();
				//PVPGameRule.instance.Spawn(team);
				AfterSpawnSetting(currentPawn,GetBuffs());
				prefabBot =PlayerManager.instance.avaibleBots[selectedBot];
				prefabGhostBot =PlayerManager.instance.ghostsBots[selectedBot];

			}
			robotTime =  charMan.GetFloatChar(CharacteristicList.PLAYER_JUGGER_TIME);
			canSpamBot=true;
		}else{
			Ray centerofScreen =Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			RaycastHit hitinfo;			
			if(CanUseJugger()&&robotPawn==null){
				robotTimer+=Time.deltaTime;
				
				if(robotTimer>robotTime&&canSpamBot){
                    if (!robotAnnonce)
                    {
                        robotAnnonce = true;
                        PlayerMainGui.instance.Annonce(AnnonceType.JUGGERREADY);
                    }
					if(InputManager.instance.GetButton("SpawnBot")){
						
						if(Physics.Raycast(centerofScreen, out hitinfo,50.0f,robotLayer)){
							if(ghostBot==null){
								GameObject ghostGameObj = Instantiate(prefabGhostBot,hitinfo.point,currentPawn.transform.rotation) as GameObject;
								ghostBot =ghostGameObj.GetComponent<GhostObject>();
							}
							ghostBot.myTransform.position = hitinfo.point;
							ghostBot.myTransform.rotation= Quaternion.LookRotation(-currentPawn.transform.forward);
							
							if(Physics.SphereCast(hitinfo.point+Vector3.up*ghostBot.size,ghostBot.size,Vector3.up,out hitinfo,100.0f)){
								//Debug.Log (hitinfo.collider);
								if(canSpawnBot){
									ghostBot.MakeBad();
									//Debug.Log (ghostBot.myRenderer.sharedMaterial.color);
								}
								canSpawnBot=false;
							}else{
								if(!canSpawnBot){
									
									ghostBot.MakeNormal();
									//Debug.Log (ghostBot.myRenderer.sharedMaterial.color);
								}
								canSpawnBot=true;
							}
						}
						
					}
                    if (InputManager.instance.GetButtonUp("SpawnBot"))
                    {
						if(ghostBot!=null&&canSpawnBot){
							Vector3 spamPoint =ghostBot.transform.position;
							spamPoint.y+= 30;
							robotPawn =(RobotPawn)PlayerManager.instance.SpawmBot(prefabBot,spamPoint,ghostBot.transform.rotation,activeSteampacks.ToArray());
							robotPawn.ChangeDefaultWeapon(selectedBot);
							//Debug.Log("robot spawn"+robotPawn);
                            AfterSpawnSetting(robotPawn, activeSteampacks.ToArray());

						
							canSpawnBot=false;							
						}
						//Debug.Log("destory chost");
						Destroy(ghostBot.gameObject);	
					}

				}
			}
				
				if(inBot){
					useTarget= null;
                    if (InputManager.instance.GetButtonDown("Use"))
                    {
						if(!robotPawn.isMutual){
							ExitBot();
                        }
                        else
                        {
                            DestroyBot();
                        }
					}
					
				}else {
					
						if(!inBot&&robotPawn!=null){
                            //Debug.Log(currentPawn.curLookTarget.gameObject +" "+ (currentPawn.myTransform.position - robotPawn.myTransform.position).sqrMagnitude);
							if(currentPawn.curLookTarget!=null&&currentPawn.curLookTarget.gameObject==robotPawn.gameObject&&(currentPawn.myTransform.position-robotPawn.myTransform.position).sqrMagnitude<SQUERED_RADIUS_OF_ACTION*2.0f){
								if(InputManager.instance.GetButtonDown("Use")){
									EnterBot();
								}
							}
						}

					if(currentPawn.curLookTarget!=null){
						if(InputManager.instance.GetButtonDown("Use")){
							useTarget = currentPawn.curLookTarget.GetComponent<UseObject>();

							if (useTarget != null && (currentPawn.myTransform.position - useTarget.myTransform.position).sqrMagnitude < SQUERED_RADIUS_OF_ACTION )
							{
								useTarget.Use(currentPawn);

							}
						
							RobotPawn robot = currentPawn.curLookTarget.GetComponent<RobotPawn>();
                            if (!inBot && robot != null && robot.isEmpty && robot.isMutual && (currentPawn.myTransform.position - robot.myTransform.position).sqrMagnitude < SQUERED_RADIUS_OF_ACTION * 2.0)
                            {
								EnterBot(robot);
							
							}
						}
						
					}else{
						useTarget= null;
					}
					//Debug.Log (currentPawn.curLookTarget);

				}
             if (InputManager.instance.GetButton("Fire2"))
             {
				currentPawn.ToggleAim(true);
				if(robotPawn!=null){
					robotPawn.ToggleAim(true);
				}
			}else{
				currentPawn.ToggleAim(false);
				if(robotPawn!=null){
					robotPawn.ToggleAim(false);
				}
			}
             if (InputManager.instance.GetButtonDown("Weapon1"))
             {
			
				if(inBot&&robotPawn!=null){
					robotPawn.ChangeWeapon (0);
				}else{
					currentPawn.ChangeWeapon (0);
				}
			}
             if (InputManager.instance.GetButtonDown("Weapon2"))
             {
				
				if(inBot&&robotPawn!=null){
					robotPawn.ChangeWeapon (1);
				}else{
					currentPawn.ChangeWeapon (1);
				}
			}
             if (InputManager.instance.GetButtonDown("Weapon3"))
             {
				
				if(inBot&&robotPawn!=null){
					robotPawn.ChangeWeapon (2);
				}else{
					currentPawn.ChangeWeapon (2);
				}
			}
             if (InputManager.instance.GetButtonDown("Suicide"))
             {
                 if (!inBot)
                 {
                     currentPawn.RequestKillMe();
                     if (robotPawn != null)
                     {
                         robotPawn.RequestKillMe();
                     }
                     PVPGameRule.instance.PlayerDeath();
                     DeathUpdate();
                     StatisticHandler.SendPlayerKillbyNPC(UID, PlayerName);
                 }
			}
		}
	
	}
    public void DeathUpdate()
    {

        killInRow = 0;
        Score.Death++;
        isStarted = false;
        isDead = true; 
        charMan.DeathUpdate();
        ItemManager.instance.RestartStimPack();
    }
	public void RobotDead(Player Killer){
		robotTimer=0;
        robotAnnonce = false;
		if (inBot) {
				inBot = false;
           
				currentPawn.Activate ();
				currentPawn.rigidbody.MovePosition (robotPawn.playerExitPositon.position);
				currentPawn.myTransform.rotation = robotPawn.playerExitPositon.rotation;
				currentPawn.transform.parent = null;
		}
	
	}
	public bool CanUseJugger(){
        return GameRule.instance.CanUseRobot;
    }
	
	
	public void PawnDead(Player Killer,Pawn killerPawn ){
	

		int pawnViewId =0;
		if (Killer != null) {
         
		
			PVPGameRule.instance.Kill (Killer.team);
			EventHolder.instance.FireEvent(typeof(LocalPlayerListener),"EventPawnDeadByPlayer",this);
		} else {
            PVPGameRule.instance.PlayerDeath();
			EventHolder.instance.FireEvent(typeof(LocalPlayerListener),"EventPawnDeadByAI",this);
		}
        if (killerPawn != null) {
            pawnViewId = killerPawn.foxView.viewID;                
        }

        DeathUpdate();
        if (Killer != null)
        {

            PlayerMainGui.instance.InitKillCam(Killer);
            if (isPlayerFriend(Killer.UID))
            {
                EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventKilledByFriend", this, Killer);
            }

            StatisticHandler.SendPlayerKillbyPlayer(UID, PlayerName, Killer.UID, Killer.PlayerName);
        }
        else
        {
            if (pawnViewId != 0)
            {
                Pawn killer = NetworkController.GetView(pawnViewId).GetComponent<Pawn>();
                PlayerMainGui.instance.InitKillCam(killer);
                StatisticHandler.SendPlayerKillbyNPC(UID, PlayerName);
            }
        }	

	}
	public void PawnKill(Player victim,Vector3 position){
		if (!playerView.isMine)
        {
			return;
		
		}
	
		if (victim != null) {
            //TODO: move text to config

            EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventPawnKillPlayer", this);
          

            if (isPlayerFriend(victim.UID))
            {
                EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventKilledAFriend", this, victim);
            }
            killInRow++;
            switch (killInRow)
            {
                case 1:
                    PlayerMainGui.instance.Annonce(AnnonceType.KILL);
                    break;
                case 2:
                    PlayerMainGui.instance.Annonce(AnnonceType.DOUBLEKILL);
                    break;
                case 3:
                    PlayerMainGui.instance.Annonce(AnnonceType.TRIPLIKILL);
                    break;
                case 4:
                    PlayerMainGui.instance.Annonce(AnnonceType.ULTRAKILL);
                    break;
                case 5:
                    PlayerMainGui.instance.Annonce(AnnonceType.MEGAKILL);
                    break;
                default:
                    PlayerMainGui.instance.Annonce(AnnonceType.RAMPAGE);
                    break;



            }
            if (!inBot)
            {
                Score.Kill++;
                robotTimer += charMan.GetFloatChar(CharacteristicList.PLAYER_JUGGER_KILL_BONUS);
            }
            else
            {
                Score.Kill++;
                Score.RobotKill++;
            }

		} else {
            //TODO: move text to config
            PlayerMainGui.instance.Annonce(AnnonceType.AIKILL);
            EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventPawnKillAI", this);
            StatisticHandler.SendPlayerKillNPC(UID, PlayerName);
		}


	}


	//Delayed external for that function that can destrupt user like VK wallpost
	public void SendDelayedExternal(){
		if(delayedExternalCallName!=""){
			if(!wasWallPost){
				Application.ExternalCall(delayedExternalCallName,delayedExternalCallData);
				delayedExternalCallName="";
				wasWallPost =true;
			}
		}
	}
		
	public void AchivmenUnlock(Achievement achv){
		PlayerMainGui.instance.AddMessage(achv.name+"\n"+achv.description,Vector3.zero,PlayerMainGui.MessageType.ACHIVEMENT);
		delayedExternalCallName ="AchivmenUnlock";
		delayedExternalCallData = achv.name + " " + achv.description;
		
	}
	
	public void DamagePawn(BaseDamage damage){
        if (!playerView.isMine)
        {
            return;
        }
		if (damage.sendMessage) {
	        PlayerMainGui.instance.AddMessage(damage.Damage.ToString("0.0"), damage.hitPosition, PlayerMainGui.MessageType.DMG_TEXT);
		}
	}
    public void DamagePawn(String damage, Vector3 position){
        if (!playerView.isMine)
        {
            return;
        }
		PlayerMainGui.instance.AddMessage(damage, position, PlayerMainGui.MessageType.DMG_TEXT);
			
	}
	public void PawnAssist(){
		Score.Assist++;
	}
	public void EnterBot(){

        if (robotPawn.isDead)
        {
            return;
        }
		inBot=true;
		currentPawn.DeActivate();
		currentPawn.transform.parent = robotPawn.transform;
		robotPawn.Activate ();
	}
	public void EnterBot(RobotPawn robot){
        if (robot.isDead)
        {
            return;
        }
		NetworkController.Instance.EnterRobotRequest(robot.foxView.viewID);
	}
	public void EnterBotSuccess(RobotPawn robot){
		inBot=true;
		robotPawn=robot;
        robotPawn.MySelfEnter();
		currentPawn.DeActivate();
		currentPawn.transform.parent = robotPawn.transform;
		robotPawn.Activate ();
	}
	public void ExitBot(){
		//robotTimer=robotTime;
		inBot= false;
		currentPawn.myTransform.parent = null;
		currentPawn.rigidbody.MovePosition (robotPawn.playerExitPositon.position);
		currentPawn.myTransform.rotation = robotPawn.playerExitPositon.rotation;
		currentPawn.Activate ();
		robotPawn.DeActivate();
		
	}
    public void DestroyBot()
    {
        inBot = false;
        currentPawn.myTransform.parent = null;
        currentPawn.rigidbody.MovePosition(robotPawn.playerExitPositon.position);
        currentPawn.myTransform.rotation = robotPawn.playerExitPositon.rotation;
        currentPawn.Activate();
        robotPawn.RequestKillMe();
    }
	public bool IsDead(){
		return isDead;

	}

	public PlayerMainGui.PlayerStats GetPlayerStats(){
		stats.robotTime = GetRobotTimer();
		Pawn curPawn = null;
		if (inBot) {
			curPawn = robotPawn;
		} else {

			curPawn= currentPawn;

		}
		if (curPawn != null) {
			stats.health = curPawn.health;
			SkillBehaviour skill = currentPawn.GetMainSkill();
		
                stats.skill = skill;
				
		
			if(curPawn.CurWeapon!=null){
			
				stats.gun  = curPawn.CurWeapon;
				stats.ammoInBag = curPawn.GetAmmoInBag ();
				stats.reloadTime = curPawn.CurWeapon.ReloadTimer();
                stats.pumpCoef = curPawn.CurWeapon.PumpCoef();
				
			}
            stats.jetPackCharge = curPawn.GetJetPackCharges() / curPawn.GetMaxJetPack() ;
		}

		
		return stats;

	}
    [Obsolete("Not used anymore", true)]
	public string IsMaster(){
        return "";
	}
	public float GetRobotTimer(){
		float result = robotTime- robotTimer;
		if (result < 0) {
			return 0;
		}
		return result;
	}
	public float GetRespawnTimer(){
		if (respawnTimer < 0) {
			return 0;
		}
		return respawnTimer;
	}
    
	
	
	public String GetName(){
		return PlayerName;
	}	
	public String GetUid(){
		return UID;
	}
	public Pawn GetCurrentPawn(){
		return currentPawn;
	}
	public Pawn GetRobot(){
		return robotPawn;
	}

	public Pawn GetActivePawn(){
		if(robotPawn!=null &&!currentPawn.isActive){
			return robotPawn;
		}else{
			return currentPawn;
		}
	
	}
	public void AISpawnSetting(Pawn pawn,   int[] buffs){
		pawn.player = this;
        pawn.team = this.team;
	}
    public void AfterSpawnSetting(Pawn pawn, int[] buffs)
    {

       PawnType type =PawnType.PAWN;
        //Debug.Log (viewid);
		
		if((pawn as RobotPawn)!=null){
		  type =PawnType.BOT;
		}
        switch (type)
        {
            case PawnType.PAWN:
                if (!playerView.isMine)
                {
					if(charMan==null){
						charMan = GetComponent<CharacteristicPlayerManager>();
                        charMan.Init();
					}
                    charMan.DeathUpdate();
                    activeSteampacks = new List<int>();


                  
                }
                foreach (int buff in buffs)
                {

                    ActualUseOfBuff(buff);
                }
                currentPawn = pawn;

                break;
            case PawnType.BOT:
                robotPawn = (RobotPawn)pawn;
                break;
        }


        pawn.player = this;
        pawn.team = this.team;
        pawn.Init();
	}
	
	

	
	
	//STIM PACK SECTION
	

    /// <summary>
    /// ACTIVATE StimPack
    /// </summary>
	public void ActivateStimpack(string id){
		int stimId;
		if(ItemManager.instance.TryUseStim(id, out stimId)){
			  activeSteampacks.Add(stimId);
		}
	
	}

    private void ActualUseOfBuff(int id)
    {
       // Debug.Log("BUFF ADd"+id);
        charMan.AddList( ItemManager.instance.GetBuff(id));
	}

   
	
	public List<CharacteristicToAdd>  GetCharacteristick(){
		return charMan.GetCharacteristick();	
	}
	
	/*[RPC]
	public void RPCReloadCharacteristic(){
		charMan.Reload();
	}*/



  
}