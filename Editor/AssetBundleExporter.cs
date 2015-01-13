// AssetBundle Exporter v.1.0
// Copyright (C) 2013 Sergey Taraban <http://staraban.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

public class AssetBundleExporter : EditorWindow 
{
	static int FileVersion = 1;
	static string[] ForbidenExtensions = {".meta"};
	bool mMakeCompressedAsset = false;
	bool mSplitPacksGroup = true;
	bool mDumpExportedFilesInFile = true;
	bool mVerifyAssetBundleAfterBuild = true;
	float mSplitPacksSize = 2.0f; //2 mb
	
	enum ETARGET_PLATFORM { 
		Current = 0, 
		iOS = BuildTarget.iPhone, 
		Android = BuildTarget.Android,
		WindowsPhone = BuildTarget.WP8Player,
		WebPlayer = BuildTarget.WebPlayer,
		MacOS = BuildTarget.StandaloneOSXUniversal,
		Windows = BuildTarget.StandaloneWindows,
		Linux = BuildTarget.StandaloneLinuxUniversal
	}
	ETARGET_PLATFORM mTargetPlatform;
	
	string splitPacksSizeStr = "2.0";
	static AssetBundleExporter window = null;
	Vector2 scrollPosition = Vector2.zero;
	string mStatusText = "";
	
	List<string> mObjectsPathList = new List<string>();
	static List<string> mExcludeFileExt = new List<string>();
	
	// Add menu named "My Window" to the Window menu
    [MenuItem("Assets/AssetBundle Exporter")]
	static void Init () 
	{
		mExcludeFileExt.AddRange(ForbidenExtensions);
		
		// Get existing open window or if none, make a new one:
		window = (AssetBundleExporter)EditorWindow.GetWindow (typeof (AssetBundleExporter));
	}
	
