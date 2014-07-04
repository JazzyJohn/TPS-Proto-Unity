using UnityEngine;
using System.Collections;

public class GameRule : MonoBehaviour {
	static public bool IsLvlChanging=false;


    protected float timer = 0.0f;

    protected float restartTimer = 0.0f;

    protected int maxScore;

    public float gameTime;

    public float restartTime = 10.0f;

    public bool isGameEnded = false;

    public bool lvlChanging = false;

    public PhotonView photonView;

    public int curStage = 0;

    public bool start = false;

    protected void Awake()
    {
        isGameEnded = false;
        lvlChanging = false;
       // PhotonNetwork.isMessageQueueRunning = true;
        photonView = GetComponent<PhotonView>();
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static GameRule s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static GameRule instance
    {
        get
        {
            if (s_Instance == null)
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
        start = true;
        if (PathfindingEngine.Instance != null)
        {
            PathfindingEngine.Instance.GenerateStaticMap();
        }
        IsLvlChanging = false;
        FindObjectOfType<AIDirector>().StartDirector();
    }
}
