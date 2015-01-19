using UnityEngine;
using System.Collections;

using System;
using System.Reflection;
using Sfs2X.Entities.Data;
using nstuff.juggerfall.extension.models;


public class FoxView : MonoBehaviour {
    public static int SCENE_OWNER_ID = -2;

	private static ISFSArray sendProj =null;

	public Pawn pawn;
	
	public BaseWeapon weapon;
	
	public ConquestPoint conquestPoint;
	
    private int ownerId;
    
    private int  subId;

    private int realId;

	public bool isMine;
	
	public bool isSceneView;

    public bool preLoad;

    public bool needRegister=true;

    public int DebufViewID;
	
	public int viewID
    {
        get {
            return realId;
           
        }
        set
        {
            // if ID was 0 for an awakened PhotonView, the view should add itself into the networkingPeer.photonViewList after setup
            realId = value;

            this.ownerId = value / NetworkController.MAX_VIEW_IDS;

            this.subId = value % NetworkController.MAX_VIEW_IDS;

            if (this.ownerId<0)
            {
                this.ownerId--;
            }
            isSceneView = ownerId == SCENE_OWNER_ID;
            if (NetworkController.smartFox != null && NetworkController.smartFox.MySelf!=null)
            {
                if (isSceneView)
                {
                    isMine = NetworkController.smartFox.MySelf.ContainsVariable("Master") && NetworkController.smartFox.MySelf.GetVariable("Master").GetBoolValue();
                }
                else
                {
                    isMine = NetworkController.smartFox.MySelf.Id == ownerId;
                }
            }
            DebufViewID = value;
          
        }
    }
    public bool isChildScene()
    {
        return ownerId == SCENE_OWNER_ID && isSceneView;
    }
	
	public bool IsOnMasterControll(){
		if(isSceneView&&pawn!=null){
			return pawn.player==null;
		}
        if (isSceneView && weapon != null&&weapon.GetOwner()!=null)
        {
            return weapon.GetOwner().player == null;
        }
		return isSceneView;
	}
    void Awake(){
		pawn = GetComponent<Pawn>();
		weapon = GetComponent<BaseWeapon>();
		conquestPoint = GetComponent<ConquestPoint>(); 
        if (preLoad)
        {
            NetworkController.RegisterSceneView(this);
        }
	}
	public void SetMine(bool isMine){
		this.isMine = isMine;
		if(pawn==null&&weapon==null&&conquestPoint==null&&needRegister){
			SimpleNetModel view= new SimpleNetModel();
            view.id = viewID;
			NetworkController.Instance.RegisterSceneViewRequest(view);
		} 
	
	}
	public void SetOwner(int ownerId){
		this.ownerId = ownerId;
	}
	public bool IsOwner(int ownerId){
		return this.ownerId == ownerId;
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
	void OnDestroy(){
		NetworkController.ClearView(viewID);
	}
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
	//	NetworkController.Instance.PawnUpdateRequest(pawn);
	}
	public void Taunt(string name){
		NetworkController.Instance.PawnTauntRequest(viewID, name);
	}
	public void KnockOut(){
		NetworkController.Instance.PawnKnockOutRequest(viewID);
	}
	public void Destroy(){
        if (preLoad)
        {
			NetworkController.Instance.DeleteSceneViewRequest(viewID);
		}else{
			NetworkController.Instance.DeleteViewRequest(viewID);
		}
	}
    public void Detonate()
    {
        NetworkController.Instance.DetonateRequest(viewID);
    }
    public void PrepareShoot(Vector3 position, Quaternion rotation, float power, float range, float minRange, int viewId, int projId)
    {
		
		ISFSObject data = new SFSObject();
		
     	data.PutClass("position",new Vector3Model(position));
		data.PutClass("direction",new QuaternionModel(rotation));
		data.PutFloat("power", power);
		data.PutFloat("range", range);
        data.PutFloat("minRange", minRange);
        data.PutInt("viewId", viewId);
        data.PutInt("projId", projId);
        data.PutLong("timeShoot", TimeManager.Instance.NetworkTime);
       
		data.PutInt("id", viewID);	
		if(sendProj==null){
			sendProj = new SFSArray();
		}
        sendProj.AddSFSObject(data);
		//NetworkController.Instance.WeaponShootRequest(data);
	}
	public void TakeInHand(){
		NetworkController.Instance.ChangeWeaponStateRequest(viewID,true);
	}
	
	public void PutAway(){
		NetworkController.Instance.ChangeWeaponStateRequest(viewID,false);
	}
	
	public void SkillCastEffect(string name){
        NetworkController.Instance.SkillCastEffectRequest(viewID, name);
	}
	public void SkillActivate(string name){
		ISFSObject data = new SFSObject();
		data.PutInt("id", viewID);	
		data.PutUtfString("name", name);			
		NetworkController.Instance.SkillActivateRequest(data);
	}
	public void SkillActivate(string name,Vector3 position){
		ISFSObject data = new SFSObject();
		data.PutInt("id", viewID);	
		data.PutUtfString("name", name);			
		data.PutClass("position", new Vector3Model(position));
		NetworkController.Instance.SkillActivateRequest(data);
	}
	public void SkillActivate(string name,int viewId){
		ISFSObject data = new SFSObject();
		data.PutInt("id", viewID);	
		data.PutUtfString("name", name);			
		data.PutClass("viewId", viewId);
		NetworkController.Instance.SkillActivateRequest(data);
	}

    public void InvokeProjectileCall(ISFSObject data)
    {
        NetworkController.Instance.InvokeProjectileCallRequest(data);
    }

    public void PawnDiedByKill(int userId)
    {
		ISFSObject data = new SFSObject();
        data.PutInt("viewId", viewID);
        data.PutInt("player", userId);
		pawn.InfoAboutDeath(data);
        NetworkController.Instance.PawnDiedByKillRequest(data);
    }
	
	public void VipSpawnedRequest(PawnModel pawn)
    {
		NetworkController.Instance.VipSpawnedRequest(pawn);
	}


    public void InPilotChange(bool isPilotIn)
    {
        NetworkController.Instance.InPilotChangeRequest(viewID,isPilotIn);
    }


    public void UpdateSimpleDestroyableObject(SimpleDestroyableModel model)
    {
        NetworkController.Instance.UpdateSimpleDestroyableObjectRequest(model);
    }
	public void UpdateConquestPoint(ConquestPointModel model)
    {
		NetworkController.Instance.UpdateConquestPointRequest(model);
	}

    public void CustomAnimStart(string animName)
    {
        NetworkController.Instance.CustomAnimStartRequest(viewID, animName);

    }
	
	public void ChangeWeaponShootState(bool state){
		  NetworkController.Instance.ChangeWeaponShootStateRequest(viewID, state);
	}
	
	public static void  SendShoot(){
		if(sendProj==null){
			return;
		}
		ISFSObject data = new SFSObject();
		data.PutSFSArray("shoots",sendProj);
		sendProj= null;
		NetworkController.Instance.WeaponShootRequest(data);
	}

    public void LowerHPRequest(BaseDamage damage,int killerId)
    {
        NetworkController.Instance.RemoteDamageOnPawnRequest(new BaseDamageModel(damage), viewID,killerId);
    }
	
}
