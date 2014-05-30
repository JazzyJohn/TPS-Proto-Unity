using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class PhotonResourceWrapper {

	public static Dictionary<string,GameObject> allobject = new Dictionary<string,GameObject>();
	
	class InstantiateQueue{
		public Hashtable evData;
		public PhotonPlayer photonPlayer;
		public GameObject resourceGameObject;
		public int count;
		
	}
	private static Dictionary<int,InstantiateQueue> lateInst = new Dictionary<int,InstantiateQueue>();
	public static void  AddToLaterInstantiate(Hashtable evData, PhotonPlayer photonPlayer, GameObject resourceGameObject, int  instantiationId){
			if(lateInst.ContainsKey(instantiationId)){
			
				lateInst[instantiationId].count++;
			}
			else{
				InstantiateQueue obj = new InstantiateQueue();
				obj.evData = evData;
				obj.photonPlayer = photonPlayer;
				obj.resourceGameObject = resourceGameObject;
				obj.count = 0;
				
				lateInst[instantiationId]  = obj;
			}
	}
	
	public static void RemoveLateObject(instantiationId){
		lateInst.Remove(instantiationId);
	}
	public static void TryToReload(){
		Dictionary<int,InstantiateQueue>.KeyCollection keys = lateInst.Keys;
		foreach(int key in keys){
				InstantiateQueue obj = lateInst[key];
			
				if(PhotonNetwork.networkingPeer.DoInstantiate(obj.evData, obj.photonPlayer, obj.resourceGameObject)!=null){
					lateInst.Remove(key);
				}else{
					if(	lateInst[instantiationId].count>3){
						Debug.LogError("PhotonNetwork error: Could not Instantiate the prefab [" + (string)obj.evData[(byte)0] + "]. Please verify you have this gameobject in a Resources folder.");
						lateInst.Remove(key);
					}
				}
		
		}
	
	}
}



