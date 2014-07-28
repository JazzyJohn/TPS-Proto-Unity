using UnityEngine;
using System.Collections;

using System;
using SmartFoxClientAPI;
using SmartFoxClientAPI.Data


public class FoxView : MonoBehaviour {
	public MonoBehaviour observed;
	
	private static string methodName  =  "UDPSerialization";
	
	public int viewID
    {
        get { return ownerId * NetworkController.MAX_VIEW_IDS + subId; }
        set
        {
            // if ID was 0 for an awakened PhotonView, the view should add itself into the networkingPeer.photonViewList after setup
            bool viewMustRegister = this.didAwake && this.subId == 0;


            this.ownerId = value / NetworkController.MAX_VIEW_IDS;

            this.subId = value % NetworkController.MAX_VIEW_IDS;

            if (viewMustRegister)
            {
               //TODO: SHOW TAHT THIS ID IS OCupied;
            }
            
        }
    }

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


}
