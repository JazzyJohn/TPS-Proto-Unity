using UnityEngine;
using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities;
using System.Collections;

public enum PawnType{PAWN,BOT};

//BIT MASK 
public enum GameClassEnum{ENGINEER,ASSAULT,SCOUT,MEDIC,ANY,ROBOTHEAVY,ROBOTMEDIUM,ROBOTLIGHT,ANYROBOT};




public class Player : MonoBehaviour {
    private static Player _localPlayer;
	public static Player localPlayer {
        
          get
        {
              if(_localPlayer==null){
                 _localPlayer= NetworkController.Instance.GetLocalPlayer();
              }
               return _localPlayer;
        }


        set
        {

            _localPlayer = value;

        }

        
    }
    public ActivateReward selectedReward;

    public ActivateReward[] rewards;

    public int rewardRating;
	
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

    public float timeSend = 0;

	public Texture2D vkAvavtar;

  
	 // Declare your serializable data.
	[System.Serializable]
	public class PlayerScore
		{
		public int Kill =0;
        public int rating = 0;
		public int Death=0;
		public int Assist=0;
		public int RobotKill=0;
        public int AIKill = 0;
        public int WaveCnt=0;
        public int Lvl = 1;
		public void Reset(){
            rating = 0;
			Assist = 0;
			AIKill = 0;
			Death = 0;
			Kill = 0;
			RobotKill = 0;
			WaveCnt=0;
		}
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
	
	private bool wasWallPost = false;

    bool winnerWallPost = false;

	public const float SQUERED_RADIUS_OF_ACTION = 16.0f;
	
	public static float BASE_SHOWTIME =3.0f;
	
	public static float BASE_MARKTIME =5.0f;
	
	public static float BASE_MINIMAPSHOWTIME =5.0f;
	
	public GlobalPlayer globalPlayer;

    private bool robotAnnonce =false;
    
    public int killInRow = 0;

    public int aiKillInRow = 0;
	
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
	public int GetLvl(){
		if(isMine){
			return LevelingManager.instance.playerLvl;
		}else{
			User user =NetworkController.Instance.GetUser(playerView.GetId());
			if(user.ContainsVariable("lvl") ){
					return user.GetVariable("lvl").	GetIntValue();
			}
			return 1;
		}
	}
	void Init(){
		
		PlayerManager.instance.addPlayer(this);
       
        if (playerView.isMine)
        {
			localPlayer= this;
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
            if (isMine)
            {
                FindObjectOfType<ServerHolder>().AfterPlayerCreate();
            }
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
        Score.Reset();
       
        respawnTimer = 0;
	}
	
	
	public void SetTeam(int intTeam){
		team = intTeam;	
        playerView.SetTeam(intTeam);
        

	}
    void OnLevelWasLoaded(int level)
    {
        Score.Reset();
    }
   
	public void GameEnd(){
        if (GameRule.instance.Winner() == team)
        {
            if (!winnerWallPost)
            {
                winnerWallPost = true;
                String text = GameRule.instance.GetWinnerText();

                ScreenShootManager.instance.TakeScreenshotToWall(text);
            }
            else
            {
                winnerWallPost = false;
            }
         
          
        }
        Choice._Team = -1;
       
	}
	public void Respawn(Pawn newPawn){
       

	}
	public virtual Pawn GetNewPawn(){
			return PlayerManager.instance.SpawmPlayer(PlayerManager.instance.pawnName[selected],team,GetBuffs());
	}
    public virtual String GetNewBot()
    {
        return PlayerManager.instance.avaibleBots[Choice._Robot];
    }
    public virtual void SpawnBot(Vector3 position, Quaternion rotation){
        robotPawn = (RobotPawn)PlayerManager.instance.SpawmBot(GetNewBot(), position, rotation, new int[0]);
        robotPawn.AfterAwake();
        AfterSpawnSetting(robotPawn, new int[0]);
        robotPawn.ChangeDefaultWeapon(Choice._Player);
  
        currentPawn.SpeedInit();
      
        NetworkController.Instance.EndPawnSpawnRequest();
    }
	void OnDestroy(){
		if (playerView.isMine)
        {
			localPlayer= null;
			
		}
	}
	public int[] GetBuffs(){
		List <int> allbuff = new List<int>();
        allbuff.AddRange(PassiveSkillManager.instance.GetSkills(selected));
		allbuff.AddRange(PremiumManager.instance.GetSkills());
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
			if(respawnTimer<=0&&isStarted&&!GameRule.instance.isGameEnded){
                isStarted = false;
				respawnTimer=respawnTime;
				currentPawn = GetNewPawn();
             
				ItemManager.instance.SaveItemForSlot();
				//PVPGameRule.instance.Spawn(team);
				AfterSpawnSetting(currentPawn,GetBuffs());
                PlayerMainGui.instance.ActivateStim(activeSteampacks);
                activeSteampacks.Clear();
                currentPawn.ChangeDefaultWeapon(Choice._Player);
               
                currentPawn.SpeedInit();
                NetworkController.Instance.EndPawnSpawnRequest();
			}
			robotTime =  charMan.GetFloatChar(CharacteristicList.PLAYER_JUGGER_TIME);
			canSpamBot=true;
		}else{
			Ray centerofScreen =Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			RaycastHit hitinfo;			
			
				
				if(inBot){
					useTarget= null;
                  					
				}else {
					
						if(robotPawn!=null){
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
						
							RobotPawn robot = currentPawn.curLookTarget.root.GetComponent<RobotPawn>();
                            if (!inBot && robot != null && ((robot.isEmpty && robot.isMutual) || robotPawn==robot) && (currentPawn.myTransform.position - robot.myTransform.position).sqrMagnitude < SQUERED_RADIUS_OF_ACTION * 2.0)
                            {
                                if (robot.isMutual)
                                    EnterBot(robot);
                                else
                                    EnterBot();
							
							}
						}
						
					}else{
						useTarget= null;
					}
					//Debug.Log (currentPawn.curLookTarget);

				}
            ButtonControll();
		}
        if (playerView.isMine)
        {
            timeSend += Time.deltaTime;
            if(timeSend>1.0f){
                timeSend = 0.0f;
                playerView.SendStats();
            }
        }
	
	}

