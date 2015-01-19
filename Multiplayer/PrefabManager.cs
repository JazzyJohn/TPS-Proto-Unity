using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Reflection;
static public class AssetBundleManager
{

    private class AssetBundleRef
    {
        public AssetBundle assetBundle = null;
        public int version;
        public string url;
        public AssetBundleRef(string strUrlIn, int intVersionIn)
        {
            url = strUrlIn;
            version = intVersionIn;
        }
    };
    // A dictionary to hold the AssetBundle references
    static private Dictionary<string, AssetBundleRef> dictAssetBundleRefs;
    static AssetBundleManager()
    {
        dictAssetBundleRefs = new Dictionary<string, AssetBundleRef>();
    }
    public static AssetBundle getAssetBundle(string url, int version)
    {
        string keyName = url + version.ToString();
        AssetBundleRef abRef;
        if (dictAssetBundleRefs.TryGetValue(keyName, out abRef))
            return abRef.assetBundle;
        else
        {
            Debug.Log("Bundle error");
            return null;
        }
    }
    public static void setAssetBundle(AssetBundle assetBundle,string url, int version)
    {

            string keyName = url + version.ToString();
            if (dictAssetBundleRefs.ContainsKey(keyName))
            {
                dictAssetBundleRefs[keyName].assetBundle = assetBundle;
            }else{
                AssetBundleRef abRef = new AssetBundleRef(url, version);
                abRef.assetBundle = assetBundle;
                dictAssetBundleRefs.Add(keyName, abRef);
            }
    }
    public static bool isHasAssetBundle(string url, int version)
    {
        string keyName = url + version.ToString();
        if (dictAssetBundleRefs.ContainsKey(keyName))
        {
         Debug.Log("MyBundle" + dictAssetBundleRefs[keyName].assetBundle);
        }
        return dictAssetBundleRefs.ContainsKey(keyName) && dictAssetBundleRefs[keyName].assetBundle != null;
    }
}

public class PrefabManager : MonoBehaviour {
	public static PrefabManager instance;
	private bool instantiated;

	public string BundleURL = "gameRes/Assets/Character.unity3d";
	public int version;

	public bool onStart = false;
	public UnityEngine.Object[] prefabObjects;
	public List<GameObject> objects = new List<GameObject>();
    [System.Serializable]
    public class InBundleData
    {
        public string type;
        public string[] objects;

    }

    public InBundleData[] allObjects;
	
	public bool isPawns = false;
	
	private bool inProgress;
	private WWW www;
	
	
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
            if (isPawns)
            {
                StartCoroutine(DownloadAndCache());
            }

		}
	}
	public GameObject[] getObjects()
	{
		return objects.ToArray();
	}
	void Update(){
        if (onStart)
        {
            if (!isPawns && GlobalPlayer.instance.loadingStage >= 2 && bundle==null)
            {
                StartCoroutine(DownloadAndCache());
            }

        }
       try
        {
            if (inProgress && www != null)
            {



                ServerHolder.progress.curLoader = www.progress * 100f;

            }
        }
        catch (Exception)
        {

            inProgress = false;
        }
      
	}
	public void DownLoad()
	{
		//StartCoroutine (DownloadAndCache());
	}

    public AssetBundle bundle;


	public IEnumerator DownloadAndCache (){
		// Wait for the Caching system to be ready
		while (!Caching.ready)
			yield return null;
		inProgress = true;
		string crossDomainesafeURL =StatisticHandler.GetNormalURL()+BundleURL;
		// Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
        if (AssetBundleManager.isHasAssetBundle(crossDomainesafeURL, version))
        {
            bundle = AssetBundleManager.getAssetBundle(crossDomainesafeURL, version);
            Debug.Log("MyBundle" + bundle);
			inProgress = false;
            yield return null;
        }
        else
        {
            Debug.Log("NO BUNLDE NEED TO LOAD" );
            using (www = WWW.LoadFromCacheOrDownload(crossDomainesafeURL, version))
            {
                yield return www;
                Debug.Log("WWW ERROR" + www.error);
                if (www.error == null)
                {


                    bundle = www.assetBundle;
                    AssetBundleManager.setAssetBundle(bundle, crossDomainesafeURL, version);
                    //Debug.Log(bundle.mainAsset);
                    //bundle
                    //Debug.Log(bundle.mainAsset);
                    //prefabObjects = bundle.LoadAll();

                  
                    /*  prefabObjects = bundle.LoadAll(Type.GetType(typeOfObject));
                      AssetBundleManager.setAssetBundle(bundle, crossDomainesafeURL,version);

                      //if(!sp) Debug.Log ("No sp");

                      //Debug.Log (prefabObjects.Length);

                      for (int i = 0; i < prefabObjects.Length; i++)
                      {


                          if (!prefabObjects[i])
                          {
                              Debug.Log("No prefabObj"); continue;
                          }


                          //Debug.Log (prefabObjects[i]);
                          //AssetDatabase.AddObjectToAsset(prefabObjects[i], "Assets/Resources/"+prefabObjects[i].name+".prefab");
                          //Debug.Log(AssetDatabase.GetAssetPath(prefabObjects[i]));



                          GameObject prefab = ((MonoBehaviour)prefabObjects[i]).gameObject;
                          if (!PhotonResourceWrapper.allobject.ContainsKey(prefab.name))
                          {
                              PhotonResourceWrapper.allobject[prefab.name] = prefab;
                          }
                          switch (typeOfObject)
                          {
                              case "BaseWeapon":
                                  //Debug.Log(prefab);
                                  ItemManager.instance.SetNewWeapon(prefab.GetComponent<BaseWeapon>());
                                  break;


                          }
                          //DestroyImmediate(obj);

                          //GameObject photonObj = PhotonNetwork.Instantiate(obj.name,sp.transform.position,sp.transform.rotation,0,null) as GameObject;
                    
                      }
                      */
                    //			Debug.Log (sp.transform.position);
                   
                    www.Dispose();
                    //sbundle.Unload(false);
                }
             
            }
            foreach (InBundleData data in allObjects)
            {
                Type type = Type.GetType(data.type);
                foreach (string obj in data.objects)
                {
                    AssetBundleRequest request = bundle.LoadAsync(obj, type);
                    request.priority = 100;
                    yield return request;
                    if (request.asset != null)
                    {
                        GameObject prefab = ((MonoBehaviour)request.asset).gameObject;
                        if (!PhotonResourceWrapper.allobject.ContainsKey(prefab.name))
                        {
                            PhotonResourceWrapper.allobject[prefab.name] = prefab;
                        }
                        switch (data.type)
                        {
                            case "BaseWeapon":
                                //Debug.Log(prefab);
                                ItemManager.instance.SetNewWeapon(prefab.GetComponent<BaseWeapon>());
                                break;


                        }
                    }
                }
                if (onStart)
                {
                    switch (data.type)
                    {
                        case "BaseWeapon":
                            ItemManager.instance.ConnectToPrefab();
                            break;
                    }
                }

            }
            inProgress = false;
            Debug.Log("PrefabManager " + BundleURL + " has been instantiated.");
            instantiated = true;
					
        }
		inProgress = false;
	}

}
