using UnityEngine;
using System.Collections;

using System;
using System.Reflection;
using Sfs2X.Entities.Data;
using nstuff.juggerfall.extension.models;


public class FoxView : MonoBehaviour {
	public MonoBehaviour observed;
	
	private static string methodName  =  "UDPSerialization";
	
    private int ownerId;
    
    private int  subId;

	public bool isMine;
	
	public bool isSceneView;
	
	public int viewID
    {
        get { return ownerId * NetworkController.MAX_VIEW_IDS + subId; }
        set
        {
            // if ID was 0 for an awakened PhotonView, the view should add itself into the networkingPeer.photonViewList after setup
			

            this.ownerId = value / NetworkController.MAX_VIEW_IDS;

            this.subId = value % NetworkController.MAX_VIEW_IDS;
            isSceneView = ownerId == 0;

            if (isSceneView)
            {
                isMine = NetworkController.smartFox.MySelf.ContainsVariable("Master") && NetworkController.smartFox.MySelf.GetVariable("Master").GetBoolValue();
            }
            else
            {
                isMine = NetworkController.smartFox.MySelf.Id == ownerId;
            }
            
            
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
	public void StartShoot(string animName){
		NetworkController.Instance.PawnChangeShootAnimStateRequest( viewID,  animName,true);
	}
	public void StopShoot(string animName){
		NetworkController.Instance.PawnChangeShootAnimStateRequest( viewID,  animName,false);
	}
	public void StartKick(int i){
		NetworkController.Instance.PawnKickRequest( viewID,i,true);
	}
	public void StopKick(){
		NetworkController.Instance.PawnKickRequest(viewID,0,false);
	}
	public void Activate(){
		NetworkController.Instance.PawnActiveStateRequest(viewID,true);
	}
	public void DeActivate(){
		NetworkController.Instance.PawnActiveStateRequest(viewID,false);
	}

    public void PawnUpdate(PawnModel pawn)
    {
		NetworkController.Instance.PawnUpdateRequest(pawn);
	}
	public void Taunt(string name){
		NetworkController.Instance.PawnTauntRequest(viewID, name);
	}
	public void KnockOut(){
		NetworkController.Instance.PawnKnockOutRequest(viewID);
	}
	public void Destroy(){
		NetworkController.Instance.DeleteViewRequest(viewID);
	}
	
    public void SendShoot(Vector3 position, Quaternion rotation, float power, float range, int viewId, int projId)
    {
		
		ISFSObject data = new SFSObject();
     
     	data.PutClass("position", position);
		data.PutClass("direction", rotation);
		data.PutFloat("power", power);
		data.PutFloat("range", range);
		data.PutFloat("viewId", viewId);
		data.PutFloat("projId", projId);	
		data.PutFloat("projId", projId);
        data.PutDouble("timeShoot", TimeManager.Instance.NetworkTime);
		data.PutFloat("id", viewID);	
		NetworkController.Instance.WeaponShootRequest(data);
	}
}