    public virtual void  ButtonControll(){
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
                    Debug.Log("Weapon");
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

          
			  if (InputManager.instance.GetButtonDown("Mark"))
             {
				
		 
				if(	GetActivePawn().curLookTarget!=null){
					Pawn target = GetActivePawn().curLookTarget.root.GetComponent<Pawn>();
					if(target!=null){
						target.InitMark();
					}
				}
			 }
             if (InputManager.instance.GetButtonDown("Suicide"))
             {
               
                     currentPawn.RequestKillMe();
                   
                     PVPGameRule.instance.PlayerDeath();
                     DeathUpdate();
                    // StatisticHandler.SendPlayerKillbyNPC(UID, PlayerName);
               
			}
            for (int i = 0; i < rewards.Length; i++)
            {
                if (InputManager.instance.GetButtonDown("Reward" + (i + 1)) && rewards[i].State()!=RewardState.NO_ACTIVE)
                {

                    if (selectedReward == rewards[i])
                    {
                        selectedReward.Activate(currentPawn);
                        selectedReward = null;
                        if (i == rewards.Length-1)
                        {
                            ResetRewards();
                        }
                    }
                    else
                    {
                        if (selectedReward != null)
                        {
                            selectedReward.Deselect(currentPawn);
                        }
                        selectedReward = rewards[i];
                        selectedReward.Select(currentPawn);

                    }
                }
            }



    }
    public void DeathUpdate()
    {

        killInRow = 0;
        Score.Death++;
      
        isDead = true; 
        charMan.DeathUpdate();
        ItemManager.instance.RestartStimPack();
		ItemManager.instance.SendChargeData();
    }
	public void RobotDead(Player Killer){
		inBot = false;
	
	}
	public bool CanUseJugger(){
        return GameRule.instance.CanUseRobot;
    }

    public void AddRating(int rating)
    {
        Score.rating += rating;
        rewardRating += rating;
//        Debug.Log("ADD REWARD " + rating);
        CheckReward();
    }

