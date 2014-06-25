using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

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

    public PhotonView photonView;

    void Awake() {
        photonView = GetComponent<PhotonView>();
        nextId =MAXPERPLAYER * (PhotonNetwork.player.ID+1);
        maxId = nextId + MAXPERPLAYER;
        minId = nextId;
    }
    public int GetNextId() {
        if(nextId>=maxId){
            nextId =MAXPERPLAYER * (PhotonNetwork.player.ID+1);
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
    public void InvokeRPC(params object[] theObjects)
    {
		
        photonView.RPC("RPCInvoke", PhotonTargets.All, theObjects);
    }
    [RPC]
    public void RPCInvoke(params object[] theObjects)
    {
		string function = (string)theObjects[0];
		int id = (int)theObjects[1];
		object[] addParams  = new object[theObjects.Length-2];
		for(int i = 2; i<theObjects.Length;i++){
			addParams[i-2] = theObjects[i];
		}
        if (allProjectile.ContainsKey(id)) {
            BaseProjectile proj = allProjectile[id];
            Type thisType = proj.GetType();
            MethodInfo theMethod = thisType.GetMethod(function);
            theMethod.Invoke(proj, addParams);
            
        }
    
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{

	}

}
