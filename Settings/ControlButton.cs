using UnityEngine;
using System.Collections;

public class ControlButton : MonoBehaviour {
    public MainMenuGUI mainScript;

    public string command;
    public void WaitForKey() {
        mainScript.WaitForKey(command);
    }
}