    public void CheckReward()
    {
        foreach (ActivateReward reward in rewards)
        {
            reward.TryOpen(Score.rating);
        }
    }
    public void WeaponChanged()
    {
        if (selectedReward != null)
        {
            selectedReward.Deselect(currentPawn);
            selectedReward = null;
        }
    }
    public void ResetRewards()
    {
        rewardRating = 0;
        foreach (ActivateReward reward in rewards)
        {
            reward.Reset();
        }
    }
	public void PawnDead(Player Killer,Pawn killerPawn,KillInfo killinfo ){


        int pawnViewId = 0;
        isDead = true;
	
		if (Killer != null) {

            PlayerMainGui.instance.KillerAnnonce(Killer.PlayerName, PlayerName, killinfo.weaponId);
			PVPGameRule.instance.Kill (Killer.team);
            EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventPawnDeadByPlayer", this, killinfo);
		} else {
            PlayerMainGui.instance.KillerAnnonce(killerPawn.publicName, PlayerName, killinfo.weaponId);
            PVPGameRule.instance.PlayerDeath();
			EventHolder.instance.FireEvent(typeof(LocalPlayerListener),"EventPawnDeadByAI",this);
		}
        if (killerPawn != null) {
            pawnViewId = killerPawn.foxView.viewID;                
        }
        GA.API.Design.NewEvent("Game:Player:AiKillInRow:", aiKillInRow);
        DeathUpdate();
        if (Killer != null)
        {

            PlayerMainGui.instance.InitKillCam(Killer);
            if (isPlayerFriend(Killer.UID))
            {
                EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventKilledByFriend", this, Killer);
            }

            //StatisticHandler.SendPlayerKillbyPlayer(UID, PlayerName, Killer.UID, Killer.PlayerName);
        }
        else
        {
            if (pawnViewId != 0)
            {
                Pawn killer = NetworkController.GetView(pawnViewId).GetComponent<Pawn>();
                PlayerMainGui.instance.InitKillCam(killer);
               // StatisticHandler.SendPlayerKillbyNPC(UID, PlayerName);
            }
        }	

	}
    public void JuggerKill(Pawn deadPawn,  Player victim, Vector3 position, KillInfo killinfo)
    {
		if (!playerView.isMine)
        {
			return;
		
		}
	
		if (victim != null) {
            EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventJuggerKill", this, killinfo);
		}
	}
	public virtual void PawnKillAssist(Pawn deadPawn,Player victim)
    {
		Score.Assist++;
		if (victim != null) {
			
            EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventPawnKillAssistPlayer", this);
		}else{
			EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventPawnKillAssistAI", this);
		}
		 
	}
    public virtual void PawnKill(Pawn deadPawn,Player victim, Vector3 position, KillInfo killinfo)
    {
		if (!playerView.isMine)
        {
			return;
		
		}
        if (GetCurrentPawn() != null)
        {
            GetCurrentPawn().KillEnemy();
        }
        AnnonceAddType addtype = AnnonceAddType.NONE;
        if (killinfo.isHeadShoot)
        {
            addtype = AnnonceAddType.HEADSHOT;
        }
		if (victim != null) {
            //TODO: move text to config
            PlayerMainGui.instance.KillerAnnonce(PlayerName, victim.PlayerName, killinfo.weaponId);
            EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventPawnKillPlayer", this, killinfo);
          

            if (isPlayerFriend(victim.UID))
            {
                EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventKilledAFriend", this, victim, killinfo);
            }
            String victimName = victim.PlayerName;
           
            killInRow++;
            switch (killInRow)
            {
                case 1:
                    PlayerMainGui.instance.Annonce(AnnonceType.KILL, addtype, victimName);
                    break;
                case 2:
                    PlayerMainGui.instance.Annonce(AnnonceType.DOUBLEKILL, addtype, victimName);
                    break;
                case 3:
                    PlayerMainGui.instance.Annonce(AnnonceType.TRIPLIKILL, addtype, victimName);
                    break;
                case 4:
                    PlayerMainGui.instance.Annonce(AnnonceType.ULTRAKILL, addtype, victimName);
                    break;
                case 5:
                    PlayerMainGui.instance.Annonce(AnnonceType.MEGAKILL, addtype, victimName);
                    break;
                default:
                    PlayerMainGui.instance.Annonce(AnnonceType.RAMPAGE, addtype, victimName);
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
            aiKillInRow++;
            Score.AIKill++;
            //TODO: move text to config
            PlayerMainGui.instance.Annonce(AnnonceType.AIKILL,addtype, deadPawn.publicName);
            EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventPawnKillAI", this, killinfo);
           // StatisticHandler.SendPlayerKillNPC(UID, PlayerName);
            PlayerMainGui.instance.KillerAnnonce(PlayerName, deadPawn.publicName, killinfo.weaponId);
		}


	}

	public void ShowDamageIndicator(Vector3 position){
		PlayerMainGui.instance. ShowDamageIndicator( position);
		
	}
	public void CrosshairType(CrosshairColor color){
		PlayerMainGui.instance. CrosshairType( color);
		
	
	}
	//Delayed external for that function that can destrupt user like VK wallpost
	public void SendDelayedExternal(){
		if(delayedExternalCallName!=""){
			if(!wasWallPost){
                StartCoroutine(_SendDelayedExternal(delayedExternalCallName, delayedExternalCallData));
				
				delayedExternalCallName="";
				wasWallPost =true;
			}
		}
	}

    public IEnumerator _SendDelayedExternal(string name, string data)
    {
        yield return new WaitForSeconds(2.0f);
        Application.ExternalCall(name, data);

    
    }
	public void AchivmenUnlock(Achievement achv){
		if(achv.type ==AchievementType.ACHIEVEMENT){
            PlayerMainGui.instance.AddMessage(achv.name + "\n" + achv.description, Vector3.zero, PlayerMainGui.MessageType.ACHIVEMENT);
            delayedExternalCallName = "AchivmenUnlock";
            string text = TextGenerator.instance.GetSimpleText("AchivmenUnlock");
            delayedExternalCallData = text + achv.name + ": " + achv.description;
            GUIHelper.Notify(text,achv.description, achv.textureIcon);
		}else{
            PlayerMainGui.instance.AddMessage(achv.description, Vector3.zero, PlayerMainGui.MessageType.ACHIVEMENT);
            delayedExternalCallName = "AchivmenUnlock";
            string text =TextGenerator.instance.GetSimpleText("TaskUnlock");
            delayedExternalCallData = text + achv.description;
            GUIHelper.Notify(text, achv.description, achv.textureIcon);
			
		}
		
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
        robotPawn.MySelfEnter(currentPawn);
		currentPawn.DeActivate();
		
		EventHolder.instance.FireEvent(typeof(LocalPlayerListener), "EventJuggerTake", this);
		robotPawn.Activate ();
	}
	public void ExitBot(){
		//robotTimer=robotTime;
		inBot= false;
		currentPawn.myTransform.parent = null;
	
		currentPawn.Activate ();
		robotPawn.DeActivate();
		
	}
    public void DestroyBot()
    {
        inBot = false;
        currentPawn.myTransform.parent = null;
        currentPawn.Activate();
        robotPawn.RequestKillMe();
    }
	public bool IsDead(){
		return isDead;

	}

	public PlayerMainGui.PlayerStats GetPlayerStats(){
		stats.robotTime = GetRobotTimer();
		Pawn curPawn = null;
		
	    curPawn= currentPawn;

		if (curPawn != null) {
			stats.health = curPawn.health;
			SkillBehaviour skill = currentPawn.GetMainSkill();
		
                stats.skill = skill;
				
		
			if(curPawn.CurWeapon!=null){
			
				stats.gun  = curPawn.GetGun();
                stats.grenade = curPawn.GetGrenade();
                stats.armor = curPawn.GetArmor();
				stats.ammoInBag = curPawn.GetAmmoInBag ();
                if (stats.grenade != null)
                    stats.grenadeAmount = curPawn.GetGrenadeInBag() + (int)stats.grenade.curAmmo;
                else
                    stats.grenadeAmount = 0;
                stats.reloadTime = stats.gun.ReloadTimer();
                stats.pumpCoef = stats.gun.PumpCoef();
				
			}
          
            stats.jetPackCharge = curPawn.GetJetPackCharges() / curPawn.GetMaxJetPack();
		}
        stats.rewards = new RewardState[rewards.Length];
        for (int i = 0; i < rewards.Length; i++)
        {
            stats.rewards[i] = rewards[i].State();
        }

        if (rewards.Length != 0)
        {
            stats.rating = (float)rewardRating / (float)rewards[rewards.Length - 1].cost;
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
    public RobotPawn GetRobot()
    {
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
    public void CleanSpawned(Pawn pawn)
    {
          PawnType type =PawnType.PAWN;
        //Debug.Log (viewid);
		
		if((pawn as RobotPawn)!=null){
		  type =PawnType.BOT;
		}
        switch (type)
        {
            case PawnType.PAWN:
                if (currentPawn!=null)
                    Destroy(currentPawn.gameObject);
                break;
            case PawnType.BOT:
                if (robotPawn != null)
                    Destroy(robotPawn.gameObject);
                break;
        }

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
            AchievementManager.instance.UnEvnetAchive(ParamLibrary.PARAM_STIM_PACK,1.0f);
			activeSteampacks.Add(stimId);
		}
	
	}

    private void ActualUseOfBuff(int id)
    {
       ///Debug.Log("BUFF ADd"+id);
        charMan.AddList( ItemManager.instance.GetBuff(id));
	}

   
	
	public List<CharacteristicToAdd>  GetCharacteristick(){
		return charMan.GetCharacteristick();	
	}
	
	/*[RPC]
	public void RPCReloadCharacteristic(){
		charMan.Reload();
	}*/

	//NON SHOT SKILLS
	
	
	public float GetShowTime(Player player){
		return BASE_SHOWTIME;
	}
	public float GetMarkTime(Player player){
		return BASE_MARKTIME;
	}
	public float GetMiniMapShowTime(Player player){
		return BASE_MINIMAPSHOWTIME;
	}
}