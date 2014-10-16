using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

public class AISwarm_PVPBots : AISwarm {

    public int maxBots;
    public override void SendData(ISFSObject swarmSend)
    {
        base.SendData(swarmSend);
        swarmSend.PutInt("maxBots", maxBots);
        

    }
}
