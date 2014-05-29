using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class PrefabManager : MonoBehaviour {
	public static PrefabManagerPickable instance;
	//private bool instantiated;

	public string BundleURL = "gameRes/Assets/pickable.unity3d";
	public int version;
	
	public UnityEngine.Object[] prefabObjects;
	public List<GameObject> objects = new List<GameObject>();
	
	public string namePrefix  = "PickUp_";
	
	/*public bool isReady()
	{
		return instantiated;
	}

	void Awake() 
	{	
		if(instance==null){
			instance = this;
			instantiated = false;
		}else{
			Destroy(this);
		}
		
		//StartCoroutine (DownloadAndCache());
	}*/
	
	public GameObject[] getObjects()
	{
		return objects.ToArray();
	}

	public void DownLoad()
	{
		StartCoroutine (DownloadAndCache());
	}
	



	public IEnumerator DownloadAndCache (){
		// Wait for the Caching system to be ready
		while (!Caching.ready)
			yield return null;

		string crossDomainesafeURL =StatisticHandler.GetNormalURL()+BundleURL;
		// Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
		using(WWW www = WWW.LoadFromCacheOrDownload (crossDomainesafeURL, version))
		{
			yield return www;
			
			if (www.error != null)
				throw new Exception("WWW download had an error:" + www.error);
			
			AssetBundle bundle = www.assetBundle;
			prefabObjects = bundle.LoadAll();

			//GameObject sp = GameObject.Find("SpawnPoint");
			//if(!sp) Debug.Log ("No sp");

			Debug.Log (prefabObjects.GetLength(0));

			for(int i=0;i<prefabObjects.GetLength(0);i++)
			{
				if(!prefabObjects[i]){
					Debug.Log("No prefabObj");continue;}
			
				if(prefabObjects[i].name.ToString().StartsWith(namePrefix))
				{
					Debug.Log (prefabObjects[i].name.ToString());
					GameObject obj = null;// = Instantiate(prefabObjects[i]) as GameObject;

					if(!obj)
						continue;

					string path = "Assets/Photon Unity Networking/Resources/";
					path = path + obj.name;
					path = path + ".prefab";

					PrefabUtility.CreatePrefab(path,obj,ReplacePrefabOptions.ReplaceNameBased);
					DestroyImmediate(obj);

					//GameObject photonObj = PhotonNetwork.Instantiate(obj.name,sp.transform.position,sp.transform.rotation,0,null) as GameObject;
				}
			}

//			Debug.Log (sp.transform.position);

			Debug.Log("PrefabManager " + namePrefix+" has been instantiated.");
			instantiated = true;
			bundle.Unload(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
