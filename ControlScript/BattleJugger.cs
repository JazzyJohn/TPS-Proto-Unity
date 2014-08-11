using UnityEngine;
using System.Collections;
using nstuff.juggerfall.extension.models;

public class BattleJugger : Pawn {

    public void Start()
    {
        base.Start();
        SetStarterTeam();
    }

    public override void NetUpdate(PawnModel pawn)
    {
      
        if (team != pawn.team)
        {
            team = pawn.team;
            SetStarterTeam();
        }
        base.NetUpdate(pawn);       
       
    }
    public void SetStarterTeam()
    {
        foreach (SpawnPoint point in GetComponentsInChildren<SpawnPoint>())
        {
            point.team = team;
        }

    }
}
