using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SinglePlayerGameRule : GameRule {
    PlayerMainGui.GameStats stats = new PlayerMainGui.GameStats();
    public override void GameEnded()
    {
        //PhotonNetwork.automaticallySyncScene = true;

        isGameEnded = true;
        //Player player = GameObject.Find ("Player").GetComponent<Player> ();
        //player.GameEnd ();
        EventHolder.instance.FireEvent(typeof(GameListener), "EventTeamWin", Winner());
        GlobalPlayer.instance.MathcEnd();
        Player.localPlayer.GameEnd();
        ItemManager.instance.MatchEnd();
        List<Pawn> pawns = PlayerManager.instance.FindAllPawn();
        foreach (Pawn pawn in pawns)
        {
            pawn.gameEnded();
        }

    }
    public override bool IsPractice()
    {
        return false;
    }
    void Update()
    {
        base.Update();
    }
    public override PlayerMainGui.GameStats GetStats()
    {
      
        stats.gameTime = gameTime - timer;
        stats.score = new int[2]{0,0};
        stats.maxScore = maxScore;
        return stats;	
    }
}
