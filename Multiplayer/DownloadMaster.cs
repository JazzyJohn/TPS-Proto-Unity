using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class DownloadMaster : MonoBehaviour
{
    private bool inProgress;

    private UnityEngine.Object asset;

    private WWW www;

    private Queue<DownloadInfo> downloadQueue = new Queue<DownloadInfo>();

    public delegate void SetGameObject(GameObject go);

    class DownloadInfo
    {
        SetGameObject linkObserver;
        public string url;
        public string name;
        public Type type;
        public int version;

        public DownloadInfo(string url, string name, Type type, int version, SetGameObject linkObserver)
        {
            this.url = url;
            this.name = name;
            this.type = type;
            this.version = version;
            this.linkObserver = linkObserver;
        }

        public void Notify(GameObject go)
        {
            linkObserver(go);
        }
    }

    void Update()
    {
        if (DownloadQueueIsNotEmpty && !inProgress)
        {
            DownloadInfo info = downloadQueue.Dequeue();

            Download(downloadQueue.Dequeue());

            info.Notify(((MonoBehaviour)asset).gameObject);
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

    public void Download(string url, string name, Type type, int version, SetGameObject linkObserver)
    {
        downloadQueue.Enqueue(new DownloadInfo(url, name, type, version, linkObserver));
        //StartCoroutine (Download());
    }

    public AssetBundle GetRegisteredBundle(string url, int version)
    {
        AssetBundle bundle = null;

        string crossDomainesafeURL = StatisticHandler.GetNormalURL() + url;

        if (AssetBundleManager.isHasAssetBundle(crossDomainesafeURL, version))
        {
            bundle = AssetBundleManager.getAssetBundle(crossDomainesafeURL, version);
        }
        else
        {
            www = new WWW(crossDomainesafeURL);

            if (www.error == null)
            {
                bundle = www.assetBundle;
                AssetBundleManager.setAssetBundle(bundle, crossDomainesafeURL, version);
                www.Dispose();
            }
        }

        if (bundle == null) Debug.LogError("BundleError");

        return bundle;
    }

    IEnumerator Download(DownloadInfo info)
    {
        AssetBundle bundle = GetRegisteredBundle(info.url, info.version);

        // Wait for the Caching system to be ready
        while (!Caching.ready)
            yield return null;

        inProgress = true;

        AssetBundleRequest request = bundle.LoadAsync(info.name, info.type);

        request.priority = 100;

        yield return request;

        if (request.asset != null)
        {
            asset = request.asset;
        }

        inProgress = false;
    }

    public bool DownloadQueueIsNotEmpty {
        get { return downloadQueue.Count > 0; }
    }
}
