using UnityEngine;
using System.Collections;

public class ControlButton : MonoBehaviour {
    public SettingGUI mainScript;

    public string command;
    public void WaitForKey() {
        if (mainScript == null)
        {
            mainScript = FindObjectOfType<SettingGUI>();
        }
        mainScript.WaitForKey(command);
    }
}
