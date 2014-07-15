using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PartMapDownloader : MapDownloader
{

    public override IEnumerator DownloadAndCache()
    {
        // Wait for the Caching system to be ready
        while (!Caching.ready)
            yield return null;
        string crossDomainesafeURL = StatisticHandler.GetNormalURL() + MapURL;
        // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
        if (AssetBundleManager.isHasAssetBundle(crossDomainesafeURL, version))
        {
            bundle = AssetBundleManager.getAssetBundle(crossDomainesafeURL, version);
            GameObject obj = Instantiate(bundle.mainAsset) as GameObject;
            yield return null;
        }
        else
        {
            using (www = WWW.LoadFromCacheOrDownload(crossDomainesafeURL, version))
            {
                inProgress = true;
                yield return www;


                //Debug.Log("STartDownloadl");
                if (www.error != null)
                    throw new Exception("WWW map download had an error:" + www.error);

                bundle = www.assetBundle;
                AssetBundleManager.setAssetBundle(bundle, crossDomainesafeURL, version);
                UnityEngine.Object[] prefabObjects = bundle.LoadAll(typeof(Part));
                Generation generator = FindObjectOfType<Generation>();
                List<Part> leftTurns = new List<Part>();
                List<Part> rightTurns = new List<Part>();
                List<Part> forwards = new List<Part>();
                for (int i = 0; i < prefabObjects.Length; i++)
                {


                    if (!prefabObjects[i])
                    {
                        Debug.Log("No prefabObj"); continue;
                    }


                    //Debug.Log (prefabObjects[i]);
                    //AssetDatabase.AddObjectToAsset(prefabObjects[i], "Assets/Resources/"+prefabObjects[i].name+".prefab");
                    //Debug.Log(AssetDatabase.GetAssetPath(prefabObjects[i]));



                    Part prefab = ((MonoBehaviour)prefabObjects[i]).GetComponent<Part>();
                   
                    switch(prefab.type){
                        case PARTDIRECTION.FORWARD:
                            forwards.Add(prefab);
                            break;
                        case PARTDIRECTION.LEFT:
                            leftTurns.Add(prefab);
                            break;
                         case PARTDIRECTION.RIGHT:
                            rightTurns.Add(prefab);
                            break;
                    }
               

                }
                generator.LeftTurnParts = leftTurns.ToArray();
                generator.RightTurnParts = rightTurns.ToArray();
                generator.Parts = forwards.ToArray();
                /*UnityEngine.Object[] prefabObjects = bundle.LoadAll();
			
                //GameObject sp = GameObject.Find("SpawnPoint");
                //if(!sp) Debug.Log ("No sp");
			

                Debug.Log(prefabObjects.Length);
                for(int i=0;i<prefabObjects.Length;i++)
                {

                    Debug.Log (prefabObjects[i].name);
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
                /*
                                if(prefabObjects[i].name.Equals(MapNAME))
                                {
                                    //Debug.Log (prefabObjects[i].name.ToString());
                                    GameObject obj = Instantiate(prefabObjects[i]) as GameObject;

                                    /*if(!obj)
                                        continue;

                                    string path = "Assets/Photon Unity Networking/Resources/";
                                    path = path + obj.name;
                                    path = path + ".prefab";

                                    PrefabUtility.CreatePrefab(path,obj,ReplacePrefabOptions.ReplaceNameBased);
                                    DestroyImmediate(obj);*/
                /*	}
                }
                */
                //			Debug.Log (sp.transform.position);

                Debug.Log("MapDownloader has been instantiated.");
                instantiated = true;
                inProgress = false;
                bundle.Unload(false);
            }
        }
        //Destroy(this);
    }
}
