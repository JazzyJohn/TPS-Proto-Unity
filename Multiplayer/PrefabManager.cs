using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Reflection;


public class PrefabManager : MonoBehaviour {
	public static PrefabManager instance;
	private bool instantiated;

	public string BundleURL = "gameRes/Assets/Character.unity3d";
	public int version;

	public bool onStart = false;
	public UnityEngine.Object[] prefabObjects;
	public List<GameObject> objects = new List<GameObject>();

	
	public String typeOfObject =  "Pawn" ;
	
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

	public void Start(){
		if (onStart) {
				StartCoroutine (DownloadAndCache());
		}
	}
	public GameObject[] getObjects()
	{
		return objects.ToArray();
	}

	public void DownLoad()
	{
		//StartCoroutine (DownloadAndCache());
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
			//Debug.Log(bundle.mainAsset);
			//bundle
			//Debug.Log(bundle.mainAsset);
			//prefabObjects = bundle.LoadAll();


				prefabObjects = bundle.LoadAll(Type.GetType(typeOfObject));

		
				//if(!sp) Debug.Log ("No sp");

				Debug.Log (prefabObjects.Length);

				for(int i=0;i<prefabObjects.Length;i++)
				{


					if(!prefabObjects[i]){
						Debug.Log("No prefabObj");continue;}
				
				
						Debug.Log (prefabObjects[i]);
						//AssetDatabase.AddObjectToAsset(prefabObjects[i], "Assets/Resources/"+prefabObjects[i].name+".prefab");
						//Debug.Log(AssetDatabase.GetAssetPath(prefabObjects[i]));
					

						
						GameObject prefab =((MonoBehaviour)prefabObjects[i]).gameObject;
						if(!	PhotonResourceWrapper.ContainsKey[prefab.name]){
							PhotonResourceWrapper.allobject[prefab.name] =prefab;	
						}
						switch(typeOfObject){
						case "BaseWeapon":
						//Debug.Log(prefab);
							ItemManager.instance.SetNewWeapon(prefab.GetComponent<BaseWeapon>());
							break;

						}
						//DestroyImmediate(obj);

						//GameObject photonObj = PhotonNetwork.Instantiate(obj.name,sp.transform.position,sp.transform.rotation,0,null) as GameObject;

				}
		
//			Debug.Log (sp.transform.position);

			Debug.Log("PrefabManager " + BundleURL+" has been instantiated.");
			instantiated = true;
			//sbundle.Unload(false);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
