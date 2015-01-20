using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

public class AISwarm_PointAttack : AISwarm_PVPBots {
	public Transform[] GetPointOfInterest(int count)
    {
        if (pointOfInterest.Count < count) {
            return pointOfInterest.ToArray();
        }
        Transform[] returnTransform = new Transform[count];
        //	Debug.Log ("patrolPoint" +count);
        ReloadList();
        for (int i = 0; i < count; i++)
        {
            int randKey = (int)(UnityEngine.Random.value * avaiblePoints.Count);
            returnTransform[i] = avaiblePoints[randKey];
            //	Debug.Log (returnTransform[i]);
            avaiblePoints.RemoveAt(randKey);
            ReloadList();
        }
        return returnTransform;
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
