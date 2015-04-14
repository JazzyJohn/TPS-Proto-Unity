using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using Sfs2X.Entities.Data;
using nstuff.juggerfall.extension.models;

class ProjectileManager: MonoBehaviour
{
    private static ProjectileManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static ProjectileManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(ProjectileManager)) as ProjectileManager;
            }


            return s_Instance;
        }
    }

    private int nextId = 0;

    private static int MAXPERPLAYER = 1000;

    private int maxId = 0;

    private int minId = 0;


    private Dictionary<int, BaseProjectile> allProjectile =new Dictionary<int, BaseProjectile>();

    

    void Awake() {
        int myId = 0;
        if (NetworkController.smartFox != null)
        {
            myId = NetworkController.smartFox.MySelf.Id;
        }
        nextId = MAXPERPLAYER * (myId + 1);
        maxId = nextId + MAXPERPLAYER;
        minId = nextId;
    }
    public int GetNextId() {
        int myId = 0;
        if (NetworkController.smartFox != null)
        {
            myId = NetworkController.smartFox.MySelf.Id;
        }
        if(nextId>=maxId){
            nextId = MAXPERPLAYER * (myId + 1);
        }
        for (int i = nextId+1; i < maxId; i++) {
            if (!allProjectile.ContainsKey(i)) {
                nextId = i;
                return i;
            }
        }
        for (int i = minId; i < nextId; i++)
        {
            if (!allProjectile.ContainsKey(i))
            {
                nextId = i;
                return i;
            }
        }
        Debug.LogError("EROROR TO MANY PROJECTILES FROM PALYER");
        return 0;
    }
    public void AddProject(int id,BaseProjectile projectile) {
        allProjectile[id] = projectile;
    }
    public void RemoveProjectile(int id){
        allProjectile.Remove(id);
    }
    
    public void InvokeRPC(string name, int projid,Vector3 position)
    {
        if (NetworkController.Instance.isSingle)
        {
            return;
        }
        ISFSObject data = new SFSObject();
        data.PutUtfString("name", name);
        data.PutInt("projid", projid);
        data.PutClass("position", new Vector3Model(position));
        NetworkController.Instance.InvokeProjectileCallRequest(data);
    }
    public void InvokeRPC(string name, int projid, Vector3 position, int count)
    {
        if (NetworkController.Instance.isSingle)
        {
            return;
        }
        ISFSObject data = new SFSObject();
        data.PutUtfString("name", name);
        data.PutInt("projid", projid);
        data.PutInt("count", count);
        data.PutClass("position", new Vector3Model(position));
        NetworkController.Instance.InvokeProjectileCallRequest(data);
       
    }
    public void InvokeRPC(string name, int projid, int count)
    {
        if (NetworkController.Instance.isSingle)
        {
            return;
        }
        ISFSObject data = new SFSObject();
        data.PutUtfString("name", name);
        data.PutInt("projid", projid);
        data.PutInt("count", count);
        NetworkController.Instance.InvokeProjectileCallRequest(data);
    }
  
    public void RemoteInvoke(ISFSObject data)
    {

        string function = data.GetUtfString("name");
        int id = data.GetInt("projid");
     
		object[] addParams  = new object[   data.Size()-2];
        int i = 0;
		if(data.ContainsKey("positon")){
            addParams[i++] = ((Vector3Model)data.GetClass("position")).GetVector();
        }
        if (data.ContainsKey("count"))
        {
            addParams[i++] = data.GetInt("count");
        }
        if (allProjectile.ContainsKey(id)) {
            BaseProjectile proj = allProjectile[id];
            Type thisType = proj.GetType();
            MethodInfo theMethod = thisType.GetMethod(function);
            if (theMethod != null)
            {
                theMethod.Invoke(proj, addParams);
            }
            
        }
    
    }
    
}
