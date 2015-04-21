using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Reflection;
public enum AssetBundleType{
	PAWN,WEAPON
}

public class MapLoader : MonoBehaviour {
   // public string assetbundlePath;
    public int myVersion;
   // public string assetbundleMapName;
    private static string mapSubDir = "/gameRes/Maps/";

    public int version;

    public AssetBundle bundle;
	public AssetBundle weaponBundle;
	public AssetBundle pawnBundle;
    private bool inProgress;
	private WWW www;
	// Use this for initialization
	void Start () {
    //    StartCoroutine(Load());
	}
    void Update(){
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

   
    public IEnumerator Load(string assetbundleMapName, string versionstr="")
    {

        while (!Caching.ready)
            yield return null;

        string crossDomainesafeURL = StatisticHandler.GetNormalURL() + mapSubDir + assetbundleMapName + versionstr + ".unity3d";
        Debug.Log(crossDomainesafeURL);
        if (AssetBundleManager.isHasAssetBundle(crossDomainesafeURL, version))
        {
            bundle = AssetBundleManager.getAssetBundle(crossDomainesafeURL, version);
            Debug.Log("MyBundle" + bundle);
            inProgress = false;
            AsyncOperation async = Application.LoadLevelAsync(assetbundleMapName);
            yield return async;
        }
        else
        {
            inProgress = true;
            Debug.Log("NO BUNLDE NEED TO LOAD : "+crossDomainesafeURL );
            using (www = WWW.LoadFromCacheOrDownload(crossDomainesafeURL, version))
            {
                yield return www;
                Debug.Log("LOADING "+crossDomainesafeURL+" WWW ERROR " + www.error);
                if (www.error == null)
                {


                  

                    // In order to make the scene available from LoadLevel, we have to load the asset bundle.
                    // The AssetBundle class also lets you force unload all assets and file storage once it is no longer needed.
                    
                    bundle = www.assetBundle;
                    Debug.Log("WWW ERROR" + www.assetBundle);

                    AssetBundleManager.setAssetBundle(bundle, crossDomainesafeURL, version);
                
                    //sbundle.Unload(false);
                }

            }
            AsyncOperation async = Application.LoadLevelAsync(assetbundleMapName);
            yield return async;
        }
        // Load the level we have just downloaded
        
    }
	public IEnumerator DownloadAndCache (string name,AssetBundleType type){
		// Wait for the Caching system to be ready
		while (!Caching.ready)
			yield return null;
		inProgress = true;
		string crossDomainesafeURL =StatisticHandler.GetNormalURL()+name;
		// Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
        if (AssetBundleManager.isHasAssetBundle(crossDomainesafeURL, version))
        {
			switch(type){
				case	AssetBundleType.PAWN:
					pawnBundle = AssetBundleManager.getAssetBundle(crossDomainesafeURL, version);
					Debug.Log("MyBundle" + pawnBundle);
					inProgress = false;
					yield return null;
				break;
				case AssetBundleType.WEAPON:
					weaponBundle = AssetBundleManager.getAssetBundle(crossDomainesafeURL, version);
					Debug.Log("MyBundle" + weaponBundle);
					inProgress = false;
					yield return null;
				break;
			}
           
        }
        else
        {
            Debug.Log("NO BUNLDE NEED TO LOAD : "+crossDomainesafeURL );
            using (www = WWW.LoadFromCacheOrDownload(crossDomainesafeURL, version))
            {
                yield return www;
                Debug.Log("LOADING "+crossDomainesafeURL+" WWW ERROR " + www.error);
                if (www.error == null)
                {
                   
                    switch (type)
                    {
                        case AssetBundleType.PAWN:
                            pawnBundle = www.assetBundle;
                            AssetBundleManager.setAssetBundle(pawnBundle, crossDomainesafeURL, version);
                            UnityEngine.Object[]  prefabObjects = pawnBundle.LoadAll(typeof(Pawn));
                            for (int i = 0; i < prefabObjects.Length; i++)
                            {


                                GameObject prefab = ((MonoBehaviour)prefabObjects[i]).gameObject;
                                //   Debug.Log(prefab.name);
                                if (!PhotonResourceWrapper.allobject.ContainsKey(prefab.name))
                                {
                                    PhotonResourceWrapper.allobject[prefab.name] = prefab;
                                }
                               

                            }
                             UnityEngine.Object[]  buildings = pawnBundle.LoadAll(typeof(Building));
                             for (int i = 0; i < buildings.Length; i++)
                            {


                                GameObject prefab = ((MonoBehaviour)buildings[i]).gameObject;
                                //   Debug.Log(prefab.name);
                                if (!PhotonResourceWrapper.allobject.ContainsKey(prefab.name))
                                {
                                    PhotonResourceWrapper.allobject[prefab.name] = prefab;
                                }
                               

                            }
                               UnityEngine.Object[]  ghosts = pawnBundle.LoadAll(typeof(GhostObject));
                               for (int i = 0; i < ghosts.Length; i++)
                            {


                                GameObject prefab = ((MonoBehaviour)ghosts[i]).gameObject;
                                //   Debug.Log(prefab.name);
                                if (!PhotonResourceWrapper.allobject.ContainsKey(prefab.name))
                                {
                                    PhotonResourceWrapper.allobject[prefab.name] = prefab;
                                }
                               

                            }
                            Debug.Log("MyBundle" + pawnBundle);
                            inProgress = false;

                            break;
                        case AssetBundleType.WEAPON:
                            weaponBundle = www.assetBundle;
                            UnityEngine.Object[] weapons = weaponBundle.LoadAll(typeof(BaseWeapon));
                            for (int i = 0; i < weapons.Length; i++)
                            {


                                GameObject prefab = ((MonoBehaviour)weapons[i]).gameObject;
                                //   Debug.Log(prefab.name);
                                if (!PhotonResourceWrapper.allobject.ContainsKey(prefab.name))
                                {
                                    PhotonResourceWrapper.allobject[prefab.name] = prefab;
                                }
                             
                                ItemManager.instance.SetNewWeapon(prefab.GetComponent<BaseWeapon>());
                               

                            }
                            UnityEngine.Object[] armors = weaponBundle.LoadAll(typeof(BaseArmor));
                            for (int i = 0; i < armors.Length; i++)
                            {


                                GameObject prefab = ((MonoBehaviour)armors[i]).gameObject;
                                //   Debug.Log(prefab.name);
                                if (!PhotonResourceWrapper.allobject.ContainsKey(prefab.name))
                                {
                                    PhotonResourceWrapper.allobject[prefab.name] = prefab;
                                }                               
                                ItemManager.instance.SetNewArmor(prefab.GetComponent<BaseArmor>());
                                 

                            }
                            AssetBundleManager.setAssetBundle(weaponBundle, crossDomainesafeURL, version);
                            Debug.Log("MyBundle" + weaponBundle);
                            inProgress = false;

                            break;
                        default:
                            prefabObjects = new UnityEngine.Object[0];
                            break;
                    }

                   
                }
                 www.Dispose();
                  
             
            }
           
            inProgress = false;
            Debug.Log("PrefabManager " + name + " has been instantiated.");
         
					
        }
		inProgress = false;
	}
	public bool IsInCache(string name){
		string crossDomainesafeURL =StatisticHandler.GetNormalURL()+name;
		return Caching.IsVersionCached(crossDomainesafeURL,version);
	}
}
