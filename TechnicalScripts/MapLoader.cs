using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Reflection;
public class MapLoader : MonoBehaviour {
   // public string assetbundlePath;
    public int myVersion;
   // public string assetbundleMapName;
    private static string mapSubDir = "/gameRes/Maps/";

    public int version;

    public AssetBundle bundle;

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
    public IEnumerator Load(string assetbundleMapName)
    {

        while (!Caching.ready)
            yield return null;

        string crossDomainesafeURL = StatisticHandler.GetNormalURL() + mapSubDir + assetbundleMapName +".unity3d";
        Debug.Log(crossDomainesafeURL);
        if (AssetBundleManager.isHasAssetBundle(crossDomainesafeURL, version))
        {
            bundle = AssetBundleManager.getAssetBundle(crossDomainesafeURL, version);
            Debug.Log("MyBundle" + bundle);
            inProgress = false;
            yield return null;
        }
        else
        {
            Debug.Log("NO BUNLDE NET TO LOAD");
            using (www = WWW.LoadFromCacheOrDownload(crossDomainesafeURL, version))
            {
                yield return www;
                Debug.Log("WWW ERROR" + www.error);
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
        }
        // Load the level we have just downloaded
        AsyncOperation async =Application.LoadLevelAsync(assetbundleMapName);
        yield return async;
    }
 
}
