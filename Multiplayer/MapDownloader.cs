using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class MapDownloader : MonoBehaviour {
	public static MapDownloader instance;
	private bool instantiated;
	
	public string MapURL = "gameRes/Maps/map_kaspi_c.unity3d";
	public string MapNAME = "MapObject";
	public int version;

	public bool isReady()
	{
		return instantiated;
	}
	
	void Awake() 
	{	
		instance = this;
		instantiated = false;
		//StartCoroutine (DownloadAndCache());
	}
	void Start(){
		StartCoroutine (DownloadAndCache());
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
				throw new Exception("WWW map download had an error:" + www.error);
			
			AssetBundle bundle = www.assetBundle;
			UnityEngine.Object[] prefabObjects = bundle.LoadAll();
			
			//GameObject sp = GameObject.Find("SpawnPoint");
			//if(!sp) Debug.Log ("No sp");
			
			Debug.Log (prefabObjects.GetLength(0));
			
			for(int i=0;i<prefabObjects.GetLength(0);i++)
			{
				if(!prefabObjects[i]){
					Debug.Log("No prefabObj");continue;}
				
				/*if(prefabObjects[i].name.ToString().StartsWith("PickUp_"))
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
				}*/

				if(prefabObjects[i].name.Equals(MapNAME))
				{
					Debug.Log (prefabObjects[i].name.ToString());
					GameObject obj = Instantiate(prefabObjects[i]) as GameObject;

					/*if(!obj)
						continue;

					string path = "Assets/Photon Unity Networking/Resources/";
					path = path + obj.name;
					path = path + ".prefab";

					PrefabUtility.CreatePrefab(path,obj,ReplacePrefabOptions.ReplaceNameBased);
					DestroyImmediate(obj);*/
				}
			}
			
			//			Debug.Log (sp.transform.position);
			
			Debug.Log("MapDownloader has been instantiated.");
			instantiated = true;
			bundle.Unload(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
