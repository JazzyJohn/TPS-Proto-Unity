using UnityEngine;
using System.Collections;
using nstuff.juggerfall.extension.models;
using Sfs2X.Entities.Data;
using System;
using System.Collections.Generic;

public class GameRule : MonoBehaviour {
	static public bool IsLvlChanging=false;

    protected int[] teamScore;

    protected float timer = 0.0f;

    protected float restartTimer = 0.0f;

    public int maxScore;

    public float gameTime;

    public float restartTime = 10.0f;

    public bool isGameEnded = false;

    public bool lvlChanging = false;

    public bool CanUseRobot = true;

    [HideInInspector]
    public MUSIC_STAGE curStage = MUSIC_STAGE.BATLLE;

    public bool start = false;

    public float DeathY= -50f;

    protected void Awake()
    {
        curStage = MUSIC_STAGE.BATLLE;
        isGameEnded = false;
        lvlChanging = false;
       // PhotonNetwork.isMessageQueueRunning = true;
       
    }
	// Use this for initialization
	void Start () {
	
	}
	
        public void Annonce() {
            if (teamScore[0] > teamScore[1] && type!=AnnonceType.INTERGRALEAD)
            {
                type = AnnonceType.INTERGRALEAD;
                PlayerMainGui.instance.Annonce(type);
            }
            if (teamScore[0] < teamScore[1] && type != AnnonceType.RESLEAD)
            {
                type = AnnonceType.RESLEAD;
                PlayerMainGui.instance.Annonce(type);
            }
        }
	// Update is called once per frame
	protected void Update () {
        timer += Time.deltaTime;
        if (isGameEnded)
        {
            restartTimer += Time.deltaTime;
        }
        #if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.L))
                {
                    GameEnded();
                }
        #endif
  
	}

    public virtual void GameEnded()
    {
        //PhotonNetwork.automaticallySyncScene = true;

        isGameEnded = true;
        //Player player = GameObject.Find ("Player").GetComponent<Player> ();
        //player.GameEnd ();
        EventHolder.instance.FireEvent(typeof(GameListener), "EventTeamWin", Winner());
        GlobalPlayer.instance.MathcEnd();
        Player.localPlayer.GameEnd();
        ItemManager.instance.RemoveOldAndExpired();
        List<Pawn> pawns = PlayerManager.instance.FindAllPawn();
        foreach (Pawn pawn in pawns)
        {
            pawn.gameEnded();
        }

    }

    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static GameRule s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static GameRule instance
    {
        get
        {
            if (Application.loadedLevel !=0 && s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(GameRule)) as GameRule;
            }

            return s_Instance;
        }
    }

    public virtual void Kill(int team)
    {
     
    }
  
    //For ticket system like in Battlefield
    public virtual void Spawn(int team)
    {
       
    }

    public virtual void PlayerDeath()
    {

    }
    public virtual PlayerMainGui.GameStats GetStats()
    {
        return null;
    }

    public virtual int Winner()
    {
        return 0;
    }
    public float GetRestartTimer()
    {
        return restartTime - restartTimer;

    }
    public virtual void StartGame()
    {
        Debug.Log("Start Game");
        start = true;
        if (PathfindingEngine.Instance != null)
        {
            PathfindingEngine.Instance.GenerateStaticMap();
        }
        IsLvlChanging = false;
        AIDirector director = FindObjectOfType<AIDirector>();
        if (director != null)
        {
            FindObjectOfType<AIDirector>().StartDirector();

        }
    }

    public virtual void SetFromModel(GameRuleModel model)
    {

    }
	
	public virtual void ReadMasterInfo(ISFSObject dt){
		
	}

    public virtual void MoveOn()
    {
       
    }
	
	public virtual bool IsPractice(){
        return false;
	}
	
	public virtual string GetWinnerText(){
		string text = TextGenerator.instance.GetSimpleText("WallPostWinner");
        text = String.Format(text, (Player.localPlayer.Score.AIKill + Player.localPlayer.Score.Kill));
		return text;
	}
	
	public virtual void PointChangeOwner(int team){
	
	}
	public virtual void PointChangeConquare(int team){
	
	}
	public virtual bool IsGameEnded(){
			if(timer>gameTime){
				return true;
			}
			return false;
		}
}
