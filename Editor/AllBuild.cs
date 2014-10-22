// C# example.
using UnityEditor;
using System.Diagnostics;
using UnityEngine;


public class AllBuild : MonoBehaviour {


    [MenuItem("BUILD/VK BUILD")]
    public static void BuildVK ()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        if (path != "")
        {


            string version = (AssetDatabase.LoadAssetAtPath("Assets/Prefab/MainPlayer.prefab", typeof(GameObject)) as GameObject).GetComponent<ServerHolder>().version;


            string[] levels = new string[EditorBuildSettings.scenes.Length];
            for(int i = 1;i<EditorBuildSettings.scenes.Length;i++){
                levels[i ] = EditorBuildSettings.scenes[i].path;

            }
            levels[0]= "Assets/map/mainmenu.unity";
         
            // Build vk Palyer
            BuildPipeline.BuildPlayer(levels, path + "/vk/builds" + version + "", BuildTarget.WebPlayer, BuildOptions.None);
        }
        
       
    }

    [MenuItem("BUILD/FACEBOOK BUILD")]
    public static void BuildFACE()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        if (path != "")
        {
            string version = (AssetDatabase.LoadAssetAtPath("Assets/Prefab/MainPlayer.prefab", typeof(GameObject)) as GameObject).GetComponent<ServerHolder>().version;


            string[] levels = new string[EditorBuildSettings.scenes.Length];
            for (int i = 1; i < EditorBuildSettings.scenes.Length; i++)
            {
                levels[i] = EditorBuildSettings.scenes[i].path;

            }

            // Build facebook Player
            levels[0] = "Assets/map/mainmenuface.unity";

            BuildPipeline.BuildPlayer(levels, path + "/face/builds" + version + "", BuildTarget.WebPlayer, BuildOptions.None);
        }
    }

    [MenuItem("BUILD/RED RAGE VK BUILD")]
    public static void BuildRageVK()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        if (path != "")
        {


            string version = (AssetDatabase.LoadAssetAtPath("Assets/Prefab/MainPlayer.prefab", typeof(GameObject)) as GameObject).GetComponent<ServerHolder>().version;


            string[] levels = new string[EditorBuildSettings.scenes.Length];
            for (int i = 1; i < EditorBuildSettings.scenes.Length; i++)
            {
                levels[i] = EditorBuildSettings.scenes[i].path;

            }
            levels[0] = "Assets/map/SimpleKaspi/MainMenu.unity";

            // Build vk Palyer
            BuildPipeline.BuildPlayer(levels, path + "/redrage/vk/builds" + version + "", BuildTarget.WebPlayer, BuildOptions.None);
        }


    }

}
