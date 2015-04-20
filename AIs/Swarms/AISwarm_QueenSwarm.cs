using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;

public class AISwarm_QueenSwarm : AISwarm
{
	public QueenPawn queen;
	public string queenPawn;
    public int maxSwarmling;

    public override void SendData(ISFSObject swarmSend)
    {
        base.SendData(swarmSend);
        swarmSend.PutUtfString("queen", queenPawn);
        swarmSend.PutInt("maxSwarmling", maxSwarmling);

    }
}
