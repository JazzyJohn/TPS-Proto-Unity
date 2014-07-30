using UnityEngine;
using System.Collections;

using System;
using System.Reflection;
using Sfs2X.Entities.Data;


public class FoxView : MonoBehaviour {
	public MonoBehaviour observed;
	
	private static string methodName  =  "UDPSerialization";
	
    private int ownerId;
    
    private int  subId;

	public int viewID
    {
        get { return ownerId * NetworkController.MAX_VIEW_IDS + subId; }
        set
        {
            // if ID was 0 for an awakened PhotonView, the view should add itself into the networkingPeer.photonViewList after setup
          


            this.ownerId = value / NetworkController.MAX_VIEW_IDS;

            this.subId = value % NetworkController.MAX_VIEW_IDS;

            
            
        }
    }
    
    /*
	public void UDP(SFSObject data)
    {
		Type type	=observed.GetType();
		MethodInfo theMethod = type.GetMethod (methodName);
		theMethod.Invoke(observed,data);
	}
	public void RPC(string method,params object[] theObjects){
		MonoBehaviour[] mbComponents = GetComponents<MonoBehaviour>();    // NOTE: we could possibly also cache MonoBehaviours per view?!
        for (int componentsIndex = 0; componentsIndex < mbComponents.Length; componentsIndex++)
        {
            MonoBehaviour monob = mbComponents[componentsIndex];
            if (monob == null)
            {
              
                continue;
            }

            Type type = monob.GetType();
			MethodInfo theMethod = type.GetMethod (method);
			if(theMethod!=null){
				theMethod.Invoke(monob,theObjects);
				return;
			}
			
		}
		
	
	}

    */

    public  void SetInit(ISFSObject data)
    {
        TransformWrite(transform, data);
        data.PutFloat("id",viewID);
    }
    public static void TransformWrite(Transform transform, ISFSObject data)
    {
        data.PutFloat("px", transform.position.x);
        data.PutFloat("py", transform.position.y);
        data.PutFloat("pz", transform.position.z);

        data.PutFloat("qx", transform.rotation.x);
        data.PutFloat("qy", transform.rotation.y);
        data.PutFloat("qz", transform.rotation.z);
        data.PutFloat("qw", transform.rotation.w);
    }
}
