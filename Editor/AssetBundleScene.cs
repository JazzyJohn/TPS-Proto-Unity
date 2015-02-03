using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class buildbundle : EditorWindow
{

    [MenuItem("Assets/Build AssetBundle")]

    static void Init()
    {
        string path = EditorUtility.SaveFilePanel("Save Scene", "", "New scene", "unity3d");
        if (path.Length != 0)
        {
            Debug.Log(path);
            buildit(path);
        }
    }

    static void buildit(string path)
    {
        List<string> Levels = new List<string>();
        EditorBuildSettingsScene[] Scenes = EditorBuildSettings.scenes;
        string version = (AssetDatabase.LoadAssetAtPath("Assets/Prefab/MainPlayer.prefab", typeof(GameObject)) as GameObject).GetComponent<ServerHolder>().version;

        foreach (EditorBuildSettingsScene Scene in Scenes)
        {
            if (!Scene.enabled)
            {
                Levels.Add(Scene.path);
                BuildPipeline.BuildStreamedSceneAssetBundle(Levels.ToArray(), Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(Scene.path) + version + ".unity3d", EditorUserBuildSettings.activeBuildTarget);
                Levels.Clear();
            }
        }
      
    }
}