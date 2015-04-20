using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

public class AISwarm_PointAttack : AISwarm_PVPBots {

    public int secondTeamSpawnCount;

    public override void SendData(ISFSObject swarmSend)
    {
        base.SendData(swarmSend);
        swarmSend.PutInt("secondTeamSpawnCount", secondTeamSpawnCount);


    }
	
	public override Transform[] GetPointOfInterest(int count,int team)
    {
         Transform[] returnTransform = new Transform[pointOfInterest.Count];
		 if(team==1){
		 
			for (int i = 0; i < pointOfInterest.Count; i++)
			{
			 
				returnTransform[i] = avaiblePoints[i];
				//	Debug.Log (returnTransform[i]);
			}
		}else{
			for (int i = 0; i < pointOfInterest.Count; i++)
			{
			 
				returnTransform[i] = avaiblePoints[pointOfInterest.Count-1-i];
				//	Debug.Log (returnTransform[i]);
			}
		
		}
        return returnTransform; 
    }

}
