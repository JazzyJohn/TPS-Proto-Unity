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
public static class PhotonResourceWrapperExtensions
{

    public static T Default<T>(this T prefab) where T : Component
    {
        string name = prefab.name.Replace("(Clone)", "");
        if (PhotonResourceWrapper.allobject.ContainsKey(name))
        {
            return PhotonResourceWrapper.allobject[name].GetComponent<T>();
        }
        GameObject resourceGameObject = (GameObject)Resources.Load(name, typeof(GameObject));
        if (resourceGameObject != null)
        {
            return resourceGameObject.GetComponent<T>();
        }
        return prefab;
    }
}


