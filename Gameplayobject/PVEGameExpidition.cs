using UnityEngine;
using System.Collections;

public class PVEGameExpidition : GameRule {

    public Pawn VipPawn;

    public bool isArrived;
    

	// Use this for initialization
	void Start () {
	
	}

    void Update()
    {
        
        if (isGameEnded)
        {

            restartTimer += Time.deltaTime;
            if (restartTimer > restartTime && !lvlChanging)
            {
                lvlChanging = true;
                FindObjectOfType<ServerHolder>().LoadNextMap();
            }
        }
        else if (!PhotonNetwork.isMasterClient)
        {
            return;
        }
        else if (IsGameEnded())
        {
            IsLvlChanging = true;
            isGameEnded = true;
            photonView.RPC("PVEGameEnded", PhotonTargets.All);
            //PhotonNetwork.automaticallySyncScene = true;
            //PhotonNetwork.LoadLevel(NextMap());

        }
        timer += Time.deltaTime;
    }

    public bool IsGameEnded()
    {
        if (!start)
        {
            return false;
        }
        return isArrived || VipPawn.health <= 0;
    }
    public override PlayerMainGui.GameStats GetStats()
    {
        PlayerMainGui.GameStats  stats = new PlayerMainGui.GameStats();
			stats.gameTime = gameTime-timer;
			stats.score = new int[]{(int)VipPawn.health, 0};
			stats.maxScore =maxScore;
			return stats;	
		
    }
    public void Arrived(){
        isArrived = true;
    }
    public override int Winner()
    {
        if (isArrived)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public void MoveOn()
    {
        curStage = 0;
    }
    public void Battle() {
        curStage = 1;
    
    }

    [RPC]
    public void PVEGameEnded()
    {
        //PhotonNetwork.automaticallySyncScene = true;

        isGameEnded = true;
        //Player player = GameObject.Find ("Player").GetComponent<Player> ();
        //player.GameEnd ();
        EventHolder.instance.FireEvent(typeof(GameListener), "EventTeamWin", Winner());
		
    }

}
