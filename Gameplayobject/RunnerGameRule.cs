using UnityEngine;
using System.Collections;

public class RunnerGameRule : GameRule {
    public Generation generator;
    public int StartRoomCnt;
    protected int roomCnt;
    public override void StartGame()
    {
        generator = GetComponent<Generation>();
        generator.Next(StartRoomCnt);

        base.StartGame();
    }
    public override PlayerMainGui.GameStats GetStats()
    {
        PlayerMainGui.GameStats stats = new PlayerMainGui.GameStats();
        stats.gameTime = gameTime - timer;
        stats.score = new int[] { (int)roomCnt, 0 };
        stats.maxScore = maxScore;
        return stats;

    }
    public void NextRoom()
    {
        roomCnt++;
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
        
    }
     public override void PlayerDeath()
    {
        Debug.Log("Palyer Death");
		EventHolder.instance.FireEvent(typeof(GameListener), "EventRestart", Winner());
        isGameEnded = true;
    }
}
