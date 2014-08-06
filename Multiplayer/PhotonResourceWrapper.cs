using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class PhotonResourceWrapper {

	public static Dictionary<string,GameObject> allobject = new Dictionary<string,GameObject>();
	
	class InstantiateQueue{
		
		public GameObject resourceGameObject;
		public int count;
		
	}
	private static Dictionary<int,InstantiateQueue> lateInst = new Dictionary<int,InstantiateQueue>();
	
	
	public static void RemoveLateObject(int instantiationId){
		lateInst.Remove(instantiationId);
	}
	
}