	void OnGUI () 
	{
		ProcessDragAndDrop();
		
		//options area
		mMakeCompressedAsset = EditorGUILayout.Toggle("Make compressed asset", mMakeCompressedAsset);
		mDumpExportedFilesInFile = EditorGUILayout.Toggle("Dump file names in file", mDumpExportedFilesInFile);
		mVerifyAssetBundleAfterBuild = EditorGUILayout.Toggle("Verify asset bundles", mVerifyAssetBundleAfterBuild);
		mTargetPlatform = (ETARGET_PLATFORM)EditorGUILayout.EnumPopup("Target Platform", mTargetPlatform);
		mSplitPacksGroup = EditorGUILayout.BeginToggleGroup ("Split Packs size(.mb)", mSplitPacksGroup);
		splitPacksSizeStr = EditorGUILayout.TextArea(splitPacksSizeStr);
		mSplitPacksSize = float.Parse(splitPacksSizeStr);
		mSplitPacksSize *= 1000000.0f;
		EditorGUILayout.EndToggleGroup ();
		
		//file path list area
		GUILayout.BeginHorizontal();
        if (window == null)
        {
            return;
        }
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width( window.position.width ), GUILayout.Height( window.position.height-150 ));
		string objPathList = "";
		foreach(var obj in mObjectsPathList) 
		{
			objPathList += obj + "\n";
		}
		GUILayout.Label(objPathList);
		
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		
		//button area
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Save")) 
		{
			string fileListPath = EditorUtility.SaveFilePanel ("Save file list", "", "dlc_file_list", "txt");
			using(StreamWriter writer = new StreamWriter(fileListPath, false))
			{
				writer.WriteLine(FileVersion.ToString());
				foreach(var objPath in mObjectsPathList)
				{
					writer.WriteLine(objPath);
				}
			}
			
		}
		if(GUILayout.Button("Load")) 
		{
			mObjectsPathList.Clear();
			string fileListPath = EditorUtility.OpenFilePanel ("Load file list", "", "txt");
			using(StreamReader reader = new StreamReader(fileListPath, false))
			{
				string fileVersion = reader.ReadLine();
				if(fileVersion != FileVersion.ToString()) {
					LogToWindow("File has unknown format. Loading failed");
					return;
				}
				while(!reader.EndOfStream)
				{
					mObjectsPathList.Add(reader.ReadLine());
				}
			}
		}
		if(GUILayout.Button("Clear")) 
		{
			mObjectsPathList.Clear();
		}
		if(GUILayout.Button("Generate")) 
		{
			string assetBundleFileName = EditorUtility.SaveFilePanel ("Save AssetBundle", "", "AssetBundle", "unity3d");
			if(mSplitPacksGroup) 
			{
				GenerateAssetBundles(assetBundleFileName);
			}
			else
			{
				BuildAssetBundle(assetBundleFileName, mObjectsPathList);
			}
		}
		
		
		GUILayout.EndHorizontal();
		
	}
	
	void LogToWindow(string text) {
		mStatusText = text;
		Debug.Log(text);
	}
	
	//Generate a lot of asset bundles using mObjectsList and mSplitPacksSize
	void GenerateAssetBundles(string assetFileName) 
	{
		float sizeCounter = 0;
		int bundleCounter = 0;
		int counter = 0;
		List<string> filesForBundle = new List<string>();
		foreach (var filepath in mObjectsPathList) 
		{
			counter++;
			string fullFilePath = GetFullPath(filepath);
			FileInfo finfo = new FileInfo(fullFilePath);
			sizeCounter += finfo.Length;
			filesForBundle.Add(filepath);
			bool isLastAsset = counter == mObjectsPathList.Count;
			if(sizeCounter >= mSplitPacksSize || isLastAsset)
			{
				string assetBundleFileName = Path.Combine(Path.GetDirectoryName(assetFileName), Path.GetFileNameWithoutExtension(assetFileName));
				assetBundleFileName = assetBundleFileName + "_" + bundleCounter.ToString() + Path.GetExtension(assetFileName);
				
				RemoveOldAssetBundle(assetBundleFileName);
				BuildAssetBundle(assetBundleFileName, filesForBundle);
				filesForBundle.Clear();
				bundleCounter++;
				sizeCounter = 0;
			}
		}
	}
	
	
	//
	void BuildAssetBundle(string assetFilePath, List<string> filesForAsset)
	{
		if(filesForAsset.Count == 0) {
			LogToWindow("Asset Bundle create failed. No files to include");
			return;
		}
		
		//generate objects list
		List<Object> objList = new List<Object>();
		foreach (var filepath in filesForAsset) 
		{
			Object obj = AssetDatabase.LoadMainAssetAtPath(filepath);
			if(obj != null)
			{
				objList.Add(obj);
			}
		}
		
		//build AssetBundle
		try {
			BuildAssetBundleOptions options = BuildAssetBundleOptions.CompleteAssets;
           
			options |= BuildAssetBundleOptions.CollectDependencies;
			if(!mMakeCompressedAsset) {
				options |= BuildAssetBundleOptions.UncompressedAssetBundle;
			}
			if(mTargetPlatform != ETARGET_PLATFORM.Current) {
				BuildPipeline.BuildAssetBundle(objList[0], objList.ToArray(), assetFilePath, options, (BuildTarget)mTargetPlatform);
			}
			else
			{
				BuildPipeline.BuildAssetBundle(objList[0], objList.ToArray(), assetFilePath, options);
			}
		}
		catch (System.Exception ex) 
		{
			LogToWindow("Failed to create Asset Bundle woth error: " + ex.Message);
			return;
		}
		
		//dump in file
		if(mDumpExportedFilesInFile) 
		{
			using(StreamWriter writer = new StreamWriter(assetFilePath + ".txt", false))
			{
				foreach(var obj in objList)
				{
					string path = AssetDatabase.GetAssetPath(obj);
					string nameWithExt = Path.GetFileName(path);
					writer.WriteLine(nameWithExt);
				}
			}
		}
		
		//verify asset bundle
		if(mVerifyAssetBundleAfterBuild && !mMakeCompressedAsset) 
		{
			bool res = TestAssetBundle(assetFilePath, objList.ToArray());
			if(res) {
				//LogToWindow("Asset Bundle passes tests");
			}
			else 
			{
				LogToWindow("Asset Bundle test filed. Asset Bundle may be invalid");
			}
		}
		
		LogToWindow("Asset Bundle was created");
	}
	
	//
	void RemoveOldAssetBundle(string assetFilePath)
	{
		File.Delete(assetFilePath);
	}
	
	//
	void ProcessDragAndDrop() 
	{
		Event evt = Event.current;
		switch (evt.type) 
		{
		case EventType.DragUpdated:
		case EventType.DragPerform:
			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			if (evt.type == EventType.DragPerform) 
			{
				DragAndDrop.AcceptDrag ();
				
				//unpack directories
				foreach(var path in DragAndDrop.paths) 
				{
					// Construct the system path of the asset folder
					string dataPath = Application.dataPath; 
					string fullPath = GetFullPath(path);
					
					if(mObjectsPathList.Contains(path)) 
					{
						continue;
					}
					
					if(!IsDirectory(fullPath)) 
					{
						mObjectsPathList.Add(path);
						continue;
					}
					
					// get the system file paths of all the files in the asset folder
					string[] filePaths = Directory.GetFiles(fullPath);
					
					// enumerate through the list of files loading the assets they represent and getting their type
					foreach (string filepath in filePaths) 
					{
						if(IsForbidenExtension(filepath)) {
							continue;
						}
						
						if(mObjectsPathList.Contains(filepath)) 
						{
							continue;
						}
						
						string relPath = filepath.Substring(dataPath.Length-6);
						mObjectsPathList.Add(relPath);
					}
				}
			}
			break;
		}
	}
	
	bool TestAssetBundle(string assetPath, Object[] objList){
		// Wait for the Caching system to be ready
		AssetBundle bundle = AssetBundle.CreateFromFile(assetPath);
		if(bundle == null) {
			Debug.Log("Failed to load Asset Bundle for test.");
			return false;
		}
		
		int errorObjects = 0;
		foreach(var obj in objList)
		{
			string path = AssetDatabase.GetAssetPath(obj);
			string nameWithoutExt = Path.GetFileNameWithoutExtension(path);
			Object testobj = bundle.Load(nameWithoutExt);
			if(testobj == null) {
				errorObjects++;
			}
		}
		if(errorObjects > 0) {
			Debug.Log("Asset bundle has invalid objects: " + errorObjects.ToString() );
			return false;
		}
		
		return true;
	}
	
	bool IsForbidenExtension(string path) 
	{
		string fileExt = Path.GetExtension(path);
		return mExcludeFileExt.Contains(fileExt);
	}
	
	bool IsDirectory(string path)
	{
		System.IO.FileAttributes fa = System.IO.File.GetAttributes(path);
		bool isDirectory = false;
		if ((fa & FileAttributes.Directory) != 0)
		{
			isDirectory = true;
		}
		return isDirectory;
	}
	
	string GetFullPath(string relPath)
	{
		string dataPath = Application.dataPath; 
		string fullPath = Path.Combine(dataPath.Substring(0, dataPath.Length - 6), relPath);
		return fullPath;
	}
}