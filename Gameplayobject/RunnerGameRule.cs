using UnityEngine;
using System.Collections;

public class RunnerGameRule : GameRule {
    public Generation generator;

    protected int roomCnt;
    public override void StartGame()
    {
        generator = GetComponent<Generation>();
        generator.Next(10);

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
     public virtual void PlayerDeath()
    {

          isGameEnded = true;
    }
}
